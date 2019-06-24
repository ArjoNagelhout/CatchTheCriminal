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
    private UIScreenManager uiScreenManager;

    [Header("UI")]
    public Text playertypeText;
    public GameObject gameStarted;
    public Image playertypeColor;

    public Text timeuntilText;

    public Material copMaterial;
    public Material criminalMaterial;

    [Header("Game")]
    public GameObject gamePrefab;
    public GameObject copTargetPositionPrefab;
    public GameObject criminalTargetPositionPrefab;
    public GameObject playerPrefab;

    public UIScreenManager copTutorialScreen;
    public UIScreenManager criminalTutorialScreen;

    public GameObject game;
    private AbstractMap _map;

    private bool hasStarted;
    private List<GameObject> targetPositionInstances = new List<GameObject>();

    private List<GameObject> playerInstances = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        serverController = FindObjectOfType<ServerController>();
        uiScreenManager = gameObject.GetComponent<UIScreenManager>();

        serverController.updateGameData.AddListener(UpdateGame);

        string playertypeString = "";
        string timeuntilString = "";
        if (serverController.playertype == Playertype.Cop)
        {
            playertypeString = "Cop";
            timeuntilString = "Time until criminal gets away:";
            playertypeColor.material = copMaterial;
            
        }
        else if (serverController.playertype == Playertype.Criminal)
        {
            playertypeString = "Criminal";
            timeuntilString = "Time until escape vehicle comes:";
            playertypeColor.material = criminalMaterial;

        }
        playertypeText.text = playertypeString;
        timeuntilText.text = timeuntilString;

        PresentHelp();

        game = Instantiate(gamePrefab);
        _map = game.GetComponentInChildren<AbstractMap>();

        game.GetComponentInChildren<PlayfieldRenderer>().UpdateInformation(serverController.game.playfield.points);
    }

    public void PresentHelp()
    {
        if (serverController.playertype == Playertype.Cop)
        {
            uiScreenManager.SendPresentBottomOverlay(copTutorialScreen);
        }
        else if (serverController.playertype == Playertype.Criminal)
        {
            uiScreenManager.SendPresentBottomOverlay(criminalTutorialScreen);
        }
    }

    // Update is called once per frame
    private void UpdateGame()
    {
        

        if (serverController.game.started)
        {
            if (!hasStarted)
            {
                gameStarted.SetActive(false);
                serverController.uiManager.ShowPopup("The game has started", serverController.uiManager.popupDuration);
                hasStarted = true;

                foreach (GameObject targetPositionInstance in targetPositionInstances)
                {
                    Destroy(targetPositionInstance);
                }
            }
        }

        

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
                GameObject otherPlayerInstance = playerInstances[i];
                OtherPlayerScript otherPlayerInstanceScript = otherPlayerInstance.GetComponent<OtherPlayerScript>();
                otherPlayerInstanceScript.UpdateInformation(player.position);
            }
        }

        if (targetPositionInstances.Count == 0)
        {
            SpawnTargetPosition(copTargetPositionPrefab, serverController.game.playfield.copTargetPosition);
            SpawnTargetPosition(criminalTargetPositionPrefab, serverController.game.playfield.criminalTargetPosition);
        }
    }

    private void CreatePlayerList()
    {
        foreach (Player player in serverController.game.players)
        {
            GameObject newPlayer = Instantiate(playerPrefab, game.transform);
            OtherPlayerScript otherPlayerScript = newPlayer.GetComponent<OtherPlayerScript>();

            otherPlayerScript.playerName = player.name;
            otherPlayerScript.ip = player.ip;
            otherPlayerScript.isHost = player.isHost;
            otherPlayerScript.playertype = player.playertype;
            if (serverController.playerName == player.name && serverController.playerIp == player.ip)
            {
                otherPlayerScript.isPlayer = true;
            }

            playerInstances.Add(newPlayer);
        }
    }

    private void SpawnTargetPosition(GameObject prefab, Coordinate coordinate)
    {
        GameObject newTargetPosition = Instantiate(prefab, game.transform);

        TargetPosition newTargetPositionScript = newTargetPosition.GetComponent<TargetPosition>();
        newTargetPositionScript.UpdateInformation(coordinate);
        targetPositionInstances.Add(newTargetPosition);
    }
}
