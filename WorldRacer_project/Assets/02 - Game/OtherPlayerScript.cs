using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OtherPlayerScript : MonoBehaviour
{
    public float movementSpeed;

    [System.NonSerialized]
    public string ip;
    [System.NonSerialized]
    public string playerName;
    [System.NonSerialized]
    public bool isHost;
    [System.NonSerialized]
    public bool isPlayer;

    public TextMeshPro nameText;

    private Vector2 targetPosition;

    public float maxSize;
    private float currentSize;

    public Transform locationUpdateSphere;


    // Start is called before the first frame update
    void Start()
    {
        nameText.text = playerName;

        if (isPlayer)
        {
            //nameText.text += " (you)";
            gameObject.SetActive(false);
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
            currentSize = Mathf.Lerp(currentSize, 0, movementSpeed);
            locationUpdateSphere.localScale = new Vector3(currentSize, currentSize, currentSize);
        }
    }

    public void UpdateInformation(Coordinate position)
    {
        //targetPosition = position;
        currentSize = maxSize;
    }
}
