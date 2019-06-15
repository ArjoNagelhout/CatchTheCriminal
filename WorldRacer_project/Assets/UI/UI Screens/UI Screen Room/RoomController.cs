using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomController : MonoBehaviour
{
    private ServerController serverController;

    public Text roomText;

    public Text timeText;


    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();

        roomText.text = string.Format("Room #{0}", serverController.roomPin);

        timeText.text = string.Format("Time: {0} minutes", serverController.game.time);
    }
}
