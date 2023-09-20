using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Spiritfarer.EasyMinigames.Patches;

public static class MiningOverride
{
    public static void ApplyPatch(Harmony harmony)
    {
        var originalMethod = AccessTools.Method(typeof(ChargePickaxe), nameof(OnStateUpdate), new[] { typeof(Animator), typeof(AnimatorStateInfo), typeof(int) });
        var overrideMethod = AccessTools.Method(typeof(MiningOverride), nameof(OnStateUpdate));
        harmony.Patch(originalMethod, new HarmonyMethod(overrideMethod));
    }

    public static bool OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex, ChargePickaxe __instance, float ____startTime)
    {
        try
        {
            // Prevent failure by keeping the original method from executing. 
            var minigameField = __instance.GetType().BaseType.GetProperty("miningMinigame", BindingFlags.NonPublic | BindingFlags.Instance);
            var minigame = minigameField.GetValue(__instance) as MiningMinigame;

            // If the action button is held and the maximum swing time has expired, prevent the original method from being executed.
            // When the button is eventually released, the success condition will be evaluated before the check on the time limit in the original method
            return !(minigame.interactingCharacter._controller.GetAction(InputType.INTERACT) && Time.time - ____startTime > minigame._swingTime);
        }
        catch (Exception ex)
        {
            EasyMinigames.logger.LogError(ex);
            return true;
        }
    }
}
