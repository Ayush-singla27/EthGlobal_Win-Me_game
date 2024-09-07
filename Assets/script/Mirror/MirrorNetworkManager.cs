using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using NBitcoin;

public class MirrorNetworkManager : NetworkManager
{
    private int playerCount = 0;
    public int noOfPlayers = 1;

    public GameObject[] carPrefabs ;
    public int carPrefabItr =0;
    public GameObject managerItr;


    public struct CreateCharacterMessage : NetworkMessage
    {
        public int kartPrefabIndex;
    }

    private void Start()
    {
        Transform firstChild = transform.GetChild(0);
        firstChild.gameObject.SetActive(true);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Client connected: " + conn.connectionId);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("on create character");
        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
    }


    public override void OnClientConnect()
    {
        base.OnClientConnect();
        CreateCharacterMessage message = new CreateCharacterMessage
        {
            kartPrefabIndex = StaticVariables.kartPrefabIndex,
        };
        Debug.Log("messaging kart prefab index" + message.kartPrefabIndex);
        NetworkClient.Send(message);

        //if (SpawnAsCharacter)
        //{
        //    // you can send the message here, or wherever else you want
        //    CreateCharacterMessage characterMessage = new CreateCharacterMessage
        //    {
        //        playerName = StaticVariables.playerName,
        //        characterNumber = StaticVariables.characterNumber,
        //        characterColour = StaticVariables.characterColour
        //    };

        //    NetworkClient.Send(characterMessage);
        //}
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        Debug.Log("Client is ready: " + conn.connectionId);
    }

    void OnCreateCharacter(NetworkConnectionToClient conn, CreateCharacterMessage message)
    {

        Debug.Log("Entered OnServerAddPlayer");
        Debug.Log("prefab index is" + message.kartPrefabIndex);

        // Check if the connection already has a player
        //if (conn.identity != null)
        //{
        //    Debug.LogWarning("Connection already has a player");
        //    return;
        //}
        //GameObject oldPlayer = conn.identity.gameObject;
        //GameObject playerObject = Instantiate(carPrefabs[message.kartPrefabIndex], oldPlayer.transform.position, oldPlayer.transform.rotation);


        //NetworkServer.ReplacePlayerForConnection(conn, playerObject, true);
        //Destroy(oldPlayer, 0.1f);

        Vector3 start = new Vector3(0, 40f, 0);
        //GameObject player = Instantiate(playerPrefab, start, Quaternion.identity);
        //GameObject player = Instantiate(carPrefabs[managerItr.GetComponent<ItrManager>().carPrefabItr], start, Quaternion.identity);
        GameObject player = Instantiate(carPrefabs[message.kartPrefabIndex], start, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("Player spawned");

        playerCount++;
        if (playerCount == noOfPlayers)
        {
            LoadGameScene();

        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //Debug.Log("Entered OnServerAddPlayer");

        //// Check if the connection already has a player
        //if (conn.identity != null)
        //{
        //    Debug.LogWarning("Connection already has a player");
        //    return;
        //}

        //Vector3 start = new Vector3(0, 40f, 0);
        ////Debug.Log("prefab index is" + message.kartPrefabIndex);
        //// GameObject player = Instantiate(playerPrefab, start, Quaternion.identity);
        //GameObject player = Instantiate(carPrefabs[managerItr.GetComponent<ItrManager>().carPrefabItr], start, Quaternion.identity);

        //NetworkServer.AddPlayerForConnection(conn, player);
        //Debug.Log("Player spawned");

        //playerCount++;
        //if (playerCount == noOfPlayers)
        //{
        //    LoadGameScene();

        //}
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("OnServerDisconnect called");
        base.OnServerDisconnect(conn);

        playerCount--;

        if (playerCount == 0)
        {
            string newSceneName = "MirrorWaitingRoom"; // Replace with your scene name
            Debug.Log("Player Disconnected!!!");
            ServerChangeScene(newSceneName);
        }
    }

    [Server]
    private void LoadGameScene()
    {
        string newSceneName = "MirrorCloverStadium"; // Replace with your scene name
        ServerChangeScene(newSceneName);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client has stopped.");
    }

}
