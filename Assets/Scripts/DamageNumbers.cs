using UnityEngine;
using TMPro;
using System;

public class DamageNumbers : MonoBehaviour
{
    // Duration
    public float floatSpeed = 30f; 
    public float fadeTime = 1f;

    // Offset
    public Vector3 randomOffsetRange = new Vector3(0.5f, 0.5f, 0f); 
    private Vector3 initialOffset; 

    private TextMeshProUGUI text;
    private Transform target;
    private float elapsed = 0f;

    // Destroy
    public Action OnDestroyed;

    public void Detach()
    {
        target = null;
    }

    public void Initialize(Transform target, string str, Vector3 spawnOffset)
    {
        this.target = target;
        text = GetComponent<TextMeshProUGUI>();
        text.text = str;

        // Add spawn offset
        initialOffset = spawnOffset;
        initialOffset.x += UnityEngine.Random.Range(-randomOffsetRange.x, randomOffsetRange.x);
        initialOffset.y += UnityEngine.Random.Range(0, randomOffsetRange.y);
    }

    void Update()
    {
        if (text == null) return;

        if (target != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + initialOffset);
            screenPos.y += floatSpeed * Time.deltaTime;
            transform.position = screenPos;
        }
        else
        {
            // Continue floating upwards if target is gone
            transform.position += new Vector3(0, floatSpeed * Time.deltaTime, 0);
        }

        // Fade out
        elapsed += Time.deltaTime;
        text.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);

        if (elapsed >= fadeTime)
        {
            OnDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
