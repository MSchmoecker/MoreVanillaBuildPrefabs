﻿// Ignore Spelling: MVBP

using Jotunn.Managers;
using MVBP.Configs;
using MVBP.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVBP.Helpers
{
    internal static class PrefabPatcher
    {
        private static readonly int PieceLayer = LayerMask.NameToLayer("piece");
        private static readonly int CharacterTriggerLayer = LayerMask.NameToLayer("character_trigger");

        /// <summary>
        ///     Fix collider and snap points on the prefab if necessary
        /// </summary>
        /// <param name="prefab"></param>
        internal static void PatchPrefabIfNeeded(GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }

            List<Vector3> pts = new();
            switch (prefab.name)
            {
                case "ArmorStand_Male":
                case "ArmorStand_Female":
                    SnapPointHelper.AddSnapPointToCenter(prefab);
                    try
                    {
                        var body = prefab?.transform?.Find("Player Pose")?.Find("Visual")?.Find("body")?.gameObject;
                        var meshRender = body.GetComponent<SkinnedMeshRenderer>();
                        meshRender.sharedMaterial = TextureHelper.GetCustomArmorStandMaterial();
                    }
                    catch
                    {
                        Log.LogWarning($"Failed to patch material for {prefab.name}");
                    }
                    break;

                case "Trailership":
                    // Fix hull
                    var meshFilter = prefab.FindDeepChild("hull").GetComponent<MeshFilter>();
                    var longShip = ZNetScene.instance?.GetPrefab("VikingShip");
                    var lsMeshFilter = longShip.FindDeepChild("hull").GetComponent<MeshFilter>();
                    if (meshFilter && lsMeshFilter)
                    {
                        meshFilter.mesh = lsMeshFilter.mesh;
                    }

                    // Fix shields
                    var shieldBanded = ZNetScene.instance?.GetPrefab("ShieldBanded");
                    var mat = shieldBanded.GetComponentInChildren<MeshRenderer>().material;
                    var storage = prefab.transform.Find("ship").Find("visual").Find("Customize").Find("storage");
                    if (mat && storage != null)
                    {
                        int children = storage.childCount;
                        for (int i = 0; i < children; ++i)
                        {
                            var child = storage.GetChild(i);
                            if (child != null && child.name.StartsWith("Shield"))
                            {
                                child.GetComponent<MeshRenderer>().material = mat;
                            }
                        }
                    }

                    // Fix Sail cloth
                    var cloth = prefab.FindDeepChild("sail_full").GetComponent<Cloth>();
                    var lsCloth = longShip.FindDeepChild("sail_full").GetComponent<Cloth>();
                    if (cloth && lsCloth)
                    {
                        cloth.coefficients = lsCloth.coefficients;
                    }

                    // Fix missing control GUI position
                    var ship = prefab.GetComponent<Ship>();
                    if (ship)
                    {
                        var controlGui = new GameObject("ControlGui");
                        controlGui.transform.parent = prefab.transform;
                        controlGui.transform.localPosition = new Vector3(1.0f, 1.696f, -6.54f);
                        ship.m_controlGuiPos = controlGui.transform;

                        // Fix sail not catching wind
                        ship.m_sailForceOffset = 2; // same as longship
                        ship.m_sailForceFactor = 0.05f; // same as longship
                    }

                    // Fix missing rudder button attachpoint
                    var shipControls = prefab.FindDeepChild("rudder_button").GetComponent<ShipControlls>();
                    var rudderAttach = prefab.transform?.Find("sit locations")?.Find("sit_box (4)")?.Find("attachpoint");
                    if (shipControls && rudderAttach != null)
                    {
                        shipControls.m_attachPoint = rudderAttach.transform;
                    }

                    break;

                // Causes the chest to break if loaded without mod
                //case "TreasureChest_dvergr_loose_stone":
                //    var boxCollider = gameObject.AddComponent<BoxCollider>();
                //    boxCollider.size = new Vector3(2, 1, 2);
                //    SnapPointHelper.AddSnapPointsToBoxColliderCorners(gameObject, boxCollider);
                //    break;

                case "TreasureChest_mountaincave":
                case "TreasureChest_trollcave":
                    SnapPointHelper.AddSnapPointsToMeshCorners(prefab, "stonechest", true);
                    break;
                //case "TreasureChest_dvergrtown":
                //    break;
                //case "TreasureChest_dvergrtower":
                //    break;
                case "TreasureChest_plains_stone":
                case "TreasureChest_fCrypt":
                case "TreasureChest_sunkencrypt":
                    SnapPointHelper.AddSnapPoints(
                       prefab,
                       new[] {
                            new Vector3(0.0f, -0.01f, 0.0f),
                            new Vector3(1.0f, -0.01f, 0.37f),
                            new Vector3(1.0f, -0.01f, -0.37f),
                            new Vector3(-1.0f, -0.01f, 0.37f),
                            new Vector3(-1.0f, -0.01f, -0.37f),
                            new Vector3(0.65f, 0.8f, 0.35f),
                            new Vector3(0.65f, 0.8f, -0.35f),
                            new Vector3(-0.65f, 0.8f, 0.35f),
                            new Vector3(-0.65f, 0.8f, -0.35f)
                       },
                       true
                    );
                    break;

                case "blackmarble_column_3":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 4, -1),
                            new Vector3(1, 4, 1),
                            new Vector3(-1, 4, 1),
                            new Vector3(1, 4, -1),

                            new Vector3(-1, 2, -1),
                            new Vector3(1, 2, 1),
                            new Vector3(-1, 2, 1),
                            new Vector3(1, 2, -1),

                            new Vector3(-1, 0, -1),
                            new Vector3(1, 0, 1),
                            new Vector3(-1, 0, 1),
                            new Vector3(1, 0, -1),

                            new Vector3(-1, -2, -1),
                            new Vector3(1, -2, 1),
                            new Vector3(-1, -2, 1),
                            new Vector3(1, -2, -1),

                            new Vector3(-1, -4, -1),
                            new Vector3(1, -4, 1),
                            new Vector3(-1, -4, 1),
                            new Vector3(1, -4, -1),
                        }
                    );
                    break;

                case "blackmarble_creep_4x1x1":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-0.5f, 2, -0.5f),
                            new Vector3(0.5f, 2, 0.5f),
                            new Vector3(-0.5f, 2, 0.5f),
                            new Vector3(0.5f, 2, -0.5f),

                            new Vector3(-0.5f, -2, -0.5f),
                            new Vector3(0.5f, -2, 0.5f),
                            new Vector3(-0.5f, -2, 0.5f),
                            new Vector3(0.5f, -2, -0.5f),

                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );

                    // ? Place the piece randomly in horizontal or vertical position ?
                    UnityEngine.Object.DestroyImmediate(prefab.transform.Find("new").gameObject.GetComponent<RandomPieceRotation>());

                    break;

                case "blackmarble_creep_4x2x1":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 2, -0.5f),
                            new Vector3(1, 2, 0.5f),
                            new Vector3(-1, 2, -0.5f),
                            new Vector3(-1, 2, 0.5f),

                            new Vector3(1, -2, -0.5f),
                            new Vector3(1, -2, 0.5f),
                            new Vector3(-1, -2, -0.5f),
                            new Vector3(-1, -2, 0.5f),

                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );

                    // ? Place the piece randomly in horizontal or vertical position ?
                    UnityEngine.Object.DestroyImmediate(prefab.transform.Find("new").gameObject.GetComponent<RandomPieceRotation>());
                    break;

                case "blackmarble_creep_slope_inverted_1x1x2":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-0.5f, 1, -0.5f),
                            new Vector3(0.5f, 1, 0.5f),
                            new Vector3(-0.5f, 1, 0.5f),
                            new Vector3(0.5f, 1, -0.5f),

                            new Vector3(0.5f, -1, -0.5f),
                            new Vector3(-0.5f, -1, -0.5f),
                        }
                    );
                    break;

                case "blackmarble_creep_slope_inverted_2x2x1":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 0.5f, -1),
                        new Vector3(1, 0.5f,1),
                        new Vector3(-1, 0.5f, 1),
                        new Vector3(1, 0.5f, -1),

                        new Vector3(-1, -0.5f, -1),
                        new Vector3(1, -0.5f, -1),
                    });
                    break;

                case "blackmarble_creep_stair":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 1, -1),
                        new Vector3(1, 1, -1),
                        new Vector3(-1, 0, -1),
                        new Vector3(1, 0, -1),
                        new Vector3(-1, 0, 1),
                        new Vector3(1, 0, 1),
                    });
                    break;

                case "blackmarble_floor_large":
                    for (int y = -1; y <= 1; y += 2)
                    {
                        for (int x = -4; x <= 4; x += 2)
                        {
                            for (int z = -4; z <= 4; z += 2)
                            {
                                pts.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPoints(prefab, pts);
                    break;

                case "blackmarble_head_big01":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 1, -1),
                        new Vector3(1, 1,1),
                        new Vector3(-1, 1, 1),
                        new Vector3(1, 1, -1),

                        new Vector3(1, -1, -1),
                        new Vector3(-1, -1, -1),
                    });
                    break;

                case "blackmarble_head_big02":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 1, -1),
                            new Vector3(1, 1,1),
                            new Vector3(-1, 1, 1),
                            new Vector3(1, 1, -1),

                            new Vector3(1, -1, -1),
                            new Vector3(-1, -1, -1),
                        }
                    );
                    break;

                case "blackmarble_out_2":
                    // Fix piece colliders via layers
                    UnityEngine.Object.DestroyImmediate(prefab.GetComponent<MeshCollider>());
                    SnapPointHelper.FixPieceLayers(prefab);
                    break;

                case "blackmarble_tile_floor_1x1":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(0.5f, 0.1f, 0.5f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(0.5f, 0.1f, -0.5f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.1f, 0.5f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.1f, -0.5f);
                    break;

                case "blackmarble_tile_floor_2x2":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, 0.1f, 1);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 0.1f, -1);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, 0.1f, 1);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 0.1f, -1);
                    break;

                case "blackmarble_tile_wall_1x1":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(0.5f, 0.95f, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.95f, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 0.45f, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(0, 0.95f, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.45f, 0.1f);
                    break;

                case "blackmarble_tile_wall_2x2":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 1.95f, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 0.95f, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.95f, 0.1f);
                    break;

                case "blackmarble_tile_wall_2x4":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 3.95f, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 3.95f, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 1.95f, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.5f, -0.05f, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 1.95f, 0.1f);
                    break;

                case "dungeon_queen_door":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2.5f, 0, 0),
                            new Vector3(-2.5f, 0, 0),
                        }
                    );
                    break;

                case "dungeon_sunkencrypt_irongate":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, -0.4f, 0),
                            new Vector3(-1, -0.4f, 0),
                        }
                    );
                    break;

                case "sunken_crypt_gate":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 0, 0),
                            new Vector3(-1, 0, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_beam":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(3, 0, 0),
                            new Vector3(-3, 0, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_pole":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0, 2, 0),
                            new Vector3(0, -2, 0),
                        }
                    );
                    break;

                case "dvergrprops_wood_wall":
                    // Patch only the floor
                    /*generateSnapPoints(gameObject, new Vector3[] {
                        new Vector3(2.2f, -2, -0.45f),
                        new Vector3(2.2f, -2, 0.45f),
                        new Vector3(-2.2f, -2, -0.45f),
                        new Vector3(-2.2f, -2, 0.45f),
                    });*/
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2.2f, 2, 0),
                            new Vector3(-2.2f, 2, 0),
                            new Vector3(2.2f, -2, 0),
                            new Vector3(-2.2f, -2, 0),
                        }
                    );
                    break;

                case "dvergrtown_arch":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 0.5f, 0),
                        }
                    );
                    break;

                case "dvergrtown_secretdoor":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2, 0, 0),
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 4, 0),
                            new Vector3(-2, 4, 0),
                        }
                    );
                    break;

                case "dvergrtown_slidingdoor":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2, 0, 0),
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 4, 0),
                            new Vector3(-2, 4, 0),
                        }
                    );
                    break;

                case "dvergrtown_stair_corner_wood_left":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0.25f, 0, -0.25f),
                            new Vector3(0.25f, 0, 0.25f),
                            new Vector3(0.25f, 1.1f, -0.25f),
                            new Vector3(0.25f, 1.1f, 0.25f),

                            new Vector3(0.25f, 0, 2),
                            new Vector3(-0.25f, 1.1f, -0.25f),

                            new Vector3(-2, 1.1f, -0.25f)
                        }
                    );
                    break;

                case "dvergrprops_shelf":
                case "dvergrprops_table":
                    var wearNTear = prefab.GetComponent<WearNTear>();
                    if (wearNTear)
                    {
                        wearNTear.m_supports = true; // allow these pieces to support other pieces
                    }
                    break;

                case "dvergrtown_wood_beam":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(3, 0, 0),
                            new Vector3(-3, 0, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_pole":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_stake":
                    // Patch only the floor
                    /*
                    SnapPointHelper.AddSnapPoints(
                        gameObject,
                        new Vector3[] {
                            new Vector3(0.15f, 0, 0.15f),
                            new Vector3(-0.15f, 0, 0.15f),
                            new Vector3(0.15f, 0, -0.15f),
                            new Vector3(-0.15f, 0, -0.15f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            Vector3.zero,
                        }
                    );
                    break;

                case "dvergrtown_wood_crane":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(0, -3, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_support":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 0, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall01":
                    // Fix collider y
                    prefab.transform.Find("wallcollider").transform.localPosition = Vector3.zero;
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-3, -2.7f, 0),
                            new Vector3(3, -2.7f, 0),
                            new Vector3(-3, 2.7f, 0),
                            new Vector3(3, 2.7f, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall02":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-3, -0.5f, 0),
                            new Vector3(3, -0.5f, 0),
                            new Vector3(-3, 4.5f, 0),
                            new Vector3(3, 4.5f, 0),
                        }
                    );
                    break;

                case "dvergrtown_wood_wall03":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1.1f, 0, 0),
                            new Vector3(-1.1f, 0, 0),

                            new Vector3(2, 2, 0),
                            new Vector3(-2, 2, 0),

                            new Vector3(1.1f, 4, 0),
                            new Vector3(-1.1f, 4, 0),
                        }
                    );
                    break;

                case "goblin_roof_45d":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 0, 1),
                            new Vector3(-1, 0, 1),
                            new Vector3(1, 2, -1),
                            new Vector3(-1, 2, -1),
                        }
                    );
                    break;

                case "goblin_roof_45d_corner":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 0, -1),
                            new Vector3(1, 0, 1),
                        }
                    );
                    break;

                case "goblin_woodwall_1m":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-0.5f, 0, 0),
                            new Vector3(0.5f, 0, 0),
                            new Vector3(-0.5f, 2, 0),
                            new Vector3(0.5f, 2, 0),
                        }
                    );
                    break;

                case "goblin_woodwall_2m":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 0, 0),
                            new Vector3(1, 0, 0),
                            new Vector3(-1, 2, 0),
                            new Vector3(1, 2, 0),
                        }
                    );
                    break;

                case "Ice_floor":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(2, 1, 2),
                            new Vector3(-2, 1, -2),
                            new Vector3(2, 1, -2),
                            new Vector3(-2, 1, 2),

                            new Vector3(2, -1, 2),
                            new Vector3(-2, -1, -2),
                            new Vector3(2, -1, -2),
                            new Vector3(-2, -1, 2),
                        }
                    );
                    break;

                case "turf_roof_top":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            // mid edge points get added by ExtraSnapPointsMadeEasy
                            new Vector3(1.0f, 0.5f, 0.0f),  // top front
                            new Vector3(-1.0f, 0.5f, 0.0f),  // top back
                        }
                    );
                    break;

                case "metalbar_1x2":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(1.0f, -1.0f, 0.0f),
                            new Vector3(1.0f, 0.0f, 0.0f),
                            new Vector3(-1.0f, -1.0f, 0.0f),
                            new Vector3(-1.0f, 0.0f, 0.0f),
                        }
                    );
                    break;

                case "stone_floor":
                    for (float y = -0.5f; y <= 0.5f; y += 1)
                    {
                        for (int x = -2; x <= 2; x += 1)
                        {
                            for (int z = -2; z <= 2; z += 1)
                            {   // skip corners that already have snap points
                                if (!(Math.Abs(x) == 2 && Mathf.Abs(z) == 2))
                                {
                                    pts.Add(new Vector3(x, y, z));
                                }
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPoints(prefab, pts);
                    break;

                case "stoneblock_fracture":
                    // x and y scale is strange for this one
                    for (float y = -0.5f; y <= 0.5f; y += 1)
                    {
                        for (int x = -1; x <= 1; x += 1)
                        {
                            for (int z = -1; z <= 1; z += 1)
                            {
                                {
                                    pts.Add(new Vector3(x, y, z));
                                }
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPoints(prefab, pts, true);

                    // Can keep the MineRock component now that I handle the item drops properly
                    // UnityEngine.Object.DestroyImmediate(prefab.GetComponent<MineRock>());

                    CollisionHelper.AddBoxCollider(prefab, Vector3.zero, new Vector3(2, 1, 2));
                    break;

                case "blackmarble_post01":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(1.0f, 0.0f, 1.0f),
                            new Vector3(1.0f, 0.0f, -1.0f),
                            new Vector3(-1.0f, 0.0f, -1.0f),
                            new Vector3(-1.0f, 0.0f, 1.0f),
                            new Vector3(1.0f, 1.0f, 1.0f),
                            new Vector3(1.0f, 1.0f, -1.0f),
                            new Vector3(-1.0f, 1.0f, -1.0f),
                            new Vector3(-1.0f, 1.0f, 1.0f),
                            new Vector3(0.0f, 3.5f, 0.0f), // for demisters/torches
                        },
                        true
                    );
                    foreach (var collider in prefab.GetComponentsInChildren<BoxCollider>())
                    {
                        if (collider.name == "collider")
                        {
                            collider.center += new Vector3(0.0f, 0.5f, 0.0f);
                            collider.size += new Vector3(0.0f, -1.0f, 0.0f);
                        }
                    }
                    break;

                case "wood_ledge":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.5f, 0.0f, 0.25f),
                            new Vector3(0.0f, 0.0f, 0.25f),
                            new Vector3(-0.5f, 0.0f, 0.25f),
                            new Vector3(-0.5f, 0.0f, -0.25f),
                            new Vector3(0.5f, 0.0f, -0.25f),
                        }
                    );
                    break;

                case "dverger_demister":
                    CollisionHelper.RemoveColliders(prefab); //remove large box collider

                    // add thin collider along post
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.2f, 0.0f), new Vector3(0.2f, 1.1f, 0.2f));

                    // add collider around the ring
                    CollisionHelper.AddBoxCollider(prefab, Vector3.zero, new Vector3(1.0f, 0.1f, 1.0f));

                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(0.0f, -0.65f, 0.0f),
                        }
                    );
                    break;

                case "dverger_demister_large":
                    CollisionHelper.RemoveColliders(prefab); //remove large box collider

                    // add thin collider along post
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.25f, 0.0f), new Vector3(0.2f, 1.3f, 0.2f));

                    // add collider around the ring
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.25f, 0.0f), new Vector3(1.0f, 0.1f, 1.0f));

                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(0.0f, -0.9f, 0.0f),
                        }
                    );
                    break;

                case "dvergrprops_hooknchain":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 2.5f, 0.0f)
                        }
                    );
                    break;

                case "barrell":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, -1.0f, 0.0f)
                        }
                    );
                    break;

                case "goblin_strawpile":
                    // needs collider to allow removal
                    CollisionHelper.AddBoxCollider(
                        prefab,
                        Vector3.zero,
                        new Vector3(1.5f, 0.02f, 1.5f)
                    );
                    break;

                case "mountainkit_chair":
                    {
                        var chair = prefab.AddComponent<Chair>();
                        var attachPoint = new GameObject("attachPoint");
                        attachPoint.transform.parent = prefab.transform;
                        attachPoint.transform.localPosition = Vector3.zero;
                        chair.m_attachPoint = attachPoint.transform;
                    }
                    break;

                case "dvergrprops_chair":
                    {
                        var chair = prefab.AddComponent<Chair>();
                        var attachPoint = new GameObject("attachPoint");
                        attachPoint.transform.parent = prefab.transform;
                        attachPoint.transform.localPosition = new Vector3(0.0f, -0.15f, 0.0f);
                        chair.m_attachPoint = attachPoint.transform;
                    }

                    break;

                case "dvergrprops_stool":
                    {
                        var chair = prefab.AddComponent<Chair>();
                        var attachPoint = new GameObject("attachPoint");
                        attachPoint.transform.parent = prefab.transform;
                        attachPoint.transform.localPosition = new Vector3(0.0f, -0.1f, 0.0f);
                        chair.m_attachPoint = attachPoint.transform;
                    }
                    break;

                default:
                    break;
            }

            if (MorePrefabs.PatchPortalTexture && prefab.name == "portal" && !GUIManager.IsHeadless())
            {
                var meshRender = prefab.transform.Find("New").Find("model").GetComponent<MeshRenderer>();
                if (meshRender)
                {
                    meshRender.material.mainTexture = TextureHelper.GetBlackMarblePortalTexture();
                    meshRender.material.SetTexture(
                        TextureHelper.TextureNames.BumpMap,
                        TextureHelper.GetBlackMarblePortalBumpMap()
                    );
                }
            }

            if (MorePrefabs.IsEnableComfortPatches)
            {
                ApplyComfortPatches(prefab);
            }
        }

        /// <summary>
        ///     Applies patches to selected pieces so they grant
        ///     comfort as expected if they were vanilla pieces.
        /// </summary>
        /// <param name="prefab"></param>
        private static void ApplyComfortPatches(GameObject prefab)
        {
            if (prefab.TryGetComponent(out Piece piece))
            {
                switch (prefab.name)
                {
                    case "dvergrprops_chair":
                    case "dvergrprops_stool":
                        piece.m_comfortGroup = Piece.ComfortGroup.Chair;
                        piece.m_comfort = 2;
                        break;

                    case "mountainkit_chair":
                        piece.m_comfortGroup = Piece.ComfortGroup.Chair;
                        piece.m_comfort = 1;
                        break;

                    case "dvergrprops_bed":
                        piece.m_comfortGroup = Piece.ComfortGroup.Bed;
                        piece.m_comfort = 2;
                        break;

                    case "goblin_bed":
                        piece.m_comfortGroup = Piece.ComfortGroup.Bed;
                        piece.m_comfort = 1;
                        break;

                    case "ArmorStand_Female":
                    case "ArmorStand_Male":
                        piece.m_comfortGroup = Piece.ComfortGroup.None;
                        piece.m_comfort = 2;
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        ///     Apply patches to player built pieces.
        ///     Called after Piece.Awake and Piece.SetCreator.
        /// </summary>
        /// <param name="piece"></param>
        internal static void PatchPlayerBuiltPieceIfNeed(Piece piece)
        {
            if (!piece || !piece.gameObject || !piece.IsPlacedByPlayer() || !InitManager.IsPatchedByMod(piece))
            {
                return;
            }
            var prefabName = InitManager.GetPrefabName(piece);

            if (piece.TryGetComponent(out Destructible destructible))
            {
                EditDestructibleSpawn(prefabName, destructible);
            }

            if (MorePrefabs.IsEnableDoorPatches)
            {
                ApplyDoorPatches(prefabName, piece.gameObject);
            }
            if (MorePrefabs.IsEnablePlayerBasePatches)
            {
                ApplyPlayerBasePatches(prefabName, piece.gameObject);
            }
            if (MorePrefabs.IsEnableBedPatches)
            {
                ApplyBedPatches(prefabName, piece.gameObject);
            }
            if (MorePrefabs.IsEnableFermenterPatches)
            {
                ApplyFermenterPatches(prefabName, piece.gameObject);
            }
            if (MorePrefabs.PatchDvergrWoodTexture)
            {
                ApplyNewDvergrTexture(prefabName, piece.gameObject);
            }
            //if (piece.gameObject.GetComponent<LiquidVolume>() != null)
            //{
            //    AddLiquidCollider(piece.gameObject);
            //}
        }

        /// <summary>
        ///     Edit the prefab and ZDO to change the value of m_spawnOnDestroyed in a
        ///     persistent way based on the default config settings.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="destructible"></param>
        private static void EditDestructibleSpawn(string name, Destructible destructible)
        {
            if (!destructible || !PrefabConfigs.DefaultConfigValues.TryGetValue(name, out var config))
            {
                return;
            }

            if (string.IsNullOrEmpty(config.spawnOnDestroyed))
            {
                return;
            }

            var spawn = ZNetScene.instance.GetPrefab(config.spawnOnDestroyed);
            if (!spawn)
            {
                return;
            }

            var zdo = destructible.m_nview.GetZDO();
            if (zdo == null)
            {
                return;
            }
            zdo.Set("HasFields", true);
            zdo.Set("HasFieldsDestructible", true);
            zdo.Set("Destructible.m_spawnWhenDestroyed", "fx_crystal_destruction");

            destructible.m_spawnWhenDestroyed = spawn;
        }

        //private static void AddLiquidCollider(GameObject gameObject)
        //{
        //    //UnityEngine.Object.DestroyImmediate(liquidVolume);
        //    //foreach (var box in clonedPrefab.GetComponentsInChildren<BoxCollider>())
        //    //{
        //    //    UnityEngine.Object.DestroyImmediate(box);
        //    //}
        //    //var boxCollider = clonedPrefab.AddComponent<BoxCollider>();
        //    var placementCollider = new GameObject("placementCollider");
        //    placementCollider.transform.parent = gameObject.transform;
        //    var collider = placementCollider.AddComponent<BoxCollider>();
        //    collider.enabled = true;
        //    collider.size = new Vector3(12f, 12f, 12f);
        //}

        /// <summary>
        ///     Sets the texture of certain dvergr pieces to use
        ///     a cleaner texture when they are above 50% health
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gameObject"></param>
        private static void ApplyNewDvergrTexture(string name, GameObject gameObject)
        {
            if (PrefabConfigs.DvergrWoodPieces.Contains(name))
            {
                Renderer[] componentsInChildren = gameObject.transform.Find("New").GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in componentsInChildren)
                {
                    renderer.material.mainTexture = TextureHelper.GetNewDvergrTexture();
                }
            }
        }

        /// <summary>
        ///     Adds PlayerBase effect to pieces based on PieceDB settings
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gameObject"></param>
        private static void ApplyPlayerBasePatches(string name, GameObject gameObject)
        {
            if (InitManager.TryGetPieceDB(name, out PieceDB pieceDB))
            {
                if (pieceDB.playerBasePatch) { AddPlayerBase(gameObject); }
            }
        }

        // TODO: can I add game objects via ZDO or just components?
        private static void AddPlayerBase(GameObject gameObject)
        {
            // create PlayerBase object as child
            var playerBase = new GameObject("PlayerBase");
            playerBase.transform.parent = gameObject.transform;
            playerBase.transform.localScale = Vector3.one;
            playerBase.transform.localPosition = Vector3.zero;
            playerBase.layer = CharacterTriggerLayer;

            // Set up character trigger sphere collider
            var collider = playerBase.AddComponent<SphereCollider>();
            collider.center = Vector3.zero;
            collider.radius = 20;
            collider.enabled = true;
            collider.isTrigger = true;

            // Add player base type EffectArea
            var playerBaseEffect = playerBase.AddComponent<EffectArea>();
            playerBaseEffect.enabled = true;
            playerBaseEffect.m_type = EffectArea.Type.PlayerBase;
        }

        private static void ApplyDoorPatches(string name, GameObject gameObject)
        {
            // Missing animations
            // dungeon_queen_door
            // dvergrtown_secretdoor

            switch (name)
            {
                case "dvergrtown_slidingdoor":
                case "dvergrtown_secretdoor":
                    {
                        if (!gameObject.TryGetComponent(out Door door))
                        {
                            return;
                        }

                        door.m_canNotBeClosed = false;
                        door.m_checkGuardStone = true;

                        if (gameObject.TryGetComponent(out ZNetView nview))
                        {
                            var zdo = nview.GetZDO();
                            if (zdo == null) { return; }
                            zdo.Set("HasFields", true);
                            zdo.Set("HasFieldsDoor", true);
                            zdo.Set("Door.m_canNotBeClosed", false);
                            zdo.Set("Door.m_checkGuardStone", true);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private static void ApplyBedPatches(string name, GameObject gameObject)
        {
            switch (name)
            {
                case "goblin_bed":
                    AddBed(gameObject, new Vector3(0, 0.45f, 0));
                    break;

                case "dvergrprops_bed":
                    AddBed(gameObject, new Vector3(0, 0.45f, 0));
                    break;

                default:
                    break;
            }
        }

        private static void AddBed(GameObject gameObject, Vector3 spawnPosition)
        {
            var attachPoint = new GameObject("spawnpoint");
            attachPoint.transform.parent = gameObject.transform;
            attachPoint.transform.localPosition = spawnPosition;
            attachPoint.layer = PieceLayer;

            var bed = gameObject.AddComponent<Bed>();
            bed.m_spawnPoint = attachPoint.transform;

            // TODO: can I add game objects via ZDO or just components?
        }

        /// <summary>
        ///     Applies patches to selected pieces so they
        ///     function as a fermenter
        /// </summary>
        /// <param name="prefab"></param>
        private static void ApplyFermenterPatches(string name, GameObject gameObject)
        {
            switch (name)
            {
                case "dvergrprops_barrel":

                    // Get child object clones
                    var FermenterPrefab = ZNetScene.instance?.GetPrefab("fermenter");
                    var add_button = FermenterPrefab.transform.Find("add_button").gameObject.DeepCopy();
                    var tap_button = FermenterPrefab.transform.Find("tap_button").gameObject.DeepCopy();
                    var roofcheckpoint = FermenterPrefab.transform.Find("roofcheckpoint").gameObject.DeepCopy();
                    var output = FermenterPrefab.transform.Find("output").gameObject.DeepCopy();
                    var _ready = FermenterPrefab.transform.Find("_ready").gameObject.DeepCopy();
                    var _fermenting = FermenterPrefab.transform.Find("_fermenting").gameObject.DeepCopy();

                    var VanillaFermenter = FermenterPrefab.GetComponent<Fermenter>();

                    // Assign to dvergrprops barrel
                    add_button.transform.parent = gameObject.transform;
                    tap_button.transform.parent = gameObject.transform;
                    roofcheckpoint.transform.parent = gameObject.transform;
                    output.transform.parent = gameObject.transform;
                    _ready.transform.parent = gameObject.transform;
                    _fermenting.transform.parent = gameObject.transform;

                    // Adjust locations
                    add_button.transform.localScale = Vector3.one;
                    add_button.transform.localPosition = new Vector3(0f, 0.75f, 0f);

                    tap_button.transform.localPosition = new Vector3(0f, 0.5f, 0.9f);
                    output.transform.localPosition = new Vector3(0f, 0.5f, 1.2f);
                    roofcheckpoint.transform.localPosition = new Vector3(0f, 1.5f, 0f);
                    _ready.transform.localPosition = new Vector3(0f, 0.75f, 0f);
                    _fermenting.transform.localPosition = new Vector3(0f, 0.75f, 0f);

                    // make fake top
                    var _top = new GameObject("_top");
                    _top.transform.parent = gameObject.transform;

                    // Set up fermenter component
                    var activeState = gameObject.activeSelf;
                    gameObject.SetActive(false);
                    var fermenter = gameObject.AddComponent<Fermenter>();

                    fermenter.m_addSwitch = add_button.GetComponent<Switch>();
                    fermenter.m_tapSwitch = tap_button.GetComponent<Switch>();
                    fermenter.m_roofCheckPoint = roofcheckpoint.transform;
                    fermenter.m_topObject = _top;
                    fermenter.m_readyObject = _ready;
                    fermenter.m_fermentingObject = _fermenting;
                    fermenter.m_outputPoint = output.transform;

                    // Copy values from vanilla fermenter
                    fermenter.m_tapDelay = VanillaFermenter.m_tapDelay;
                    fermenter.m_updateCoverTimer = VanillaFermenter.m_updateCoverTimer;
                    fermenter.m_fermentationDuration = VanillaFermenter.m_fermentationDuration * 0.7f;
                    fermenter.m_name = VanillaFermenter.m_name;

                    // fix effects
                    fermenter.m_addedEffects = VanillaFermenter.m_addedEffects;
                    fermenter.m_tapEffects = VanillaFermenter.m_tapEffects;
                    fermenter.m_spawnEffects = VanillaFermenter.m_spawnEffects;
                    fermenter.m_conversion = VanillaFermenter.m_conversion;

                    gameObject.SetActive(activeState);

                    AddPlayerBase(gameObject);
                    break;

                default:
                    break;
            }
        }
    }
}