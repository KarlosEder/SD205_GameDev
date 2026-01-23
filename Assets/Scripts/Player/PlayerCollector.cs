using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        // Check GO for ICollectible interface
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            // If yes - collect
            collectible.collect();
        }
    }
}
