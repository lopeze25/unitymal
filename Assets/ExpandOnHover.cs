using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VRButtonExpand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector2 originalSize;
    public float expandFactor = 500f; // How much bigger the button gets
    public float speed = 20f; // Speed of the expansion

    private bool isHovered = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        Vector2 targetSize = isHovered ? originalSize * expandFactor : originalSize;
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, targetSize, deltaTime * speed);
    }

    // When pointer hovers over the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    // When pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}