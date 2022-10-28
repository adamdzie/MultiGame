using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Projectile : NetworkBehaviour
{
    protected ulong ownerId;
    public float velocity;
    protected Vector3 direction;
    protected Vector3 startPosition;
    public float range;

    [SerializeField]
    private ParticleSystem whenHit;

    private NetworkVariable<MovementInterpolator.PositionInTime> Position = new NetworkVariable<MovementInterpolator.PositionInTime>();

    public override void OnNetworkSpawn()
    {
        
        
        if (IsClient)
        {
            Position.OnValueChanged += OnPositionChange;

        }
        if (IsServer)
        {
            
            NetworkManager.Singleton.NetworkTickSystem.Tick += UpdateOnTick;
            Position.Value = new MovementInterpolator.PositionInTime(0f, new Vector2(transform.position.x, transform.position.z));
        }

    }
    public override void OnNetworkDespawn()
    {
        
        if(IsClient)
        {
            Position.OnValueChanged -= OnPositionChange;
        }
        if (IsServer)
            NetworkManager.Singleton.NetworkTickSystem.Tick -= UpdateOnTick;


    }
    private void UpdateOnTick()
    {
        if (IsServer)
        {
            
            Position.Value = new MovementInterpolator.PositionInTime(NetworkManager.Singleton.ServerTime.TimeAsFloat, new Vector2(Position.Value.Position.x + direction.x * velocity, Position.Value.Position.y + direction.z * velocity));

            transform.position = new Vector3(Position.Value.Position.x, transform.position.y, Position.Value.Position.y);

            if (Vector3.Distance(startPosition, transform.position) >= range)
                gameObject.GetComponent<NetworkObject>().Despawn();

        }
    }
    void OnPositionChange(MovementInterpolator.PositionInTime previous, MovementInterpolator.PositionInTime current)
    {
        if (Application.isFocused)
        {
            gameObject.GetComponent<MovementInterpolator>().HandleNewTick(current.Time, current.Position);
        }
            
    }
    public void InitializeProjectile(Vector3 _direction,ulong _ownerId)
    {
        ownerId = _ownerId;
        direction = _direction;
        Quaternion LookAtRotation = Quaternion.LookRotation(_direction);
        Quaternion LookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, LookAtRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = LookAtRotationOnly_Y;
        startPosition = transform.position;
        Position.Value = new MovementInterpolator.PositionInTime(NetworkManager.Singleton.ServerTime.TimeAsFloat, new Vector2(transform.position.x,transform.position.z));
        
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if(other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Player>().playerId != ownerId)
            {
                OnHitPlayer(other.gameObject.GetComponent<Player>());

                
                

            }
        }
        else gameObject.GetComponent<NetworkObject>().Despawn();

    }

    public virtual void OnHitPlayer(Player player)
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
}
