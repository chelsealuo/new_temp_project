using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockBehavior : MonoBehaviour
{
    // Field of View: Should not detect neighbours behind this fish
    [SerializeField] private float FOVAngle;    // Field of view of the current fish
    [SerializeField] private float smoothDamp;    // lower value will rotates faster
    [SerializeField] private LayerMask obstacleMask;    // layer to avoid
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles;
    
    private List<FlockBehavior> cohesionNeighbors = new List<FlockBehavior>();
    private List<FlockBehavior> alignmentNeighbors = new List<FlockBehavior>();
    private List<FlockBehavior> avoidanceNeighbors = new List<FlockBehavior>();
    
    private FlockManager flockManager;
    private Vector3 currentVelocity;
    private Vector3 currentObstacleAvoidanceVector;
    private float speed;
    
    public Transform myTransform { get; set; }
    
    // private float speed;
    private void Awake()
    {
        myTransform = transform;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    public void AssignFlockManager(FlockManager flockManager)
    {
        this.flockManager = flockManager;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }
    
    public void MoveFish()
    {
        FindNeighbours();
        CalculateSpeed();
        
        var cohesionVector = CalculateCohesionVector() * flockManager.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * flockManager.avoidanceWeight;
        var alignmentVector = CalculateAlignmentVector() * flockManager.alignmentWeight;
        var boundsVector = CalculateBoundsVector() * flockManager.boundsWeight;
        var obstacleVector = CalculateObstacleVector() * flockManager.obstacleWeight;
        
        var moveVector = cohesionVector + avoidanceVector + alignmentVector + boundsVector + obstacleVector;
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
            moveVector = transform.forward;
        
        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;
    }

    // Calculate the speed of current fish by its neighbors.
    private void CalculateSpeed()
    {
        if (cohesionNeighbors.Count == 0)
            return;
        speed = 0;
        for (int i = 0; i < cohesionNeighbors.Count; i++)
        {
            speed += cohesionNeighbors[i].speed;
        }

        speed /= cohesionNeighbors.Count;
        speed = Mathf.Clamp(speed, flockManager.minSpeed, flockManager.maxSpeed);
    }

    // Find local neighbors of the current fish.
    private void FindNeighbours()
    {
        cohesionNeighbors.Clear();
        avoidanceNeighbors.Clear();
        alignmentNeighbors.Clear();
        
        var allFish = flockManager.allFish;
        for (int i = 0; i < allFish.Length; i++)
        {
            var currentFish = allFish[i];
            if (currentFish != this)
            {
                float currentNeighbourDistanceSqr =
                    Vector3.SqrMagnitude(currentFish.myTransform.position - myTransform.position);
                if (currentNeighbourDistanceSqr <= flockManager.cohesionDistance * flockManager.cohesionDistance)
                {
                    cohesionNeighbors.Add(currentFish);
                }
                if (currentNeighbourDistanceSqr <= flockManager.avoidanceDistance * flockManager.avoidanceDistance)
                {
                    avoidanceNeighbors.Add(currentFish);
                }
                if (currentNeighbourDistanceSqr <= flockManager.alignmentDistance * flockManager.alignmentDistance)
                {
                    alignmentNeighbors.Add(currentFish);
                }
            }
        }
    }
    
    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;
        
        // No fish in the Field of View
        if (cohesionNeighbors.Count == 0)
            return cohesionVector;
        
        // Check if there are any fish in the Field of View
        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbors.Count; i++)
        {
            if (IsInFOV(cohesionNeighbors[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbors[i].myTransform.position;
            }
        }
        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;    // World to local position
        return cohesionVector;
    }
    
    private Vector3 CalculateAlignmentVector()
    {
        var alignmentVector = myTransform.forward;
        if (alignmentNeighbors.Count == 0)
            return alignmentVector;
        int neighboursInFOV = 0;
        for (int i = 0; i < alignmentNeighbors.Count; i++)
        {
            if (IsInFOV(alignmentNeighbors[i].myTransform.position))
            {
                neighboursInFOV++;
                alignmentVector += alignmentNeighbors[i].myTransform.forward;
            }
        }
        alignmentVector /= neighboursInFOV;
        alignmentVector = alignmentVector.normalized;
        return alignmentVector;

    }

    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;
        if (avoidanceNeighbors.Count == 0)
            return avoidanceVector;
        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbors.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbors[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbors[i].myTransform.position);
            }
        }
        avoidanceVector /= neighboursInFOV;
        avoidanceVector = avoidanceVector.normalized;
        return avoidanceVector;
    }
    
    private Vector3 CalculateBoundsVector()
    {
        var offsetToCenter = flockManager.transform.position - myTransform.position;
        bool isNearCenter = (offsetToCenter.magnitude >= flockManager.boundsDistance * 0.9f);
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }
    
    // Use Raycast to check if the fish will hit the obstacle on the obstacle layer.
    private Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, flockManager.obstacleDistance, obstacleMask))
        {
            obstacleVector = FindDirectionToAvoidObstacle();
        }
        else
        {
            currentObstacleAvoidanceVector = Vector3.zero;
        }
        
        return obstacleVector;
    }

    // Iterate though possible directions to turn to.
    // Return a direction that is furthest away from the obstacle.  
    private Vector3 FindDirectionToAvoidObstacle()
    {
        if (currentObstacleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, flockManager.obstacleDistance,
                obstacleMask))
            {
                return currentObstacleAvoidanceVector;
            }
        }
        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;
        for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++)
        {
            RaycastHit hit;
            var currentDirection = myTransform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);
            if (Physics.Raycast(myTransform.position, currentDirection, out hit, flockManager.obstacleDistance, obstacleMask))
            {
                float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
                if (currentDistance > maxDistance)
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                selectedDirection = currentDirection;
                currentObstacleAvoidanceVector = currentDirection.normalized;
                return selectedDirection.normalized;
            }
        }
        return selectedDirection.normalized;
    }
    
    // Check if other fish is in the field of view of the current fish.
    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }
}
