using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class SetTimeSliderManager : MonoBehaviour
{
    public UICircleSlider uiCircleSlider;
    public Text timeText;

    public int minTime; // in minutes
    public int maxTime; // in minutes
    public int currentTime; // in minutes

    private void Awake()
    {
        uiCircleSlider.onValueChange.AddListener(UpdateTime);

        uiCircleSlider.minValue = minTime;
        uiCircleSlider.maxValue = maxTime;
        uiCircleSlider.currentValue = currentTime;
    }

    // Update is called once per frame
    private void UpdateTime()
    {
        currentTime = (int)RoundValue(uiCircleSlider.currentValue, 10);
        timeText.text = currentTime.ToString();
    }

    float RoundValue(float value, float multiplesOf)
    {
        value = value - (value % multiplesOf);

        return value;
    }
}
