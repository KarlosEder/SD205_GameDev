using UnityEngine;
using TMPro;
using System;

public class DamageNumbers : MonoBehaviour
{
    // Duration
    public float floatSpeed = 30f; 
    public float fadeTime = 1f;

    // Offset
    public Vector2 randomOffsetRange = new Vector2(0.5f, 0.5f);
    private Vector3 initialOffset; 

    private TextMeshProUGUI text;
    private Transform target;
    private float elapsed = 0f;

    // DOT
    private bool isBurning;

    // Destroy
    public Action OnDestroyed;

    public void Detach()
    {
        target = null;
    }

    public void Initialize(Transform target, string amountText, Vector3 spawnOffset, bool isBurning)
    {
        this.target = target;
        this.isBurning = isBurning;

        text = GetComponent<TextMeshProUGUI>();
        if (text == null )
        {
            Debug.LogError("DamageNumbers prefab is missing a component");
            return;
        }

        text.text = amountText;

        // Offset variation
        initialOffset = spawnOffset;
        initialOffset.x += UnityEngine.Random.Range(-randomOffsetRange.x, randomOffsetRange.x);
        initialOffset.y += UnityEngine.Random.Range(0, randomOffsetRange.y);

        text.color = isBurning ? new Color(1f, 0.4f, 0.1f) : Color.white;
    }

    void Update()
    {
        if (text == null) return;

        if (target != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + initialOffset);

            // Add floating effect
            screenPos.y += floatSpeed * Time.deltaTime;

            // Apply the position to the UI element
            transform.position = screenPos;
        }
        else
        {
            // Continue floating upwards if target is gone
            transform.position += new Vector3(0, floatSpeed * Time.deltaTime, 0);
        }

        // Fade out over time
        elapsed += Time.deltaTime;
        text.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);

        if (elapsed >= fadeTime)
        {
            OnDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
