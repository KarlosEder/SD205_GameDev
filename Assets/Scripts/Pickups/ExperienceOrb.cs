using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public int experienceGained;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.IncreaseExperience(experienceGained);
            Destroy(gameObject);
        }
    }
}
