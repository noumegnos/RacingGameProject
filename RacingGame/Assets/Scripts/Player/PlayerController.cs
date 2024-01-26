using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

//based on Kart Example, this script handles taking input and calculating velocity of the ship

public class PlayerController : MonoBehaviour
{
    //a little bool we set as true for human players; AI agents get their input from somewhere else than keyboard
    public bool isHuman = true;

    public Transform lastRespawnPoint;
    public bool respawnPointIsOrigin = true;
    public Quaternion lastRespawnRotation;
    public Vector3 originPoint;
    public Quaternion originRotation;

    public float currentSpeed;
    public float accelerationPower;


    //current checkpoint int is updated every time the racer crosses a checkpoint
    //lastCheckpointKnown and lastCheckpointTouch are almost the same, but one is forgotten; the other one is used by RaceManager so needs to stay current
    public int currentCheckpoint = 0;
    public int lastCheckpointKnown; //this last checkpoint is forgotten when respawned
    public int lastCheckpointTouched; //the last checkpoint touched, regardless if it's the correct one
    public int timeToReachNextCheckpoint = 3000;
    public float timeLeft;

    public float fallingTime;
    public float maxFallingTime;


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
    public float weight = 1f; //weight impacts collisions between ships, the one with the lesser is bumped more
    public float jumpSpeed = 2; //jumping is a little problematic, I turned down jump speed significantly in order to fix a problem with gravity and rotations

    //public float jumpTime = 1f;
    //public float jumpCooldown = 1f;
    //private float defJumpTime;
    //private float defJumpCooldown;
    //public Vector3 moveDirection;

    //public bool _isBoosting = false;
    //public float boostTime = 1f;
    //public float defBoostTime;
    //public float boostMod = 0f;

    public float fallTime = 5f;
    public float defFallTime;

    public bool _isGrounded = true;
    //public bool _isJumping = false;
    //public bool _canJump = true;

    //public float GroundDistance = 0.5f;
    //public Transform _groundChecker;
    //public Transform _groundCheckerLeft;
    //public Transform _groundCheckerRight;
    //public Transform _groundCheckerFront;
    //public Transform _groundCheckerBack;
    //public Transform _groundCheckerAhead;
    public LayerMask Ground;
    public LayerMask allCollidingLayers;

    //public Vector3 groundAngleLeft;
    //public Vector3 groundAngleRight;
    //public Vector3 groundAngleFront;
    //public Vector3 groundAngleBack;

    //public bool ground1;
    //public bool ground2;
    //public bool ground3;
    //public bool ground4;

    public Rigidbody _body;

    public float gravity = 9.8f;
    public float gravityMod = 1f;
    //public float gravityMod;
    //float defGravityMod;
    //public float gravityRange = 10f;
    //public Vector3 gravityDirection;
    //public Vector3 groundDirection;
    //public Vector3 storedDirection;
    //public Vector3 additionalDirection;
    //public float gravityRatio;


    public float input;

    //public Quaternion rotation;
    //public Quaternion camRotation;
    //public float rotSmooth = 50f;
    //public Quaternion rotationZ;

    //public bool useMouse = true;

    //public bool lockCursor = false;

    public bool stopped = false;

    //public Text speedDisplay;

    //camera field of view
    //public Camera cam;
    //public float camFoV;
    //public float camSpeed = 2f;
    //public Transform camPos;

    //public GameObject spaceship;
    //public float spaceshipLeaning;

    //public float lerpTime = 0.0f;

    //public Transform facingDir;
    //Vector3 facing;
    //public Vector3 faceDirOrigin;
    //public Transform facer;
    //public Transform upDir;
    public float maxSteer;
    float steer;

    //part of old steering system, which worked well
    //public Quaternion rotationShip;
    //float roty;


    //Vector3 facerDir;
    //float savedFacing;

    //public float distToGround;
    //public float desiredDistToGround;
    //public Vector3 desiredGravSpot;

    ////public Vector3 axisOfTravel;
    //public Vector3 frontPoint;
    //public Vector3 backPoint;

    //public Vector3 lastFrontPoint;
    //public Vector3 lastBackPoint;

    //public Vector3 speedForce;
    //public float mass = 1f;


    //KART!
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

    Quaternion m_DriftOffset = Quaternion.identity;
    Vector3 lastKnownGroundInfoNormal = Vector3.zero;

    public float k_Deadzone = 0.01f;

    RaycastHit[] m_RaycastHitBuffer = new RaycastHit[16];

    //temporary like this; think these are the same
    public bool rotating = false;
    public bool doRoll = false;

    //when backing up this will reverse steering, which makes sense if we have wheels
    //probably still needed even if we're driving an anti-gravity spaceship, simply because it is more intuitive
    float forwardReverseSwitch;

    //wutr
    Collider[] m_ColliderBuffer = new Collider[8];

    public CheckpointManager checkpointManager;
    public LevelGenerator levelGenerator;


    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();

        checkpointManager = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
        levelGenerator= GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();

        StartCoroutine(SpeedUp());

        originPoint = transform.position;
        originRotation = transform.rotation;

        //lastRespawnPoint = originPoint.position;

        //defJumpTime = jumpTime;
        //defBoostTime = boostTime;
        //defFallTime = fallTime;
        //defGravityMod = gravityMod;
        //defJumpCooldown = jumpCooldown;

        _cap = GetComponent<CapsuleCollider>();

        timeLeft = timeToReachNextCheckpoint;

        fallingTime = maxFallingTime;


        //savedFacing = facingDir.position.y;

        //camFoV = 60f;

        //if (lockCursor)
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}

        //a default value for the gravity is given right at the start, to be overwritten as soon as actual data comes in
        //gravityDirection = Vector3.up;

    }

    // Update is called once per frame
    void Update()
    {
        //    //makes the camera give sense of speed
        //    CameraSpeedUpFoV();

        //updates UI with speed and maxSpeed
        //SetUI();

        //if human controller
        if(isHuman)
            input = Input.GetAxis("Horizontal");
        else
            input = gameObject.GetComponent<MLADrive>().steering;

        //if (input > 0)
        //    input = 1;
        //else if (input < 0)
        //    input = -1;
        //else
        //    input = 0;

        //how do I add the dang Agents actions to input and accelerate
        //input = gameObject.GetComponent<MLADrive>().

        //if (useMouse) //ill advised
        //{
        //    input = Input.GetAxis("Mouse X");
        //}
        //else
        //{
        //    input = Input.GetAxis("Horizontal");
        //}

        //decides on force and direction
        //CalculateForce();

        timeLeft -= Time.deltaTime;
        fallingTime -= Time.deltaTime;

        //reset timeLeft in respawn function
        if (timeLeft < 0f)
        {
            if (!isHuman)
            {
                gameObject.GetComponent<MLADrive>().SetReward(-1f);
                gameObject.GetComponent<MLADrive>().EndEpisode();
            }

            RespawnShip();

            //Destroy(this.gameObject, 2f); //destroy the ship if out of time
            print("outta time");
        }

        if (m_CurrentGroundInfo.isCloseToGround)
            fallingTime = maxFallingTime;

        if(fallingTime < 0f){
            RespawnShip();
        }



        if (!stopped)
        {
            if(isHuman)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    acceleration = 1f;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    acceleration = -1f;
                }
                else
                {
                    acceleration = 0f;
                }

                if (Input.GetKey(KeyCode.Z))
                {
                    doRoll = true;
                }
                else
                {
                    doRoll = false;
                }
            }
            else
            {
                acceleration = gameObject.GetComponent<MLADrive>().accelerate;



                //if(acceleration > 0)
                //    acceleration = 1;
                //if(acceleration < 0)
                //    acceleration = -1;
                //else
                //    acceleration = 0;
            }
        }


        //restart and quit
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

    }

    void LateUpdate()
    {

        //all of this is KART ed now
        ////this is the gravity
        //Gravity();

        //if (!stopped)
        //{
        //    //movement adds force according to speed and direction
        //    Movement();

        //    //aligns with ground
        //    Alignment();
        //}
        //else
        //{
        //    speed = 0f;
        //}
    }

    private void FixedUpdate()
    {

        //KART!
        _bodyPosition = _body.position;

        Quaternion rotationStream = _body.rotation;

        float deltaTime = Time.deltaTime;

        m_CurrentGroundInfo = GetGroundInfo(deltaTime, rotationStream, Vector3.zero);

        Jump(rotationStream, m_CurrentGroundInfo);

        _isGrounded = m_CurrentGroundInfo.isGrounded;

        GroundInfo nextGroundInfo = GetGroundInfo(deltaTime, rotationStream, _velocity * deltaTime);

        GroundNormal(deltaTime, ref rotationStream, m_CurrentGroundInfo, nextGroundInfo);

        if (doRoll) // && !m_IsGrounded)
        {
            RotateShip(deltaTime, ref rotationStream);
            rotating = true;
        }
        else
        {
            TurnShip(deltaTime, ref rotationStream);
            rotating = false;
        }

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


        //_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);

        //the following is an attempt to learn something from the Kart game 
        //Quaternion rotationStream = _body.rotation;

        //Vector3 bodyVelocity = new Vector3(1,1,1);


        //_body.velocity = bodyVelocity;

        //the previous is an attempt to learn something from the Kart game 



        //RaycastHit groundRay;
        //Ray rayGround = new Ray(_groundChecker.position, -transform.up);
        //if (Physics.Raycast(rayGround, out groundRay, GroundDistance, Ground))
        //{
        //    _isGrounded = true;
        //}
        //else
        //{
        //    _isGrounded = false;
        //}


        //this keeps track of where the ground is
        //GroundCheckers();
        //now handled by the KART version

        //facing lets you know which way to go
        //facing = facingDir.position - transform.position;

        ////this is the gravity --- testing in LateUpdate
        //Gravity();

        ////regular movement
        //if (!stopped)
        //{
        //    //makes ship go
        //    //Movement();
        //    //Accelerate();

        //    //aligns with ground
        //    Alignment();

        //    //aligns the ship itself
        //    //ShipAlignment();



        //    //go left or right
        //    //Strafing();

        //    //give movement to camera
        //    //CameraMovement();


        //}
        //else
        //{
        //    speed = 0f;
        //}

        //if (_isJumping)
        //{
        //    _canJump = false;
        //    jumpTime -= Time.deltaTime;

        //    if (jumpTime <= 0.0f)
        //    {
        //        JumpingEnded();
        //    }
        //}

        //if (!_canJump)
        //{
        //    jumpCooldown -= Time.deltaTime;

        //    if (jumpCooldown <= 0.0f)
        //    {
        //        _canJump = true;
        //        jumpCooldown = defJumpCooldown;
        //    }
        //}



        //if (_isBoosting)
        //{
        //    boostTime -= Time.deltaTime;

        //    boostMod = boostMod * boostTime;

        //    if (boostTime <= 0.0f)
        //    {
        //        EndBoost();
        //    }
        //}

        //this little bit starts counting down while player is airborne, the intent being to keep track of when player has fallen off the world
        //however, as I'll implement anti-gravity power soon, it will become a problem
        //add in a check here that if you are not also doing the AG move, it will count down
        //for now lets disable it
        //if (!_isGrounded)
        //{
        //    fallTime -= Time.deltaTime;
        //    //gravityMod += .1f;

        //    if (fallTime <= 0.0f)
        //    {
        //        DieInSpace();
        //    }
        //}
        //else
        //{
        //    //gravityMod = defGravityMod;
        //    fallTime = defFallTime;
        //}
    }

    //void CalculateForce()
    //{
    //    //speedForce = facing.normalized * (speed * mass);
    //    //speedForce = facing.normalized * (speed * acceleration) * mass; //this could be modified by gravity when not grounded to give a better arc of flight
    //    speedForce = transform.forward * (speed * acceleration) * mass; //this could be modified by gravity when not grounded to give a better arc of flight



    //    //print(speedForce);
    //}

    //void GroundCheckers()
    //{
    //    //down-pointing
    //    RaycastHit hitDF;
    //    RaycastHit hitDB;
    //    RaycastHit hitDL;
    //    RaycastHit hitDR;

    //    //up-pointing
    //    RaycastHit hitUF;
    //    RaycastHit hitUB;
    //    RaycastHit hitUL;
    //    RaycastHit hitUR;

    //    //front-facing
    //    RaycastHit hitFF;

    //    //ahead-looking
    //    RaycastHit hitAhe;

    //    Ray rayDL = new Ray(_groundCheckerLeft.position, -transform.up);
    //    Ray rayUL = new Ray(_groundCheckerLeft.position, transform.up);
    //    Ray rayDR = new Ray(_groundCheckerRight.position, -transform.up);
    //    Ray rayUR = new Ray(_groundCheckerRight.position, transform.up);

    //    Ray rayDF = new Ray(_groundCheckerFront.position, -transform.up);
    //    Ray rayUF = new Ray(_groundCheckerFront.position, transform.up);
    //    Ray rayDB = new Ray(_groundCheckerBack.position, -transform.up);
    //    Ray rayUB = new Ray(_groundCheckerBack.position, transform.up);

    //    Ray rayFF = new Ray(_groundChecker.position, transform.forward);
    //    Ray rayAhe = new Ray(_groundCheckerAhead.position, -transform.up);

    //    bool hit1;
    //    bool hit2;
    //    bool hit3;
    //    bool hit4;
    //    bool hit5Ahe;



    //    if (Physics.Raycast(rayDL, out hitDL, gravityRange, Ground))
    //    {
    //        groundAngleLeft = hitDL.normal;
    //        hit1 = true;

    //        if(hitDL.distance < GroundDistance)
    //        {
    //            ground1 = true;
    //        }
    //        else
    //        {
    //            ground1 = false;
    //        }
    //    }
    //    else if (Physics.Raycast(rayUL, out hitUL, gravityRange, Ground))
    //    {
    //        groundAngleLeft = hitUL.normal;
    //        hit1 = true;
    //    }
    //    else
    //    {
    //        //no hit
    //        hit1 = false;
    //    }

    //    if (Physics.Raycast(rayDR, out hitDR, gravityRange, Ground))
    //    {
    //        groundAngleRight = hitDR.normal;
    //        hit2 = true;

    //        if (hitDR.distance < GroundDistance)
    //        {
    //            ground2 = true;
    //        }
    //        else
    //        {
    //            ground2 = false;
    //        }
    //    }
    //    else if (Physics.Raycast(rayUR, out hitUR, gravityRange, Ground))
    //    {
    //        groundAngleRight = hitUR.normal;
    //        hit2 = true;
    //    }
    //    else
    //    {
    //        //no hit
    //        hit2 = false;

    //    }

    //    if (Physics.Raycast(rayDF, out hitDF, gravityRange, Ground))
    //    {
    //        groundAngleFront = hitDF.normal;
    //        hit3 = true;

    //        if (hitDF.distance < GroundDistance)
    //        {
    //            ground3 = true;
    //        }
    //        else
    //        {
    //            ground3 = false;
    //        }

    //    }
    //    else if (Physics.Raycast(rayUF, out hitUF, gravityRange, Ground))
    //    {
    //        groundAngleFront = hitUF.normal;
    //        hit3 = true;

    //    }
    //    else
    //    {
    //        //no hit
    //        hit3 = false;

    //    }

    //    if (Physics.Raycast(rayDB, out hitDB, gravityRange, Ground))
    //    {
    //        groundAngleBack = hitDB.normal;
    //        hit4 = true;

    //        if (hitDB.distance < GroundDistance)
    //        {
    //            ground4 = true;
    //        }
    //        else
    //        {
    //            ground4 = false;
    //        }

    //    }
    //    else if (Physics.Raycast(rayUB, out hitUB, gravityRange, Ground))
    //    {
    //        groundAngleBack = hitUB.normal;
    //        hit4 = true;

    //    }
    //    else
    //    {
    //        //no hit
    //        hit4 = false;
    //    }

    //    //if any of the down-pointing raycasts detect ground within ground distance, _isGrounded = true
    //    if(ground1 | ground2 | ground3 | ground4)
    //    {
    //        _isGrounded = true;
    //    }


    //    //this raycast is in case the ship gets stuck on its nose
    //    if (Physics.Raycast(rayFF, out hitFF, 25f, Ground))
    //    {
    //        //gravityDirection = hitFF.normal;
    //        //print("nose!");
    //    }

    //    //this ray is to help us figure out if the terrain ahead is good
    //    if (Physics.Raycast(rayAhe, out hitAhe, gravityRange, Ground))
    //    {
    //        hit5Ahe = true;
    //    }
    //    else
    //    {
    //        hit5Ahe = false;
    //    }





    //    //gravityRatio = speed / currentMaxSpeed;

    //    bool anyHit;
    //    if (hit1 || hit2 || hit3 || hit4)
    //    {
    //        anyHit = true;
    //    }
    //    else
    //    {
    //        anyHit = false;
    //    }

    //    Vector3 targetVector = hitAhe.point - hitDF.point;
    //    float angle = Vector3.Angle(targetVector, transform.up);

    //    //print(angle);
    //    if (!_isJumping && _isGrounded)
    //    {
    //        if (angle > 45 && angle < 95) //if the angle of the ground point found ahead compared to transform.up is acceptable the ship will follow it
    //        {
    //            print("good angle, following ground");
    //            groundDirection = groundAngleLeft + groundAngleRight + groundAngleFront + groundAngleBack;
    //            storedDirection = groundDirection;

    //        }
    //        else if (angle > 179) //hitting a wall
    //        {
    //            print("hit a wall, stopping");
    //            speed = 0f;
    //        }
    //        else
    //        {
    //            //print(angle);
    //            if (!anyHit) //if nothing is hit groundDirection is slowly reset to gravityDirection
    //            {
    //                print("hitting nothing! this should not occur");
    //                gravityDirection = Vector3.Lerp(gravityDirection, Vector3.up, Time.deltaTime * 1f);
    //                groundDirection = gravityDirection;

    //                //maybe gravityDirection should be defined once in the start to be = the groundDirection of the starting block
    //            }
    //            else  //here the difference of angles is found to be large, so the ship will attempt to continue as if it actually had momentum
    //            {
    //                //perhaps this should depend on speed: if you are going slowly, the ship adjusts angles and alignment more easily than when going fast
    //                if (speed > (currentMaxSpeed / 4))
    //                {
    //                    print("going fast; using storedDirection");
    //                    groundDirection = storedDirection;
    //                    gravityDirection = groundDirection;
    //                }
    //                else
    //                {
    //                    print("going slow, following ground");
    //                    groundDirection = groundAngleLeft + groundAngleRight + groundAngleFront + groundAngleBack;
    //                    storedDirection = groundDirection;
    //                }

    //            }
    //        }
    //    }
    //    else if (!_isGrounded && !_isJumping)
    //    {
    //        //how does it work when falling?



    //        if (anyHit)
    //        {
    //            print("Im falling but I can see the ground");
    //            groundDirection = (groundAngleLeft + groundAngleRight + groundAngleFront + groundAngleBack);
    //            //groundDirection = Vector3.Lerp(groundDirection, (groundAngleLeft + groundAngleRight + groundAngleFront + groundAngleBack), Time.deltaTime * 25f);
    //            gravityDirection = Vector3.Lerp(gravityDirection, groundDirection, Time.deltaTime * 25f);
    //            //gravityDirection = groundDirection;
    //            //groundDirection = groundAngleLeft + groundAngleRight + groundAngleFront + groundAngleBack;

    //        }
    //        else
    //        {
    //            print("im falling and i dont know where i am");
    //            gravityDirection = Vector3.Lerp(gravityDirection, Vector3.up, Time.deltaTime * 100f);
    //            groundDirection = Vector3.Lerp(groundDirection, Vector3.up, Time.deltaTime * 1f); //if i could make this groundDirection be more clever the ship would fall better
    //            //groundDirection = Vector3.Lerp(groundDirection, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z + 1), Time.deltaTime * 1f);
    //            //groundDirection = gravityDirection;
    //        }
    //    }
    //    else if (_isJumping)
    //    {
    //        print("jumping, use stored direction");
    //        groundDirection = storedDirection;
    //        gravityDirection = groundDirection;
    //    }
    //}

    //void Accelerate()
    //{
    //    //determines speed
    //    if (speed < currentMaxSpeed)
    //    {
    //        //speed = speed + boostMod + (acceleration * Time.deltaTime);
    //        speed = speed + acceleration * Time.deltaTime;

    //    }


    //    if (currentMaxSpeed > maxSpeed)
    //    {
    //        currentMaxSpeed -= acceleration * Time.deltaTime;
    //    }
    //    if (speed > currentMaxSpeed)
    //    {
    //        speed -= acceleration * Time.deltaTime;
    //    }
    //}

    //void Decelerate()
    //{
    //    if (speed >= 0f)
    //    {
    //        speed -= acceleration * Time.deltaTime;
    //    }
    //}

    //void Movement()
    //{
    //    if (speed < currentMaxSpeed)
    //    {
    //        //this works now, thanks to RIGIDBODY DRAG
    //        _body.AddForce(speedForce * Time.deltaTime, ForceMode.Acceleration);
    //    }






    //    //makes ship go in direction at speed
    //    //facing = facingDir.position - transform.position;

    //    //translate works best so far, but not perfect - wobbly
    //    //transform.Translate(facing * speed * Time.deltaTime, Space.World);
    //    //transform.Translate(facing.normalized * speed * Time.smoothDeltaTime, Space.World);

    //    //seems identical to translate
    //    //transform.position += facing * speed * Time.deltaTime;

    //    //these are smoother but its like driving on ice
    //    //_body.AddForce(facing.normalized * speed, ForceMode.VelocityChange);

    //    //_body.AddForce(facing.normalized * speed * Time.smoothDeltaTime, ForceMode.VelocityChange);

    //    //_body.velocity = facing * speed;
    //    //_body.AddRelativeForce(facing * speed);

    //    //transform.position = Vector3.Lerp(transform.position, facing, speed * Time.smoothDeltaTime);
    //}

    //void MovementNew()
    //{
    //    speed = speed + acceleration;

    //}

    //void Strafing()
    //{
    //    if (useMouse) //ill advised
    //    {
    //        transform.Translate(new Vector3(0f, 0f, -input * 28f * Time.fixedDeltaTime), Space.Self);
    //    }
    //    else
    //    {
    //        //might need a more dynamic sensitivity
    //        //decent: -intput * 4
    //        //too slow: -input * speed / 10
    //        //seems good: -input * currentMaxSpeed / 5
    //        //transform.Translate(new Vector3(input * currentMaxSpeed / 5, 0f,0f), Space.Self);

    //        //Vector3 upAxis = upDir.position - transform.position;
    //        //facer.transform.Rotate(upAxis, input, Space.Self);

    //    }
    //}

    //this aligns the whole gameobject with the ground
    //public void Alignment()
    //{
    //    //float alignSmoothing = speed / 400;
    //    //if(alignSmoothing >= 50f)
    //    //{
    //    //    alignSmoothing = 50f;
    //    //}

    //    Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundDirection) * transform.rotation;
    //    //rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSmooth * Time.deltaTime);
    //    rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100f);
    //    transform.rotation = rotation;




    //}

    ////this aligns the ship model itself
    ////just makes everything spin now
    //void ShipAlignment()
    //{
    //    Quaternion targetRotation = Quaternion.FromToRotation(transform.up, groundDirection) * spaceship.transform.rotation;
    //    rotation = Quaternion.RotateTowards(spaceship.transform.rotation, targetRotation, 20f * Time.deltaTime);
    //    spaceship.transform.rotation = rotation;
    //}

    //void SteeringTorque()
    //{
    //    //didnt work?
    //    _body.AddTorque(transform.up * input * 10f);
    //}

    //void Steering()
    //{
    //    //this bit seemed to work on flat ground
    //    //now seems to work inside pipes
    //    steer = steer + input;

    //    if(input == 0f)
    //    {
    //        steer = 0f;
    //    }

    //    float finalSteer = Mathf.Clamp(steer, -maxSteer, maxSteer);

    //    //ship
    //    //Vector3 targetDir = facing;
    //    //Vector3 targetDir = transform.forward;
    //    //targetDir.y = 0;
    //    //var rotationShip = Quaternion.LookRotation(targetDir);
    //    var rotationShip = Quaternion.LookRotation(transform.forward);
    //    rotationShip = rotation;
    //    //roty = rotation.y - 90 + steer; //in case the object already has some rotation, this can compensate

    //    if(finalSteer != 0f)
    //    {
    //        //roty = rotation.y + steer;

    //        roty = Mathf.Lerp(roty, rotation.y + finalSteer, Time.deltaTime * 10f);

    //    }
    //    else
    //    {
    //        roty = Mathf.Lerp(roty, 0f, Time.deltaTime * 10f);
    //    }

    //    rotationShip *= Quaternion.Euler(rotation.x, roty, rotation.z);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, rotationShip, Time.deltaTime * rotSmooth);
    //}



    //void CameraMovement()
    //{
    //    Vector3 offsetPosition;

    //    if (input != 0)
    //    {
    //        offsetPosition = new Vector3(Mathf.MoveTowards(camPos.transform.position.x, camPos.transform.position.x - input * 5f, Time.deltaTime * 10f), camPos.transform.position.y + 10, camPos.transform.position.z - 40);

    //    }
    //    else
    //    {
    //        offsetPosition = new Vector3(Mathf.MoveTowards(camPos.transform.position.x, 0f, Time.deltaTime * 2f), camPos.transform.position.y + 10, camPos.transform.position.z - 40);

    //    }
    //    camPos.position = offsetPosition;
    //}

    //public void Gravity()
    //{
    //    //gravity is only active while not jumping to prevent conflict(necessary??)
    //    //if (!_isJumping && !_isGrounded)
    //    //{
    //    //_body.velocity = gravityDirection * (gravity * gravityMod);

    //    //if (!_isJumping)
    //    //{
    //    if (!_isGrounded)
    //    {
    //        //gravity = -9.8f;

    //        _body.AddForce(gravityDirection * (gravity) * Time.deltaTime, ForceMode.VelocityChange); //is this force being constantly reapplied and magnified?
    //        //_body.AddForce(gravityDirection * (gravity * gravityMod) * Time.deltaTime, ForceMode.VelocityChange); //is this force being constantly reapplied and magnified?

    //        //_body.velocity = gravityDirection * (gravity);


    //    }
    //    else
    //    {
    //        //gravity = 0f;
    //    }

    //    //}
    //    //else
    //    //{

    //    //}


    //    //transform.Translate(gravityDirection.normalized * (gravity * gravityMod) * Time.deltaTime, Space.World);


    //    //}
    //    //if(desiredDistToGround != distToGround)
    //    //{
    //    //transform.position = Vector3.MoveTowards(transform.position, desiredGravSpot, 500f * Time.deltaTime);
    //    //transform.position = Vector3.Lerp(transform.position, desiredGravSpot, 50f * Time.deltaTime);

    //    //transform.Translate((transform.position - desiredGravSpot) * gravity * Time.smoothDeltaTime, Space.World);
    //    //}
    //    //print(desiredGravSpot);
    //}

    //private void Jumping()
    //{
    //    //while held, ship goes up for max 1 sec
    //    //when released early, ship goes down early
    //    if (Input.GetKey(KeyCode.Space) && !_isJumping && _isGrounded && _canJump)
    //    {
    //        Jump();
    //    }
    //    else if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        JumpingEnded();
    //    }
    //    else if (!Input.GetKey(KeyCode.Space) && _isJumping)
    //    {
    //        JumpingEnded();
    //    }
    //}

    //void Jump()
    //{
    //    //the jump action itself
    //    _isJumping = true;
    //    _body.AddForce(groundDirection * jumpSpeed, ForceMode.VelocityChange);


    //    //_body.AddForce(groundDirection * jumpSpeed, ForceMode.VelocityChange);

    //    //_body.AddForce(groundDirection * jumpSpeed, );

    //    //transform.position = Vector3.MoveTowards(transform.position, frontPoint, 1000f * Time.smoothDeltaTime);

    //    //frontPoint = Vector3.Lerp(new Vector3(frontPoint.x, frontPoint.y, frontPoint.z),  new Vector3(frontPoint.x, frontPoint.y + 2f, frontPoint.z), speed * Time.smoothDeltaTime);

    //    //transform.Translate(Vector3.up * jumpSpeed * 10f * Time.deltaTime);
    //}

    //void JumpingEnded()
    //{
    //    //rests the jumpy variables
    //    jumpTime = defJumpTime;
    //    _isJumping = false;
    //}

    //public void Boost()
    //{
    //    //boost briefly increases speed, as well as adds to max speed
    //    boostMod = 2f;
    //    speed = speed + boostMod;
    //    currentMaxSpeed = currentMaxSpeed + 1f;
    //    _isBoosting = true;
    //}

    //public void EndBoost()
    //{
    //    boostTime = defBoostTime;
    //    _isBoosting = false;
    //}

    public void DieInFire()
    {
        print("haha you are dead in fire");
        stopped = true;
    }

    public void DieInSpace()
    {
        print("haha you are dead in space");
        //gravityDirection = transform.up;
    }

    //void SetUI()
    //{
    //    speedDisplay.text = "Speed: " + speed.ToString() + " Max: " + currentMaxSpeed.ToString();
    //}


    //should improve this
    //just adds +1 speed every 10 seconds
    //but acceleration already increases speed
    private IEnumerator SpeedUp()
    {
        yield return new WaitForSeconds(10f);

        //if (speed < currentMaxSpeed && !stopped)
        //{
        //    //speed = speed + 1;


        //}

        if (currentMaxSpeed < maxSpeed && !stopped)
        {
            currentMaxSpeed = currentMaxSpeed + 1f;
        }

        StartCoroutine(SpeedUp());
    }

    //this function respawns the ship if crashed or fell off map
    //should make a cool version, but for now keep it simple and just respawn at the centre of the last checkpoint
    public void RespawnShip(){

        print("fell off track");



        if (!isHuman)
        {
            gameObject.GetComponent<MLADrive>().AddReward(-1f);
            gameObject.GetComponent<MLADrive>().EndEpisode();

        }

        timeLeft = timeToReachNextCheckpoint;

        _bodyMovement = Vector3.zero;
        _velocity = Vector3.zero;
        _body.angularVelocity = Vector3.zero;

        if(lastRespawnPoint == null && !respawnPointIsOrigin)
        {
            lastRespawnPoint = levelGenerator.FindOldestExistingCheckpoint();



        }

        if (lastRespawnPoint != null)
        {
            respawnPointIsOrigin = false;

            float x = originPoint.x;
            //float y = originPoint.y;
            float z = originPoint.z;

            _body.MovePosition(new Vector3(lastRespawnPoint.position.x + x, lastRespawnPoint.position.y + 5f, lastRespawnPoint.position.z + z));
            _body.MoveRotation(lastRespawnRotation);
        }
        else
        {
            _body.MovePosition(new Vector3(originPoint.x, originPoint.y + 5f, originPoint.z));
            _body.MoveRotation(originRotation);
        }

        _bodyMovement = Vector3.zero;
        _velocity = Vector3.zero;

        lastCheckpointKnown = 0;

        fallingTime = maxFallingTime;
    }

    ////this is to give a sense of speed
    //void CameraSpeedUpFoV()
    //{
    //    if (camFoV <= 90f && camFoV >= 50f)
    //    {
    //        camFoV = 4f * speed;
    //    }

    //    if (camFoV <= 50f)
    //    {
    //        camFoV = 50f;
    //    }
    //    if (camFoV >= 90f)
    //    {
    //        camFoV = 90f;
    //    }

    //    cam.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, camFoV, Time.deltaTime * camSpeed);
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "levelPart")
    //    {
    //        //_isGrounded = true;
    //        JumpingEnded();
    //    }

    //    if (collision.gameObject.tag == "obstacle")
    //    {
    //        stopped = true;
    //        DieInFire();
    //    }

    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag == "levelPart")
    //    {
    //        //_isGrounded = false;
    //    }
    //}

    //attempting to learn from Kart example:
    //code added to above stuff will have a "KART" note added
    //I made a copy of the current code to a text file just in case
    //note:
    //rotationStream
    //groundInfo
    //drift offset
    //m_velocity
    //grip stat

    //GroundInfo is a struct of 4 things; the 3 bools isCapsuleTouching, isGrounded, isCloseToGround, and the normal of the ground
    //this data is gathered by a series of raycasts, which continuously check the ground below the ship
    //the function is run twice every frame, to gather current and next groundInfo
    GroundInfo GetGroundInfo(float deltaTime, Quaternion rotationStream, Vector3 offset)
    {
        GroundInfo groundInfo = new GroundInfo();
        Vector3 defaultPosition = offset + _velocity * deltaTime;
        Vector3 direction = rotationStream * Vector3.down;

        float capsuleRadius = _cap.radius;
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

        //if the ship ends up on its side, this will update the groundInfo with that information
        //should add one on the roof as well, in case the ship falls onto its head
        //TEMP REMOVED TO TEST A BUG
        // Ray rayL = new Ray(leftSideRaycast.position, -transform.right);
        // Ray rayR = new Ray(rightSideRaycast.position, transform.right);

        // bool didHitRightSide = GetNearestFromRaycast(rayL, 5f, Ground, QueryTriggerInteraction.Ignore, out RaycastHit rightSideHit);
        // if (didHitRightSide)
        // {
        //     if (!_isGrounded)
        //     {
        //         groundInfo.normal = rightSideHit.normal;
        //     }
        // }
        // bool didHitLeftSide = GetNearestFromRaycast(rayR, 5f, Ground, QueryTriggerInteraction.Ignore, out RaycastHit leftSideHit);
        // if (didHitLeftSide)
        // {
        //     if (!_isGrounded)
        //     {
        //         groundInfo.normal = leftSideHit.normal;

        //     }
        // }


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

                //print("air");
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

        //steer = input > 0 ? 1f : -1f;

        if(input > 0)
        {
            steer = 1f;

        }else if(input < 0)
        {
            steer = -1f;
        }
        else
        {
            steer = 0f;
        }

        float modifiedSteering = steer * forwardReverseSwitch;

            //m_HasControl ? m_Input.Steering * forwardReverseSwitch : 0f;

        float speedProportion = _velocity.sqrMagnitude > 0f ? 1f : 0f;
        float turn = maxSteer * modifiedSteering * speedProportion * deltaTime;

        Quaternion deltaRotation = Quaternion.Euler(0f, turn, 0f);
        rotationStream = rotationStream * deltaRotation;


    }

    //rotation in barrel roll style
    void RotateShip(float deltaTime, ref Quaternion rotationStream)
    {

        float modifiedSteering = input; //needs some massaging?
        //float pitchInput = m_Input.Pitch;
        //Input.GetAxisRaw("Mouse X");
        //print("steer: " + modifiedSteering);

        //float speedProportion = m_Velocity.sqrMagnitude > 0f ? 1f : 0f;
        float rotate = modifiedSteering * 150f * deltaTime;
        //float pitch = pitchInput * 100f * deltaTime;
        //float rotate = m_ModifiedStats.turnSpeed * modifiedSteering * speedProportion * deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, 0f, rotate);
        rotationStream = rotationStream * deltaRotation;
    }

    //handles the basic calculation of velocity
    void CalculateDrivingVelocity(float deltaTime, GroundInfo groundInfo, Quaternion rotationStream)
    {
        Vector3 localVelocity = Quaternion.Inverse(rotationStream) * Quaternion.Inverse(m_DriftOffset) * _velocity;
        if (groundInfo.isGrounded)
        {
            //strafe moves were tested and though they worked fine, it was decided it did not fit the gameplay currently being developed

            ////float strafing = m_Input.Strafing;
            ////    //Input.GetAxis("Mouse X") * 400f;

            //if (m_Input.Strafing != 0)
            //{
            //    //while ship is grounded, horizontal mouse movement is used to strafe the ship
            //    localVelocity.x = Mathf.MoveTowards(localVelocity.x, m_Input.Strafing * 100f, 200f * deltaTime);
            //}
            //else //if no strafing value is passed, reset horizontal velocity to 0 by grip
            //{
            //    localVelocity.x = Mathf.MoveTowards(localVelocity.x, 0f, m_ModifiedStats.grip * deltaTime);  
            //    //horizontal movement is slowly eased up by grip; low grip = slippy
            //}

            localVelocity.x = Mathf.MoveTowards(localVelocity.x, 0f, grip * deltaTime);



            //float acceleration = m_HasControl ? m_Input.Acceleration : localVelocity.z > 0.05f ? -1f : 0f;
            //may need to re implement this, my current acceleration value is significantly different
            //m_Input.Acceleration is always 1,0, or -1
            //fixed this simply





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

        //if (rotating)
        //{
        //    localVelocity.y = Mathf.MoveTowards(localVelocity.y, 0f, m_ModifiedStats.coastingDrag * deltaTime);
        //}

        currentSpeed = localVelocity.z;

        _velocity = m_DriftOffset * rotationStream * localVelocity;

        //gravity is applied to the ship when it is not touching the ground
        if (!groundInfo.isCapsuleTouching && groundInfo.isCloseToGround)
        {
            //_velocity += -transform.up * m_ModifiedStats.gravity * deltaTime; //gravity is always applied in the object downward direction

            //float gravityPower = (gravity + gravityMod) * deltaTime;

            //_velocity += -transform.up * (gravity + gravityMod) * deltaTime; //this one is ok
            //_velocity += -transform.up * gravity * deltaTime; //this one is modified; when close to ground no need for strong gravity? NO it was needed

            Vector3 fallVelocity = _velocity;

            fallVelocity += -transform.up * (gravity + gravityMod) * deltaTime;

            fallVelocity.y = Mathf.Clamp(fallVelocity.y, -currentMaxSpeed, currentMaxSpeed);
            fallVelocity.x = Mathf.Clamp(fallVelocity.x, -currentMaxSpeed, currentMaxSpeed);
            fallVelocity.z = Mathf.Clamp(fallVelocity.z, -currentMaxSpeed, currentMaxSpeed);

            _velocity = fallVelocity;


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
                if (!doRoll) //if player holds the button, gravity does not get applied; will add cooldown/timer
                {
                    //velocity += -transform.up * m_ModifiedStats.gravity * deltaTime;
                    //_velocity += -transform.up * (gravity + gravityMod) * deltaTime; //remember this one is ok
                    //_velocity += Vector3.down * (gravity + gravityMod) * deltaTime;

                    Vector3 fallVelocity = _velocity;

                    fallVelocity += -transform.up * (gravity + gravityMod) * deltaTime;

                    fallVelocity.y = Mathf.Clamp(fallVelocity.y, -currentMaxSpeed, currentMaxSpeed);
                    fallVelocity.x = Mathf.Clamp(fallVelocity.x, -currentMaxSpeed, currentMaxSpeed);
                    fallVelocity.z = Mathf.Clamp(fallVelocity.z, -currentMaxSpeed, currentMaxSpeed);

                    _velocity = fallVelocity;

                    //print(_velocity.y);

                    //Vector3 max_velocity = _velocity + -transform.up * currentMaxSpeed;

                    //print(max_velocity);

                    //_velocity = Vector3.Min(_velocity, max_velocity);
                }

            }
            else
            {
                //this part ought to only occur when lastKnownGroundInfoNormal = Vector3.zero

                //_velocity += Vector3.down * m_ModifiedStats.gravity * deltaTime;
                _velocity += Vector3.down * gravity * deltaTime;
                //print("this should only be printed at the very start");
            }

        }
    }

    void Jump(Quaternion rotationStream, GroundInfo currentGroundInfo)
    {
        if (currentGroundInfo.isGrounded && Input.GetKey(KeyCode.Space))
        {
            //print("velocity: " + _velocity + " rotationStram: " + rotationStream + " Vector3.Up: " + Vector3.up);

            _velocity += rotationStream * Vector3.up * jumpSpeed;
            //_velocity += rotationStream * transform.up * jumpSpeed;


            //print("velocity: " + _velocity);
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
                        //to be really cool, the punishment could depend on the velocity returned by ModifyVelocity, so if it hits something hard it's worse
                        //not sure how to get the "size" of the velocity return
                        if(!isHuman)
                            this.gameObject.GetComponent<MLADrive>().AddReward(-0.1f);
                            //this.gameObject.GetComponent<MLADrive>().EndEpisode();


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

                //if a wall is found
                if (m_RaycastHitBuffer[i].collider.GetComponent<WallCollider>() != null)
                {
                    _velocity = m_RaycastHitBuffer[i].collider.GetComponent<WallCollider>().ModifyVelocity(this.gameObject, m_RaycastHitBuffer[i]);

                    if (Mathf.Abs(Vector3.Dot(m_RaycastHitBuffer[i].normal, transform.up)) <= .2f) //from vEctor3.up
                    {
                        //to be really cool, the punishment could depend on the velocity returned by ModifyVelocity, so if it hits something hard it's worse
                        //not sure how to get the "size" of the velocity return
                        if(!isHuman)
                            this.gameObject.GetComponent<MLADrive>().AddReward(-0.1f);
                            //this.gameObject.GetComponent<MLADrive>().EndEpisode();



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

    //public GameObject GetColliderScript(Collider foundCollider)
    //{
    //    //returns either WallCollider or PlayerController, whichever is found in the foundCollider
    //    GameObject thingy;
    //        //= foundCollider.GetComponent<>

    //    if(foundCollider.GetComponent<WallCollider>() != null)
    //    {
    //        thingy = foundCollider.gameObject;
    //        return thingy;
    //    }
    //    else if(foundCollider.GetComponent<PlayerController>() != null)
    //    {
    //        thingy = foundCollider.gameObject;
    //        return thingy;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

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

                return GetComponent<PlayerController>()._velocity + toCollidingShip * (1f - weightDifference);
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

                return GetComponent<TrafficController>()._velocity + toCollidingShip * (1f - weightDifference);
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

    // public Vector3 ModifyVelocity(PlayerController collidingShip, RaycastHit collisionHit)
    // {
    //     float weightDifference = collidingShip.weight - weight;
    //     if (weightDifference <= 0f)
    //     {
    //         Vector3 toCollidingShip = (collidingShip.transform.position - _bodyPosition).normalized;

    //         //print("bumped other ship!");
    //         //invoke ship collision
    //         //OnShipCollision.Invoke();

    //         //the 10f here was set as a variable, but never updated, so I'll hardcode it I guess
    //         return collidingShip._velocity + toCollidingShip * (10f - weightDifference);


    //     }
    //     //print("bumped other ship?");
    //     return collidingShip._velocity;
    // }

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
        var capsuleAxis = rotationStream * Vector3.forward * _cap.height * 0.5f;
        var point0 = _bodyPosition + capsuleAxis + summedOffset;
        var point1 = _bodyPosition - capsuleAxis + summedOffset;
        var shipCapsuleHitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, _cap.radius, m_ColliderBuffer, Ground, QueryTriggerInteraction.Ignore);
        //var shipCapsuleHitCount = Physics.OverlapCapsuleNonAlloc(point0, point1, _cap.radius, m_ColliderBuffer, allCollidingLayers, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < shipCapsuleHitCount; i++)
        {
            var hitCollider = m_ColliderBuffer[i];
            if (hitCollider == _cap) //ignore ship's own collider
                continue;

            var hitColliderTransform = hitCollider.transform;
            if (Physics.ComputePenetration(_cap, _bodyPosition + summedOffset, rotationStream, hitCollider, hitColliderTransform.position, hitColliderTransform.rotation, out Vector3 separationDirection, out float separationDistance))
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

    //public Vector3 ModifyVelocity(GameObject collidingShip, RaycastHit collisionHit)
    //{
    //    float weightDifference = collidingShip.weight - m_ModifiedStats.weight;
    //    if (weightDifference <= 0f)
    //    {
    //        Vector3 toCollidingShip = (collidingShip.Position - m_RigidbodyPosition).normalized;

    //        //print("bumped other ship!");
    //        //invoke ship collision
    //        //OnShipCollision.Invoke();

    //        return collidingShip.Velocity + toCollidingShip * (shipToBump - weightDifference);


    //    }
    //    //print("bumped other ship?");
    //    return collidingShip.Velocity;
    //}
}
