using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public interface ISkill
{
    public void OnUse();

}
public class Player : NetworkBehaviour
{
    public NetworkVariable<float> Health = new NetworkVariable<float>();
    public ulong playerId;
    public float movementSpeed;

    public Dictionary<int, GameObject> Skills = new Dictionary<int, GameObject>();

    public static Action<ulong> OnDespawnCallback;

    [SerializeField]
    private GameObject PlayerModel;

    private Animator animator;

    public override void OnNetworkSpawn()
    {
        playerId = OwnerClientId;
        if (IsServer)
        {
            Health.Value = 100;
        }
        

       
    }
    private void Update()
    {
        if (IsServer)
        {
            IsDead();
        }
    }
    private void Start()
    {
        animator = PlayerModel.GetComponent<Animator>();
    }
    private void IsDead()
    {
        if (Health.Value <= 0)
        {
            if (OnDespawnCallback != null) OnDespawnCallback(playerId);
            if (IsServer)
            {
                gameObject.GetComponent<NetworkObject>().Despawn();
            }            
        }
    }
    public void ReceiveDamage(float damage)
    {
        if (IsServer)
        {
            Health.Value -= damage;
        }
    }

}
