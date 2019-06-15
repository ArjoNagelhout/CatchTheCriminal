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
        path = Application.dataPath + "/playername.txt";
        if (File.Exists(path))
        {
            inputField.text = File.ReadAllText(path);
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
