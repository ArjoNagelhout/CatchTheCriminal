using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NameInputField : MonoBehaviour
{
    public UIScreenManager uiScreenManager;

    public InputField inputField;

    private string path;

    void Start()
    {
        path = Application.persistentDataPath + "/settings.json";
        if (File.Exists(path))
        {
            string playerName = File.ReadAllText(path);
            inputField.text = playerName;

            uiScreenManager.SetName(playerName);
        }

    }

    public void SetName()
    {
        // Write to file

        string content = inputField.text;
        File.WriteAllText(path, content);

        uiScreenManager.SetName(inputField.text);
    }
}
