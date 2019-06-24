using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrestedButton : MonoBehaviour
{
    private ServerController serverController;

    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();
        if (serverController.playertype != Playertype.Criminal)
        {
            gameObject.SetActive(false);
        }
    }

}
