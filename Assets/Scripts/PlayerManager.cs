using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerManager : NetworkSingleton<PlayerManager>
{
    public const float respawnTime = 3f;

    public Dictionary<ulong, bool> ActivePrefabs = new Dictionary<ulong, bool>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += StorePlayer;
            Player.OnDespawnCallback += RespawnAfterTime;
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= StorePlayer;
            Player.OnDespawnCallback -= RespawnAfterTime;
        }
        
    }
    private void StorePlayer(ulong clientId)
    {
        ObjectSpawner.Instance.SpawnPlayer(clientId);
    }
    private void RespawnAfterTime(ulong clientId)
    {
        StartCoroutine(WaitRespawnTime(clientId));

    }
    IEnumerator WaitRespawnTime(ulong clientId)
    {
        
        yield return new WaitForSeconds(respawnTime);
        ObjectSpawner.Instance.SpawnPlayer(clientId);

    }
}
