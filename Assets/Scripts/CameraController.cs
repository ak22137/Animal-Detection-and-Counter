using UnityEngine;

public class CameraController : MonoBehaviour
{    [Header("Movement Settings")]
    public float movementSpeed = 5f;
    public float rotationSpeed = 3f; // Increased from 2f to 3f
    public float zoomSpeed = 2f;
    
    [Header("Key Bindings")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode upKey = KeyCode.Q;
    public KeyCode downKey = KeyCode.E;
    
    private Camera cameraComponent;
    private Vector3 lastMousePosition;
    
    void Start()
    {
        cameraComponent = GetComponent<Camera>();
        if (cameraComponent == null)
        {
            Debug.LogError("CameraController requires a Camera component!");
        }
    }
    
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }
    
    void HandleMovement()
    {
        Vector3 movement = Vector3.zero;
        
        // Forward/Backward
        if (Input.GetKey(forwardKey))
            movement += transform.forward;
        if (Input.GetKey(backwardKey))
            movement -= transform.forward;
        
        // Left/Right
        if (Input.GetKey(leftKey))
            movement -= transform.right;
        if (Input.GetKey(rightKey))
            movement += transform.right;
        
        // Up/Down
        if (Input.GetKey(upKey))
            movement += transform.up;
        if (Input.GetKey(downKey))
            movement -= transform.up;
        
        // Apply movement
        transform.position += movement * movementSpeed * Time.deltaTime;
    }
    
    void HandleRotation()
    {
        // Mouse look when right mouse button is held
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }
        
        if (Input.GetMouseButton(1))
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
            
            // Horizontal rotation (Y-axis)
            transform.Rotate(0, deltaMousePosition.x * rotationSpeed * Time.deltaTime, 0, Space.World);
            
            // Vertical rotation (X-axis)
            transform.Rotate(-deltaMousePosition.y * rotationSpeed * Time.deltaTime, 0, 0, Space.Self);
            
            lastMousePosition = Input.mousePosition;
        }
    }
    
    void HandleZoom()
    {
        // Scroll wheel zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && cameraComponent != null)
        {
            if (cameraComponent.orthographic)
            {
                cameraComponent.orthographicSize -= scroll * zoomSpeed;
                cameraComponent.orthographicSize = Mathf.Clamp(cameraComponent.orthographicSize, 1f, 20f);
            }
            else
            {
                cameraComponent.fieldOfView -= scroll * zoomSpeed * 10f;
                cameraComponent.fieldOfView = Mathf.Clamp(cameraComponent.fieldOfView, 10f, 120f);
            }
        }
    }
}
