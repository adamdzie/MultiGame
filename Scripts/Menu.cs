using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Menu : MonoBehaviour
{
    public Button startServer;
    public Button startClient;
    public GameObject panel;

    //Call only when force to start server build or client
  //private void Start()
 //{
  //      onStartServer();
  //}

    public void onStartServer()
    {
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
}
