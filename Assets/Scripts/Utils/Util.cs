using System.Collections;
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

        public static IEnumerator GetRandomPointOnNavMesh(Vector3 center, float radiusMax, System.Action<Vector3> onComplete, float radiusMin = 0f)
        {
            int maxAttempts = 50;

            NavMeshHit hit;

            int areaMask = 1;

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * Random.Range(radiusMin, radiusMax);
                randomDirection += center;

                if (NavMesh.SamplePosition(randomDirection, out hit, radiusMax, 1))
                {
                    onComplete?.Invoke(hit.position);
                    yield break;
                }
                yield return null;
            }

            if (NavMesh.SamplePosition(center, out hit, radiusMax, areaMask))
                onComplete?.Invoke(hit.position);

            throw new System.Exception("Failed to find a valid NavMesh point.");
        }

        public static IEnumerator AdjustFOV(float targetFov, float duration, Camera camera)
        {
            float startFov = camera.fieldOfView;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                camera.fieldOfView = Mathf.Lerp(startFov, targetFov, elapsedTime / duration);
                yield return null;
            }

            camera.fieldOfView = targetFov;
        }
    }
}