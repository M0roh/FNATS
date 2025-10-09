using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace VoidspireStudio.FNATS.Utils
{
    public class Util
    {
        public static Vector3 GetRandomDir()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        public static async UniTask<Vector3> GetRandomPointOnNavMesh(Vector3 center, float radiusMax, float radiusMin = 0f, CancellationToken cancellationToken = default)
        {
            int maxAttempts = 50;

            NavMeshHit hit;

            int areaMask = 1;

            for (int i = 0; i < maxAttempts; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Vector3 randomDirection = Random.insideUnitSphere * Random.Range(radiusMin, radiusMax);
                randomDirection += center;

                if (NavMesh.SamplePosition(randomDirection, out hit, radiusMax, 1))
                {
                    return hit.position;
                }
                await UniTask.Yield(cancellationToken: cancellationToken).SuppressCancellationThrow();
            }

            if (NavMesh.SamplePosition(center, out hit, radiusMax, areaMask))
                return hit.position;

            throw new System.Exception("Failed to find a valid NavMesh point.");
        }

        public static async UniTask AdjustFOV(float targetFov, float duration, Camera camera, CancellationToken cancellationToken = default)
        {
            float startFov = camera.fieldOfView;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(startFov, targetFov, elapsedTime / duration);
                await UniTask.Yield(cancellationToken: cancellationToken).SuppressCancellationThrow();
            }

            camera.fieldOfView = targetFov;
        }

        public static string GetFloorMaterialName(Vector3 position)
        {
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit))
            {
                var mat = hit.collider.sharedMaterial;

                if (mat != null)
                    return mat.name;


                Terrain terrain = hit.collider.GetComponent<Terrain>();
                if (terrain != null)
                {
                    Vector3 terrainPos = position - terrain.transform.position;
                    TerrainData tData = terrain.terrainData;

                    int mapX = Mathf.FloorToInt(terrainPos.x / tData.size.x * tData.alphamapWidth);
                    int mapZ = Mathf.FloorToInt(terrainPos.z / tData.size.z * tData.alphamapHeight);

                    float[,,] splatmapData = tData.GetAlphamaps(mapX, mapZ, 1, 1);

                    int maxIndex = 0;
                    float maxMix = 0;
                    for (int i = 0; i < splatmapData.GetLength(2); i++)
                    {
                        if (splatmapData[0, 0, i] > maxMix)
                        {
                            maxIndex = i;
                            maxMix = splatmapData[0, 0, i];
                        }
                    }

                    return tData.terrainLayers[maxIndex].name;
                }
            }                

            return string.Empty;
        }
    }
}