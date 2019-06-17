
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    public bool isHost;
}


public class Game
{
    public bool starting;
    public int time;
    public Playfield playfield;
    public List<Player> players;
}


public class ServerController : MonoBehaviour
{
    [NonSerialized]
    public string serverAddress;

    private Dictionary<string, string> settings = new Dictionary<string, string>();
    
    [NonSerialized]
    public string roomPin;
    [NonSerialized]
    public string playerIp;
    [NonSerialized]
    public string playerName;
    [NonSerialized]
    public bool isHost;

    public UIManager uiManager;

    public UIScreenManager uiScreenRoom;
    public UIScreenManager uiScreenHome;
    public UIScreenManager uiScreenGame;


    public Game game;

    [System.NonSerialized]
    public UnityEvent updateRoomData = new UnityEvent();
    private readonly float updateRoomDataDelay = 5f;
    private bool continueUpdatingRoomData;

    private readonly float startGameDelay = 6f;


    public void UpdateFields(JSONObject fieldsJson)
    {
        for (int i = 0; i < fieldsJson.list.Count; i++)
        {
            string key = fieldsJson.keys[i];
            string value = fieldsJson.list[i].str;
            settings[key] = value;
            
        }
    }


    public string GetField(string key)
    {
        return settings.ContainsKey(key) ? settings[key] : "";
    }


    public void TestConnection()
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "test_connection");

        StartCoroutine(SendRequest(sendObject, false, TestConnectionCallback));
    }


    private void TestConnectionCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            uiManager.ShowPopup("Server address is valid", uiManager.popupDuration);

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

        sendObject.AddField("name", GetField("name"));

        StartCoroutine(SendRequest(sendObject, true, CreateGameCallback));
    }


    private void CreateGameCallback(JSONObject incomingJson)
    {
        roomPin = incomingJson.GetField("room_pin").str;
        isHost = true;

        string status = incomingJson.GetField("status").str;

        // Create game object with right variables
        PopulateRoom(incomingJson);

        if (status == "success")
        {
            Debug.Log("Room created");
            playerIp = incomingJson.GetField("ip").str;
            playerName = incomingJson.GetField("name").str;
            uiManager.NextScreen(uiScreenRoom);


            StartUpdatingRoomData();

        } else if (status == "failed")
        {
            Debug.Log("Room not created");
            uiManager.ShowPopup("Couldn't create game.", uiManager.popupDuration);
        }
    }


    public void JoinGame(string newRoomPin)
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "join_game");
        sendObject.AddField("room_pin", newRoomPin);

        sendObject.AddField("name", GetField("name"));

        StartCoroutine(SendRequest(sendObject, true, JoinGameCallback));
    }


    private void JoinGameCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            Debug.Log("Room found");
            playerIp = incomingJson.GetField("ip").str;
            playerName = incomingJson.GetField("name").str;
            roomPin = incomingJson.GetField("room_pin").str;
            isHost = false;

            // Create game object with right variables
            PopulateRoom(incomingJson);

            StartUpdatingRoomData();

            uiManager.NextScreen(uiScreenRoom);      
        }
        else if (status == "failed")
        {
            Debug.Log("Room not found");
            uiManager.ShowPopup("Room not found", uiManager.popupDuration);
        }
    }


    public void KickPlayer(string kickIp, string kickName)
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "kick_player");
        sendObject.AddField("room_pin", roomPin);
        sendObject.AddField("kick_ip", kickIp);
        sendObject.AddField("kick_name", kickName);

        StartCoroutine(SendRequest(sendObject, true, KickPlayerCallback));
    }

    private void KickPlayerCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            Debug.Log("Kicked player");
            uiManager.ShowPopup(string.Format("Kicked {0} from room", incomingJson.GetField("kick_name").str), uiManager.popupDuration);

            UpdateRoomData();
        }
        else if (status == "failed")
        {
            Debug.Log("Couldn't kick player");
            uiManager.ShowPopup("Couldn't kick player", uiManager.popupDuration);
        }
    }


    public void LeaveGame()
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "leave_game");
        sendObject.AddField("room_pin", roomPin);
        sendObject.AddField("name", GetField("name"));
        sendObject.AddField("is_host", isHost);

        StartCoroutine(SendRequest(sendObject, true, LeaveGameCallback));
    }


    private void LeaveGameCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            Debug.Log("Left game");
            uiManager.PreviousScreen(uiScreenHome);

            StopUpdatingRoomData();

        } else if (status == "failed")
        {
            Debug.Log("Failed to leave game");

            uiManager.PreviousScreen(uiScreenHome);

            uiManager.ShowPopup("Room doesn't exist anymore", uiManager.popupDuration);

            StopUpdatingRoomData();

        }
    }


    private void PopulateRoom(JSONObject incomingJson)
    {
        game = new Game
        {
            time = (int)incomingJson.GetField("time").i,
            playfield = new Playfield(incomingJson.GetField("playfield")),
            starting = false,
            players = new List<Player>()
        };

        JSONObject playerlistJson = incomingJson.GetField("playerlist");
        foreach (JSONObject playerJson in playerlistJson)
        {
            Player newPlayer = new Player
            {
                name = playerJson.GetField("name").str,
                ip = playerJson.GetField("ip").str,
                isHost = playerJson.GetField("is_host").b
            };
            game.players.Add(newPlayer);
        }
    }


    public void UpdateRoomData()
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "update_room_data");
        sendObject.AddField("room_pin", roomPin);

        StartCoroutine(SendRequest(sendObject, false, UpdateRoomDataCallback));
    }


    public void StartUpdatingRoomData()
    {
        continueUpdatingRoomData = true;
        StartCoroutine(CycleUpdateRoomData());
    }


    public void StopUpdatingRoomData()
    {
        continueUpdatingRoomData = false;
    }


    private void UpdateRoomDataCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            Debug.Log("Room found");

            // Create game object with right variables
            JSONObject playerlistJson = incomingJson.GetField("playerlist");
            game.players = new List<Player>();
            bool kicked = true;
            foreach (JSONObject playerJson in playerlistJson)
            {
                Player newPlayer = new Player
                {
                    name = playerJson.GetField("name").str,
                    ip = playerJson.GetField("ip").str,
                    isHost = playerJson.GetField("is_host").b
                };
                if (newPlayer.name == playerName && newPlayer.ip == playerIp)
                {
                    kicked = false;

                    if (newPlayer.isHost)
                    {
                        isHost = true;
                    }
                }
                game.players.Add(newPlayer);
            }

            if (kicked)
            {
                StopUpdatingRoomData();

                uiManager.ShowPopup("You have been kicked from this room", uiManager.popupDuration);
                uiManager.PreviousScreen(uiScreenHome);
            }

            if (incomingJson.GetField("starting").b == true)
            {
                float delay = incomingJson.GetField("delay").f;
                Debug.Log("Time before start: " + delay.ToString());
                uiManager.ShowPopup(string.Format("Game will start in {0} seconds", delay), uiManager.popupDuration);

                StartCoroutine(StartGameAfter(delay));

                StopUpdatingRoomData();
            }

            updateRoomData.Invoke();

        }
        else if (status == "failed")
        {
            Debug.Log("Room deleted");
            uiManager.ShowPopup("This room doesn't exist anymore", uiManager.popupDuration);
            uiManager.PreviousScreen(uiScreenHome);

            StopUpdatingRoomData();
        }
    }


    public IEnumerator CycleUpdateRoomData()
    {
        while (continueUpdatingRoomData)
        {
            UpdateRoomData();
            yield return new WaitForSeconds(updateRoomDataDelay);
        }
    }


    public void RequestStartGame()
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "request_start_game");
        sendObject.AddField("room_pin", roomPin);
        sendObject.AddField("delay", startGameDelay);

        StartCoroutine(SendRequest(sendObject, false, RequestStartGameCallback));
    }


    private void RequestStartGameCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            uiManager.ShowPopup(string.Format("Game will start in {0} seconds", startGameDelay), uiManager.popupDuration);

            StartCoroutine(StartGameAfter(startGameDelay));

            StopUpdatingRoomData();
        }
        else if (status == "failed")
        {
            Debug.Log("Room deleted");
            uiManager.ShowPopup("This room doesn't exist anymore", uiManager.popupDuration);
            uiManager.PreviousScreen(uiScreenHome);

            StopUpdatingRoomData();
        }
    }


    IEnumerator StartGameAfter(float time)
    {
        yield return new WaitForSeconds(time);

        uiManager.NextScreen(uiScreenGame);

        StopUpdatingRoomData();
    }


    private IEnumerator SendRequest(JSONObject outgoingJson, bool disableScreen, Action<JSONObject> callback = null)
    {
        if (disableScreen)
        {
            uiManager.DeactivateScreen(uiManager.currentScreen);

            if (uiManager.currentOverlayScreen != null)
            {
                uiManager.DeactivateScreen(uiManager.currentOverlayScreen);
            }
        }
        

        Debug.Log(outgoingJson);

        string jsonString = outgoingJson.ToString();
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

        string address = GetField("serverAddress");

        using (UnityWebRequest webRequest = UnityWebRequest.Put("http://"+address, bytes))
        {
            webRequest.method = "POST";
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (disableScreen)
            {
                uiManager.ActivateScreen(uiManager.currentScreen);
                if (uiManager.currentOverlayScreen != null)
                {
                    uiManager.ActivateScreen(uiManager.currentOverlayScreen);
                }
            }

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
                uiManager.ShowPopup("Couldn't connect to server", uiManager.popupDuration);
            }
            else
            {
                byte[] answer = webRequest.downloadHandler.data;
                string answerString = System.Text.Encoding.UTF8.GetString(answer);

                JSONObject incomingJson = new JSONObject(answerString);
                Debug.Log(incomingJson);
                callback?.Invoke(incomingJson);

                

            }
        }
    }
}
