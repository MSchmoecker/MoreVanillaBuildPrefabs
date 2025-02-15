﻿// Ignore Spelling: MVBP
using HarmonyLib;

namespace MVBP.Patches
{
    // Opened secret door
    // [Info   :MorePrefabs] Door state was changed
    // [Info: MorePrefabs] Previous State: 0
    // [Info: MorePrefabs] Current State: -1

    // Closed secret door
    // [Info: MorePrefabs] Door state was changed
    // [Info   :MorePrefabs] Previous State: -1
    // [Info: MorePrefabs] Current State: 0

    [HarmonyPatch(typeof(Door))]
    internal static class SecretDoorPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Door.RPC_UseDoor))]
        private static void UseDoorPrefix(Door __instance, out int? __state)
        {
            //if (!MorePrefabs.IsEnableDoorPatches) {
            //    __state = null;
            //    return;
            //}
            __state = GetDoorState(ref __instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Door.RPC_UseDoor))]
        private static void UseDoorPostfix(Door __instance, int? __state)
        {
            //if (!MorePrefabs.IsEnableDoorPatches) { return; }

            int? currentState = GetDoorState(ref __instance);
            if (currentState == null || __state == null) { return; }

            var prefabName = InitManager.GetPrefabName(__instance);
            if (prefabName == "dvergrtown_secretdoor")
            {
                bool isDoorClosing = (__state == -1 || __state == 1) && currentState == 0;

                if (isDoorClosing && __instance.gameObject.TryGetComponent(out ZNetView nview) && nview != null && nview.IsValid())
                {
                    if (!nview.IsOwner())
                    {
                        nview.ClaimOwnership();
                    }

                    // Reset to default state
                    __instance.m_animator.Rebind();
                    __instance.m_animator.Update(0f);
                }
            }
        }

        private static int? GetDoorState(ref Door door)
        {
            if (door.m_nview.IsValid())
            {
                return door.m_nview.GetZDO().GetInt(ZDOVars.s_state);
            }
            return null;
        }
    }
}