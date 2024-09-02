using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Cinemachine;

public class MainMenuUI : MonoBehaviour
{
    NetworkManager manager;
    [SerializeField] private GameObject playerNameInputField;
    private string playerName;

    public GameObject mainMenuCanvas;
    public GameObject carSelectCanvas;

    public CinemachineVirtualCamera mainMenuCam;
    public CinemachineVirtualCamera shopCam;

    private int[] shopCamYPos ;
    private int shopCamYPosItr = 0 ;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
        manager.networkAddress = "localhost";

        if (Transport.active is PortTransport portTransport)
        {
            if (ushort.TryParse("localhost", out ushort port))
                portTransport.Port = port;
        }

        shopCamYPos = new int[]{9, 0 , -9};
    }

    public void StartButtonClicked()
    {
        if (manager == null)
        {
            Debug.LogError("NetworkManager not found");
            return;
        }

        manager.StartClient();
        StartCoroutine(WaitForLocalPlayerAndSetPlayerName());
    }

    private IEnumerator WaitForLocalPlayerAndSetPlayerName()
    {
        
        // while (!NetworkClient.ready)
        // {
        //     Debug.Log("Waiting for local readyyy...");
        //     yield return null;
        // }
        // if (!NetworkClient.ready)
        // {
        //     // NetworkClient.Ready();
        // }
        // if (NetworkClient.localPlayer == null )
        // {
        //     Debug.Log("Attempting to add player");

        //     // NetworkClient.AddPlayer();
        // }
        while (NetworkClient.localPlayer == null)   
        {
            Debug.Log("Waiting for local player...");
            yield return null;
        }

        PlayerManager localPlayer = NetworkClient.localPlayer.GetComponent<PlayerManager>();
        playerName = playerNameInputField.GetComponent<TMP_InputField>().text.ToString();
        
        localPlayer.SetPlayerName(playerName);
    }

    public void StartServerButtonClicked()
    {
        if (!NetworkClient.active)
        {
            Debug.Log("Start Server Button Clicked");

            if (manager == null)
            {
                Debug.LogError("NetworkManager not found");
                return;
            }

            manager.StartServer();

            Debug.Log("Server Started");
            if (Transport.active is PortTransport portTransport)
            {
                Debug.Log(portTransport.Port);
            }
        }
    }

    public void SelectCarButtonClicked()
    {
        if (!NetworkClient.active)
        {
            Debug.Log("select car Button Clicked");

            if (manager == null)
            {
                Debug.LogError("NetworkManager not found");
                return;
            }

            mainMenuCam.Priority = 0;
            shopCam.Priority = 10;
            mainMenuCanvas.SetActive(false);
            carSelectCanvas.SetActive(true);
             
        }
    }


    // carShopCanvas button function below
    public void BackToMainMenuButtonClicked()
    {
        if (!NetworkClient.active)
        {
            if (manager == null)
            {
                Debug.LogError("NetworkManager not found");
                return;
            }

            mainMenuCam.Priority = 10;
            shopCam.Priority = 0;
            carSelectCanvas.SetActive(false);
            mainMenuCanvas.SetActive(true);
            
        }
    }

    public void RightButtonClicked()
    {
        if (!NetworkClient.active)
        {

            if (manager == null)
            {
                Debug.LogError("NetworkManager not found");
                return;
            }
            shopCamYPosItr = (shopCamYPosItr+1)%3;
            shopCam.transform.position = new Vector3(shopCamYPos[shopCamYPosItr], 7f, 7);
            
        }
    }

    public void LeftButtonClicked()
    {
        if (!NetworkClient.active)
        {
            if (manager == null)
            {
                Debug.LogError("NetworkManager not found");
                return;
            }
            shopCamYPosItr = (shopCamYPosItr-1)%3;
            shopCam.transform.position = new Vector3(shopCamYPos[shopCamYPosItr], 7f, 7);
            
        }
    }
    
    public void SelectButtonClicked()
    {
        if (!NetworkClient.active)
        {
            if (manager == null)
            {
                Debug.LogError("NetworkManager not found");
                return;
            }

            BackToMainMenuButtonClicked();
            // UpdateCarArrayItr();
            GameObject.Find("NetworkManager").GetComponent<MirrorNetworkManager>().carPrefabItr = shopCamYPosItr;
            
        }
    }

    // [Command]
    // public void UpdateCarArrayItr(){
    //     GameObject.Find("NetworkManager").GetComponent<MirrorNetworkManager>().carPrefabItr = shopCamYPosItr;
    // }

}
