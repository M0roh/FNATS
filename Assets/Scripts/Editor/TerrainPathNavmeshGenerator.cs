#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;

namespace VoidspireStudio.Editor
{
    public class TerrainPathNavmeshGenerator : EditorWindow
    {
        Terrain terrain;
        TerrainLayer targetLayer;
        float sampleSpacing = 1f;
        float volumeSize = 2f;
        int navArea = 1; // Default Walkable

        [MenuItem("Tools/Auto Generate Path Volumes")]
        public static void ShowWindow()
        {
            GetWindow(typeof(TerrainPathNavmeshGenerator));
        }

        void OnGUI()
        {
            GUILayout.Label("Path Volume Generator", EditorStyles.boldLabel);
            terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true);
            targetLayer = (TerrainLayer)EditorGUILayout.ObjectField("Target Terrain Layer", targetLayer, typeof(TerrainLayer), false);
            sampleSpacing = EditorGUILayout.FloatField("Sample Spacing", sampleSpacing);
            volumeSize = EditorGUILayout.FloatField("Volume Size", volumeSize);
            navArea = EditorGUILayout.IntField("NavMesh Area (Index)", navArea);

            if (GUILayout.Button("Generate Volumes"))
            {
                if (terrain == null || targetLayer == null)
                {
                    Debug.LogError("Terrain и Target Layer об€зательны!");
                    return;
                }
                GenerateVolumes();
            }
        }

        void GenerateVolumes()
        {
            TerrainData data = terrain.terrainData;
            int layerIndex = -1;

            for (int i = 0; i < data.terrainLayers.Length; i++)
            {
                if (data.terrainLayers[i] == targetLayer)
                {
                    layerIndex = i;
                    break;
                }
            }

            if (layerIndex == -1)
            {
                Debug.LogError("”казанный TerrainLayer не найден на террейне.");
                return;
            }

            float[,,] maps = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);
            float width = data.size.x;
            float height = data.size.z;

            float scaleX = (float)data.alphamapWidth / width;
            float scaleZ = (float)data.alphamapHeight / height;

            int created = 0;

            GameObject parent = new GameObject("GeneratedNavMeshVolumes");

            for (float x = 0; x < width; x += sampleSpacing)
            {
                for (float z = 0; z < height; z += sampleSpacing)
                {
                    int mapX = Mathf.FloorToInt(x * scaleX);
                    int mapZ = Mathf.FloorToInt(z * scaleZ);

                    if (mapX < 0 || mapZ < 0 || mapX >= data.alphamapWidth || mapZ >= data.alphamapHeight)
                        continue;

                    float blend = maps[mapZ, mapX, layerIndex];
                    if (blend > 0.5f)
                    {
                        Vector3 pos = terrain.transform.position + new Vector3(x, 0, z);
                        pos.y = terrain.SampleHeight(pos) + terrain.transform.position.y;

                        GameObject volumeObj = new GameObject($"Volume_{x}_{z}");
                        volumeObj.transform.position = pos;
                        volumeObj.transform.parent = parent.transform;

                        var vol = volumeObj.AddComponent<NavMeshModifierVolume>();
                        vol.center = Vector3.zero;
                        vol.size = new Vector3(volumeSize, 5f, volumeSize);
                        vol.area = navArea;
                    }
                }
            }

            Debug.Log("—оздание NavMeshModifierVolume завершено.");
        }
    }
}
#endif