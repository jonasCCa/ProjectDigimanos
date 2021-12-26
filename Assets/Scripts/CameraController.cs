using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Object References")]
    public GameObject gameCamera;
    public GameObject cameraTarget;
    [SerializeField] private MultiplayerManager manager;

    [Header("Camera Configs")]
    public float singlePlayerDistance = 5f;
    public float distanceRatio = .5f;
    public float maxCameraDistance = 10f;
    public float heightRatio = .5f;
    public float maxHeightAngle = 10f;
    public float cameraSmoothTime = 0.2f;
    public float cameraMaxSpeed = 50f;
    public Vector3 cameraOffset = new Vector3(0,3.15f,-3.99f);
    
    [Header("Read Only")]
    [SerializeField] private bool isSinglePlayer = true;
    [SerializeField] private float longestPlayerDistance;
    [SerializeField] private float heightAngle;
    [SerializeField] private Vector3 targetVelocity = Vector3.zero;
    [SerializeField] private Vector3 cameraVelocity = Vector3.zero;
    [SerializeField] private Vector3 cameraTargetPosition = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        gameCamera = GameObject.Find("MainCamera");
        manager = GetComponent<MultiplayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        // Calculates camera target position
        cameraTargetPosition = manager.calculateTargetPosition();

        heightAngle = Mathf.Clamp(manager.calculatePlayerHeightDifference() * heightRatio, 0, maxHeightAngle);

        // Smooths camera target position
        cameraTarget.transform.position = Vector3.SmoothDamp(cameraTarget.transform.position, cameraTargetPosition,
                                                            ref targetVelocity, cameraSmoothTime, cameraMaxSpeed, Time.deltaTime);
        
        // Smooths camera position
        gameCamera.transform.position = Vector3.SmoothDamp(gameCamera.transform.position, calculateCameraPosition() + cameraOffset + new Vector3(0,heightAngle,0),
                                                            ref cameraVelocity, cameraSmoothTime, cameraMaxSpeed, Time.deltaTime);
        // Points camera to target
        gameCamera.transform.LookAt(cameraTarget.transform);
    }

    Vector3 calculateCameraPosition() {
        Vector3 auxResult = new Vector3();

        // If it's SinglePlayer, use single player distance, if else use distance ratio
        if(isSinglePlayer)
            auxResult = Vector3.ClampMagnitude(gameCamera.transform.forward*(singlePlayerDistance), maxCameraDistance);
        else {
            // Calculates the longest distance between players
            longestPlayerDistance = manager.calculatePlayerDistance();

            auxResult = Vector3.ClampMagnitude(gameCamera.transform.forward*(longestPlayerDistance * distanceRatio), maxCameraDistance);
        }
        return cameraTarget.transform.position - auxResult;
    }

    public void setSinglePlayerFlag(bool value) {
        isSinglePlayer = value;
    }

    // Draws lines between centroid and players
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        foreach(GameObject player in manager.GetPlayerList()) {
            Gizmos.DrawLine(cameraTarget.transform.position, player.transform.position);
        }
    }
}
