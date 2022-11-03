using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetworkConfig : MonoBehaviour
{
    // Start is called before the first frame update

    private void Start()
    {
       //Application.targetFrameRate = 70;
       // QualitySettings.vSyncCount = 0;
        
        Setup();
    }
    private void Setup()
{
    NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
}

private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
{
    // The client identifier to be authenticated
    var clientId = request.ClientNetworkId;

    // Additional connection data defined by user code
    var connectionData = request.Payload;

    // Your approval logic determines the following values
    response.Approved = true;
    response.CreatePlayerObject = false;

    // The prefab hash value of the NetworkPrefab, if null the default NetworkManager player prefab is used
    response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = new Vector3(0, 1, 0);

    // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
    response.Rotation = Quaternion.identity;

    // If additional approval steps are needed, set this to true until the additional steps are complete
    // once it transitions from true to false the connection approval response will be processed.
    response.Pending = false;
}
}
