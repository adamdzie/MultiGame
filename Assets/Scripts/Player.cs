using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface ISkill
{
    public void OnUse();

}
public class Player : NetworkBehaviour
{
    public int health;
    public ulong playerId;
    public float movementSpeed;

    public Dictionary<int, GameObject> Skills = new Dictionary<int, GameObject>();

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
