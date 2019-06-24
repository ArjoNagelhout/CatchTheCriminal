using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;

    private ServerController serverController;

    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (serverController.game != null)
        {
            if (serverController.game.started)
            {
                
                timerText.text = string.Format("{0}:{1}", TimeString(serverController.currentTime / 60), TimeString(serverController.currentTime % 60));
            }
        }
    }

    string TimeString(float time)
    {
        string inputString = ((int)time).ToString();

        string outputString;
        if (inputString.Length < 2)
        {
            outputString = "0" + inputString;
        } else
        {
            outputString = inputString;
        }
        return outputString;
    }
}
