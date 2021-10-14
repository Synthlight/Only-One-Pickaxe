using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Base_Mod;
using HarmonyLib;
using JetBrains.Annotations;

namespace Only_One_Pickaxe {
    [UsedImplicitly]
    public class Plugin : BaseGameMod {
        protected override      bool               UseHarmony => true;
        public static readonly  GUID               PICKAXE     = GUID.Parse("c36fdd64ef80d8648803c6ca6463fd63");
        private static readonly GUID               COG_PICKAXE = GUID.Parse("06949b6bb6015ef4ea677324c635ddc1");
        public static           ToolItemDefinition cogPickaxeTool;

        public override void OnInitData() {
            cogPickaxeTool = RuntimeAssetDatabase.Get<ToolItemDefinition>().FirstOrDefault(def => def.AssetId == COG_PICKAXE);

            base.OnInitData();
        }
    }

    [HarmonyPatch]
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public static class HideNormalPickaxeWhenYouHaveCogPickaxe {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(Tools).GetMethod("GetAvailableTools", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {}, null);
        }

        private static ArrayList<ToolItemDefinition> GetAvailableTools(ref int m_availableToolsVersion, ref Inventory m_inventory, ref ArrayList<ToolItemDefinition> m_availableTools, ref ToolItemDefinition[] m_defaultTools) {
            var hasCogPickaxe = m_inventory?.Contains(Plugin.cogPickaxeTool, 1) ?? false;

            if (m_availableToolsVersion != m_inventory?.Version) {
                m_availableTools.Clear();
                foreach (var tool in m_defaultTools) {
                    if (tool.AssetId == Plugin.PICKAXE && hasCogPickaxe) continue;
                    if (!m_availableTools.Contains(tool)) m_availableTools.Add(tool);
                }
                for (var i = 0; i < m_inventory?.SlotCount; i++) {
                    if (m_inventory.TryGetSlot(i, out var item) && item.Item is ToolItemDefinition toolItem) {
                        if (toolItem.AssetId == Plugin.PICKAXE && hasCogPickaxe) continue;
                        if (!m_availableTools.Contains(toolItem)) {
                            m_availableTools.Add(toolItem);
                        }
                    }
                }
                m_availableTools.Sort();
                m_availableToolsVersion = m_inventory?.Version ?? 0;
            }
            return m_availableTools;
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool Prefix(ref ArrayList<ToolItemDefinition> __result, ref Inventory ___m_inventory, ref int ___m_availableToolsVersion, ref ArrayList<ToolItemDefinition> ___m_availableTools, ref ToolItemDefinition[] ___m_defaultTools) {
            __result = GetAvailableTools(ref ___m_availableToolsVersion, ref ___m_inventory, ref ___m_availableTools, ref ___m_defaultTools);
            return false;
        }
    }
}