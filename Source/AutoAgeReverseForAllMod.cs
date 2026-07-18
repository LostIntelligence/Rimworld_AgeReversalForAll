using HarmonyLib;
using Verse;

namespace AutoAgeReverseForAll
{
    public class AutoAgeReverseForAllMod : Mod
    {
        public AutoAgeReverseForAllMod(ModContentPack content) : base(content)
        {
            new Harmony("yourname.autoagereverseforall").PatchAll();
        }
    }
}