using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject[] zombiePrefabs;
    public Transform player;
    public GameObject damageNumbersPrefab;
    public Canvas canvas;

    [Header("Spawn Settings")]
    public float minSpawnDistance = 20f;
    public float maxSpawnDistance = 40f;
    public float spawnHeight = 1f;

    [Header("Wave Settings")]
    public List<Wave> waves = new List<Wave>();
    public float timeBetweenWaves = 5f;

    [Header("UI/Debug")]
    public GameObject winScreen;
    public bool autoStartWaves = true;
    public TMPro.TextMeshProUGUI waveText;

    private int currentWave = 0;
    private int zombiesAlive = 0;
    private bool waveInProgress = false;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (autoStartWaves)
        {
            StartCoroutine(StartWaveSystem());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !waveInProgress)
        {
            StartCoroutine(SpawnWave());
        }

        if (waveText != null)
        {
            waveText.text = $"Wave: {currentWave} | Zombies: {zombiesAlive}";
        }
    }

    IEnumerator StartWaveSystem()
    {
        yield return new WaitForSeconds(3f);

        while (currentWave < waves.Count)
        {
            yield return StartCoroutine(SpawnWave());
            
            yield return new WaitUntil(() => zombiesAlive <= 0);

            Debug.Log($"Wave {currentWave} complete! Next wave in {timeBetweenWaves}s...");
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("All waves completed!");
        ShowWinScreen();
    }

    void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            Time.timeScale = 0f;


            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.LogWarning("Win screen not assigned!");
        }
    }

    IEnumerator SpawnWave()
    {
        if (currentWave >= waves.Count)
        {
            Debug.Log("No more waves!");
            yield break;
        }

        waveInProgress = true;
        Wave wave = waves[currentWave];

        Debug.Log($"Starting Wave {currentWave + 1}: {wave.zombieCount} zombies, {wave.damagePerZombie} damage each");

        for (int i = 0; i < wave.zombieCount; i++)
        {
            SpawnZombie(wave.damagePerZombie, wave.healthPerZombie);
            
            yield return new WaitForSeconds(wave.spawnDelay);
        }

        currentWave++;
        waveInProgress = false;
    }

    void SpawnZombie(float damage, float health)
    {
        Vector3 spawnPos = GetRandomSpawnPosition();

        if (spawnPos == Vector3.zero)
        {
            Debug.LogWarning("Could not find valid spawn position!");
            return;
        }

        // Pick a random zombie variant
        GameObject randomZombiePrefab = zombiePrefabs[Random.Range(0, zombiePrefabs.Length)];
        GameObject zombie = Instantiate(randomZombiePrefab, spawnPos, Quaternion.identity);

        // Make animator independent so zombies don't interfere with each other
        Animator zombieAnimator = zombie.GetComponent<Animator>();
        if (zombieAnimator != null)
        {
            zombieAnimator.runtimeAnimatorController = Instantiate(zombieAnimator.runtimeAnimatorController);
        }

        // Configure ZombieAI
        ZombieAI zombieAI = zombie.GetComponent<ZombieAI>();
        if (zombieAI != null)
        {
            zombieAI.attackDamage = damage;
            zombieAI.player = player;
        }

        // Configure Target
        Target zombieTarget = zombie.GetComponent<Target>();
        if (zombieTarget != null)
        {
            zombieTarget.health = health;
            
            if (damageNumbersPrefab != null)
                zombieTarget.damageNumbers = damageNumbersPrefab;
            
            if (canvas != null)
                zombieTarget.uiCanvas = canvas;
                
            if (player != null)
            {
                zombieTarget.player = player.GetComponent<Player>();
                
                Gun gun = player.GetComponentInChildren<Gun>();
                if (gun != null)
                    zombieTarget.gun = gun;
            }
        }

        zombiesAlive++;

        StartCoroutine(TrackZombieDeath(zombie));
    }

    IEnumerator TrackZombieDeath(GameObject zombie)
    {
        while (zombie != null)
        {
            yield return null;
        }

        zombiesAlive--;
    }

    Vector3 GetRandomSpawnPosition()
    {
        int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

            Vector3 randomDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
            Vector3 potentialSpawnPos = player.position + (randomDirection * randomDistance);
            potentialSpawnPos.y += spawnHeight;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(potentialSpawnPos, out hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("Failed to find valid spawn position after 30 attempts!");
        return Vector3.zero;
    }

    public void StartNextWave()
    {
        if (!waveInProgress)
        {
            StartCoroutine(SpawnWave());
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, minSpawnDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, maxSpawnDistance);
    }
}

[System.Serializable]
public class Wave
{
    public int zombieCount = 10;
    public float damagePerZombie = 5f;
    public float healthPerZombie = 100f;
    public float spawnDelay = 0.5f;
}