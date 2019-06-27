using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameQuitMessage : MonoBehaviour
{
    private ServerController serverController;
    public Text text;

    [TextArea]
    public string ifCriminal;
    [TextArea]
    public string ifCop;
	[TextArea]
	public string ifLastCop;

    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();

        if (serverController.playertype == Playertype.Criminal)
        {
            text.text = ifCriminal;
        } else
        {
            if (serverController.game.players.Count > 2)
            {
                text.text = ifCop;
            } else
            {
                text.text = ifLastCop;
            }
        }
		
    }
}
