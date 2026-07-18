using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AutoAgeReverseForAll
{
    [HarmonyPatch(typeof(CompBiosculpterPod), nameof(CompBiosculpterPod.CompGetGizmosExtra))]
    public static class GizmoPatch
    {
        public static IEnumerable<Gizmo> Postfix(
            IEnumerable<Gizmo> __result,
            CompBiosculpterPod __instance)
        {
            foreach (Gizmo gizmo in __result)
                yield return gizmo;

            Pawn pawn = Traverse.Create(__instance)
                .Field("biotunedTo")
                .GetValue<Pawn>();

            if (pawn == null)
                yield break;
            if (pawn.Ideo?.HasPrecept(PreceptDefOf.AgeReversal_Demanded) == true)
                yield break;

            var toggle = new Command_Toggle_DynamicDesc
            {
                defaultLabel = "AutoAgeReverseForAllLabel".Translate(pawn.Named("PAWN")),
                defaultDesc = "AutoAgeReverseForAllDesc".Translate(pawn.Named("PAWN")),
                dynamicDesc = () => GetAgeReverseDescription(pawn),
                icon = ContentFinder<Texture2D>.Get("UI/Gizmos/BiosculpterAutoAgeReversal"),

                isActive = () =>
                    Traverse.Create(__instance)
                        .Field("autoAgeReversal")
                        .GetValue<bool>(),

                toggleAction = () =>
                {
                    bool current = Traverse.Create(__instance)
                        .Field("autoAgeReversal")
                        .GetValue<bool>();

                    Traverse.Create(__instance)
                        .Field("autoAgeReversal")
                        .SetValue(!current);
                }
            };
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEV: Force age reversal due",
                    defaultDesc = "Sets this pawn's age reversal deadline to now.",
                    action = () =>
                    {
                        Traverse.Create(pawn.ageTracker)
                            .Field("ageReversalDemandedAtAgeTicks")
                            .SetValue(pawn.ageTracker.AgeBiologicalTicks);
                    }
                };
            }
            yield return toggle;
        }
        private static string GetAgeReverseDescription(Pawn pawn)
        {
            long ticks = pawn.ageTracker.AgeReversalDemandedDeadlineTicks;

            string time;

            if (ticks <= 0)
                time = "AutoAgeReverseForAllDueNow".Translate();
            else
                time = ((int)ticks).ToStringTicksToPeriodVague();

            return "AutoAgeReverseForAllDesc".Translate(pawn.Named("PAWN")) +
                   "\n\n" +
                   "Next age reversal: " + time;
        }
    }
}