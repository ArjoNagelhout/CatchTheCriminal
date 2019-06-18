using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class RoomController : MonoBehaviour
{
    private ServerController serverController;

    public Text roomText;

    public Text timeText;

    public GameObject playerList;
    public GameObject playerRow;

    public GameObject startGameObject;

    public RectTransform playerListTransform;

    private float height;

    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();

        roomText.text = string.Format("Room #{0}", serverController.roomPin);
        timeText.text = string.Format("Time: {0} minutes", serverController.game.time);

        if (serverController.isHost)
        {
            startGameObject.SetActive(true);
            playerListTransform.offsetMin = new Vector2(playerListTransform.offsetMin.x, 60);
        }
        else
        {
            startGameObject.SetActive(false);
            playerListTransform.offsetMin = new Vector2(playerListTransform.offsetMin.x, 10);
        }

        RectTransform playerRowRectTransform = playerRow.GetComponent<RectTransform>();
        height = playerRowRectTransform.rect.height;

        UpdatePlayerList();

        serverController.updateRoomData.AddListener(UpdatePlayerList);
    }

    void UpdatePlayerList()
    {

		// Remove existing rows

		PlayerRow[] oldPlayerRows = FindObjectsOfType<PlayerRow>();

        foreach (PlayerRow oldPlayerRow in oldPlayerRows)
        {
            Destroy(oldPlayerRow.gameObject);
        }

		int i = 0;
        // Create list of players
        foreach (Player player in serverController.game.players)
        {
            GameObject playerRowInstance = Instantiate(playerRow, playerList.transform, false);

            // Put rows under eachother
            RectTransform playerRowRectTransform = playerRowInstance.GetComponent<RectTransform>();
            playerRowRectTransform.localPosition -= new Vector3(0, height*i, 0);

            PlayerRow playerRowComponent = playerRowInstance.GetComponent<PlayerRow>();
            playerRowComponent.playerName = player.name;
            playerRowComponent.ip = player.ip;
            playerRowComponent.isHost = player.isHost;
            if (serverController.playerName == player.name)
            {
                playerRowComponent.isPlayer = true;
            }

            if (serverController.isHost)
            {
                if (!player.isHost)
                {
                    playerRowComponent.canKick = true;
                }
            }

            i++;
        }
    }
}
