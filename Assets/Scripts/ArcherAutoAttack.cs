using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ArcherAutoAttack : Projectile
{
    public float damage = 20f;
    public override void OnHitPlayer(Player player)
    {
        ObjectSpawner.Instance.SpawnVfxEffect(transform.position, direction);
        player.ReceiveDamage(damage);
        gameObject.GetComponent<NetworkObject>().Despawn();

    }
}
