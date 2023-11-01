﻿// Ignore Spelling: MVBP

using Jotunn.Configs;
using Jotunn.Managers;
using MVBP.Configs;
using MVBP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.Helpers
{
    internal class InitManager
    {
        internal static readonly Dictionary<string, GameObject> PrefabRefs = new();
        internal static readonly Dictionary<string, Piece> DefaultPieceClones = new();
        internal static Dictionary<string, PieceDB> PieceRefs = new();

        internal static bool HasInitializedPrefabs => PrefabRefs.Count > 0;

        /// <summary>
        ///     Returns a bool indicating if the prefab has been changed by mod.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        internal static bool IsPatchedByMod(string prefabName)
        {
            return PrefabRefs.ContainsKey(prefabName);
        }

        /// <summary>
        ///     Returns true if the piece is one the mod touches and it
        ///     is currently enabled for building. Returns false if the
        ///     piece is not a custom piece or it is not enabled.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        internal static bool IsCustomPieceEnabled(string prefabName)
        {
            if (PieceRefs.ContainsKey(prefabName))
            {
                return PieceRefs[prefabName].enabled;
            }
            return false;
        }

        internal static void InitPrefabRefs()
        {
            if (PrefabRefs.Count > 0)
            {
                return;
            }

            Log.LogInfo("Initializing prefabs");

            // Find eligible prefabs for adding
            var PieceNameCache = PieceHelper.GetExistingPieceNames();
            var EligiblePrefabs = new Dictionary<string, GameObject>();
            foreach (var prefab in ZNetScene.instance.m_prefabs)
            {
                if (prefab.transform.parent == null && !PieceNameCache.Contains(prefab.name))
                {
                    if (PrefabFilter.GetEligiblePrefab(prefab, out GameObject result))
                    {
                        if (!EligiblePrefabs.ContainsKey(result.name))
                        {
                            EligiblePrefabs.Add(result.name, result);
                        }
                    }
                }
            }

            foreach (var prefab in EligiblePrefabs.Values)
            {
                if (!PieceHelper.EnsureNoDuplicateZNetView(prefab))
                {
                    continue;
                }

                if (Config.IsVerbosityHigh) { Log.LogPrefab(prefab); }

                // Always patching means it only runs once and
                // prevents trailership being unusable if disabled.
                try
                {
                    PrefabPatcher.PatchPrefabIfNeeded(prefab);
                }
                catch (Exception ex)
                {
                    Log.LogWarning($"Failed to patch prefab {prefab.name}: {ex}");
                }

                PrefabRefs.Add(prefab.name, prefab);
            }

            Log.LogInfo($"Found {PrefabRefs.Count()} prefabs");

            InitDefaultPieceClones();
        }

        /// <summary>
        ///     Initializes all prefabs to have pieces and
        ///     stores a deep copy of the piece component
        ///     to provide a template to reset to upon
        ///     re-initialization.
        /// </summary>
        private static void InitDefaultPieceClones()
        {
            if (!HasInitializedPrefabs)
            {
                return; // can't run without PrefabRefs
            }

            if (DefaultPieceClones.Count > 0)
            {
                return; // should only ever run once
            }

            Log.LogInfo("Initializing default pieces");

            // Get a default icon to use if piece doesn't have an icon.
            // Need this to prevent NRE's if other code references the piece
            // before the coroutine that is rendering the icons finishes. (Such as PlanBuild)
            var defaultIcon = Resources.FindObjectsOfTypeAll<Sprite>()
                ?.Where(spr => spr.name == "mapicon_hildir1")
                ?.First();

            foreach (var prefab in PrefabRefs.Values)
            {
                var defaultPiece = PieceHelper.InitPieceComponent(prefab);
                if (defaultPiece.m_icon == null)
                {
                    defaultPiece.m_icon = defaultIcon;
                }
            }

            if (Config.IsVerbosityMedium)
            {
                Log.LogInfo("Initializing default icons");
            }

            IconHelper.Instance.GeneratePrefabIcons(PrefabRefs.Values);

            foreach (var prefab in PrefabRefs.Values)
            {
                var go = new GameObject();
                var pieceClone = go.AddComponent<Piece>();
                pieceClone.CopyFields(prefab.GetComponent<Piece>());
                DefaultPieceClones.Add(prefab.name, pieceClone);
            }
        }

        /// <summary>
        ///     Initializes references to pieces
        ///     and their configuration settings
        /// </summary>
        internal static void InitPieceRefs()
        {
            Log.LogInfo("Initializing piece refs");

            if (PieceRefs.Count > 0)
            {
                PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);

                foreach (PieceDB pdb in PieceRefs.Values)
                {
                    // remove pieces from hammer build table
                    // and sheath hammer if table is open
                    PieceHelper.RemovePieceFromPieceTable(pdb.Prefab, hammerTable);

                    if (Player.m_localPlayer?.GetRightItem()?.m_shared.m_name == "$item_hammer")
                    {
                        Log.LogWarning("Hammer updated through config change, unequipping hammer");
                        Player.m_localPlayer.HideHandItems();
                    }
                }
                PieceRefs.Clear();
            }
            PieceRefs = GeneratePieceRefs();
        }

        /// <summary>
        ///     Create a set of piece refs with each prefab's
        ///     piece reset to the default state and the PieceDB
        ///     containing the configuration settings to apply.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, PieceDB> GeneratePieceRefs()
        {
            Dictionary<string, PieceDB> newPieceRefs = new();
            foreach (var prefab in PrefabRefs.Values)
            {
                // reset piece component to match the default piece clone
                prefab.GetComponent<Piece>().CopyFields(DefaultPieceClones[prefab.name]);

                // create piece ref
                newPieceRefs.Add(
                    prefab.name,
                    new PieceDB(
                        Config.LoadPrefabDB(prefab),
                        prefab.GetComponent<Piece>()
                    )
                );
            }
            return newPieceRefs;
        }

        /// <summary>
        ///     Apply the configuration settings from the
        ///     PieceDB for each piece in PieceRefs.
        /// </summary>
        internal static void InitPieces()
        {
            Log.LogInfo("Initializing pieces");
            foreach (var pieceDB in PieceRefs.Values)
            {
                var piece = PieceHelper.ConfigurePiece(pieceDB);
                SfxHelper.FixPlacementSfx(piece);
            }
        }

        /// <summary>
        ///     Add all pieces that are enabled in the cfg file to the hammer build
        ///     table according to CreatorShop related cfg settings. Allow sets
        ///     all pieces added to the hammer permit deconstruction by players.
        /// </summary>
        internal static void InitHammer()
        {
            Log.LogInfo("Initializing hammer");

            var pieceGroups = new SortedPieceGroups();

            foreach (var pieceDB in PieceRefs.Values)
            {
                // Check if piece is enabled by the mod
                if (!pieceDB.enabled && !Config.IsForceAllPrefabs)
                {
                    continue;
                }

                // Prevent adding creative mode pieces if not in CreativeMode
                if (!Config.IsCreativeMode
                    && PieceCategoryHelper.IsCreativeModePiece(pieceDB.piece))
                {
                    continue;
                }

                // Only add vanilla crops if enabled
                if (!Config.IsEnableHammerCrops
                    && pieceDB.pieceGroup == PieceGroup.VanillaCrop)
                {
                    continue;
                }

                // Prevent CreativeMode pieces and any clones of them
                // from being removable.
                // (Player.RemovePiece patch allows removing player-built instances).
                // Mimic Vanilla, make ships/carts non-removable.
                if (!PieceCategoryHelper.IsCreativeModePiece(pieceDB.piece)
                    && !pieceDB.Prefab.HasComponent<Ship>()
                    && !pieceDB.Prefab.HasComponent<Vagon>())
                {
                    pieceDB.piece.m_canBeRemoved = true;
                }

                // Restrict placement of CreatorShop pieces to Admins only
                if (Config.IsCreatorShopAdminOnly
                    && PieceCategoryHelper.IsCreatorShopPiece(pieceDB.piece)
                    && !SynchronizationManager.Instance.PlayerIsAdmin)
                {
                    continue;
                }

                pieceGroups.Add(pieceDB);
            }

            PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);
            foreach (List<GameObject> pieceGroup in pieceGroups)
            {
                foreach (var prefab in pieceGroup)
                {
                    PieceHelper.AddPieceToPieceTable(prefab, hammerTable);
                }
            }
        }

        /// <summary>
        ///     Method to re-initialize the plugin when the configuration
        ///     has been updated based on whether the piece or placement
        ///     settings have been changed for any of the config entries.
        /// </summary>
        /// <param name="msg"></param>
        internal static void ReInitPlugin(string msg, bool saveConfig = true)
        {
            if (!HasInitializedPrefabs) { return; }

            if (!Config.UpdatePieceSettings && !Config.UpdatePlacementSettings)
            {
                // Don't update unless settings have actually changed
                return;
            }

            var watch = new System.Diagnostics.Stopwatch();
            if (Config.IsVerbosityMedium)
            {
                watch.Start();
            }

            Log.LogInfo(msg);
            if (Config.UpdatePieceSettings)
            {
                InitPieceRefs();
                InitPieces();
                InitHammer();
            }
            if (Config.UpdatePlacementSettings)
            {
                UpdateNeedsCollisionPatch();
            }

            if (Config.IsVerbosityMedium)
            {
                watch.Stop();
                Log.LogInfo($"Time to re-initialize: {watch.ElapsedMilliseconds} ms");
            }
            else
            {
                Log.LogInfo("Re-initializing complete");
            }

            if (Config.UpdatePieceSettings)
            {
                ModCompat.UpdateExtraSnaps();
                ModCompat.UpdatePlanBuild();
            }

            Config.UpdatePieceSettings = false;
            Config.UpdatePlacementSettings = false;
            if (saveConfig) { Config.Save(); }
        }

        /// <summary>
        ///     Updates HashSet of prefabs needing a collision patch.
        /// </summary>
        private static void UpdateNeedsCollisionPatch()
        {
            if (!HasInitializedPrefabs) return;

            if (Config.IsVerbosityMedium)
            {
                Log.LogInfo("Initializing collision patches");
            }

            foreach (var prefabName in InitManager.PrefabRefs.Keys)
            {
                if (Config.PrefabDBConfigsMap[prefabName].placementPatch == null)
                {
                    // No placement patch config entry means that prefab is already
                    // placed in the NeedsCollisionPatchForGhost HashSet by default
                    continue;
                }

                if (Config.PrefabDBConfigsMap[prefabName].placementPatch.Value)
                {
                    // config is true so add it if not already in HashSet
                    if (!Config.NeedsCollisionPatchForGhost(prefabName))
                    {
                        Config._NeedsCollisionPatch.Add(prefabName);
                    }
                }
                else if (Config.NeedsCollisionPatchForGhost(prefabName))
                {
                    // config is false so remove it from list if it's in HashSet
                    Config._NeedsCollisionPatch.Remove(prefabName);
                }
            }
        }
    }
}