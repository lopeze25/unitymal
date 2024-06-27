//Allows for a button to expand when hovered. This is helpful for small buttons in VR.
//Created by Ezekiel Lopez, 26 June 2024
//Modified by James Vanderhyde, 26 June 2024
//  Added shrinkOnClick option

using UnityEngine;
using UnityEngine.EventSystems;

public class VRButtonExpand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Tooltip("How much bigger the button gets")]
    public float expandFactor = 500f;
    [Tooltip("Speed of the expansion")]
    public float speed = 20f; 
    [Tooltip("Whether the button should change after a click")]
    public bool shrinkOnClick = true;

    private bool isHovered = false;
    private RectTransform rectTransform;
    private Vector2 originalSize;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (this.shrinkOnClick)
            isHovered = false;
    }

}
