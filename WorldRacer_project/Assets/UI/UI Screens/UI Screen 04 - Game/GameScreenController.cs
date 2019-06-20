using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreenController : MonoBehaviour
{
    public Text playertypeText;

    private ServerController serverController;

    public Text gameStartedText;

    // Start is called before the first frame update
    private void Start()
    {
        serverController = FindObjectOfType<ServerController>();

        serverController.updateGameData.AddListener(UpdateGameScreen);

        string playertypeString = "";
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
    private void UpdateGameScreen()
    {
        string gameStartedString;
        if (serverController.game.started)
        {
            gameStartedString = "The game has started";
        }
        else
        {
            gameStartedString = "Please move to your start position";
        }

        gameStartedText.text = gameStartedString;
    }
}
