using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LocomotionSwim : MonoBehaviour
{
    [SerializeField] private float swimmingForce;
    [SerializeField] private float resistanceForce;
    [SerializeField] private float deadZone;
    [SerializeField] private Transform trackingSpace;
    private new Rigidbody playerRB;
    private Vector3 currentDirection;



    public Transform LeftHand;
    public Transform RightHand;
    public Transform player;

    Vector3 prev;
    Vector3 cur;
    Vector3 target;
    Vector3 direction;
    bool pressing;
    bool moving;
    // Start is called before the first frame update
    private void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        pressing = false;
        moving = false;
        //playerRb = playerObj.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (pressing)
            {
                cur = Vector3.Lerp(LeftHand.position, RightHand.position, 0.5f);
                if (Vector3.Distance(cur, prev) > 0.0f)
                {
                    target += (prev - cur)*100f;
                    moving = true;
                    prev = cur;
                }

            }
            else
            {
                prev = Vector3.Lerp(LeftHand.position, RightHand.position, 0.5f);
                pressing = true;
                moving = true;
            }

        }
        else
        {
            pressing = false;
        }
        
        if (!moving)
        {
            target = player.position;
        }
        else
        {
            player.position = Vector3.MoveTowards(player.position, target, 2.0f * Time.deltaTime);
            if (Vector3.Distance(player.position, target) < 0.001f)
                moving = false; 
        }  */

    }

    private void FixedUpdate()
    {
        if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch)) &&
           ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))))
        {
            Vector3 localVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch) + OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            localVelocity *= -1.0f;
            if (localVelocity.sqrMagnitude > deadZone * deadZone)
            {
                Vector3 worldSpaceVelocity =trackingSpace.TransformDirection(localVelocity);
                playerRB.AddForce(worldSpaceVelocity * swimmingForce, ForceMode.Acceleration);
                currentDirection = worldSpaceVelocity.normalized;
            }

        }
        if (playerRB.velocity.sqrMagnitude > 0.01f && currentDirection != Vector3.zero)
        {
            playerRB.AddForce(-playerRB.velocity * resistanceForce, ForceMode.Acceleration);
        }
        else
        {
            currentDirection = Vector3.zero;
        }
    }
}
