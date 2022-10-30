using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperShoot : Projectile
{
    public float damage = 10f;

    public override void OnHitPlayer(Player player)
    {
        ObjectSpawner.Instance.SpawnVfxEffect(transform.position, direction);
    }
}
