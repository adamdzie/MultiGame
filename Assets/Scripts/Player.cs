using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Player : NetworkBehaviour
{
    public int health;
    public ulong playerId;
    public float movementSpeed;

    [SerializeField]
    private GameObject PlayerModel;

    private Animator animator;

    public override void OnNetworkSpawn()
    {
        playerId = OwnerClientId;
        health = 100;

       
    }
    private void Update()
    {
      
    }
    private void Start()
    {
        animator = PlayerModel.GetComponent<Animator>();
    }

}
