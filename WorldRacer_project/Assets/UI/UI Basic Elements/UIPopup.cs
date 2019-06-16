using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup : MonoBehaviour
{
    [System.NonSerialized]
    public string displayString;
    [System.NonSerialized]
    public int displayTime;

    public Text textComponent;

    void Start()
    {
        textComponent.text = displayString;
        StartCoroutine(DestroyAfter(displayTime));
    }

    IEnumerator DestroyAfter(int time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
