using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;

public class OtherPlayerScript : MonoBehaviour
{
    private AbstractMap _map;

    public float resizeSpeed;

    public float movementSpeed;

    [System.NonSerialized]
    public string ip;
    [System.NonSerialized]
    public string playerName;
    [System.NonSerialized]
    public bool isHost;
    [System.NonSerialized]
    public bool isPlayer;
    [System.NonSerialized]
    public Playertype playertype;

    public Material copMaterial;
    public Material criminalMaterial;

    public MeshRenderer meshRenderer;

    public TextMeshPro nameText;

    private Vector2 targetPosition;

    public float maxSize;
    private float currentSize;

    public Transform locationUpdateSphere;


    // Start is called before the first frame update
    void Start()
    {
        _map = FindObjectOfType<AbstractMap>();


        nameText.text = playerName;

        if (isPlayer)
        {
            //nameText.text += " (you)";
            gameObject.SetActive(false);
        }

        if (playertype == Playertype.Cop)
        {
            meshRenderer.material = copMaterial;
        }
        else if (playertype == Playertype.Criminal)
        {
            meshRenderer.material = criminalMaterial;
        }

        currentSize = maxSize;
    }

    private void Update()
    {
        Vector2 oldPosition = new Vector2(transform.position.x, transform.position.z);
        Vector2 newPosition = Vector2.Lerp(oldPosition, targetPosition, movementSpeed);
        transform.position = new Vector3(newPosition.x, 0, newPosition.y);

        if (currentSize > 0)
        {
            currentSize = Mathf.Lerp(currentSize, 0, resizeSpeed);
            locationUpdateSphere.localScale = new Vector3(currentSize, currentSize, currentSize);
        }
    }

    public void UpdateInformation(Coordinate coordinate)
    {
        //targetPosition = position;
        currentSize = maxSize;

        Vector2d vector2d = new Vector2d(coordinate.latitude, coordinate.longitude);
        targetPosition = _map.GeoToWorldPosition(vector2d);
    }
}
