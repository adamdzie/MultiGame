using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectSpawner : NetworkSingleton<ObjectSpawner>
{
    [SerializeField]
    private GameObject objectPrefab;
    
    public void SpawnObject(Vector3 _spawnPosition, Vector3 _direction, ulong playerId)
    {
        GameObject go = Instantiate(objectPrefab, _spawnPosition,Quaternion.identity);
        go.GetComponent<Projectile>().InitializeProjectile(_direction,playerId);
        go.GetComponent<NetworkObject>().Spawn();
    }

    
}
