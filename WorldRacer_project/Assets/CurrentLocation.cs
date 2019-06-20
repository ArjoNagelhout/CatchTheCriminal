using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using UnityEngine;
using UnityEngine.Events;

public class CurrentLocation : MonoBehaviour
{
    
    bool _isInitialized;

    public UnityEvent onLoad = new UnityEvent();

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
        LocationProviderFactory.Instance.mapManager.OnInitialized += () =>
        {
            _isInitialized = true;
            onLoad.Invoke();
        };
    }

    void LateUpdate()
    {
        if (_isInitialized)
        {
            var map = LocationProviderFactory.Instance.mapManager;
            transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
        }
    }
}