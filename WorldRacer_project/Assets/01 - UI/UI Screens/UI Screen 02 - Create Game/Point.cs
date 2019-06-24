using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    private MapEditorManager mapEditorManager;
    private Vector3 distance;

    // Start is called before the first frame update
    void Start()
    {
        mapEditorManager = FindObjectOfType<MapEditorManager>();
    }
    

    void OnMouseDown()
    {
        distance = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z)) - transform.position;
        mapEditorManager.cameraMovement.enabled = false;
        
        mapEditorManager.canUpdateCoordinates = false;
    }

    void OnMouseDrag()
    {
        Vector3 distance_to_screen = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 pos_move = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen.z));
        transform.position = new Vector3(pos_move.x - distance.x, transform.position.y, pos_move.z - distance.z);
        mapEditorManager.MovedPoints();
    }

    private void OnMouseUp()
    {
        mapEditorManager.cameraMovement.enabled = true;
        mapEditorManager.MovedPoints();
        mapEditorManager.canUpdateCoordinates = true;
    }
}
