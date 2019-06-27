using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class PlayfieldRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private AbstractMap _map;
    private List<Coordinate> targetCoordinates;

    void Awake()
    {
        _map = FindObjectOfType<AbstractMap>();
    }

    public void UpdateInformation(List<Coordinate> coordinates)
    {
        targetCoordinates = coordinates;
        _map.OnInitialized += _map_OnInitialized;
    }

    private void _map_OnInitialized()
    {
        int length = targetCoordinates.Count;
        lineRenderer.positionCount = length;
        for (int i=0; i<length; i++)
        {
            Coordinate coordinate = targetCoordinates[i];

            Vector2d vector2d = new Vector2d(coordinate.latitude, coordinate.longitude);
            
            lineRenderer.SetPosition(i, MoveTowardsCamera(_map.GeoToWorldPosition(vector2d)));
        }
        
    }

    Vector3 MoveTowardsCamera(Vector3 inputVector)
    {
        Vector3 outputVector = new Vector3(inputVector.x, inputVector.y + 10, inputVector.z);
        return outputVector;
    }
}
