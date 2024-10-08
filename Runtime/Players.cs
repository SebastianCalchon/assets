using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Players : MonoBehaviour
{
    private List<Player> playerList = new List<Player>();
    private PlayerUpdateData[] dataPlayersServe;
    private int amountPlayers = 0;
    private bool isCreateAvatars = false;
    private int cacheAmountPLayers = 0;
    public GameObject Head;
    public GameObject Body;

    public GameObject handright;
    public GameObject handleft;


    public int GetAmountPlayers()
    {
        if (cacheAmountPLayers != amountPlayers)
        {
            // isCreateAvatars = false;
            cacheAmountPLayers = amountPlayers;
            return amountPlayers;
        }
        return amountPlayers;
    }

    public void SetAmountPlayers(int currentPlayersConnect)
    {
        amountPlayers = currentPlayersConnect;
        isCreateAvatars = false;

        Debug.Log("Cantidad de jugadores conectados: " + amountPlayers + " - " + isCreateAvatars);
    }

    public void AddPlayer(Player player)
    {
        try
        {
            playerList.Add(player);
            amountPlayers += 1;
            Debug.Log("CANTIDAD DE JUGADORES: " + playerList.Count);
        }
        catch (Exception ex)
        {

            // Manejo del error
            Debug.LogError("Error: " + ex.Message);
        }
    }

    private void DestroyIfNotNull(GameObject obj)
    {
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    public void RemovePlayer(string playerId)
    {
        var playerToRemove = playerList.FirstOrDefault(p => p.GetId() == playerId);

        if (playerToRemove != null)
        {
            DestroyIfNotNull(playerToRemove.GetBodyObject());
            DestroyIfNotNull(playerToRemove.GetHeadObject());
            DestroyIfNotNull(playerToRemove.GetLeftHandObject());
            DestroyIfNotNull(playerToRemove.GetRightHandObject());

            playerList.Remove(playerToRemove);
            amountPlayers -= 1;

            Debug.Log($"Jugador con ID {playerId} removido. Jugadores restantes: {playerList.Count}");
        }
        else
        {
            Debug.LogWarning($"No se encontró un jugador con el ID {playerId}");
        }
    }



    public Player GetPlayer(string playerId)
    {
        return playerList.FirstOrDefault(p => p.GetId() == playerId);
    }

    public void MoveAllPlayers(PlayerUpdateData[] players)
    {
        dataPlayersServe = players;
    }

    public void Start()
    {
        Body = GameObject.Find("Body");
        Body.transform.position = new Vector3(0, -2, 0);

        Head = GameObject.Find("Head");
        Head.transform.position = new Vector3(0, -2, 0);

        handright = GameObject.Find("HandRight");
        handright.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        handright.transform.position = new Vector3(0, -2, 0);

        handleft = GameObject.Find("HandLeft");
        handleft.transform.localScale = new Vector3(-0.25f, 0.25f, 0.25f);
        handleft.transform.position = new Vector3(0, -2, 0);
    }

    public void Update()
    {
        try
        {

            if (dataPlayersServe == null || dataPlayersServe.Length == 0 || GetAmountPlayers() <= 0)
            {
                return;
            }
            RemoveDisconnectedPlayers();


            if (!isCreateAvatars && Head != null && Body != null)
            {
                CreatePlayers();
            }


            if (isCreateAvatars)
            {
                UpdatePlayerPositionsAndRotations();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error en Update: " + ex.Message);
        }
    }

    public void RemoveDisconnectedPlayers()
    {
        // Crear una lista temporal de IDs de jugadores conectados
        List<string> connectedPlayerIds = dataPlayersServe.Select(p => p.id).ToList();

        // Buscar jugadores en la lista actual que no estén en los datos de jugadores conectados
        var disconnectedPlayers = playerList.Where(p => !connectedPlayerIds.Contains(p.GetId())).ToList();

        // Eliminar a los jugadores desconectados
        foreach (var player in disconnectedPlayers)
        {
            RemovePlayer(player.GetId());
            Debug.Log($"Jugador con ID {player.GetId()} desconectado y eliminado.");
        }
    }

    private void CreatePlayers()
    {
        // Crear instancias para jugadores nuevos que no estén en la lista de jugadores actual
        foreach (var item in dataPlayersServe)
        {
            if (item == null) continue;

            // Si el jugador ya está en la lista, no lo creamos de nuevo
            if (playerList.Any(p => p.GetId() == item.id))
            {
                Debug.LogWarning("Jugador con ID " + item.id + " ya existe. No se creará un nuevo avatar.");
                continue;
            }

            Player newPlayer = new Player();
            newPlayer.SetId(item.id);

            if (Head != null && handright != null && Body != null)
            {
                // Instanciar cuerpo y cabeza
                GameObject body = Instantiate(Body, new Vector3(0, -10, 0), Quaternion.identity);
                GameObject head = Instantiate(Head, new Vector3(0, -10, 0), Quaternion.identity);

                GameObject leftHand = Instantiate(handleft, new Vector3(0, -10, 0), Quaternion.identity);
                GameObject rightHand = Instantiate(handright, new Vector3(0, -10, 0), Quaternion.identity);

                // Asignar cuerpo y cabeza al jugador
                newPlayer.SetBodyObject(body);
                newPlayer.SetHeadObject(head);
                newPlayer.SetLeftHandObject(leftHand);
                newPlayer.SetRightHandObject(rightHand);
            }

            playerList.Add(newPlayer);
        }

        Debug.Log($"Jugadores creados: {playerList.Count}");
        isCreateAvatars = true;
    }


    private void UpdatePlayerPositionsAndRotations()
    {
        foreach (var dataPlayer in dataPlayersServe)
        {
            foreach (var player in playerList)
            {
                if (player == null || player.GetId() != dataPlayer.id)
                {
                    continue;
                }

                Vector3 position = new Vector3(dataPlayer.position.x, dataPlayer.position.y, dataPlayer.position.z);

                Vector3 bodyRotation = new Vector3(0, dataPlayer.rotation.y, 0);
                Vector3 headRotation = new Vector3(dataPlayer.rotation.x, dataPlayer.rotation.y, dataPlayer.rotation.z);

                player.SetPosition(position);
                player.SetRotation(bodyRotation, headRotation);

                if (dataPlayer.leftHandPosition != null)
                {
                    Vector3 leftHandPosition = new Vector3(dataPlayer.leftHandPosition.x, dataPlayer.leftHandPosition.y, dataPlayer.leftHandPosition.z);
                    player.SetLeftHandPosition(leftHandPosition);
                }

                if (dataPlayer.leftHandRotation != null)
                {
                    Vector3 leftHandRotation = new Vector3(dataPlayer.leftHandRotation.x + 180, dataPlayer.leftHandRotation.y, dataPlayer.leftHandRotation.z + 180);
                    player.SetLeftHandRotation(leftHandRotation);
                }

                if (dataPlayer.rightHandPosition != null)
                {
                    Vector3 rightHandPosition = new Vector3(dataPlayer.rightHandPosition.x, dataPlayer.rightHandPosition.y, dataPlayer.rightHandPosition.z);
                    player.SetRightHandPosition(rightHandPosition);
                }

                if (dataPlayer.rightHandRotation != null)
                {
                    Vector3 rightHandRotation = new Vector3(dataPlayer.rightHandRotation.x + 180, dataPlayer.rightHandRotation.y, dataPlayer.rightHandRotation.z + 180);
                    player.SetRightHandRotation(rightHandRotation);
                }
            }
        }
    }

}

