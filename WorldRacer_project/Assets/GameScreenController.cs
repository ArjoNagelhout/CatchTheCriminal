using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreenController : MonoBehaviour
{
    public Text playertypeText;

    private ServerController serverController;

    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();
        string playertypeString = "You are nobody";
        Debug.Log(serverController.playertype);
        if (serverController.playertype == Playertype.Cop)
        {
            playertypeString = "You are a cop";
        }
        else if (serverController.playertype == Playertype.Criminal)
        {
            playertypeString = "You are a crook";
        }
        playertypeText.text = playertypeString;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
