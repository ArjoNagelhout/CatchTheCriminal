
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

enum Playertype { Tobedetermined, Criminal, Cop };

public class Playfield
{
    public List<Vector2> points = new List<Vector2>();

    public Playfield(JSONObject playfieldJson)
    {
        foreach (JSONObject pointJson in playfieldJson)
        {
            float longitude = pointJson.GetField("longitude").f;
            float latitude = pointJson.GetField("latitude").f;

            Vector2 newPoint = new Vector2(longitude, latitude);

            points.Add(newPoint);
        }
    }

    public Playfield()
    {

    }
}

public class Player
{
    public string ip;
    public string name;
}

public class Game
{
    public int time;
    public Playfield playfield;
    public List<Player> players;
}

public class ServerController : MonoBehaviour
{
    public string serverAddress;
    private string uri;

    [System.NonSerialized]
    public string roomPin;
    [System.NonSerialized]
    public string playerName;
    [System.NonSerialized]
    public bool isHost;

    public UIManager uiManager;

    public UIScreenManager uiScreenRoomPlayer;
    public UIScreenManager uiScreenHome;

    public Game game;


    void Start()
    {

        string prefix = "http://";

        if (serverAddress.StartsWith(prefix, System.StringComparison.Ordinal))
        {
            uri = serverAddress;
        }
        else
        {
            uri = string.Format("http://{0}", serverAddress);
        }
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

        sendObject.AddField("name", playerName);

        StartCoroutine(SendRequest(sendObject, CreateGameCallback));
    }

    private void CreateGameCallback(JSONObject incomingJson)
    {
        roomPin = incomingJson.GetField("room_pin").str;

        isHost = true;

        string status = incomingJson.GetField("status").str;
        
        if (status == "success")
        {
            Debug.Log("Room created");
            uiManager.NextScreen(uiScreenRoomPlayer);

        } else if (status == "failed")
        {
            Debug.Log("Room not created");
            uiManager.ShowPopup("Couldn't create game.", uiManager.popupDuration);
        }

        // Create game object with right variables

        game = new Game()
        {
            time = (int)incomingJson.GetField("time").i,
            playfield = new Playfield(incomingJson.GetField("playfield"))

        };
    }


    public void JoinGame(string newRoomPin)
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "join_game");
        sendObject.AddField("room_pin", newRoomPin);

        sendObject.AddField("name", playerName);

        StartCoroutine(SendRequest(sendObject, JoinGameCallback));
    }

    private void JoinGameCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            Debug.Log("Room found");
            roomPin = incomingJson.GetField("room_pin").str;
            isHost = false;

            uiManager.NextScreen(uiScreenRoomPlayer);

            game = new Game()
            {
                time = (int)incomingJson.GetField("time").i,
                playfield = new Playfield(incomingJson.GetField("playfield"))

            };
        }
        else if (status == "failed")
        {
            Debug.Log("Room not found");
            uiManager.ShowPopup("Room not found", uiManager.popupDuration);
        }
    }


    public void LeaveGame()
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "leave_game");

        sendObject.AddField("room_pin", roomPin);

        sendObject.AddField("name", playerName);

        


        StartCoroutine(SendRequest(sendObject, LeaveGameCallback));
    }

    private void LeaveGameCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            Debug.Log("Left game");
            uiManager.PreviousScreen(uiScreenHome);
        } else if (status == "failed")
        {
            Debug.Log("Failed to leave game");
        }
    }


    IEnumerator SendRequest(JSONObject outgoingJson, Action<JSONObject> callback = null)
    {
        uiManager.DeactivateScreen(uiManager.currentScreen);

        if (uiManager.currentOverlayScreen != null)
        {
            uiManager.DeactivateScreen(uiManager.currentOverlayScreen);
        }

        Debug.Log(outgoingJson);

        string jsonString = outgoingJson.ToString();
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

                JSONObject incomingJson = new JSONObject(answerString);
                Debug.Log(incomingJson);
                callback?.Invoke(incomingJson);

                uiManager.ActivateScreen(uiManager.currentScreen);
                if (uiManager.currentOverlayScreen != null)
                {
                    uiManager.ActivateScreen(uiManager.currentOverlayScreen);
                }

            }
        }
    }
}
