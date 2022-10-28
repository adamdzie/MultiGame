using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ObjectSpawner : NetworkSingleton<ObjectSpawner>
{
    [SerializeField]
    private GameObject ProjectilePrefab;

    [SerializeField]
    private GameObject VfxEffect;
    
    public void SpawnObject(Vector3 _spawnPosition, Vector3 _direction, ulong playerId)
    {
        GameObject go = Instantiate(ProjectilePrefab, _spawnPosition,Quaternion.identity);
        go.GetComponent<Projectile>().InitializeProjectile(_direction,playerId);
        go.GetComponent<NetworkObject>().Spawn();
    }
    public void SpawnVfxEffect(Vector3 _spawnPosition, Vector3 direction)
    {
        Quaternion newRotation = Quaternion.LookRotation(direction);

        GameObject go = Instantiate(VfxEffect, _spawnPosition, newRotation);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<ParticleSystem>().Play();
    }
    
}
