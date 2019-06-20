using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMapScreenManager : MonoBehaviour
{
    public GameObject mapEditorManagerPrefab;

    private GameObject mapEditorManagerInstance;

    // Start is called before the first frame update
    private void Start()
    {
        mapEditorManagerInstance = Instantiate(mapEditorManagerPrefab);
    }

    private void OnDestroy()
    {
        Destroy(mapEditorManagerInstance);
    }

}
