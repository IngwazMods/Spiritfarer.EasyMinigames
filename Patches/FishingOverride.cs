using HarmonyLib;
using UnityEngine;

namespace Spiritfarer.EasyMinigames.Patches;

public static class FishingOverride
{
    public static void ApplyPatch(Harmony harmony)
    {
        var originalMethod = AccessTools.Method(typeof(FightingPhaseState), nameof(OnStateUpdate), new[] { typeof(Animator), typeof(AnimatorStateInfo), typeof(int) });
        var overrideMethod = AccessTools.Method(typeof(FishingOverride), nameof(OnStateUpdate));

        harmony.Patch(originalMethod, new HarmonyMethod(overrideMethod));
    }

    public static bool OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex, ref float ____startTime)
    {
        // The reel tension depends on a few factors, one of which is the duration of the action button being held down (time_held_down = current_time - start_time_button_press).
        // By constantly setting the button press start time equal to the current time, we should prevent the tension from ever reaching breaking point.
        ____startTime = Time.time;

        return true;
    }
}
