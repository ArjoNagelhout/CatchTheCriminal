using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SetTimeSliderManager : MonoBehaviour
{
    public UICircleSlider uiCircleSlider;
    public Text timeText;

    public int minTime; // in minutes
    public int maxTime; // in minutes
    [System.NonSerialized]
    public int currentTime;

    private ServerController serverController;

    public string entry;
    private string path;
    private JSONObject json;

    void Start()
    {
        

    }

    public void UpdateField()
    {
        float data = uiCircleSlider.currentValue - uiCircleSlider.minValue;

        json.SetField(entry, data);

        File.WriteAllText(path, json.ToString());

        serverController.UpdateFields(json);
    }

    private void Awake()
    {
        uiCircleSlider.onValueChange.AddListener(UpdateTime);

        uiCircleSlider.minValue = minTime;
        uiCircleSlider.maxValue = maxTime;

        serverController = FindObjectOfType<ServerController>();

        json = new JSONObject();

        path = Application.persistentDataPath + "/settings.json";
        //File.Delete(path);
        if (File.Exists(path))
        {
            string fileContents = File.ReadAllText(path);
            json = new JSONObject(fileContents);

            if (json.HasField(entry))
            {
                float data = json.GetField(entry).f;
                uiCircleSlider.currentValue = data;
            }

        }

        UpdateField();
    }

    // Update is called once per frame
    private void UpdateTime()
    {
        currentTime = (int)RoundValue(uiCircleSlider.currentValue, 10);
        timeText.text = currentTime.ToString();

        UpdateField();
    }

    float RoundValue(float value, float multiplesOf)
    {
        value = value - (value % multiplesOf);

        return value;
    }
}
