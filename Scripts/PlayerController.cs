using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject shootPoint;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private NetworkVariable<float> moveSpeed = new NetworkVariable<float>();


    private NetworkVariable<MovementInterpolator.PositionInTime> heroPos = new NetworkVariable<MovementInterpolator.PositionInTime>();

    [SerializeField]
    private NetworkVariable<Quaternion> heroRotation = new NetworkVariable<Quaternion>();

    [SerializeField]
    public NetworkVariable<Vector3> mousePoint = new NetworkVariable<Vector3>();

    private NetworkVariable<bool> IsTick = new NetworkVariable<bool>();

    private bool OldTick;


    [SerializeField]
    public static Vector3 SpawnPosition = new Vector3(0, 1, 0);

    [SerializeField]
    private Animator animator;

    private float _moveSpeed;

    private Vector3 _mousePoint;

    private int tickCounter;

    private void Awake()
    {

    }
    


    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
           // systemTick = new SystemTick();
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
        _moveSpeed = gameObject.GetComponent<Player>().movementSpeed;
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
       
        Vector3 point = new Vector3(mousePoint.Value.x, 1, mousePoint.Value.z);
        Vector2 movePoint = new Vector2(point.x, point.z);

        

        Vector2 currentPosition = Vector2.MoveTowards(new Vector2(heroPos.Value.Position.x, heroPos.Value.Position.y), movePoint, _moveSpeed);

        MovementInterpolator.PositionInTime currentPosTime = new MovementInterpolator.PositionInTime(NetworkManager.Singleton.ServerTime.TimeAsFloat, currentPosition);

        

        transform.position = new Vector3(currentPosition.x,1,currentPosition.y);

        if (heroPos.Value.Position.x == transform.position.x && heroPos.Value.Position.y == heroPos.Value.Position.y) animator.SetBool("IsMoving", false);
        else animator.SetBool("IsMoving", true);

        heroPos.Value = currentPosTime;
 
        
        
        //TickCallClientRpc(NetworkManager.Singleton.ServerTime.TimeAsFloat, transform.position);
    }
    private void UpdateClient()
    {
        
        
        //systemTick.Refresh(Time.deltaTime);


        //Update Rotations
       
        Vector3 temp = transform.position;
        temp.y = 0f;
        Vector3 _relativepos = mousePoint.Value - temp;
        

        Quaternion LookAtRotation = Quaternion.LookRotation(_relativepos);



        if (_relativepos != Vector3.zero)
        {
            transform.rotation = LookAtRotation;
        }
        
       

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
                    
                        UpdateClientMousePointServerRpc(_mousePoint);
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector3 temp = transform.position;
                        temp.y = 0;
                        Vector3 _relativepos = raycasthit.point - temp;

                        ClientShootingServerRpc(_relativepos.normalized,OwnerClientId);
                    }
                }
            
            
        }
        
    }

    [ServerRpc]
    public void UpdateClientMousePointServerRpc(Vector3 _mousePoint)
    {
        mousePoint.Value = _mousePoint;
    }
    [ServerRpc]
    public void ClientShootingServerRpc(Vector3 _direction, ulong playerId)
    {
        ObjectSpawner.Instance.SpawnObject(shootPoint.transform.position, _direction, playerId);
    }



    public void InitializeClientParams()
    {
        transform.position = new Vector3(0, 1, 0);
        moveSpeed.Value = _moveSpeed;
        heroPos.Value = new MovementInterpolator.PositionInTime(0f,new Vector2(SpawnPosition.x, SpawnPosition.y));
        heroRotation.Value = Quaternion.identity;
        
    }
    
    [ClientRpc]
    //public void TickCallClientRpc(float time, Vector3 position)
    public void TickCallClientRpc(float time, Vector3 position)
    {

        //systemTick.HandleNewTick(NetworkManager.Singleton.ServerTime.TimeAsFloat, position);
   
        //Debug.Log($"New tick time is: {time}");
        Debug.Log($"New tick time arrived at server: {NetworkManager.Singleton.ServerTime.TimeAsFloat}");
        //Debug.Log($"New tick time arrived at local: {NetworkManager.Singleton.LocalTime.TimeAsFloat}");


    }
}
