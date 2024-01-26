using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//based on Kart Example, this script handles taking input and calculating velocity of the ship

public class TrafficController : MonoBehaviour
{
    //current checkpoint int is updated every time the racer crosses a checkpoint
    public int currentCheckpoint = 0;
    //here are some stats, which could be separated out of here into some kind of stat sheet that each player/ship has
    //public float speed = 1f;
    float acceleration = 1f;
    public float accelerationStat = 1f;
    public float reverseAcceleration = 1f;
    public float reverseSpeed = 1f;
    public float currentMaxSpeed = 20f; //current max speed, which is affected by modifiers on the track, and which increases to maxSpeed over time
    public float maxSpeed = 50f; //real maximum speed, above which ship cannot go
    public float speedMod = 0f; //added to currentMaxSpeed and accelerationStat when hit speed boost
    public float slowMod = 1f; //currentSpeed and accelerationStat is divided by slowMod when hit slow thing
    public float coastingDrag = 1f;
    public float braking = 1f;
    public float grip = 1f; //100 was good testing value ALSO 10 is fine
    public float weight = 1f;

    public bool _isGrounded = true;

    public LayerMask Ground;
    public LayerMask allCollidingLayers;
    public LayerMask Checkpoint;


    public Rigidbody _body;

    public float gravity = 9.8f;
    public float gravityMod = 1f;

    public float input;

    public bool stopped = false;

    public float maxSteer;
    float steer;

    Vector3 _bodyPosition;
    Vector3 _bodyMovement; 
    public Vector3 _velocity;

    public Transform frontGroundRaycast;
    public Transform rightGroundRaycast;
    public Transform leftGroundRaycast;
    public Transform rearGroundRaycast;

    public Transform leftSideRaycast;
    public Transform rightSideRaycast;
    bool leftSideHit;
    bool rightSideHit;

    public GroundInfo m_CurrentGroundInfo;

    CapsuleCollider _cap;
    MeshCollider _mCol;

    Quaternion m_DriftOffset = Quaternion.identity;
    Vector3 lastKnownGroundInfoNormal = Vector3.zero;

    public float k_Deadzone = 0.01f;

    RaycastHit[] m_RaycastHitBuffer = new RaycastHit[16];

    //temporary like this; think these are the same
    public bool rotating = false;

    //when backing up this will reverse steering, which makes sense if we have wheels
    //probably still needed even if we're driving an anti-gravity spaceship, simply because it is more intuitive
    float forwardReverseSwitch;

    //wutr
    Collider[] m_ColliderBuffer = new Collider[8];



    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();

        //_cap = GetComponent<CapsuleCollider>();
        _mCol = GetComponent<MeshCollider>();

    }

    // Update is called once per frame
    void Update()
    {

        input = FollowCheckpointNormals();
        



        acceleration = 1f;


        // if (!stopped)
        // {

        //     if (Input.GetKey(KeyCode.UpArrow))
        //     {
        //         acceleration = 1f;

        //     }
        //     else if (Input.GetKey(KeyCode.DownArrow))
        //     {
        //         acceleration = -1f;
        //     }
        //     else
        //     {
        //         acceleration = 0f;
        //     }
        // }

    }

    //this script needs to check the checkpoint it can see ahead and get its normal
    //then, it will compare this normal to its own forward vector
    //and decide if the ships should steer left or right
    //then it returns this as the input for steering in the form of 1 to -1
    float FollowCheckpointNormals(){

        RaycastHit hitF;
        Ray rayF = new Ray(frontGroundRaycast.position, transform.forward);

        // if (Physics.Raycast(rayF, out hitF, 10f, Checkpoint)){

        //     //at 180 degrees angle the normal and transform.forward are pointing at each other
        //     if(Vector3.Angle(hitF.normal, -transform.forward) > 0){
        //         print(" sqwueek " + Vector3.Angle(hitF.normal, transform.forward));
        //         return -1f;
        //     }
        //     else if(Vector3.Angle(hitF.normal, -transform.forward) < 0){
        //         print(" squawk " + Vector3.Angle(hitF.normal, transform.forward));
        //         return 1f;

        //     }
        //     else{
        //         return 0f;
        //     }
        // }

        // return 0f;

        if (Physics.Raycast(rayF, out hitF, 10f, Checkpoint)){

            Vector3 targetDir = hitF.normal;

            Vector3 forward = transform.forward;

            float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

            if (angle < -1.0F)
                return -1f;
            else if (angle > 1.0F)
                return 1f;
            else
                return 0f;

        }

        return 0f;

        // = target.position - transform.position;





    }

    void LateUpdate()
    {


    }

    private void FixedUpdate()
    {

        //KART!
        _bodyPosition = _body.position;

        Quaternion rotationStream = _body.rotation;

        float deltaTime = Time.deltaTime;

        m_CurrentGroundInfo = GetGroundInfo(deltaTime, rotationStream, Vector3.zero);

        _isGrounded = m_CurrentGroundInfo.isGrounded;

        GroundInfo nextGroundInfo = GetGroundInfo(deltaTime, rotationStream, _velocity * deltaTime);

        GroundNormal(deltaTime, ref rotationStream, m_CurrentGroundInfo, nextGroundInfo);

        TurnShip(deltaTime, ref rotationStream);


        CalculateDrivingVelocity(deltaTime, m_CurrentGroundInfo, rotationStream);

        Vector3 penetrationOffset = SolvePenetration(rotationStream);
        penetrationOffset = ProcessVelocityCollisions(deltaTime, rotationStream, penetrationOffset);


        rotationStream = Quaternion.RotateTowards(_body.rotation, rotationStream, 100f);
        //rotationStream = Quaternion.RotateTowards(_body.rotation, rotationStream, 100f * deltaTime);

        AdjustVelocityByPenetrationOffset(deltaTime, ref penetrationOffset);


        _bodyMovement = _velocity * deltaTime;

        _body.MoveRotation(rotationStream);
        _body.MovePosition(_bodyPosition + _bodyMovement);
        //KART!

    }


    //GroundInfo is a struct of 4 things; the 3 bools isCapsuleTouching, isGrounded, isCloseToGround, and the normal of the ground
    //this data is gathered by a series of raycasts, which continuously check the ground below the ship
    //the function is run twice every frame, to gather current and next groundInfo
    GroundInfo GetGroundInfo(float deltaTime, Quaternion rotationStream, Vector3 offset)
    {
        GroundInfo groundInfo = new GroundInfo();
        Vector3 defaultPosition = offset + _velocity * deltaTime;
        Vector3 direction = rotationStream * Vector3.down;

        //changing caps to mesh collider, hope it works
        //float capsuleRadius = _mCol.radius;
        float capsuleRadius = 1f;
        float capsuleTouchingDistance = capsuleRadius + Physics.defaultContactOffset;
        float groundedDistance = capsuleTouchingDistance + 0.225f;
        float closeToGroundDistance = Mathf.Max(groundedDistance + capsuleRadius, _velocity.y);

        int hitCount = 0;

        Ray ray = new Ray(defaultPosition + frontGroundRaycast.position, direction);

        bool didHitFront = GetNearestFromRaycast(ray, closeToGroundDistance, Ground, QueryTriggerInteraction.Ignore, out RaycastHit frontHit);
        if (didHitFront)
            hitCount++;

        ray.origin = defaultPosition + rightGroundRaycast.position;

        bool didHitRight = GetNearestFromRaycast(ray, closeToGroundDistance, Ground, QueryTriggerInteraction.Ignore, out RaycastHit rightHit);
        if (didHitRight)
            hitCount++;

        ray.origin = defaultPosition + leftGroundRaycast.position;

        bool didHitLeft = GetNearestFromRaycast(ray, closeToGroundDistance, Ground, QueryTriggerInteraction.Ignore, out RaycastHit leftHit);
        if (didHitLeft)
            hitCount++;

        ray.origin = defaultPosition + rearGroundRaycast.position;

        bool didHitRear = GetNearestFromRaycast(ray, closeToGroundDistance, Ground, QueryTriggerInteraction.Ignore, out RaycastHit rearHit);
        if (didHitRear)
            hitCount++;

        groundInfo.isCapsuleTouching = frontHit.distance <= capsuleTouchingDistance || rightHit.distance <= capsuleTouchingDistance || leftHit.distance <= capsuleTouchingDistance || rearHit.distance <= capsuleTouchingDistance;
        groundInfo.isGrounded = frontHit.distance <= groundedDistance || rightHit.distance <= groundedDistance || leftHit.distance <= groundedDistance || rearHit.distance <= groundedDistance;
        groundInfo.isCloseToGround = hitCount > 0;
        //print("ground" + groundInfo.isGrounded);

        if (hitCount == 0)
        {
            groundInfo.normal = Vector3.up; //this appears to turn the ship in the direction of 0,0
        }
        else if (hitCount == 1)
        {
            if (didHitFront)
            {
                groundInfo.normal = frontHit.normal;
                lastKnownGroundInfoNormal = groundInfo.normal;
            }
            else if (didHitRight)
            {
                groundInfo.normal = rightHit.normal;
                lastKnownGroundInfoNormal = groundInfo.normal;
            }
            else if (didHitLeft)
            {
                groundInfo.normal = leftHit.normal;
                lastKnownGroundInfoNormal = groundInfo.normal;
            }
            else if (didHitRear)
            {
                groundInfo.normal = rearHit.normal;
                lastKnownGroundInfoNormal = groundInfo.normal;
            }
        }
        else if (hitCount == 2)
        {
            groundInfo.normal = (frontHit.normal + rightHit.normal + leftHit.normal + rearHit.normal) * 0.5f;
            lastKnownGroundInfoNormal = groundInfo.normal;

        }
        else if (hitCount == 3)
        {
            if (!didHitFront)
            {
                groundInfo.normal = Vector3.Cross(rearHit.point - rightHit.point, leftHit.point - rightHit.point);
                lastKnownGroundInfoNormal = groundInfo.normal;

            }
            if (!didHitRight)
            {
                groundInfo.normal = Vector3.Cross(rearHit.point - frontHit.point, leftHit.point - frontHit.point);
                lastKnownGroundInfoNormal = groundInfo.normal;

            }
            if (!didHitLeft)
            {
                groundInfo.normal = Vector3.Cross(rightHit.point - frontHit.point, rearHit.point - frontHit.point);
                lastKnownGroundInfoNormal = groundInfo.normal;

            }
            if (!didHitRear)
            {
                groundInfo.normal = Vector3.Cross(leftHit.point - rightHit.point, frontHit.point - rightHit.point);
                lastKnownGroundInfoNormal = groundInfo.normal;

            }
        }
        else
        {
            Vector3 normal0 = Vector3.Cross(rearHit.point - rightHit.point, leftHit.point - rightHit.point);
            Vector3 normal1 = Vector3.Cross(rearHit.point - frontHit.point, leftHit.point - frontHit.point);
            Vector3 normal2 = Vector3.Cross(rightHit.point - frontHit.point, rearHit.point - frontHit.point);
            Vector3 normal3 = Vector3.Cross(leftHit.point - rightHit.point, frontHit.point - rightHit.point);

            groundInfo.normal = (normal0 + normal1 + normal2 + normal3) * 0.25f;
            lastKnownGroundInfoNormal = groundInfo.normal;

        }

        if (groundInfo.isGrounded)
        {
            float dot = Vector3.Dot(groundInfo.normal, _velocity.normalized);
            if (dot > 0.5f)
            {
                groundInfo.isGrounded = false;
            }
        }
        return groundInfo;
    }
    bool GetNearestFromRaycast(Ray ray, float rayDistance, int layerMask, QueryTriggerInteraction query, out RaycastHit hit)
    {
        int hits = Physics.RaycastNonAlloc(ray, m_RaycastHitBuffer, rayDistance, layerMask, query);

        hit = new RaycastHit();
        hit.distance = float.PositiveInfinity;

        bool hitSelf = false;
        for (int i = 0; i < hits; i++)
        {
            if (m_RaycastHitBuffer[i].collider == _cap) // || m_RaycastHitBuffer[i].collider == trig_Collider)
            {
                hitSelf = true;
                continue;
            }

            if (m_RaycastHitBuffer[i].distance < hit.distance)
                hit = m_RaycastHitBuffer[i];
        }
        if (hitSelf)
            hits--;

        return hits > 0;
    }

    //adjusts the ship according to groundInfo.normal, such that this is the direction of gravity
    //this keeps the ship aligned to the ground, but needs to handle airborne movement better
    //if airborne, the ship currently does not align
    //when airborne, the ship should slowly turn according to the arc of its trajectory (how??)
    void GroundNormal(float deltaTime, ref Quaternion rotationStream, GroundInfo currentGroundInfo, GroundInfo nextGroundInfo)
    {
        //Vector3 localVelocity = Quaternion.Inverse(rotationStream) * Quaternion.Inverse(m_DriftOffset) * m_Velocity; //testing using this for targetRotationFromVelocity, in order to align ship towards direction of travel when falling

        Vector3 rigidbodyUp = _body.rotation * Vector3.up;
        Quaternion currentTargetRotation = Quaternion.FromToRotation(rigidbodyUp, currentGroundInfo.normal);
        Quaternion nextTargetRotation = Quaternion.FromToRotation(rigidbodyUp, nextGroundInfo.normal);
        //Quaternion targetRotationFromVelocity = Quaternion.FromToRotation(rigidbodyUp, localVelocity.normalized);

        //if is not rotating, rotationStream here works as normal; ship aligns to ground
        if (!rotating)
        {
            if (nextGroundInfo.isCloseToGround)
            {
                //airborne but close to ground
                //here the ship will rotate towards ground normal at a high speed
                rotationStream = Quaternion.RotateTowards(currentTargetRotation, nextTargetRotation, 2000f) * rotationStream;
                //rotationStream = Quaternion.RotateTowards(currentTargetRotation, nextTargetRotation, .5f) * rotationStream;

                //print("not air");
            }
            else //airborne
            {

                print("air");
                //MUCH OF THIS IS GOOD, THE ANGLE STUFF WAS USED TO GREAT SUCCESS, BUT BARREL ROLL SYSTEM MESSED WITH IT
                //if (!Input.GetKey(KeyCode.Mouse1))
                //{


                ////compare last known groundNormal to current orientation
                ////if current orientation is < 90 degrees off from last known ground normal
                ////then make ship tumble
                ////also make tumbling speed depend on how far off from last known ground normal, so it slows down over time

                ////Quaternion deltaRotation = Quaternion.Euler(1f, 0f, 0f);


                //float angle = Vector3.Angle(lastKnownGroundInfoNormal, transform.up);

                //if (angle < 1f) //in order to prevent this value from being stuck at 0, which prevents the angling done below
                //    angle = 1f;

                //if (angle < 75f) //maximum angle at which the ship keeps tumbling, should be planned so that ship does not get stuck on its nose, which happened in testing at 90 degrees
                //{
                //    Quaternion deltaRotation = Quaternion.Euler(angle / 20f, 0f, 0f); //angle is divided by a number to keep the tumbling speed from being as large as the angle number, 10-20 have been ok values so far
                //    rotationStream = rotationStream * deltaRotation;
                //    //print("angle found less than 90: " + angle + " " + lastKnownGroundInfoNormal);
                //}
                ////else if(angle < 45f)
                ////{

                ////}
                //else
                //{
                //    rotationStream = Quaternion.RotateTowards(rotationStream, nextTargetRotation, .2f * deltaTime);
                //}
                //}

                //while airborne, the ship will rotate towards world up angle at a slower speed
                rotationStream = Quaternion.RotateTowards(rotationStream, nextTargetRotation, 20f * deltaTime);



                //rotationStream = Quaternion.RotateTowards(rotationStream, targetRotationFromVelocity, 200f * deltaTime);
                //rotationStream = Quaternion.RotateTowards(rotationStream, nextTargetRotation, .2f * deltaTime);
            }

        } //if is rotating, do not do anything with rotationStream here
    }

    //turn the ship according to input
    void TurnShip(float deltaTime, ref Quaternion rotationStream)
    {
        Vector3 localVelocity = Quaternion.Inverse(rotationStream) * Quaternion.Inverse(m_DriftOffset) * _velocity;



        //= m_Input.Acceleration > 0 ? 1f : -1f;
            //= Mathf.Sign(localVelocity.z);

        //when acceleration and velocity are positive, steering is normal
        //when these are negative, so is the steering
        //this would simulate wheels, but does it make sense for a spaceship?
        if (acceleration > 0 && Mathf.Sign(localVelocity.z) > 0)
        {
            forwardReverseSwitch = 1f;
        }
        else if (acceleration < 0 && Mathf.Sign(localVelocity.z) < 0)
        {
            forwardReverseSwitch = -1f;
            
        }


        float modifiedSteering = input * forwardReverseSwitch;

            //m_HasControl ? m_Input.Steering * forwardReverseSwitch : 0f;

        float speedProportion = _velocity.sqrMagnitude > 0f ? 1f : 0f;
        float turn = maxSteer * modifiedSteering * speedProportion * deltaTime;

        Quaternion deltaRotation = Quaternion.Euler(0f, turn, 0f);
        rotationStream = rotationStream * deltaRotation;


    }

    //handles the basic calculation of velocity
    void CalculateDrivingVelocity(float deltaTime, GroundInfo groundInfo, Quaternion rotationStream)
    {
        Vector3 localVelocity = Quaternion.Inverse(rotationStream) * Quaternion.Inverse(m_DriftOffset) * _velocity;
        if (groundInfo.isGrounded)
        {

            localVelocity.x = Mathf.MoveTowards(localVelocity.x, 0f, grip * deltaTime);

            if (acceleration > -k_Deadzone && acceleration < k_Deadzone) //no acceleration input, coastingDrag is the speed at which velocity is lost
                localVelocity.z = Mathf.MoveTowards(localVelocity.z, 0f, coastingDrag * deltaTime);
            else if (acceleration > k_Deadzone) //accelerating
                localVelocity.z = Mathf.MoveTowards(localVelocity.z, (currentMaxSpeed + speedMod) / slowMod, acceleration * (speedMod + accelerationStat * deltaTime));
            else if (localVelocity.z > k_Deadzone) //negative acceleration and going forwards
                localVelocity.z = Mathf.MoveTowards(localVelocity.z, 0f, -acceleration * braking * deltaTime);
            else //negative acceleration and going backwards
                localVelocity.z = Mathf.MoveTowards(localVelocity.z, -reverseSpeed, -acceleration * reverseAcceleration * deltaTime);

            //print(localVelocity.z);
        }

        //if the ship's collider is touching the ground, movement in the y axis is prevented from going negative
        //this should keep the ship from falling through the ground
        //even appears to work fine when the ship is upside down
        if (groundInfo.isCapsuleTouching)
            localVelocity.y = Mathf.Max(0f, localVelocity.y);

        localVelocity.y = Mathf.Min(maxSpeed, localVelocity.y);
        //localVelocity.y = Mathf.Min(m_ModifiedStats.topSpeed, localVelocity.y);

        _velocity = m_DriftOffset * rotationStream * localVelocity;

        //gravity is applied to the ship when it is not touching the ground
        if (!groundInfo.isCapsuleTouching && groundInfo.isCloseToGround)
        {
            //_velocity += -transform.up * m_ModifiedStats.gravity * deltaTime; //gravity is always applied in the object downward direction
            _velocity += -transform.up * (gravity + gravityMod) * deltaTime; //gravity is always applied in the object downward direction
            //_velocity += -Vector3.up * (gravity + gravityMod) * deltaTime; //gravity is always applied in the object downward direction
            //print("not touching ground but close");

            //if (LocalSpeed < m_ModifiedStats.topSpeed / 2)
            //{
            //    //this was default and worked fine, only it won't let ships drive along walls and ceilings
            //    m_Velocity += Vector3.down * m_ModifiedStats.gravity * deltaTime; //gravity is always applied in the world downward direction

            //}
            //else
            //{
            //    //this becomes quite wonky when dealing with ramps
            //    m_Velocity += -transform.up * m_ModifiedStats.gravity * deltaTime; //gravity is always applied in the object downward direction

            //}

            ////gravity applied toward last known ground normal; allows ship to stay on walls and ceilings
            ////this ought to first check the velocity of the ship, so that a slow-moving ship will fall off walls and ceilings


        }
        else if (!groundInfo.isCapsuleTouching && !groundInfo.isCloseToGround)
        {
            //not touching ground and not close to ground
            if (lastKnownGroundInfoNormal != Vector3.zero)  //it is Vector3.zero only when the race has just begun, before data comes in
            {

                _velocity += -transform.up * (gravity + gravityMod) * deltaTime;


            }
            else
            {
                //this part ought to only occur when lastKnownGroundInfoNormal = Vector3.zero

                //_velocity += Vector3.down * m_ModifiedStats.gravity * deltaTime;
                _velocity += Vector3.down * gravity * deltaTime;
                print("this should only be printed at the very start");
            }

        }
    }

    //the following is some more advanced stuff
    //ideally I would not use it, but it must be tested in case it solves a problem

    //this one function uses some stuff I currently have not implemented
    //ProcessVelocityCollisions is for calculating what happens when the ship hits another ship

    //processes collisions and changes velocity accordingly
    Vector3 ProcessVelocityCollisions(float deltaTime, Quaternion rotationStream, Vector3 penetrationOffset)
    {
        Vector3 rayDirection = _velocity * deltaTime + penetrationOffset;
        float rayLength = rayDirection.magnitude + .2f; //.2f
        if (rayLength > 4f)
            rayLength = 4f;
        Ray sphereCastRay = new Ray(_bodyPosition, rayDirection);
        //int hits = Physics.SphereCastNonAlloc(sphereCastRay, 0.75f, m_RaycastHitBuffer, rayLength, Ground, QueryTriggerInteraction.Collide);
        int hits = Physics.SphereCastNonAlloc(sphereCastRay, 0.75f, m_RaycastHitBuffer, rayLength, allCollidingLayers, QueryTriggerInteraction.Collide);
        //print(rayLength);


        for (int i = 0; i < hits; i++)
        {
            if (m_RaycastHitBuffer[i].collider == _cap) //if the hit is the capsule of the object, skip
                continue;


            //IShipModifier shipModifier = m_RaycastHitBuffer[i].collider.GetComponent<IShipModifier>();
            //if (shipModifier != null)
            //{
            //    m_CurrentModifiers.Add(shipModifier);
            //    m_TempModifiers.Add(shipModifier);
            //}

            //this part may be of concern later
            //eventually AI agents will control the ships found, should probably rename PlayerController
            //also, would be better to set it up so that the script does not need to check if it found PlayerController or WallCollider
            //in the original example this code is based on, an interface was used as an intermediary
            //Collider otherCollider = m_RaycastHitBuffer[i].collider;

            //GetColliderScript(otherCollider);

            if (m_RaycastHitBuffer[i].collider.GetComponent<PlayerController>() != null)
            {
                //otherCollider = m_RaycastHitBuffer[i].collider.GetComponent<PlayerController>();

                if (Mathf.Approximately(m_RaycastHitBuffer[i].distance, 0f))
                    if (Physics.Raycast(_bodyPosition, rotationStream * Vector3.down, out RaycastHit hit, rayLength + 0.5f, Ground, QueryTriggerInteraction.Collide))
                        m_RaycastHitBuffer[i] = hit;

                //if another ship is found
                if (m_RaycastHitBuffer[i].collider.GetComponent<PlayerController>() != null)
                {
                    _velocity = m_RaycastHitBuffer[i].collider.GetComponent<PlayerController>().ModifyVelocity(this.gameObject, m_RaycastHitBuffer[i]);

                    if (Mathf.Abs(Vector3.Dot(m_RaycastHitBuffer[i].normal, transform.up)) <= .2f) //from vEctor3.up
                    {
                        print("something collided: " + m_RaycastHitBuffer[i].transform.name);
                        //compare the angle of the normal of the raycast hit to the up angle of the object
                        //this means it is trying to figure out if if the raycast hit is close to a right angle to the object
                        //this would mean objects here found ought to be walls?
                        //this can only be called if the other object has a ship collider? WallCollider is also IShipCollider
                        //either way something should happen here, possibly simply reset the ship as if it had been destroyed
                        //originally used to call event OnShipCollision, but haven't seen the print, so am unsure if works

                        //now I can see the print all the time when touching walls with the player's ship
                        //could call an agent punishment here
                        //if (isAgent)
                        //{
                        //    GetComponent<ShipAgent>().OnFailure();

                        //    //if(GetComponent<ShipAgent>() != null)
                        //    //{

                        //    //}
                        //}
                    }
                }
                else
                {

                    penetrationOffset = ComputePenetrationOffset(rotationStream, penetrationOffset);
                }

            }
            else if (m_RaycastHitBuffer[i].collider.GetComponent<WallCollider>() != null)
            {
                if (Mathf.Approximately(m_RaycastHitBuffer[i].distance, 0f))
                    if (Physics.Raycast(_bodyPosition, rotationStream * Vector3.down, out RaycastHit hit, rayLength + 0.5f, Ground, QueryTriggerInteraction.Collide))
                        m_RaycastHitBuffer[i] = hit;

                //if another ship is found
                if (m_RaycastHitBuffer[i].collider.GetComponent<WallCollider>() != null)
                {
                    _velocity = m_RaycastHitBuffer[i].collider.GetComponent<WallCollider>().ModifyVelocity(this.gameObject, m_RaycastHitBuffer[i]);

                    if (Mathf.Abs(Vector3.Dot(m_RaycastHitBuffer[i].normal, transform.up)) <= .2f) //from vEctor3.up
                    {
                        print("something collided: " + m_RaycastHitBuffer[i].transform.name);
                        //compare the angle of the normal of the raycast hit to the up angle of the object
                        //this means it is trying to figure out if if the raycast hit is close to a right angle to the object
                        //this would mean objects here found ought to be walls?
                        //this can only be called if the other object has a ship collider? WallCollider is also IShipCollider
                        //either way something should happen here, possibly simply reset the ship as if it had been destroyed
                        //originally used to call event OnShipCollision, but haven't seen the print, so am unsure if works

                        //now I can see the print all the time when touching walls with the player's ship
                        //could call an agent punishment here
                        //if (isAgent)
                        //{
                        //    GetComponent<ShipAgent>().OnFailure();

                        //    //if(GetComponent<ShipAgent>() != null)
                        //    //{

                        //    //}
                        //}
                    }
                }
                else
                {

                    penetrationOffset = ComputePenetrationOffset(rotationStream, penetrationOffset);
                }

            }


        }

        return penetrationOffset;
    }

    public Vector3 ModifyVelocity(GameObject collidingShip, RaycastHit collisionHit)
    {

        Vector3 returnVelocity = Vector3.zero;

        if(collidingShip.GetComponent<PlayerController>() != null){

            float weightDifference = GetComponent<PlayerController>().weight - weight;

            if (weightDifference <= 0f)
            {
                Vector3 toCollidingShip = (GetComponent<PlayerController>().transform.position - _bodyPosition).normalized;

                //print("bumped other ship!");
                //invoke ship collision
                //OnShipCollision.Invoke();

                return GetComponent<PlayerController>()._velocity + toCollidingShip * (10f - weightDifference);
            }
        }
        else if(collidingShip.GetComponent<TrafficController>() != null){
            float weightDifference = GetComponent<TrafficController>().weight - weight;

            if (weightDifference <= 0f)
            {
                Vector3 toCollidingShip = (GetComponent<TrafficController>().transform.position - _bodyPosition).normalized;

                //print("bumped other ship!");
                //invoke ship collision
                //OnShipCollision.Invoke();

                return GetComponent<TrafficController>()._velocity + toCollidingShip * (10f - weightDifference);
            }
        }

        if(collidingShip.GetComponent<PlayerController>() != null){
            returnVelocity = collidingShip.GetComponent<PlayerController>()._velocity;
        }
        else if(collidingShip.GetComponent<TrafficController>() != null){
            returnVelocity = collidingShip.GetComponent<TrafficController>()._velocity;
        }

        //print("bumped other ship?");
        return returnVelocity;
    }

    //decides how much to move the ship on collision overlap
    Vector3 SolvePenetration(Quaternion rotationStream)
    {
        Vector3 summedOffset = Vector3.zero;
        for (var solveIterations = 0; solveIterations < 3; solveIterations++) //k_MaxPenetrationSolves = 3
        {
            summedOffset = ComputePenetrationOffset(rotationStream, summedOffset);
        }
        return summedOffset;
    }

    //uses Physics.ComputePenetration to figure out where the ship's colliders are in relation to objects it is colliding with
    Vector3 ComputePenetrationOffset(Quaternion rotationStream, Vector3 summedOffset)
    {
        //var capsuleAxis = rotationStream * Vector3.forward * _cap.height * 0.5f;
        var capsuleAxis = rotationStream * Vector3.forward * 1f * 0.5f;
        var point0 = _bodyPosition + capsuleAxis + summedOffset;
        var point1 = _bodyPosition - capsuleAxis + summedOffset;
        //var shipCapsuleHitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, _cap.radius, m_ColliderBuffer, Ground, QueryTriggerInteraction.Ignore);
        var shipCapsuleHitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, 0.5f, m_ColliderBuffer, Ground, QueryTriggerInteraction.Ignore);
        //var shipCapsuleHitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, _cap.radius, m_ColliderBuffer, allCollidingLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < shipCapsuleHitCount; i++)
        {
            var hitCollider = m_ColliderBuffer[i];
            if (hitCollider == _mCol) //ignore ship's own collider
                continue;

            var hitColliderTransform = hitCollider.transform;
            if (Physics.ComputePenetration(_mCol, _bodyPosition + summedOffset, rotationStream, hitCollider, hitColliderTransform.position, hitColliderTransform.rotation, out Vector3 separationDirection, out float separationDistance))
            {
                Vector3 offset = separationDirection * (separationDistance + Physics.defaultContactOffset);
                if (Mathf.Abs(offset.x) > Mathf.Abs(summedOffset.x))
                    summedOffset.x = offset.x;
                if (Mathf.Abs(offset.y) > Mathf.Abs(summedOffset.y))
                    summedOffset.y = offset.y;
                if (Mathf.Abs(offset.z) > Mathf.Abs(summedOffset.z))
                    summedOffset.z = offset.z;
            }
        }
        return summedOffset;
    }

    //to keep velocity from forcing ship into collider, velocity reduced and pushed in the direction given by penetration offset
    void AdjustVelocityByPenetrationOffset(float deltaTime, ref Vector3 penetrationOffset)
    {
        //how much velocity in penetration direction
        Vector3 penetrationProjection = Vector3.Project(_velocity * deltaTime, penetrationOffset);

        //if opposite direction
        if (Vector3.Dot(penetrationOffset, penetrationProjection) < 0f)
        {
            //if offset larger than projection
            if (penetrationOffset.sqrMagnitude > penetrationProjection.sqrMagnitude)
            {
                //reduce velocity by velocity of projection and offset by the projection
                _velocity -= penetrationProjection / deltaTime;
                penetrationOffset += penetrationProjection;
            }
            else //offset smaller than projection
            {
                _velocity += penetrationOffset / deltaTime;
                penetrationOffset = Vector3.zero;
            }
        }

        _bodyMovement = _velocity * deltaTime + penetrationOffset;
    }
}

