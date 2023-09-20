using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Spiritfarer.EasyMinigames.Patches;

public static class CrusherOverride
{
    public static void ApplyPatch(Harmony harmony)
    {
        var originalMethod = typeof(CrusherMinigame).GetMethod(nameof(CrushItem), BindingFlags.NonPublic | BindingFlags.Instance);
        var overrideMethod = AccessTools.Method(typeof(CrusherOverride), nameof(CrushItem));
        harmony.Patch(originalMethod, new HarmonyMethod(overrideMethod));
    }

    public static bool CrushItem(
        ItemData item,
        CrusherMinigame __instance,
        bool ____isUpgraded,
        ref CrusherMinigameData ____currentData,
        ref IEnumerator __result)
    {
        try
        {
            return CrushItemOverride(item, __instance, ____isUpgraded, ref ____currentData, ref __result);
        }
        catch (Exception ex)
        {
            EasyMinigames.logger.LogError(ex);
            return true;
        }
    }

    private static bool CrushItemOverride(
        ItemData item,
        CrusherMinigame __instance,
        bool ____isUpgraded,
        ref CrusherMinigameData ____currentData,
        ref IEnumerator __result)
    {
        __result = Enumerable.Empty<CustomYieldInstruction>().GetEnumerator();

        // >>> Logic from the original method we need to keep
        var minigameData = item.GetData<CrusherMinigameData>();

        if (!__instance.running || minigameData == null)
        {
            return false;
        }

        ____currentData = minigameData;
        // <<< Logic from the original method

        // The crushing minigame seems very demanding for people with certain physical disabilities. And more generally, in my not-so-humble opinion, is just a drag.
        // It should be possible to simplify it by requiring a single crush action (button mash) per item, but I'd rather patch it out altogether and be done with it.
        __instance._components.rockContainer.RemoveItem(item, 1, false);

        // This is what the original method does, not sure if this is needed... left it in for now
        GameUtil.EventManager.FireEvent(GameEventType.ITEMS_CRAFTED, new CraftItemsGameEventParams(__instance.interactingCharacter, ____currentData.output.GetItemDatas()));

        var outputItems = ____currentData.output.GetItemDatas();
        if (outputItems.Count > 0)
        {
            var dupeItem = outputItems[0];
            int amountToAdd = (int)(outputItems.Count * (____isUpgraded ? __instance._parameters.upgradeYieldMultiplier : 1) - outputItems.Count);
            for (int i = 0; i < amountToAdd; i++)
            {
                outputItems.Add(dupeItem);
            }
        }
        __instance._components.spawner.SpawnGroup(outputItems);

        return false;
    }
}
