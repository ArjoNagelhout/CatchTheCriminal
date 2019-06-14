using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Playfield
{
    public List<Vector2> points;
}

public class ServerController : MonoBehaviour
{
    public string serverAddress;

    private string uri;


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

        Playfield playfield = new Playfield
        {
            points = new List<Vector2>()
        };

        for (int i = 0; i < 10; i++)
        {
            playfield.points.Add(new Vector2(i, i));
        }
        

        CreateGame(100, playfield);
        //StartCoroutine(Communicate());
    }

    public void CreateGame(int time, Playfield playfield)
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "create_game");
        sendObject.AddField("time", time);

        JSONObject playfieldObject = new JSONObject();

        List<Vector2> points = playfield.points;
        foreach (Vector2 point in points)
        {
            JSONObject pointObject = new JSONObject();
            pointObject.AddField("longitude", point.x);
            pointObject.AddField("latitude", point.y);

            playfieldObject.Add(pointObject);
        }
        sendObject.AddField("playfield", playfieldObject);

        sendObject.AddField("ip", "123.456.12.34");
        sendObject.AddField("name", "Arjo");

        Debug.Log(sendObject);

        StartCoroutine(Communicate(sendObject.ToString()));
    }

    IEnumerator Communicate(string jsonString)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

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
                byte[] answer = webRequest.downloadHandler.data;
                string answerString = System.Text.Encoding.UTF8.GetString(answer);

                JSONObject json = new JSONObject(answerString);
                Debug.Log(json.GetField("answer"));
            }
        }
    }
}
