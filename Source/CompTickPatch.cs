using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;

namespace AutoAgeReverseForAll
{
    [HarmonyPatch(typeof(CompBiosculpterPod), nameof(CompBiosculpterPod.CompTick))]
    public static class CompTickPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stfld &&
                    codes[i].operand is FieldInfo field &&
                    field.Name == "autoAgeReversal")
                {
                    // Remove:
                    // ldarg.0
                    // ldc.i4.0
                    // stfld autoAgeReversal

                    codes[i - 2].opcode = OpCodes.Nop;
                    codes[i - 1].opcode = OpCodes.Nop;
                    codes[i].opcode = OpCodes.Nop;
                }
            }

            return codes;
        }
    }
}