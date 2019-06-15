using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomTitle : MonoBehaviour
{
    private ServerController serverController;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();
        text.text = string.Format("Room #{0}", serverController.roomPin);
    }
}
