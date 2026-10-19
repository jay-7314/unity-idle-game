using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterTapEffect : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform target;
    [SerializeField] private float pressedScale = 0.85f;
    [SerializeField] private float returnSpeed = 12f;

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = target.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        target.localScale = baseScale * pressedScale;
    }

    private void Update()
    {
        target.localScale = Vector3.Lerp(target.localScale, baseScale, returnSpeed * Time.deltaTime);
    }
}