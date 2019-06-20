using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;

public class GameController : MonoBehaviour
{
    private ServerController serverController;

    [Header("UI")]
    public Text playertypeText;
    public Text gameStartedText;

    [Header("Game")]
    public GameObject gamePrefab;
    public GameObject copTargetPositionPrefab;
    public GameObject criminalTargetPositionPrefab;
    public GameObject playerPrefab;

    private GameObject game;
    private AbstractMap abstractMap;

    private List<GameObject> playerInstances = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        serverController = FindObjectOfType<ServerController>();

        serverController.updateGameData.AddListener(UpdateGame);

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

        game = Instantiate(gamePrefab);
    }

    // Update is called once per frame
    private void UpdateGame()
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

        if (playerInstances.Count == 0)
        {
            CreatePlayerList();
        }
        else
        {
            List<Player> playerList = serverController.game.players;
            for (int i=0; i<playerList.Count; i++)
            {
                Player player = playerList[i];
                GameObject playerInstance = playerInstances[i];
                PlayerScript playerInstanceScript = playerInstance.GetComponent<PlayerScript>();
                playerInstanceScript.UpdateInformation(player.position);
            }
        }
        Debug.Log(playerInstances.Count);
    }

    private void CreatePlayerList()
    {
        foreach (Player player in serverController.game.players)
        {
            GameObject newPlayer = Instantiate(playerPrefab, game.transform);
            PlayerScript playerScript = newPlayer.GetComponent<PlayerScript>();

            playerScript.playerName = player.name;
            playerScript.ip = player.ip;
            playerScript.isHost = player.isHost;
            if (serverController.playerName == player.name && serverController.playerIp == player.ip)
            {
                playerScript.isPlayer = true;
            }

            playerInstances.Add(newPlayer);
        }
    }
}
