using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    [System.NonSerialized]
    public Coordinate position;

    private ServerController serverController;

    public MeshRenderer meshRenderer;

    public Material copMaterial;
    public Material criminalMaterial;

    public TextMeshPro nameText;

    bool _isInitialized;

    ILocationProvider _locationProvider;
    ILocationProvider LocationProvider
    {
        get
        {
            if (_locationProvider == null)
            {
                _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            }

            return _locationProvider;
        }
    }

    Vector3 _targetPosition;

    void Start()
    {
        LocationProviderFactory.Instance.mapManager.OnInitialized += () => _isInitialized = true;

        serverController = FindObjectOfType<ServerController>();

        nameText.text = serverController.playerName + " (you)";

        if (serverController.playertype == Playertype.Cop)
        {
            meshRenderer.material = copMaterial;
        }
        else if (serverController.playertype == Playertype.Criminal)
        {
            meshRenderer.material = criminalMaterial;
        }
    }

    void LateUpdate()
    {
        if (_isInitialized)
        {
            var map = LocationProviderFactory.Instance.mapManager;
            Mapbox.Utils.Vector2d location = LocationProvider.CurrentLocation.LatitudeLongitude;
            position = new Coordinate(location.x, location.y);
            transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
        }
    }
}
