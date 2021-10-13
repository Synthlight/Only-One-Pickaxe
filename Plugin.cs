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
    public static class HideNormalPickaxeWhenYouHaveCogPickaxe {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(Tools).GetMethod(nameof(Tools.CanUseTool), BindingFlags.Public | BindingFlags.Instance);
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool Prefix(ref ToolItemDefinition tool, ref bool __result, ref Inventory ___m_inventory) {
            if (tool?.AssetId == Plugin.PICKAXE && ___m_inventory?.Contains(Plugin.cogPickaxeTool, 1) == true) {
                __result = false;
                return false;
            }

            return true;
        }
    }
}