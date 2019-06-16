using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;
using System;
using System.Reflection;

public class SettingsInputField : MonoBehaviour
{
    private ServerController serverController;
    public InputField inputField;

    public string entry;
    private string path;
    private JSONObject json;

    void Start()
    {
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
                string data = json.GetField(entry).str;
                inputField.text = data;
            }
            
        }

        UpdateField();

    }

    public void UpdateField()
    {
        string data = inputField.text;

        json.SetField(entry, data);

        File.WriteAllText(path, json.ToString());

        serverController.UpdateFields(json);
    }
}
