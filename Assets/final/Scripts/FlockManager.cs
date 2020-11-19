using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    [Header("Fish Spawn Setting")]
    [SerializeField] private FlockBehavior fishPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;    // Swim Limit: Box around flock manager
    
    
    [Header("Speed")] 
    [Range(0.0f, 10f)] [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0.0f, 10f)] [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }
    
    
    [Header("Detection Distances")]
    [Range(0.0f, 10f)] [SerializeField] private float _cohesionDistance;
    public float cohesionDistance { get { return _cohesionDistance; } }
    
    [Range(0.0f, 10f)] [SerializeField] private float _avoidanceDistance;
    public float avoidanceDistance { get { return _avoidanceDistance; } }
    
    [Range(0.0f, 10f)] [SerializeField] private float _alignmentDistance;
    public float alignmentDistance { get { return _alignmentDistance; } }
    
    [Range(0.0f, 100f)] [SerializeField] private float _boundsDistance;
    public float boundsDistance { get { return _boundsDistance; } }
    
    [Range(0.0f, 100f)] [SerializeField] private float _obstacleDistance;
    public float obstacleDistance { get { return _obstacleDistance; } }
    
    
    [Header("Behavior Weights")]
    [Range(0.0f, 10f)] [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }
    
    [Range(0.0f, 10f)] [SerializeField] private float _avoidanceWeight;
    public float avoidanceWeight { get { return _avoidanceWeight; } }
    
    [Range(0.0f, 10f)] [SerializeField] private float _alignmentWeight;
    public float alignmentWeight { get { return _alignmentWeight; } }
    
    [Range(0.0f, 10f)] [SerializeField] private float _boundsWeight;
    public float boundsWeight { get { return _boundsWeight; } }
    
    [Range(0.0f, 10f)] [SerializeField] private float _obstacleWeight;
    public float obstacleWeight { get { return _obstacleWeight; } }
    
    
    
    public FlockBehavior[] allFish { get; set;}
    
    
    void Start()
    {
        GenerateFishFlock();
    }

    void Update()
    {
        for (int i = 0; i < allFish.Length; i++)
        {
            allFish[i].MoveFish();
        }
    }
    
    private void GenerateFishFlock()
    {
        allFish = new FlockBehavior[flockSize];
        
        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var spawnRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            allFish[i] = Instantiate(fishPrefab, spawnPosition, spawnRotation);
            allFish[i].AssignFlockManager(this);
            allFish[i].InitializeSpeed(Random.Range(minSpeed, maxSpeed));
        }
    }
}
