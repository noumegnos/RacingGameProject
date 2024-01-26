using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the purpose of this script is to control the basic traffic cars, allowing them to follow the track by going from checkpoint to checkpoint
//it also handles the traffic cars physics and orientation so the model follows the track
//many of these things are taken from the main PlayerController script

//steering is an early test of the non-smart AI
//mainly, this car will attempt to steer toward the next checkpoint, offset to the right by some value so they drive on one side of the road
//it has a max speed value it travels at, but if there is another traffic car in front, it will not exceed that one's speed
//additionally, if a player ship approaches, the traffic car should either steer suddenly to a side, or break hard

//so first of all lets make it align to track and travel forward

//so I tried to implement only the relevant parts of the PlayerController, thinking I could simplify it
//but it seems not so easy, as there is quite a lot of important stuff
//instead I have to copy it, and start removing what doesn't belong
//working on TrafficController instead

public class CarController : MonoBehaviour
{

    public float speed;

    //system for aligning to track, lifted from player controller
    public Vector3 groundDirection;
    public Transform frontGroundRaycast;
    public Transform rightGroundRaycast;
    public Transform leftGroundRaycast;
    public Transform rearGroundRaycast;

    Vector3 groundAngleRight;
    Vector3 groundAngleLeft;
    Vector3 groundAngleRear;
    Vector3 groundAngleFront;

    public float groundRange;

    public Rigidbody rbody;

    public Vector3 velocity;
    public Vector3 movement;
    Vector3 rbposition;

    Quaternion rotation;
    
    float deltaTime;

    public LayerMask Ground;

    public bool grounded;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate(){

        rbposition = rbody.position;

        deltaTime = Time.deltaTime;


        movement = velocity * deltaTime;
        rbody.MovePosition(rbposition + movement);


    }

    void LateUpdate(){

        GroundChecking();

        Alignment();

        Gravity();
    }


    //so in order to make the car go, a speed value is added to the forward axis of the car's rigidbody velocity
    void Drive(){

    }

    //steering might be a little complex, and there may be a lot more needed to make it ok
    //the basic version just takes the next checkpoint ahead, adds the offset to make car drive on the right side, and steers towards this point
    void Steering(){

    }


    //gravity is very simplified here, merely makes sure the car doesn't just float away
    //gravit is added to the Ridigbody when the car is not touching the ground
    void Gravity(){

        if(!grounded){
            velocity += -transform.up * (9.8f) * deltaTime;
        }        

    }

    //this system for checking the ground's normal angles should be a simple version, though I'm not sure how it actually compares to the advanced one in PlayerController
    public void GroundChecking(){

        //Raycast Hits, this will get filled in with data from the ground object, especially the normal angle which allows us to align the car to the track
        RaycastHit hitF;
        RaycastHit hitB;
        RaycastHit hitL;
        RaycastHit hitR;

        //Raycast Rays define the ray which is used to check information from the object it hits, in this case it hits the ground object
        Ray rayF = new Ray(frontGroundRaycast.position, -transform.up);
        Ray rayB = new Ray(rightGroundRaycast.position, -transform.up);
        Ray rayL = new Ray(leftGroundRaycast.position, -transform.up);
        Ray rayR = new Ray(rearGroundRaycast.position, -transform.up);

        bool hit1;
        bool hit2;
        bool hit3;
        bool hit4;

        //5f is the range, just a randomly picked number which should be plenty, though it may be more than needed but doesn't matter too much 
        //it does actually matter somewhat, since we should make gravity only apply in case the car isn't touching the ground, so the range below should be as close to the ground as possible
        if (Physics.Raycast(rayL, out hitL, groundRange, Ground)){
            groundAngleLeft = hitL.normal;

            hit1 = true;
        } else {
            hit1 = false;
        }


        if (Physics.Raycast(rayR, out hitR, groundRange, Ground)){
            groundAngleRight = hitR.normal;

            hit2 = true;
        } else{
            hit2 = false;
        }

        if (Physics.Raycast(rayB, out hitB, groundRange, Ground)){
            groundAngleRear = hitB.normal;

            hit3 = true;
        }
        else{
            hit3 = false;
        }

        if (Physics.Raycast(rayF, out hitF, groundRange, Ground)){
            groundAngleFront = hitF.normal;

            hit4 = true;
        }
        else{
            hit4 = false;
        }

        if(hit1|hit2|hit3|hit4){
            grounded = true;
        }
        else{
            grounded = false;
        }

    } 

    //this aligns the whole gameobject with the ground
    public void Alignment()
    {

        groundDirection = groundAngleLeft + groundAngleRear + groundAngleRight + groundAngleFront;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundDirection) * transform.rotation;
        //rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSmooth * Time.deltaTime);
        rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100f);
        transform.rotation = rotation;

    }
}
