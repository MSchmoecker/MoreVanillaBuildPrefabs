﻿using System.Text.RegularExpressions;
using UnityEngine;

namespace MoreVanillaBuildPrefabs
{
    internal class PrefabNames
    {
        static readonly Regex PrefabNameRegex = new(@"([a-z])([A-Z])");
        public static string FormatPrefabName(string prefabName)
        {
            return PrefabNameRegex
                .Replace(prefabName, "$1 $2")
                .TrimStart(' ')
                .Replace('_', ' ')
                .Replace("  ", " ");
        }

        public static string GetPrefabDescription(GameObject prefab)
        {
            if (prefab.name == "metalbar_1x2") return "Enforced marble 1x2";

            HoverText hover = prefab.GetComponent<HoverText>();
            if (hover) return hover.m_text;

            ItemDrop item = prefab.GetComponent<ItemDrop>();
            if (item) return item.m_itemData.m_shared.m_name;

            Character chara = prefab.GetComponent<Character>();
            if (chara) return chara.m_name;

            RuneStone runestone = prefab.GetComponent<RuneStone>();
            if (runestone) return runestone.m_name;

            ItemStand itemStand = prefab.GetComponent<ItemStand>();
            if (itemStand) return itemStand.m_name;

            MineRock mineRock = prefab.GetComponent<MineRock>();
            if (mineRock) return mineRock.m_name;

            Pickable pickable = prefab.GetComponent<Pickable>();
            if (pickable) return GetPrefabDescription(pickable.m_itemPrefab);

            CreatureSpawner creatureSpawner = prefab.GetComponent<CreatureSpawner>();
            if (creatureSpawner) return GetPrefabDescription(creatureSpawner.m_creaturePrefab);

            SpawnArea spawnArea = prefab.GetComponent<SpawnArea>();
            if (spawnArea && spawnArea.m_prefabs.Count > 0)
            {
                return GetPrefabDescription(spawnArea.m_prefabs[0].m_prefab);
            }

            Piece piece = prefab.GetComponent<Piece>();
            if (piece && !string.IsNullOrEmpty(piece.m_name)) return piece.m_name;

            return prefab.name;
        }

        public static string GetPrefabName(Piece piece)
        {
            return RemoveFromEnd(piece.gameObject.name, "(Clone)");
        }

        public static string RemoveFromEnd(string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }
    }
}
