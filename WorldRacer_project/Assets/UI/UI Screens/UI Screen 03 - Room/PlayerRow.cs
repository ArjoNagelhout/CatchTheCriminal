using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour
{
    [System.NonSerialized]
    public bool canKick;

    [System.NonSerialized]
    public string ip;
    [System.NonSerialized]
	public string playerName;
    [System.NonSerialized]
    public bool isHost;
    [System.NonSerialized]
    public bool isPlayer;

	public GameObject kickButton;
    public Text nameText;

    private ServerController serverController;

    // Start is called before the first frame update
    void Start()
    {
        if (!canKick)
		{
            kickButton.SetActive(false);
		}
        
        nameText.text = playerName;
        if (isHost)
        {
            nameText.text += " (host)";
        }

        if (isPlayer)
        {
            nameText.text += " (you)";
        }

        serverController = FindObjectOfType<ServerController>();
    }

    public void KickPlayer()
    {
        serverController.KickPlayer(ip, playerName);
    }
}
