using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomQuitMessage : MonoBehaviour
{
    private ServerController serverController;
    public Text text;

    [TextArea]
    public string ifHost;
    [TextArea]
    public string ifNotHost;
	[TextArea]
	public string ifLastPlayer;

    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();

        if (serverController.game.players.Count == 1)
        {
            text.text = ifLastPlayer;
        } else
        {
            if (serverController.isHost == true)
            {
                text.text = ifHost;
            }
            else
            {
                text.text = ifNotHost;
            }
        }
		
    }
}
