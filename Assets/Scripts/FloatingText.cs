using System;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private float lifeTime = 0.8f;
    [SerializeField] private float riseSpeed = 200f;

    private RectTransform rect;
    private Action<FloatingText> release;
    private float elapsed;

    private void Awake()
    {
        rect = (RectTransform)transform;
    }

    public void Play(string text, Vector2 startPosition, Action<FloatingText> releaseCallback)
    {
        label.text = text;
        rect.anchoredPosition = startPosition;
        release = releaseCallback;
        elapsed = 0f;
        SetAlpha(1f);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        rect.anchoredPosition += Vector2.up * (riseSpeed * Time.deltaTime);
        SetAlpha(1f - elapsed / lifeTime);

        if (elapsed >= lifeTime)
        {
            Action<FloatingText> callback = release;
            release = null;
            callback?.Invoke(this);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = label.color;
        c.a = Mathf.Clamp01(alpha);
        label.color = c;
    }
}