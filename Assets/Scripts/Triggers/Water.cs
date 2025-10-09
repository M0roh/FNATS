using UnityEngine;

namespace VoidspireStudio.FNATS.Triggers
{
    public class Water : MonoBehaviour
    {
        [Header("Water colliders")]
        [SerializeField] private Collider _surfaceCollider;
        [SerializeField] private Collider _underwaterCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (other == null) return;

            if (other.bounds.Intersects(_surfaceCollider.bounds))
            {
                Debug.Log("Игрок на поверхности воды");
            }

            if (other.bounds.Intersects(_underwaterCollider.bounds))
            {
                Player.Player.Instance.IsUnderWater = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (other.bounds.Intersects(_surfaceCollider.bounds) == false)
            {
                Debug.Log("Игрок вышел из воды");
            }

            if (other.bounds.Intersects(_underwaterCollider.bounds) == false)
            {
                Player.Player.Instance.IsUnderWater = false;
            }
        }
    }
}
