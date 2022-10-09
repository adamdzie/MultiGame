using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Player : NetworkBehaviour
{
    public int health;
    public ulong playerId;
    public float movementSpeed;

    public override void OnNetworkSpawn()
    {
        playerId = OwnerClientId;
        health = 100;
    }
}
