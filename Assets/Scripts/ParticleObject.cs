using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ParticleObject : NetworkBehaviour
{
    [SerializeField]
    private float lifetime;

    private float spawned_time;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            NetworkManager.NetworkTickSystem.Tick += UpdateServerTick;
            spawned_time = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        }
        
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if(IsServer)
        NetworkManager.NetworkTickSystem.Tick -= UpdateServerTick;
    }
    //la
    void UpdateServerTick()
    {
        if (IsServer)
        {
            if(NetworkManager.Singleton.ServerTime.TimeAsFloat > spawned_time + lifetime)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
