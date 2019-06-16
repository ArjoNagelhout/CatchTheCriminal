using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour
{
    [System.NonSerialized]
    public bool canKick;
    [System.NonSerialized]
	public string playerName;
    [System.NonSerialized]
    public string ip;

	public GameObject kickButton;
    public Text nameText;

    // Start is called before the first frame update
    void Start()
    {
        if (!canKick)
		{
            kickButton.SetActive(false);
		}

        nameText.text = playerName;
    }
}
