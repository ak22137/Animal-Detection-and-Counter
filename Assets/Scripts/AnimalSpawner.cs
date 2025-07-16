using UnityEngine;
using System.Collections.Generic;

public class AnimalSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject humanPrefab;
    public GameObject cowPrefab;
    public GameObject sheepPrefab;
    public GameObject chickenPrefab;
    
    [Header("Spawn Settings")]
    public int maxAnimals = 20;
    public float spawnRadius = 50f;
    public Vector3 spawnCenter = Vector3.zero;
    public LayerMask groundLayer = 1;
    
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 30f;
    public float changeDirectionInterval = 3f;
    
    private List<GameObject> spawnedAnimals = new List<GameObject>();
    private List<AnimalController> animalControllers = new List<AnimalController>();
    
    void Start()
    {
        SpawnAnimals();
    }
    
    void SpawnAnimals()
    {
        // Spawn different types of animals randomly
        for (int i = 0; i < maxAnimals; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject animalPrefab = GetRandomAnimalPrefab();
            
            if (animalPrefab != null)
            {
                GameObject spawnedAnimal = Instantiate(animalPrefab, spawnPosition, Quaternion.identity);
                spawnedAnimals.Add(spawnedAnimal);
                
                // Add animal controller if it doesn't exist
                AnimalController controller = spawnedAnimal.GetComponent<AnimalController>();
                if (controller == null)
                {
                    controller = spawnedAnimal.AddComponent<AnimalController>();
                }
                
                controller.Initialize(moveSpeed, rotationSpeed, changeDirectionInterval, spawnRadius, spawnCenter);
                animalControllers.Add(controller);
            }
        }
        
        Debug.Log($"Spawned {spawnedAnimals.Count} animals");
    }
    
    Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPosition;
        int attempts = 0;
        
        do
        {
            // Generate random position within spawn radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            randomPosition = spawnCenter + new Vector3(randomCircle.x, 0, randomCircle.y);
            
            // Raycast to find ground
            RaycastHit hit;
            if (Physics.Raycast(randomPosition + Vector3.up * 100, Vector3.down, out hit, 200f, groundLayer))
            {
                randomPosition = hit.point;
                break;
            }
            
            attempts++;
        } while (attempts < 50);
        
        return randomPosition;
    }
    
    GameObject GetRandomAnimalPrefab()
    {
        List<GameObject> availablePrefabs = new List<GameObject>();
        
        if (humanPrefab != null) availablePrefabs.Add(humanPrefab);
        if (cowPrefab != null) availablePrefabs.Add(cowPrefab);
        if (sheepPrefab != null) availablePrefabs.Add(sheepPrefab);
        if (chickenPrefab != null) availablePrefabs.Add(chickenPrefab);
        
        if (availablePrefabs.Count == 0)
        {
            Debug.LogWarning("No animal prefabs assigned to AnimalSpawner!");
            return null;
        }
        
        return availablePrefabs[Random.Range(0, availablePrefabs.Count)];
    }
    
    public void RespawnAnimals()
    {
        ClearAnimals();
        SpawnAnimals();
    }
    
    public void ClearAnimals()
    {
        foreach (GameObject animal in spawnedAnimals)
        {
            if (animal != null)
                DestroyImmediate(animal);
        }
        
        spawnedAnimals.Clear();
        animalControllers.Clear();
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnCenter, spawnRadius);
    }
}
