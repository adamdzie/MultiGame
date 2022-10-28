using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject shootPoint;

    private Vector3 currentShootPoint;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private NetworkVariable<float> moveSpeed = new NetworkVariable<float>();


    private NetworkVariable<MovementInterpolator.PositionInTime> heroPos = new NetworkVariable<MovementInterpolator.PositionInTime>();

    [SerializeField]
    private NetworkVariable<Quaternion> heroRotation = new NetworkVariable<Quaternion>();

    [SerializeField]
    public NetworkVariable<Vector3> mousePoint = new NetworkVariable<Vector3>();



    [SerializeField]
    public static Vector3 SpawnPosition = new Vector3(0, 1, 0);

    [SerializeField]
    private Animator animator;

    private NetworkAnimator networkAnimator;

    private Player player;

    private float _moveSpeed;

    private Vector3 _mousePoint;

    private int tickCounter;

    private PlayerAnimationController _playerAnimationController;

    private Vector3 _targetPos;

    public NetworkVariable<PlayerMovementState> playerMovementState = new NetworkVariable<PlayerMovementState>();
    public enum PlayerMovementState
    {
        Stay,
        Move,
    }
    public override void OnNetworkSpawn()
    {
        
        if (IsClient)
        {
            transform.position = SpawnPosition;

            tickCounter = 0;
            heroPos.OnValueChanged += OnSomeValueChange;
        }
        
        if (IsClient && IsOwner)
        {
            
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            

            PlayerCamera.Instance.SetTarget(transform);
            PlayerCamera.Instance.SetRotation();

        }
        if (IsServer)
        {
            InitializeClientParams();
            NetworkManager.NetworkTickSystem.Tick += UpdateServerTick;
        }
        
    }
    public override void OnNetworkDespawn()
    {
        if(IsServer)
        NetworkManager.NetworkTickSystem.Tick -= UpdateServerTick;
        if(IsClient)
        {
            heroPos.OnValueChanged -= OnSomeValueChange;
        }
    }
    private void Start()
    {
        transform.position = SpawnPosition;
        currentShootPoint = shootPoint.transform.position;
        _moveSpeed = gameObject.GetComponent<Player>().movementSpeed;
        _playerAnimationController = animator.GetComponent<PlayerAnimationController>();
        networkAnimator = animator.GetComponent<NetworkAnimator>();
        player = gameObject.GetComponent<Player>();
        if(IsClient && IsOwner)
        _playerAnimationController.OnShoot += OnShootCallback;

    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateServer();
        }

        if (IsClient && IsOwner)
        {
            DetectInput();
           
        }
        if (IsClient)
        {
            UpdateClient();
        }
    }

    private void OnShootCallback()
    {
        currentShootPoint = shootPoint.transform.position;
        ClientShootingServerRpc(_targetPos, OwnerClientId, currentShootPoint);
    }

    void OnSomeValueChange(MovementInterpolator.PositionInTime previous, MovementInterpolator.PositionInTime current)
    {
        tickCounter++;
        Debug.Log($"Time: {NetworkManager.Singleton.ServerTime.TimeAsFloat}, and tick nr: {tickCounter}");
        if(Application.isFocused)
        gameObject.GetComponent<MovementInterpolator>().HandleNewTick(current.Time, current.Position);
    }
    void UpdateServer()
    {

    }

    void UpdateServerTick()
    {
        if(playerMovementState.Value == PlayerMovementState.Stay)
        {
            mousePoint.Value = new Vector3(heroPos.Value.Position.x, mousePoint.Value.y, heroPos.Value.Position.y);
        }


        Vector3 point = new Vector3(mousePoint.Value.x, 1, mousePoint.Value.z);
        Vector2 movePoint = new Vector2(point.x, point.z);

        

        Vector2 currentPosition = Vector2.MoveTowards(new Vector2(heroPos.Value.Position.x, heroPos.Value.Position.y), movePoint, _moveSpeed);

        MovementInterpolator.PositionInTime currentPosTime = new MovementInterpolator.PositionInTime(NetworkManager.Singleton.ServerTime.TimeAsFloat, currentPosition);

        transform.position = new Vector3(currentPosition.x,1,currentPosition.y);

        if (heroPos.Value.Position.x == transform.position.x && heroPos.Value.Position.y == heroPos.Value.Position.y) animator.SetBool("IsMoving", false);
        else animator.SetBool("IsMoving", true);

        heroPos.Value = currentPosTime;

    }
    private void UpdateClient()
    {
        //Update Rotations
        UpdateRotation();
        

    }
    
    private void DetectInput()
    {
        if (Input.anyKey)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit raycasthit, float.MaxValue, layerMask))
                {
                    if (Input.GetMouseButtonDown(1) || Input.GetMouseButton(1))
                    {
                        Vector3 temp = transform.position;
                        temp.y = 0;
                    
                        Vector3 _relativepos = raycasthit.point - temp;
                        _mousePoint = raycasthit.point;

                        animator.SetBool("IsShooting", false);
                        ForceMovementStateServerRpc(PlayerMovementState.Move);
                        UpdateClientMousePointServerRpc(_mousePoint);
                    }
                    if (Input.GetMouseButtonDown(0) && !animator.GetBool("IsShooting"))
                    {
                        Vector3 temp = transform.position;
                        temp.y = 0;
                        Vector3 _relativepos = raycasthit.point - temp;

                        Vector3 relative = raycasthit.point - ray.origin;

                        
                        
                        _mousePoint = GetPointOnRay(ray.origin, relative, shootPoint.transform.position.y);
                        Vector3 targetPosition = _mousePoint;
                        _mousePoint.y = raycasthit.point.y;

                        UpdateRotation();

                        _targetPos = targetPosition - shootPoint.transform.position;
                        _targetPos = _targetPos.normalized;

                        

                        
                        ForceMovementStateServerRpc(PlayerMovementState.Stay);
                        ShootBroServerRpc(_mousePoint);
                        
                    }
                }
        }
    }
    [ServerRpc]
    public void ShootBroServerRpc(Vector3 _mousePoint)
    {
        mousePoint.Value = _mousePoint;

            animator.SetBool("IsShooting", true);

    }
    [ServerRpc]
    public void ForceMovementStateServerRpc(PlayerMovementState state)
    {
        playerMovementState.Value = state;
        //animator.SetBool("IsShooting", false);
    }
    [ServerRpc]
    public void UpdateClientMousePointServerRpc(Vector3 _mousePoint)
    {
        mousePoint.Value = _mousePoint;
    }

    [ServerRpc]
    public void ClientShootingServerRpc(Vector3 _direction, ulong playerId, Vector3 _shootPoint)
    {
        ObjectSpawner.Instance.SpawnObject(_shootPoint, _direction, playerId);
    }

    public void InitializeClientParams()
    {
        transform.position = new Vector3(0, 1, 0);
        moveSpeed.Value = _moveSpeed;
        heroPos.Value = new MovementInterpolator.PositionInTime(0f,new Vector2(SpawnPosition.x, SpawnPosition.y));
        heroRotation.Value = Quaternion.identity;
        playerMovementState.Value = PlayerMovementState.Stay;
        
    }
    //Return Vector3 point of ray between camera origin and raycasthitpoint where y define height where ray reaches this value.
    public Vector3 GetPointOnRay(Vector3 origin, Vector3 relative, float y)
    {
        Vector3 result;
        float x = ((2 - origin.y) / relative.y) * relative.x + origin.x;
        float z = ((2 - origin.y) / relative.y) * relative.z + origin.z;

        result = new Vector3(x, y, z);
        return result;
    }
    public void UpdateRotation()
    {
        Vector3 temp = transform.position;
        temp.y = 0f;
        Vector3 _relativepos = _mousePoint - temp;

        Quaternion LookAtRotation = Quaternion.LookRotation(_relativepos);

        if (_relativepos != Vector3.zero)
        {
            transform.rotation = LookAtRotation;
        }
    }
}
