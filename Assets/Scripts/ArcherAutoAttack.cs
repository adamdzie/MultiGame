using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ArcherAutoAttack : Projectile
{
    public override void OnHitPlayer(Player player)
    {
        ObjectSpawner.Instance.SpawnVfxEffect(transform.position, direction);
        gameObject.GetComponent<NetworkObject>().Despawn();

    }
}
