using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICircleSlider : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public Image fillImage;
    public RectTransform handleRect;
    public RectTransform foregroundRect;

    public float minValue;
    public float maxValue;

    public float currentValue;

    private float bandSize;

    [System.NonSerialized]
    public float sliderRadius;

    private int previousQuadrant;

    public UnityEvent onValueChange = new UnityEvent();



    public void Awake()
    {
        bandSize = foregroundRect.offsetMin.x;

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        sliderRadius = rectTransform.rect.height / 2 - bandSize / 2;

        float initialAngle = (currentValue / (maxValue - minValue)) * 360;
        previousQuadrant = (int)initialAngle / 90;

        UpdateValue(new Vector2(transform.position.x, transform.position.y)+AngleToVector2(initialAngle));
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateValue(eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateValue(eventData.position);
    }

    void UpdateValue(Vector2 pointerPosition) {

        // Get angle in radians
        Vector2 sliderPosition = transform.position;

        float deltaY = pointerPosition.y - sliderPosition.y;
        float deltaX = pointerPosition.x - sliderPosition.x;

        float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;

        angle = (-angle + 450) % 360;

        int currentQuadrant = (int)angle / 90;
        if (Mathf.Abs(previousQuadrant - currentQuadrant) == 3) {
            if (previousQuadrant == 0)
            {
                angle = 0;
            }
            if (previousQuadrant == 3)
            {
                angle = 360;
            }

        }
        else
        {
            previousQuadrant = currentQuadrant;

        }

        float fillAmount = angle / 360;
        fillImage.fillAmount = fillAmount;

        handleRect.localPosition = AngleToVector2(angle) * sliderRadius;

        // Update currentValue
        currentValue = (maxValue - minValue) * (angle / 360) + minValue;

        onValueChange.Invoke();
    }

    Vector2 AngleToVector2(float angle)
    {
        float angleRad = (-angle + 90) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}
