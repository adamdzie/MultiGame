using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CameraController : NetworkBehaviour
{
    public static CameraController instance;

    public Transform target;
    public Vector3 offset;
    public bool freezeCamera = true;
    public float cameraSpeed = 1f;
    public float edgeOffset = 40f;
    private string cameraDirection = "";

    Vector3 worldPosition;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        

        
    }
    void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = target.transform.position + offset;
            Vector3 relativePos = target.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(relativePos);
        }

        

    }
    private void Update()
    {
        detectEdgeMousePosition();
        
        
    }
    void LateUpdate()
    {

        cameraMovement();

        
    }
    void cameraMovement()
    {
        if (freezeCamera && IsOwner && IsClient)
        {
            transform.position = target.transform.position + offset;
            return;
        }
        if (cameraDirection == "_left") transform.position += Vector3.left * cameraSpeed;
        if (cameraDirection == "_right") transform.position += Vector3.right * cameraSpeed;
        if (cameraDirection == "_forward") transform.position += Vector3.forward * cameraSpeed;
        if (cameraDirection == "_back") transform.position += Vector3.back * cameraSpeed;
        if (cameraDirection == "_right_forward") transform.position += (Vector3.right + Vector3.forward) * cameraSpeed;
        if (cameraDirection == "_right_back") transform.position += (Vector3.right + Vector3.back) * cameraSpeed;
        if (cameraDirection == "_left_forward") transform.position += (Vector3.left + Vector3.forward) * cameraSpeed;
        if (cameraDirection == "_left_back") transform.position += (Vector3.left + Vector3.back) * cameraSpeed;
    }
    void detectEdgeMousePosition()
    {
        cameraDirection = "";

        if (Input.mousePosition.x < edgeOffset && Input.mousePosition.x < Screen.width - edgeOffset) cameraDirection += "_left";
        if (Input.mousePosition.x > Screen.width - edgeOffset && Input.mousePosition.x >  edgeOffset) cameraDirection += "_right";
        if (Input.mousePosition.y > Screen.height - edgeOffset && Input.mousePosition.y > edgeOffset) cameraDirection += "_forward";
        if (Input.mousePosition.y < edgeOffset && Input.mousePosition.y < Screen.height - edgeOffset) cameraDirection += "_back";
    }
}
