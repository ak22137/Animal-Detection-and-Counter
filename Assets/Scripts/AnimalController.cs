using UnityEngine;

public class AnimalController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 30f;
    public float changeDirectionInterval = 3f;
    
    [Header("Boundaries")]
    public float boundaryRadius = 50f;
    public Vector3 centerPoint = Vector3.zero;
    
    [Header("Animal Info")]
    public string animalType = "Unknown";
    
    private Vector3 targetDirection;
    private float lastDirectionChangeTime;
    private Rigidbody rb;
    private bool isMoving = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Set up rigidbody for realistic movement
        rb.mass = 1f;
        rb.linearDamping = 2f;
        rb.angularDamping = 5f;
        
        // Set initial direction
        ChangeDirection();
        
        // Determine animal type from name or tag
        DetermineAnimalType();
    }
    
    public void Initialize(float speed, float rotSpeed, float changeInterval, float radius, Vector3 center)
    {
        moveSpeed = speed;
        rotationSpeed = rotSpeed;
        changeDirectionInterval = changeInterval;
        boundaryRadius = radius;
        centerPoint = center;
    }
    
    void Update()
    {
        if (isMoving)
        {
            HandleMovement();
            HandleRotation();
            CheckBoundaries();
            
            // Change direction periodically
            if (Time.time - lastDirectionChangeTime > changeDirectionInterval)
            {
                ChangeDirection();
            }
        }
    }
    
    void HandleMovement()
    {
        // Move forward in the current direction
        Vector3 movement = transform.forward * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }
    
    void HandleRotation()
    {
        // Gradually rotate towards target direction
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    void CheckBoundaries()
    {
        // Check if animal is outside boundary
        float distanceFromCenter = Vector3.Distance(transform.position, centerPoint);
        
        if (distanceFromCenter > boundaryRadius)
        {
            // Turn towards center
            Vector3 directionToCenter = (centerPoint - transform.position).normalized;
            targetDirection = directionToCenter;
            lastDirectionChangeTime = Time.time;
        }
    }
    
    void ChangeDirection()
    {
        // Generate random direction
        float randomAngle = Random.Range(0f, 360f);
        targetDirection = new Vector3(Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0, Mathf.Cos(randomAngle * Mathf.Deg2Rad));
        lastDirectionChangeTime = Time.time;
        
        // Occasionally stop moving
        if (Random.Range(0f, 1f) < 0.1f) // 10% chance to stop
        {
            isMoving = false;
            Invoke(nameof(StartMoving), Random.Range(1f, 3f));
        }
    }
    
    void StartMoving()
    {
        isMoving = true;
        ChangeDirection();
    }
    
    void DetermineAnimalType()
    {
        string objectName = gameObject.name.ToLower();
        
        if (objectName.Contains("human") || objectName.Contains("person") || objectName.Contains("man") || objectName.Contains("woman"))
        {
            animalType = "person";
        }
        else if (objectName.Contains("cow") || objectName.Contains("cattle"))
        {
            animalType = "cow";
        }
        else if (objectName.Contains("sheep") || objectName.Contains("lamb"))
        {
            animalType = "sheep";
        }
        else if (objectName.Contains("chicken") || objectName.Contains("bird"))
        {
            animalType = "chicken";
        }
        
        // Set tag for easier identification
        if (!string.IsNullOrEmpty(animalType))
        {
            try
            {
                gameObject.tag = animalType;
            }
            catch
            {
                // Tag might not exist, that's okay
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Avoid other animals by changing direction
        if (collision.gameObject.GetComponent<AnimalController>() != null)
        {
            Vector3 avoidDirection = (transform.position - collision.transform.position).normalized;
            targetDirection = avoidDirection;
            lastDirectionChangeTime = Time.time;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw movement direction
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, targetDirection * 2f);
        
        // Draw boundary
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerPoint, boundaryRadius);
    }
}
