using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        public string name;
        public GameObject itemPrefab;
        public float dropRate;
    }

    public List<Drops> drops;

    private Vector3 cachedPosition;

    private void Update()
    {
        cachedPosition = transform.position;
    }

    public void SpawnDrop()
    {
        float randomNumber = Random.Range(0f, 100f);
        List<Drops> possibleDrops = new List<Drops>();

        foreach (Drops rate in drops)
        {
            if (randomNumber <= rate.dropRate)
                possibleDrops.Add(rate);
        }

        if (possibleDrops.Count > 0)
        {
            Drops drop = possibleDrops[Random.Range(0, possibleDrops.Count)];
            Vector3 spawnPos = GetDropPosition(); // includes small upward offset
            Instantiate(drop.itemPrefab, spawnPos, Quaternion.identity);
        }
    }

    Vector3 GetDropPosition()
    {
        // Try renderer first 
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            return rend.bounds.center + Vector3.up * (rend.bounds.extents.y + 0.2f);
        }

        // Fallback to collider
        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
        {
            return col.bounds.max + Vector3.up * 0.2f;
        }

        // Absolute fallback
        return transform.position + Vector3.up * 1f;
    }
}