using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplay;
using Unity.Services.Core;
using System;
using Unity.Netcode;

public class Menu : MonoBehaviour
{
    public Button startServer;
    public Button startClient;
    public GameObject panel;

    //Call only when force to start server build or client
private void Start()
 {
       //onStartServer();
 }

    public void onStartServer()
    {
        //InitSDK();
        //NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", GetServerPort(), "0.0.0.0");
        NetworkManager.Singleton.StartServer();
        panel.SetActive(false);
    }
    public void onStartClient()
    {
        NetworkManager.Singleton.StartClient();
        panel.SetActive(false);
    }
    public void onStartHost()
    {
        NetworkManager.Singleton.StartHost();
        panel.SetActive(false);
    }

    
    #region Unity Services
    public static ushort GetServerPort()
    {
        var serverConfig = MultiplayService.Instance.ServerConfig;
        return serverConfig.Port;
        //Debug.Log($"Server ID[{serverConfig.ServerId}]");
        //Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        //Debug.Log($"Port[{serverConfig.Port}]");
        // Debug.Log($"QueryPort[{serverConfig.QueryPort}");
        // Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");
    }
    async void InitSDK()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    #endregion
}
