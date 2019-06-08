using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerController : MonoBehaviour
{
    public string serverAddress;

    private string uri;

    public string json_data;


    void Start()
    {
        //StartCoroutine(Upload());
        string prefix = "http://";

        if (serverAddress.StartsWith(prefix, System.StringComparison.Ordinal)) {
            uri = serverAddress;
        }
        else
        {
            uri = string.Format("http://{0}", serverAddress);
        }




        StartCoroutine(Communicate());
    }

    IEnumerator Communicate()
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json_data);

        using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, bytes))
        {
            webRequest.method = "POST";
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log("Success");
            }
        }
    }
}
