using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Projectile : NetworkBehaviour
{
    private ulong ownerId;
    public float velocity;
    private Vector3 direction;
    private Vector3 startPosition;
    public float range;



    public override void OnNetworkSpawn()
    {
        if(IsServer)
        NetworkManager.Singleton.NetworkTickSystem.Tick += UpdateOnTick;


    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
            NetworkManager.Singleton.NetworkTickSystem.Tick -= UpdateOnTick;


    }
    private void UpdateOnTick()
    {
        if (IsServer)
        {
            
            transform.position += direction * velocity;
            if (Vector3.Distance(startPosition, transform.position) >= range)
                gameObject.GetComponent<NetworkObject>().Despawn();

        }
    }
    public void InitializeProjectile(Vector3 _direction,ulong _ownerId)
    {
        ownerId = _ownerId;
        direction = _direction;
        Quaternion LookAtRotation = Quaternion.LookRotation(_direction);
        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(90f, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = LookAtRotationOnly_Y;
        startPosition = transform.position;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if(other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Player>().playerId != ownerId)
            {
                Debug.Log($"COLLISION WITH: {other.gameObject.GetComponent<Player>().playerId}");

                gameObject.GetComponent<NetworkObject>().Despawn();

            }
        }
        
    }

    /* private void OnCollisionEnter(Collision collision)
     {
         Debug.Log($"COLLISION WITH: {collision.gameObject}");
        // if(collision.gameObject.GetComponent<Player>().playerId != ownerId && collision.gameObject.tag == "Player")
        // {
        //     Destroy(gameObject);
        // }

     }*/
}
