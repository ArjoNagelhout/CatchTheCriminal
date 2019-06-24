using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Mapbox.Examples;

public class MapEditorManager : MonoBehaviour
{
    public AbstractMap _map;

    [SerializeField]
    GameObject pointPrefab;

    [System.NonSerialized]
    public List<GameObject> points = new List<GameObject>();

    public int pointAmount;
    public float size;

    public List<Coordinate> coordinates = new List<Coordinate>();
    public bool canUpdateCoordinates = true;

    public QuadTreeCameraMovement cameraMovement;
    public CurrentLocation currentLocation;

    public LineRenderer lineRenderer;

    private ServerController serverController;

    private void Start()
    {
        
        serverController = FindObjectOfType<ServerController>();

        if (serverController.editingPlayfield.points.Count == 0)
        {
            currentLocation.onLoad.AddListener(CreatePoints);
        } else
        {
            currentLocation.onLoad.AddListener(LoadPoints);
        }
        
    }

    void CreatePoints()
    {

        // Create points around current location
        Vector3 center = transform.position;

        lineRenderer.positionCount = pointAmount;
        for (int i = 0; i < pointAmount; i++)
        {

            int rotation = i * (360 / pointAmount);
            Vector3 position = OnCircle(center, size, rotation);
            GameObject newPoint = Instantiate(pointPrefab, gameObject.transform, true);
            newPoint.transform.localPosition = position;
            Vector2d vector2d = _map.WorldToGeoPosition(position);
            Coordinate coordinate = new Coordinate(vector2d.x, vector2d.y);
            coordinates.Add(coordinate);
            points.Add(newPoint);
            lineRenderer.SetPosition(i, MoveTowardsCamera(newPoint.transform.localPosition));
        }
    }

    void LoadPoints()
    {
        coordinates = serverController.editingPlayfield.points;
        

        pointAmount = coordinates.Count;
        lineRenderer.positionCount = pointAmount;
        
        for (int i = 0; i < pointAmount; i++)
        {
            
            GameObject newPoint = Instantiate(pointPrefab, gameObject.transform, true);

            Coordinate coordinate = coordinates[i];
            Vector2d vector2d = new Vector2d(coordinate.latitude, coordinate.longitude);
            newPoint.transform.localPosition = _map.GeoToWorldPosition(vector2d);

            points.Add(newPoint);
            lineRenderer.SetPosition(i, MoveTowardsCamera(newPoint.transform.localPosition));
        }
    }

    private void OnDestroy()
    {
        serverController.editingPlayfield.points = coordinates;
    }


    private void Update()
    {
        if (canUpdateCoordinates)
        {
            for (int i = 0; i < points.Count; i++)
            {
                GameObject point = points[i];
                Coordinate coordinate = coordinates[i];
                Vector2d vector2d = new Vector2d(coordinate.latitude, coordinate.longitude);
                point.transform.localPosition = _map.GeoToWorldPosition(vector2d);
                lineRenderer.SetPosition(i, MoveTowardsCamera(point.transform.localPosition));
            }
        }
    }

    public void MovedPoints()
    {
        for (int i=0; i < points.Count; i++)
        {
            GameObject point = points[i];
            Vector2d vector2d = _map.WorldToGeoPosition(point.transform.localPosition);
            Coordinate coordinate = new Coordinate(vector2d.x, vector2d.y);
            coordinates[i] = coordinate;
            lineRenderer.SetPosition(i, MoveTowardsCamera(point.transform.localPosition));
        }
    }

    Vector3 OnCircle(Vector3 center, float radius, int rotation)
    {
        Vector3 position;
        position.x = center.x + radius * Mathf.Sin(rotation * Mathf.Deg2Rad);
        position.z = center.z + radius * Mathf.Cos(rotation * Mathf.Deg2Rad);
        position.y = center.y;
        return position;
    }

    Vector3 MoveTowardsCamera(Vector3 inputVector)
    {
        Vector3 outputVector = new Vector3(inputVector.x, inputVector.y + 1, inputVector.z);
        return outputVector;
    }
}
