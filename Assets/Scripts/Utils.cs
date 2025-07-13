using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Utils
{
    public static Vector3 GetRandomDir()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public static bool IsPointOutsideCamera(Vector3 point)
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(point);

        if (viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0f || viewportPos.y > 1f)
            return true;

        return false;
    }

    public static IEnumerator GetRandomPointOnNavMesh(Vector3 center, float radiusMax, System.Action<Vector3> onComplete, NavMeshAgent agent = null, float radiusMin = 0f, bool searchOutsideCamera = false)
    {
        int maxAttempts = 50;

        NavMeshHit hit;

        int areaMask = 1;

        if (agent != null)
        {
            NavMeshQueryFilter filter = new NavMeshQueryFilter()
            {
                agentTypeID = agent.agentTypeID,
                areaMask = agent.areaMask
            };
        }
        else
            areaMask = 1 << NavMesh.GetAreaFromName("Walkable");

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * Random.Range(radiusMin, radiusMax);
            randomDirection += center;

            if (NavMesh.SamplePosition(randomDirection, out hit, radiusMax, areaMask))
            {
                if (!searchOutsideCamera || IsPointOutsideCamera(hit.position))
                {
                    onComplete?.Invoke(hit.position);
                    yield break;
                }
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
