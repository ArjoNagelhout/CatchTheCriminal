using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetServerController : MonoBehaviour
{
	private ServerController serverController;

    // Start is called before the first frame update
    void Start()
    {
        serverController = FindObjectOfType<ServerController>();
        serverController.editingPlayfield = new Playfield
        {
            points = new List<Coordinate>()
        };
    }
}
