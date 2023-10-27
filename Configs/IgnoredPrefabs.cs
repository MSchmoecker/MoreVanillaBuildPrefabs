﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class IgnoredPrefabs
    {
        /// <summary>
        ///     Checks prefab to see if it is eligble for making a custom piece.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static bool ShouldIgnorePrefab(GameObject prefab)
        {
            // Ignore specific prefab names
            if (_IgnoredPrefabs.Contains(prefab.name))
            {
                return true;
            }

            // Customs filters
            if (prefab.GetComponent("Projectile") != null ||
                prefab.GetComponent("Humanoid") != null ||
                prefab.GetComponent("AnimalAI") != null ||
                prefab.GetComponent("Character") != null ||
                prefab.GetComponent("CreatureSpawner") != null ||
                prefab.GetComponent("SpawnArea") != null ||
                prefab.GetComponent("Fish") != null ||
                prefab.GetComponent("RandomFlyingBird") != null ||
                prefab.GetComponent("MusicLocation") != null ||
                prefab.GetComponent("Aoe") != null ||
                prefab.GetComponent("ItemDrop") != null ||
                prefab.GetComponent("DungeonGenerator") != null ||
                prefab.GetComponent("TerrainModifier") != null ||
                prefab.GetComponent("EventZone") != null ||
                prefab.GetComponent("LocationProxy") != null ||
                prefab.GetComponent("LootSpawner") != null ||
                prefab.GetComponent("Mister") != null ||
                prefab.GetComponent("Ragdoll") != null ||
                prefab.GetComponent("MineRock5") != null ||
                prefab.GetComponent("TombStone") != null ||
                prefab.GetComponent("LiquidVolume") != null ||
                prefab.GetComponent("Gibber") != null ||
                prefab.GetComponent("TimedDestruction") != null ||
                prefab.GetComponent("ShipConstructor") != null ||
                prefab.GetComponent("TriggerSpawner") != null ||
                prefab.GetComponent("TeleportAbility") != null ||
                prefab.GetComponent("TeleportWorld") != null ||

                prefab.name.StartsWith("_") ||
                prefab.name.StartsWith("OLD_") ||
                prefab.name.EndsWith("OLD") ||
                prefab.name.StartsWith("vfx_") ||
                prefab.name.StartsWith("sfx_") ||
                prefab.name.StartsWith("fx_") ||
                prefab.name.Contains("Random")
            )
            {
                return true;
            }

            // Ignore pieces added by other mods
            if (prefab.name.StartsWith("BBH_") || // Azumat's BowsBeforeHoes mod
                prefab.name.StartsWith("rrr_")) // RRR prefabs
            {
                return true;
            }
            return false;
        }

        private static readonly HashSet<string> _IgnoredPrefabs = new() {
            "Player",
            "Valkyrie",
            "HelmetOdin",
            "CapeOdin",
            "CastleKit_pot03",
            "Ravens",
            "TERRAIN_TEST",
            "PlaceMarker",
            "Circle_section",
            "guard_stone_test",
            "Haldor",
            "odin",
            "dvergrprops_wood_stake",
            "Hildir",
            "Flies",
            "Pickable_DvergerThing",
            "rock_mistlands2", // Explodes into a boulder "___MineRock5"
            "demister_ball", // Placement is glitchy
            "CargoCrate", // Deletes itself on placement because it's empty
            "SunkenKit_int_towerwall_LOD", // is not an actual prefab for building
            "fuling_turret", // Duplicate of vanilla ballista
            "dragoneggcup", // It's invisible and I don't want to patch it
            "FishingRodFloat", // placement is broken
            "Pickable_RandomFood", // not sure what this is meant to be
            "horizontal_web", // has no mesh?
            "tolroko_flyer", // It's a little space ship! Instantiating it throws errors though.
            "turf_roof_wall", // Duplicate of "wood_roof_wall"
            "IceBlocker", // disappears after placing
            "Pickable_Item", // buggy and weird
            "Pickable_Barley_Wild",
            "Pickable_Flax_Wild",

            // Duplicates of wood chest
            "TreasureChest_blackforest",
            "TreasureChest_heath",
            "TreasureChest_heath_hildir",
            "TreasureChest_heath_hildir",
            "TreasureChest_meadows",
            "TreasureChest_meadows_buried",
            "TreasureChest_mountains",
            "TreasureChest_swamp",
            "shipwreck_karve_chest",
            "loot_chest_wood",

            // Duplicate of fcrypt
            "TreasureChest_forestcrypt",
            "stonechest", // Inventory name is weird

            // Duplicate of loot_chest_stone
            "TreasureChest_plains_stone",

            // Duplicate of "TreasureChest_dvergrtown"
            "TreasureChest_mountaincave_hildir",
            "TreasureChest_plainsfortress_hildir",
            "TreasureChest_forestcrypt_hildir",

            // Duplicate of mountainkit_brazier
            "CastleKit_brazier",

            // Not useful or different
            "CastleKit_groundtorch_unlit",
            "CastleKit_metal_groundtorch_unlit",
            "dvergrprops_lantern",
        };
    }
}