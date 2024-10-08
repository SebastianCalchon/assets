using UnityEngine;
using System;
using SocketIOClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


public class NetworkManager : MonoBehaviour
{
    private SocketIO _socket;
    private Transform xrPlayerTransform;
    private Players players;
    private string MyID;
    private GameObject leftController;
    private GameObject rightController;



    async void Start()
    {
        players = gameObject.AddComponent<Players>();


        if (leftController == null)
        {
            GameObject cameraOffset = GameObject.Find("Camera Offset");
            if (cameraOffset != null)
            {
                leftController = cameraOffset.transform.Find("Left Controller")?.gameObject;
                if (leftController == null)
                {
                    Debug.LogWarning("No se encontró el controlador izquierdo en Camera Offset.");
                }
            }
        }
        if (rightController == null)
        {
            GameObject cameraOffset = GameObject.Find("Camera Offset");
            if (cameraOffset != null)
            {
                rightController = cameraOffset.transform.Find("Right Controller")?.gameObject;
                if (leftController == null)
                {
                    Debug.LogWarning("No se encontró el controlador Derecho en Camera Offset.");
                }
            }
        }

        if (xrPlayerTransform == null)
        {
            xrPlayerTransform = GameObject.Find("Main Camera").transform;
        }

        var uri = "http://192.168.25.182:3000";
        var options = new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                { "token", "UNITY" }
            }
        };

        _socket = new SocketIO(uri, options);

        _socket.OnConnected += (sender, e) =>
        {
            MyID = _socket.Id;
        };
        _socket.OnDisconnected += (sender, e) =>
        {
            players.RemovePlayer(MyID);
            Debug.Log("Desconectado del servidor.");
        };

        _socket.On("playerCountChanged", e =>
        {
            int playerCount = e.GetValue<int>();
            players.SetAmountPlayers(playerCount);
        });

        _socket.On("getPlayers", evnt =>
        {
            try
            {

                string jsonString = evnt.ToString();


                PlayerUpdateData[][] playerDataArray = JsonConvert.DeserializeObject<PlayerUpdateData[][]>(jsonString);


                PlayerUpdateData[] flatArray = playerDataArray.SelectMany(arr => arr).ToArray();

                PlayerUpdateData[] filteredArray = flatArray.Where(player => player.id != MyID).ToArray();


                players.MoveAllPlayers(filteredArray);

                players.RemoveDisconnectedPlayers();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error deserializing JSON: " + ex.Message);
            }


        });

        _socket.On("message", e =>
        {
            Debug.Log("Message from server: " + e.GetValue<string>());
        });

        _socket.On("error", e =>
        {
            Debug.LogError("Error: " + e.GetValue<string>());
        });

        _socket.On("close", e =>
        {
            Debug.Log("Connection closed");
        });

        await _socket.ConnectAsync();
        await HandleSendingData();
    }




    private async Task HandleSendingData()
    {
        while (true)
        {
            if (xrPlayerTransform != null)
            {
                await SendPositionAndRotation();
            }
        }
    }

    private async Task SendPositionAndRotation()
    {
        Vector3 position = xrPlayerTransform.position;
        Vector3 rotation = xrPlayerTransform.rotation.eulerAngles;
        Vector3 leftHandPosition = leftController.transform.position;
        Vector3 leftHandRotation = leftController.transform.rotation.eulerAngles;
        Vector3 rightHandPosition = rightController.transform.position;
        Vector3 rightHandRotation = rightController.transform.rotation.eulerAngles;


        var data = new
        {
            position = new { x = position.x, y = position.y, z = position.z },
            rotation = new { x = rotation.x, rotation.y, z = rotation.z },
            leftHandPosition = new { x = leftHandPosition.x, y = leftHandPosition.y, z = leftHandPosition.z },
            leftHandRotation = new { x = leftHandRotation.x, y = leftHandRotation.y, z = leftHandRotation.z },
            rightHandPosition = new { x = rightHandPosition.x, y = rightHandPosition.y, z = rightHandPosition.z },
            rightHandRotation = new { x = rightHandRotation.x, y = rightHandRotation.y, z = rightHandRotation.z }


        };


        string jsonData = JsonConvert.SerializeObject(data);

        await _socket.EmitAsync("player", jsonData);
    }

}


[System.Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;

}

[System.Serializable]
public class Rotation
{
    public float x;
    public float y;
    public float z;


}
[System.Serializable]
public class LeftHandPosition
{
    public float x;
    public float y;
    public float z;
}
[System.Serializable]
public class LeftHandRotation
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class RightHandPosition
{
    public float x;
    public float y;
    public float z;
}
public class RightHandRotation
{
    public float x;
    public float y;
    public float z;
}


[System.Serializable]
public class PlayerUpdateData
{
    public string id;
    public Position position;
    public Rotation rotation;
    public LeftHandPosition leftHandPosition;

    public LeftHandRotation leftHandRotation;
    public RightHandPosition rightHandPosition;
    public RightHandRotation rightHandRotation;
}
