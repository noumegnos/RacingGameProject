using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class ShipController : MonoBehaviour
{
    //essential variables, used to control the basic movement of the ship
    float acceleration;
    Rigidbody rb;
    Vector3 rbPosition;
    Vector3 rbMovement;
    Vector3 rbVelocity;
    Quaternion unrotation = Quaternion.identity; //Quaternion.identity is no rotation
    Collider col; //the collider attached to this gameobject

    //Permanent Stats. These may vary based on ship or driver, which I will implement later
    public float accelerationStat; //controls how quickly you accelerate
    public float maxSpeedStat; //maximum speed
    public float maxStrafe; //maximum speed of horizontal movement
    public float reverseSpeed; //maximum speed when reversing
    public float reverseAcceleration; //acceleration when reversing
    public float coastingDrag = 1f; //rate at which speed is lost when not accelerating or braking
    public float braking = 1f; //rate at which speed is lost when braking
    public float grip = 1f; //how quickly lateral movement is lost; slipperiness
    public float weight = 1f; //weight impacts collisions between ships, the one with the lesser is bumped more
    public float jumpSpeed = 2; //how high you can jump; everyone should have same jumpiness
    float defJumpSpeed; //if jumpSpeed is limited by something, it can be reset to this value after it is no longer limited
    public float gravity = 9.8f; //gravity controls rate of descent
    public float gravityMod = 1f; //how quickly gravity is applied
    public float maxFuel; //fuel is depleted when using nitro booster
    public float fuelConsumption; //rate of fuel loss
    public float fuel; //current fuel levels
    public float nitroBoostStat; //stat controlling amount of speed boost gained while using fuel
    float nitroBoost;
    public float maxAmmo; //total ammo for weapon
    public float ammo; //current ammo for weapon
    public float ammoConsumption; //rate of ammo loss while shooting
    public float weaponDamage; //damage done by weapon
    bool canShoot;

    //public Transform shotPoint;

    public float currentSpeed;

    //Temporary Stats. These are added to the ship when you hit a power up
    public float speedMod = 0f; //added to maxSpeed and accelerationStat when hit speed boost
    public float slowMod = 1f; //currentSpeed and accelerationStat is divided by slowMod when hit slow thing
    float defaultSpeedMod; //a default value for speed and slow mods is used to reset to normal values after boosting or slowing
    float defaultSlowMod;
    public bool isBoosting; //boosting is true which speed mod or slow mod are being affected

    //cooldown timers are used to make sure certain events only happen in one frame
    //when jumping, we need to make sure the jumpSpeed variable isn't added to the velocity more than once
    //similarly, I will later add cooldowns for triggering powerups in the world; eg a speed boost should only have speed added to ship once
    public float jumpCooldown = 1f;
    float defaultJumpCooldown; //to reset the cooldown once the event is over, the default value is set in Start function; this way we can edit the cooldown from outside the script and it will still work
    public float speedCooldown = 1f; //a cooldown for speedMod and slowMod 
    float defaultSpeedCooldown;
    public float shootCooldown;
    public float defaultShootCooldown;

    //layers are used to detect the world around the ship via Raycasts
    public LayerMask Ground; //only the ground layer (but could make it all layers that can act like ground, since you can drive on top of cars as well - but they can be ground layer no problem)
    public LayerMask allCollidingLayers; //including ground, no triggers
    public LayerMask allLayers; //including triggers

    //information about the ground under the ship
    public bool isGrounded;
    public float groundedDistance; //distance from ground at which ship is considered grounded
    public Transform frontGroundRaycast;
    public Transform rightGroundRaycast;
    public Transform leftGroundRaycast;
    public Transform backGroundRaycast;
    Vector3 groundAngleLeft;
    Vector3 groundAngleRight;
    Vector3 groundAngleFront;
    Vector3 groundAngleBack;
    Vector3 groundNormal;
    Vector3 lastKnownGroundNormal;

    //information about impending collisions
    public float collisionDistance; //probably a minimum of 0.5f; ray's origin is at 0,0,0 in the ship, with ship's collider at radius of 0.5, but collisionDistance could be a little outside that
    public Transform forwardCollisionRaycast;
    public Transform leftDiagonalCollisionRaycast;
    public Transform rightDiagonalCollisionRaycast;
    public Transform leftSideCollisionRaycast;
    public Transform rightSideCollisionRaycast;

    //hits bools are used if ground is detected below the ship at a distance
    bool hit1;
    bool hit2;
    bool hit3;
    bool hit4;

    //fall timer is used to check if the ship has fallen off the track
    //3 seconds is an ok number
    public float fallTime;
    float defFallTime;

    //ground bools are used if detected ground below ship is within groundedDistance
    public bool ground1;
    public bool ground2;
    public bool ground3;
    public bool ground4;
    public float hitDistance; //how far away is the furthest ground checker hit, if this exceeds the ground checker rays distance, give agent a penalty for being off track

    public bool canJump;
    public bool isJumping;

    string objectUnderMeTag;
    bool jumpCheckTag = false; //when jumping, we attempt to detect the gameobject we jump off of, and compare it to what we land on and if anything was different - this bool tells us this is done
    bool didDetectChange = false;

    //ML Agent
    public bool isHuman;
    public bool useTimeout;
    public float timeout = 60f; //if agent is stuck somewhere, a timer will run out and reposition ship
    public float defTimeout;

    //controls
    public float input;
    public float deadzone = 0.05f; //deadzone is used for input and acceleration; if these have values close to 0 they will count as being zero 
    public bool controlsEnabled; //sometimes control must be taken away, such as the very start of the race, and after ship has reached the goal, and when being respawned

    //other script references
    DamageManager damageManager; //damage manager handles hit points and taking damage; all damageable objects have this script
    public CheckpointManager checkpointManager; //checkpoint manager contains a list of all checkpoints... is this necessary?
    public LevelGenerator levelGenerator;
    public RaceManager raceManager;

    //checkpoint handling
    //checkpoints are used to keep track of how far a ship has traveled, and to reposition the ship in case it gets destroyed or falls off the track
    public GameObject lastCheckpoint; //when crossing a checkpoint trigger, that checkpoint is set to be this object
    //public Transform lastCheckpointTransform; //when crossing a checkpoint trigger, that checkpoint is set to be this object
    //public GameObject nextCheckpoint; //not sure if needed, more of a test of a system (could be used for ML agents)
    public Vector3 nextCheckpointTransform; //not sure if needed, more of a test of a system (could be used for ML agents)
    public Vector3 originPoint; //this is the starting location
    public float distanceToNextCheckpoint;
    public bool respawnIsOrigin; //respawn point is the origin point at the start, at which point lastCheckpoint is null. But last checkpoint can also become null if the track is long enough, in which case FindOldestExistingCheckpoint is called from level generator
    public int lastCheckpointNumber;

    public float aimShootTime; //when you hold the shoot button for this amount of time, the shot will attempt to aim at a nearby ship
    public float defAimShootTime;
    public float aimWidth; //this determines how far away on the X and Y axes an aim shot can be - 0 would mean it can only shoot straight ahead, from testing 5 can shoot nearly 90degrees
    public float shootRange;

    public float weaponSlowMod; //when a ship shoots another, that ship is slowed by this value

    public float gotShotMod; //if a ship is hit by a weapon from another ship, it slows down a little by this value
    float defGotShotMod;

    public int myPositionInRace;

    public float turnValue; //this float is based on input, before collision modifies it; used by animations

    public WeaponsSystems weaponsSystems; //given by create ship model
    public bool waitForTarget;
    public bool targetting;
    //public float targettingTime;

    //if career mode is on, ships are not automatically repaired when repositioned, and racer will lose if hit points go to 0
    public bool careerMode;

    //while the agent is training, a number of its stats are randomized
    public bool inTraining;

    public MainMenuScript mainMenu;

    //private void Awake()
    //{
    //    if (shotPoint == null)
    //    {
    //        shotPoint = transform;
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        damageManager = GetComponent<DamageManager>();
        checkpointManager = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();

        levelGenerator = GameObject.FindObjectOfType<LevelGenerator>();
        raceManager = GameObject.FindObjectOfType<RaceManager>();

        mainMenu = GameObject.FindObjectOfType<MainMenuScript>();

        //mainMenu = GameObject.Find("MainMenu").GetComponent<MainMenuScript>();

        defaultJumpCooldown = jumpCooldown;
        defaultSpeedCooldown = speedCooldown;
        defaultSlowMod = slowMod;
        defaultSpeedMod = speedMod;
        defaultShootCooldown = shootCooldown;
        fuel = maxFuel;
        ammo = maxAmmo;
        defFallTime = fallTime;
        defJumpSpeed = jumpSpeed;

        defGotShotMod = gotShotMod;

        defTimeout = timeout;

        defAimShootTime = aimShootTime;

        respawnIsOrigin = true;
        originPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        nextCheckpointTransform = checkpointManager.dictOfChecks[1];



    }

    

    // Update is called once per frame
    void Update()
    {
        if(isHuman)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                print("pause key");
                if (mainMenu.pauseMenuButtonHolder.activeSelf)
                {
                    //ToggleMenuOff();

                    mainMenu.PauseMenuOff();

                    //mainMenu.UnpauseTime();

                }
                else
                {
                    //ToggleMenuOn();
                    mainMenu.PauseMenuOn();
                    //mainMenu.PauseTime();
                }
            }
        }


        //if(controlsEnabled)
        //{
        //    if (isHuman)
        //    {
        //        input = Input.GetAxis("Horizontal");

        //        if (Input.GetKey(KeyCode.UpArrow))
        //        {
        //            acceleration = 1f;
        //        }
        //        else if (Input.GetKey(KeyCode.DownArrow))
        //        {
        //            acceleration = -1f;
        //        }
        //        else
        //        {
        //            acceleration = 0f;
        //        }

        //        if (Input.GetKey(KeyCode.Z))
        //        {
        //            //nitro boost
        //            if(fuel >= 0f)
        //            {
        //                fuel -= fuelConsumption * Time.deltaTime;
        //                nitroBoost = nitroBoostStat;
        //            }
        //        }
        //        else
        //        {
        //            nitroBoost = 0f;
        //        }

        //        if (Input.GetKey(KeyCode.X)) 
        //        {
        //            //shooting
        //            //the shot origin point should be at the ship's model, instead of this object, which means I have to GetComponent it somehow 
        //            //but lets make it work properly first

        //            if(canShoot)
        //            {
        //                Shoot();
        //                canShoot = false;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //ML inputs
        //        input = gameObject.GetComponent<MLADrive2>().steering;
        //        acceleration = gameObject.GetComponent<MLADrive2>().accelerate;

        //        //jump is in FixedUpdate

        //        //also nitro
        //        if(GetComponent<MLADrive2>().boost != 0)
        //        {
        //            if (fuel >= 0f)
        //            {
        //                fuel -= fuelConsumption * Time.deltaTime;
        //                nitroBoost = nitroBoostStat;
        //            }
        //        }
        //        else
        //        {
        //            nitroBoost = 0f;
        //        }

        //        //also shooting
        //        if(GetComponent<MLADrive2>().shoot != 0)
        //        {
        //            if (canShoot)
        //            {
        //                Shoot();
        //                canShoot = false;
        //            }
        //        }
        //    }

        //    if (!canShoot)
        //    {
        //        shootCooldown -= Time.deltaTime;

        //        if (shootCooldown <= 0f)
        //        {
        //            canShoot = true;
        //            shootCooldown = defaultShootCooldown;
        //        }
        //    }
        //}

        if (controlsEnabled && !isHuman && useTimeout)
        {
            timeout -= Time.deltaTime;

            if (timeout < 0f)
            {
                RepositionAfterDelay();

                timeout = defTimeout;
            }
        }

        //if(nextCheckpoint != null)
        //{
        //    print(rb.transform.position + " " + nextCheckpoint.transform.position);
        //    distanceToNextCheckpoint = Vector3.Distance(rb.transform.position, nextCheckpoint.transform.position);
        //}

        if(nextCheckpointTransform != Vector3.zero)
        {
            distanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckpointTransform);

        }
        else
        {
            nextCheckpointTransform = checkpointManager.dictOfChecks[1];
        }


        //Level Generator now has its own ResetLevel function instead
        ////restart and quit; mainly used for debugging purposes now
        //if (Input.GetKey(KeyCode.R))
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //}

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        //a cooldown prevents jumping from happening more than once a frame, otherwise multiple jumps can happen randomly
        if(isJumping)
        {
            jumpCooldown -= Time.deltaTime;

            if (jumpCooldown <= 0f)
            {
                isJumping = false;

                jumpCooldown = defaultJumpCooldown;
            }
        }

        //a cooldown for speed up and slow down boosters will reset to default values
        if(isBoosting)
        {
            speedCooldown -= Time.deltaTime;

            if(speedCooldown <= 0f)
            {
                isBoosting = false;
                slowMod = defaultSlowMod;
                speedMod = defaultSpeedMod;
                speedCooldown = defaultSpeedCooldown;
            }
        }

        if(gotShotMod != defGotShotMod)
        {
            gotShotMod = Mathf.Lerp(gotShotMod, defGotShotMod, 2f * Time.deltaTime);
        }
    }

    public void Inputs()
    {
        if (controlsEnabled)
        {
            if (isHuman)
            {
                input = Input.GetAxis("Horizontal");

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

                if (Input.GetKey(KeyCode.Z) && fuel >= 0f)
                {
                    //nitro boost
                    fuel -= fuelConsumption * Time.deltaTime;
                    nitroBoost = nitroBoostStat;
                }
                else
                {
                    nitroBoost = 0f;
                }

                if(!waitForTarget)
                {

                    if (Input.GetKey(KeyCode.X) && canShoot)
                    {
                        aimShootTime -= Time.deltaTime;

                        if (aimShootTime <= 0f)
                        {
                            if (canShoot)
                            {
                                TakeAim();
                                canShoot = false;
                                aimShootTime = defAimShootTime;
                            }
                        }
                    }
                    else
                    {
                        if (aimShootTime < defAimShootTime)
                        {
                            aimShootTime = defAimShootTime;

                            if (canShoot)
                            {
                                weaponsSystems.Fire(null);

                                canShoot = false;
                            }
                        }
                    }
                }
                else
                {
                    //wait for target to be acquired before shooting while holding button

                    if(Input.GetKey(KeyCode.X))
                    {
                        targetting = true;
                        AcquireTarget();
                    }
                    else if (targetting)
                    {
                        if (canShoot)
                        {
                            TakeAim();
                            targetting = false;
                            canShoot = false;
                        }
                    }
                    else
                    {
                        targetting = false;

                    }
                }

            }
            else
            {
                //ML inputs
                input = gameObject.GetComponent<MLADrive2>().steering;
                acceleration = gameObject.GetComponent<MLADrive2>().accelerate;

                //jump is in FixedUpdate

                //also nitro
                if (GetComponent<MLADrive2>().boost != 0 && fuel >= 0f)
                {
                    fuel -= fuelConsumption * Time.deltaTime;
                    nitroBoost = nitroBoostStat;

                }
                else
                {
                    nitroBoost = 0f;
                }

                //also shooting
                if (GetComponent<MLADrive2>().shoot != 0)
                {
                    aimShootTime -= Time.deltaTime;

                    if (aimShootTime <= 0f)
                    {
                        if (canShoot)
                        {
                            TakeAim();
                            canShoot = false;
                            aimShootTime = defAimShootTime;
                        }
                    }
                }
                else
                {
                    if(aimShootTime < defAimShootTime)
                    {
                        aimShootTime = defAimShootTime;

                        if(canShoot)
                        {
                            weaponsSystems.Fire(null);
                            canShoot = false;
                        }
                    }

                    aimShootTime = defAimShootTime;
                }
            }

            if (!canShoot)
            {
                shootCooldown -= Time.deltaTime;

                if (shootCooldown <= 0f)
                {
                    canShoot = true;
                    shootCooldown = defaultShootCooldown;
                }
            }
        }
    }

    public void AcquireTarget()
    {
        if (myPositionInRace > 0)
        {
            if (raceManager.Orders[myPositionInRace - 1].transform.position.z - transform.position.z < shootRange)
            {
                GameObject target = raceManager.Orders[myPositionInRace - 1];

                if (Mathf.Abs(raceManager.Orders[myPositionInRace - 1].transform.position.x - transform.position.x) < aimWidth && Mathf.Abs(raceManager.Orders[myPositionInRace - 1].transform.position.y - transform.position.y) < aimWidth)
                {
                    if(canShoot)
                    {
                        weaponsSystems.Fire(target);
                        canShoot = false;
                        targetting = false;

                    }
                }
            }
        }
    }

    public void TakeAim()
    {
        if (myPositionInRace > 0 && raceManager.Orders.Count > 0)
        {
            if (raceManager.Orders[myPositionInRace - 1].transform.position.z - transform.position.z < shootRange)
            {

                GameObject target = raceManager.Orders[myPositionInRace - 1];

                if (Mathf.Abs(raceManager.Orders[myPositionInRace - 1].transform.position.x - transform.position.x) < aimWidth && Mathf.Abs(raceManager.Orders[myPositionInRace - 1].transform.position.y - transform.position.y) < aimWidth)
                {
                    weaponsSystems.Fire(target);
                }
                else
                {
                    //target too far aside, normal shot
                    weaponsSystems.Fire(null);
                }
            }
            else
            {
                //no target, do normal shot
                weaponsSystems.Fire(null);
            }
        }
        else
        {
            //my position is race is 0, just shoot straight forward
            weaponsSystems.Fire(null);
        }
    }

    //public void Fire(Vector3 direction)
    //{
    //    if (ammo > 0f)
    //    {
    //        ammo -= ammoConsumption;

    //        RaycastHit hit;

    //        Ray ray = new Ray(transform.position, direction);

    //        if (Physics.Raycast(ray, out hit, shootRange, allCollidingLayers))
    //        {
    //            Debug.DrawRay(transform.position, direction * hit.distance, Color.yellow, 0.3f);

    //            if (hit.collider.GetComponent<DamageManager>() != null)
    //            {
    //                hit.collider.GetComponent<DamageManager>().TakeDamage(weaponDamage);

    //                if (hit.collider.gameObject.GetComponent<ShipController>() != null)
    //                {

    //                    hit.collider.gameObject.GetComponent<ShipController>().gotShotMod = weaponSlowMod;

    //                    if (!isHuman)
    //                    {
    //                        GetComponent<MLADrive2>().AddReward(0.002f);


    //                    }
    //                }
    //                else
    //                {
    //                    if (!isHuman)
    //                    {
    //                        GetComponent<MLADrive2>().AddReward(0.001f);


    //                    }
    //                }


    //            }
    //            else
    //            {
    //                //shot something indestructible
    //                if (!isHuman)
    //                {
    //                    GetComponent<MLADrive2>().AddReward(-0.01f);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            //shot missed

    //            Debug.DrawRay(transform.position, direction * shootRange, Color.yellow, 0.3f);

    //            if (!isHuman)
    //            {
    //                GetComponent<MLADrive2>().AddReward(-0.01f);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        //no ammo
    //        if (!isHuman)
    //        {
    //            GetComponent<MLADrive2>().AddReward(-0.01f);
    //        }
    //    }
    //}

    //public void Shoot()
    //{
    //    if(ammo > 0f)
    //    {
    //        ammo -= ammoConsumption;


    //        RaycastHit hit;

    //        Ray ray = new Ray(transform.position, transform.forward);

    //        if (Physics.Raycast(ray, out hit, 20f, allCollidingLayers))
    //        {
    //            Debug.DrawRay(transform.position, Vector3.forward * hit.distance, Color.yellow, 0.3f);

    //            if (hit.collider.GetComponent<DamageManager>() != null)
    //            {
    //                hit.collider.GetComponent<DamageManager>().TakeDamage(weaponDamage);

    //                if (!isHuman)
    //                {
    //                    GetComponent<MLADrive2>().AddReward(0.001f);
    //                }
    //            }
    //            else
    //            {
    //                //shot something indestructible
    //                if (!isHuman)
    //                {
    //                    GetComponent<MLADrive2>().AddReward(-0.001f);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            //shot missed
    //            if (!isHuman)
    //            {
    //                GetComponent<MLADrive2>().AddReward(-0.005f);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        //no ammo
    //        if (!isHuman)
    //        {
    //            GetComponent<MLADrive2>().AddReward(-0.001f);
    //        }
    //    }

    //}

    public void FixedUpdate()
    {


        rbPosition = rb.position;

        Quaternion rotationStream = rb.rotation;

        //if (!controlsEnabled)
        //{
        //    rotationStream = Quaternion.identity;
        //}

        float deltaTime = Time.deltaTime;

        //need some ground info

        if(controlsEnabled)
        {
            if (isHuman)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    Jump(rotationStream);
                }
            }
            else
            {
                if(GetComponent<MLADrive2>().jump != 0)
                {
                    Jump(rotationStream);
                }
            }

        }
        if(!isHuman)
        {
            TagChecker();

        }

        GroundChecking(deltaTime, ref rotationStream);



        CalculateVelocity(deltaTime, rotationStream);

        Inputs();

        //turnValue is used by animation
        turnValue = input;
        //if(turnValue > -deadzone && turnValue < deadzone)
        //{
        //    noTurnValue = true;
        //}
        //else
        //{
        //    noTurnValue = false;
        //}

        CollisionDetection();



        //print(rotationStream);

        if (controlsEnabled)
        {
            rotationStream = Quaternion.RotateTowards(rb.rotation, rotationStream, 300f);

            rbMovement = rbVelocity * deltaTime;

            rb.MoveRotation(rotationStream);
            rb.MovePosition(rbPosition + rbMovement);
        }
        //else
        //{
        //    rotationStream = Quaternion.RotateTowards(rb.rotation, Quaternion.identity, 300f);
        //    rb.MoveRotation(rotationStream);


        //}

    }

    public void LateUpdate()
    {
        //test if this works!
        //seems to work but keep an eye on it
        ResetDrivingVector();

        //CheckIfInsideCollider();
    }

    public void RepositionAfterDelay()
    {
        if (!isHuman)
        {
            GetComponent<MLADrive2>().AddReward(-.05f);
            //GetComponent<MLADrive2>().EndEpisode();

        }

        controlsEnabled = false;

        if (GetComponent<Collider>().enabled)
        {
            GetComponent<Collider>().enabled = false;
        }

        Invoke("RepositionShip", 2f);
        //if(!isHuman )
        //{
        //    GetComponent<MLADrive2>().EndEpisode();

        //}

    }

    //this function is used to respawn the ship if destroyed, using last checkpoint transform
    public void RepositionShip()
    {
        if (!isHuman)
        {
            GetComponent<MLADrive2>().EndEpisode();

        }

        controlsEnabled = false;

        //ensure that no momentum carries over when being repositioned
        rbMovement = Vector3.zero;
        rbVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;

        lastCheckpointNumber = 0;


        if (lastCheckpoint != null && lastCheckpoint.GetComponentInChildren<SetRespawnRotation>() != null)
        {
            respawnIsOrigin = false;

            float x = lastCheckpoint.GetComponentInChildren<SetRespawnRotation>().transform.position.x;
            float y = lastCheckpoint.GetComponentInChildren<SetRespawnRotation>().transform.position.y;
            float z = lastCheckpoint.GetComponentInChildren<SetRespawnRotation>().transform.position.z;

            transform.position = new Vector3(x + originPoint.x, y, z + originPoint.z);

            //rb.position = new Vector3(x, y, z);
            //print("respawn at " + x + " " + y + " " + z);

            //print("rotation to " + lastCheckpoint.GetComponentInChildren<SetRespawnRotation>().gameObject.transform.rotation);

            transform.rotation = lastCheckpoint.transform.rotation;
            //rb.rotation = lastCheckpoint.transform.rotation;
            //rb.MoveRotation(lastCheckpoint.GetComponentInChildren<SetRespawnRotation>().gameObject.transform.rotation);

            //transform.rotation = 
        }
        else if(respawnIsOrigin)
        {
            transform.position = originPoint;
        }
        else
        {
            Transform pos = levelGenerator.FindOldestExistingCheckpoint();

            float x = pos.transform.position.x;
            float y = pos.transform.position.y;
            float z = pos.transform.position.z;

            transform.position = new Vector3(x + originPoint.x, y, z + originPoint.z);
            transform.rotation = pos.transform.rotation;


        }

        if(!GetComponent<Collider>().enabled)
        {
            GetComponent<Collider>().enabled = true;
        }


        fallTime = defFallTime;

        controlsEnabled = true;
    }

    //collision detection uses a series of Raycasts to detect if any object is too close, and reduces speed if so
    //this still needs to check the angles of what it is hitting; now if you hit a ramp it will decide to stop you going up I think (test it! could be a matter of how slopey the slope is)
    //also, this may require some checking if the object being hit is another ship
    public void CollisionDetection()
    {
        RaycastHit hitForward;
        RaycastHit hitDiagonalLeft;
        RaycastHit hitDiagonalRight;
        RaycastHit hitLeftSide;
        RaycastHit hitRightSide;
        RaycastHit hitUp;
        RaycastHit hitBack;

        Ray rayForward = new Ray(forwardCollisionRaycast.position, forwardCollisionRaycast.transform.forward);
        Ray rayDiagonalLeft = new Ray(leftDiagonalCollisionRaycast.position, leftDiagonalCollisionRaycast.transform.forward);
        Ray rayDiagonalRight = new Ray(rightDiagonalCollisionRaycast.position, rightDiagonalCollisionRaycast.transform.forward);
        Ray raySideLeft = new Ray(leftSideCollisionRaycast.position, leftSideCollisionRaycast.transform.forward);
        Ray raySideRight = new Ray(rightSideCollisionRaycast.position, rightSideCollisionRaycast.transform.forward);
        Ray rayUp = new Ray(transform.position, transform.up);
        Ray rayBack = new Ray(transform.position, -transform.forward);

        if(Physics.Raycast(rayUp, out hitUp, 5f, allCollidingLayers))
        {
            if(hitUp.distance < 5f)
            {
                jumpSpeed = hitUp.distance;

            }
        }
        else
        {
            jumpSpeed = defJumpSpeed;
        }

        if (Physics.Raycast(rayForward, out hitForward, collisionDistance, allCollidingLayers))
        {
            if(hitForward.distance < collisionDistance)
            {
                if (!isHuman)
                {
                    GetComponent<MLADrive2>().AddReward(-.005f);
                }


                if (rbVelocity.z > maxSpeedStat / 10) //just testing some values; 10 is fine, 20% of maxSpeed sounds cool
                {

                    damageManager.TakeDamage(1);

                    if(hitForward.collider.GetComponent<DamageManager>() != null)
                    {
                        hitForward.collider.GetComponent<DamageManager>().TakeDamage(1);
                    }
                }

                //TEST
                rb.velocity = Vector3.zero;


                rbVelocity.z = Mathf.Min(-1f, rbVelocity.z);

            }
        }

        if (Physics.Raycast(rayDiagonalLeft, out hitDiagonalLeft, collisionDistance, allCollidingLayers))
        {
            if (hitDiagonalLeft.distance < collisionDistance)
            {
                //if (rbVelocity.z > maxSpeedStat / 5) //just testing some values; 10 is fine, 20% of maxSpeed sounds cool
                //{

                //    damageManager.TakeDamage(1);
                //}

                if (!isHuman)
                {
                    GetComponent<MLADrive2>().AddReward(-.005f);
                }

                //rbVelocity.z = Mathf.Min(1f, rbVelocity.z);

                //TEST
                rb.velocity = Vector3.zero;

                input = Mathf.Max(0.5f, input);

                //rbVelocity.x = Mathf.Max(0f, rbVelocity.x);
            }
        }

        if (Physics.Raycast(rayDiagonalRight, out hitDiagonalRight, collisionDistance, allCollidingLayers))
        {
            if (hitDiagonalRight.distance < collisionDistance)
            {
                //if (rbVelocity.z > maxSpeedStat / 5) //just testing some values; 10 is fine, 20% of maxSpeed sounds cool
                //{

                //    damageManager.TakeDamage(1);
                //}

                if (!isHuman)
                {
                    GetComponent<MLADrive2>().AddReward(-.005f);
                }

                //rbVelocity.z = Mathf.Min(1f, rbVelocity.z);

                //TEST
                rb.velocity = Vector3.zero;

                input = Mathf.Min(-0.5f, input);

                //rbVelocity.x = Mathf.Min(0f, rbVelocity.x);
            }
        }

        if (Physics.Raycast(raySideLeft, out hitLeftSide, collisionDistance * 2, allCollidingLayers))
        {
            if (hitLeftSide.distance < collisionDistance)
            {
                if (!isHuman)
                {
                    GetComponent<MLADrive2>().AddReward(-.005f);
                }

                input = Mathf.Max(0.5f, input);

                //TEST
                rb.velocity = Vector3.zero;

                //rbVelocity.x = Mathf.Max(0f, rbVelocity.x);
            }
        }

        if (Physics.Raycast(raySideRight, out hitRightSide, collisionDistance * 2, allCollidingLayers))
        {
            if (hitRightSide.distance < collisionDistance)
            {
                if (!isHuman)
                {
                    GetComponent<MLADrive2>().AddReward(-.005f);
                }

                input = Mathf.Min(-0.5f, input);

                //TEST
                rb.velocity = Vector3.zero;

                //rbVelocity.x = Mathf.Min(0f, rbVelocity.x);
            }
        }

        if (Physics.Raycast(rayBack, out hitBack, collisionDistance, allCollidingLayers))
        {
            if (hitBack.distance < collisionDistance)
            {
                if (!isHuman)
                {
                    GetComponent<MLADrive2>().AddReward(-.005f);
                }
            }
        }

    }

    //debugging tool
    //this function attempts to discover if the ship has ended up inside a collider
    //while this does what it's supposed to, it appears the problem I was trying to figure out wasn't that the ship is inside a collider (or is it)
    //increased the radius of OverlapSphere, and find that we do get caught inside some colliders sometimes
    //what do we do about it?
    //one idea is to destroy ship if the colliders exceed a certain number (2?)
    //ehh this went badly: at the edge of track pieces while going to another one the number of hits can get larger as well (3?)
    //but that's the same number as during the problem case
    //the problem may also not be as bad as all that, since you can somehow jiggle yourself out by backing up and jumping
    //gonna leave this here regardless
    public void CheckIfInsideCollider()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0f);

        int hits = 0;

        if (hitColliders.Length > 1)
        {
            //Debug.Log("Test");

            for (int i = 0; i < hitColliders.Length; i++)
            {

                if (!hitColliders[i].isTrigger)
                {
                    if (hitColliders[i] == col)
                    {
                        hits--;
                    }

                    hits++;
                }

                if(hits > 0)
                    print(hits);

                if (hits > 1)
                    RepositionAfterDelay();

            }


        }
    }

    //used to reset the rotation of the ship, which is sometimes affected by collisions
    public void ResetDrivingVector()
    {
        if(rb.rotation.eulerAngles.y != 0f)
        {
            Quaternion desiredRotation = Quaternion.Euler(rb.transform.eulerAngles.x, 0f, rb.transform.eulerAngles.z);

            rb.rotation = Quaternion.RotateTowards(rb.rotation, desiredRotation, Time.deltaTime * 100f);

        }
    }

    //this is a system that allows the ship to detect if it passed over an object when jumping
    //used for the ML agent to determine if it jumped over a barrier
    public void TagChecker()
    {
        if(isJumping)
        {
            RaycastHit tagHit;
            Ray tagRay = new Ray(frontGroundRaycast.position, -transform.up);

            if (Physics.Raycast(tagRay, out tagHit, 20f, allLayers))
            {
                if (!jumpCheckTag)
                {
                    objectUnderMeTag = tagHit.collider.gameObject.tag;

                    jumpCheckTag = true;

                    //print("jumpChecking");
                }

                if (jumpCheckTag)
                {
                    if (tagHit.collider.gameObject.tag != objectUnderMeTag)
                    {
                        didDetectChange = true;

                        //print("detected change");
                    }
                }

                if (isGrounded)
                {
                    if (didDetectChange)
                    {
                        //reward add
                        //print("reward here");
                        GetComponent<MLADrive2>().AddReward(0.011f);
                    }

                    jumpCheckTag = false;
                    didDetectChange = false;
                }
            }
        }

    }

    //ground checking is used to determine what is beneath the ship, and slightly ahead
    //mainly this determines is the ship is considered grounded or not
    //and also decides on the basic rotation, to follow the ground's angles
    public void GroundChecking(float deltaTime, ref Quaternion rotationStream)
    {
        //a RaycastHit is an object containing information which is returned by the Ray
        RaycastHit hitFront;
        RaycastHit hitBack;
        RaycastHit hitLeft;
        RaycastHit hitLeftSide;
        RaycastHit hitRight;
        RaycastHit hitRightSide;

        //a Ray points in a direction, returning a Hit containing information about the object it touched
        Ray rayFront = new Ray(frontGroundRaycast.position, -transform.up);
        Ray rayBack = new Ray(backGroundRaycast.position, -transform.up);
        Ray rayLeft = new Ray(leftGroundRaycast.position, -transform.up);
        Ray rayLeftSide = new Ray(leftGroundRaycast.position, -transform.right);
        Ray rayRight = new Ray(rightGroundRaycast.position, -transform.up);
        Ray rayRightSide = new Ray(rightGroundRaycast.position, transform.right);

        if (Physics.Raycast(rayFront, out hitFront, 20f, Ground))
        {
            hitDistance = hitFront.distance;

            groundAngleFront = hitFront.normal;
            hit1 = true;

            if (hitFront.distance < groundedDistance)
            {
                ground1 = true;
            }
            else
            {
                ground1 = false;
            }
        }
        else
        {
            hit1 = false;
            ground1 = false;

            hitDistance = 999f;

        }

        if (Physics.Raycast(rayBack, out hitBack, 20f, Ground))
        {
            groundAngleBack = hitBack.normal;
            hit2 = true;

            if (hitBack.distance < groundedDistance)
            {
                ground2 = true;
            }
            else
            {
                ground2 = false;
            }
        }
        else
        {
            hit2 = false;
            ground2 = false;

        }

        if (Physics.Raycast(rayLeft, out hitLeft, 20f, Ground))
        {
            groundAngleLeft = hitLeft.normal;

            if (hitLeft.distance < groundedDistance)
            {
                ground3 = true;
            }
            else
            {
                ground3 = false;
            }
            //groundAngleLeft = hitLeft.normal;
            hit3 = true;

            //if (hitLeft.distance < groundedDistance)
            //{
            //    ground3 = true;
            //}
            //else
            //{
            //    ground3 = false;
            //}
        }
        else
        {
            hit3 = false;
            ground3 = false;

        }

        //if (Physics.Raycast(rayRight, out hitRight, 10f, Ground))
        //{
        //    groundAngleRight = hitRight.normal;

        //    if (hitRight.distance < groundedDistance)
        //    {
        //        ground4 = true;
        //    }
        //    else
        //    {
        //        ground4 = false;
        //    }

        //    //groundAngleRight = hitRight.normal;
        //    hit4 = true;

        //    //if (hitRight.distance < groundedDistance)
        //    //{
        //    //    ground4 = true;
        //    //}
        //    //else
        //    //{
        //    //    ground4 = false;
        //    //}
        //}
        //else
        //{
        //    hit4 = false;
        //    ground4 = false;

        //}

        if(Physics.Raycast(rayRight, out hitRight, 20f, Ground))
        {
            groundAngleRight = hitRight.normal;

            hit4 = true;

            if(hitRight.distance < groundedDistance)
            {
                ground4 = true;
            }
            else
            {
                ground4 = false;
            }
        }
        else
        {
            ground4 = false;
            hit4 = false;
        }

        if(Physics.Raycast(rayRightSide, out hitRightSide, 20f, Ground))
        {
            if(hitRightSide.distance < 3f)
            {
                groundAngleRight = hitRightSide.normal;
            }
        }

        if (Physics.Raycast(rayLeftSide, out hitLeftSide, 20f, Ground))
        {
            if (hitLeftSide.distance < 3f)
            {
                groundAngleLeft = hitLeftSide.normal;
            }
        }





        //if (Physics.Raycast(rayRightSide, out hitRightSide, 2f, Ground))
        //{

        //    groundAngleRight = hitRightSide.normal;

        //    if(hitRightSide.distance < groundedDistance)
        //    {
        //        ground4 = true;
        //    }

        //    //if (Vector3.Angle(hitRightSide.normal, rb.rotation * Vector3.up) > 90f)
        //    //{
        //    //    groundAngleRight = hitRightSide.normal;
        //    //}
        //    //else
        //    //{
        //    //    rbVelocity.x = 0f;
        //    //    print("blop");

        //    //}
        //}

        //if (Physics.Raycast(rayLeftSide, out hitLeftSide, 2f, Ground))
        //{
        //    groundAngleLeft = hitLeftSide.normal;

        //    //if (Vector3.Angle(hitLeftSide.normal, rb.rotation * Vector3.up) > 90f)
        //    //{
        //    //    groundAngleLeft = hitLeftSide.normal;
        //    //}
        //    //else
        //    //{
        //    //    rbVelocity.x = 0f;
        //    //    print("blip");

        //    //}
        //}

        //if any of the down-pointing raycasts detect ground within ground distance, isGrounded = true
        if (ground1 | ground2 | ground3 | ground4)
        {
            isGrounded = true;

            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            isGrounded = false;
        }

        //anyHit isn't used yet, but it could be used to check if the ship has fallen through a hole
        //the hits check if it can find anything nearby, and will cause a timer to run down if not, at the end of which ship is repositioned to last checkpoint
        bool anyHit;
        if (hit1 || hit2 || hit3 || hit4)
        {
            anyHit = true;

            fallTime = defFallTime;
        }
        else
        {
            anyHit = false;

            fallTime -= Time.deltaTime;

            if(fallTime < 0f && controlsEnabled)
            {
                //if(!isHuman)
                //{
                //    GetComponent<MLADrive2>().AddReward(-.005f);
                //}

                RepositionAfterDelay();

                lastKnownGroundNormal = hitFront.normal;
            }
        }

        //as a test I made this go here; this is the code that actually aligns the ship with the ground
        //code below has different versions for being airborne or grounded, but I brought them close enough the difference is irrelevant
        //however, with this version you cannot fall off the track if it has Ground layer nearby: this can be fixed by separating the parts of the track model you want to allow falling
        groundNormal = groundAngleLeft + groundAngleRight + groundAngleFront + groundAngleBack;
        lastKnownGroundNormal = groundNormal;

        Vector3 rigidbodyUp = rb.rotation * Vector3.up;
        Quaternion targetRotation = Quaternion.FromToRotation(rigidbodyUp, groundNormal);

        rotationStream = Quaternion.RotateTowards(rotationStream, targetRotation, 2000f) * rotationStream;



        ////if the ship detects the ground nearby, it will align with the normal angle of the ground
        //if (isGrounded)
        //{
        //    groundNormal = groundAngleLeft + groundAngleRight + groundAngleFront + groundAngleBack;
        //    lastKnownGroundNormal = groundNormal;

        //    Vector3 rigidbodyUp = rb.rotation * Vector3.up;
        //    Quaternion targetRotation = Quaternion.FromToRotation(rigidbodyUp, groundNormal);
        //    //Quaternion nextTargetRotation = Quaternion.FromToRotation(rigidbodyUp, groundNormal);

        //    //rotationStream = Quaternion.RotateTowards(rotationStream, targetRotation, 2000000f);

        //    rotationStream = Quaternion.RotateTowards(rotationStream, targetRotation, 2000f) * rotationStream;


        //}
        //else //when airborne
        //{
        //    Vector3 rigidbodyUp = rb.rotation * Vector3.up;

        //    //if no any hits, there is no ground nearby where the ship is falling
        //    //in which case, should the ship return to default rotation?
        //    //doing so would help in a few cases, where the ship is driving along a wall which ends in a barrier leading offtrack
        //    //and if you drive on the ceiling, and fall in the big hole, you'd realign
        //    //these scenarios don't sound right to me; rather if no anyHit you'd continue along lastKnownGroundNormal
        //    if(!anyHit)
        //    {
        //        Quaternion targetRotation = Quaternion.FromToRotation(rigidbodyUp, Vector3.up);

        //        rotationStream = Quaternion.RotateTowards(rotationStream, targetRotation, 0.2f) * rotationStream;

        //    }
        //    else
        //    {
        //        groundNormal = groundAngleLeft + groundAngleRight + groundAngleFront + groundAngleBack;
        //        lastKnownGroundNormal = groundNormal;

        //        Quaternion targetRotation = Quaternion.FromToRotation(rigidbodyUp, groundNormal);

        //        rotationStream = Quaternion.RotateTowards(rotationStream, targetRotation, 2f) * rotationStream;
        //    }

        //    //Quaternion nextTargetRotation = Quaternion.FromToRotation(rigidbodyUp, groundNormal);

        //    //rotationStream = Quaternion.RotateTowards(rotationStream, targetRotation, 2000000f);


        //}
    }

    public void CalculateVelocity(float deltaTime, Quaternion rotationStream)
    {
        //local variable Vector3 which is used to calculate movement
        Vector3 localVelocity = Quaternion.Inverse(rotationStream) * Quaternion.Inverse(unrotation) * rbVelocity;

        //horizontal movement is lost over time at rate of grip stat if no input given
        if (input > -deadzone && input < deadzone)
            localVelocity.x = Mathf.MoveTowards(localVelocity.x, 0f, grip * deltaTime);
        else if (input > deadzone)
            localVelocity.x = Mathf.MoveTowards(localVelocity.x, maxStrafe, grip * 4f * deltaTime);
        else if (input < -deadzone)
            localVelocity.x = Mathf.MoveTowards(localVelocity.x, -maxStrafe, grip * 4f * deltaTime);

        if (isGrounded)
        {


            //vertical movement is restricted when not airborne
            localVelocity.y = Mathf.Max(0f, localVelocity.y);

            if (acceleration > -deadzone && acceleration < deadzone) //no acceleration input, coastingDrag is the speed at which velocity is lost
                localVelocity.z = Mathf.MoveTowards(localVelocity.z, 0f, coastingDrag * deltaTime);
            else if (acceleration > deadzone) //accelerating
                localVelocity.z = Mathf.MoveTowards(localVelocity.z, (maxSpeedStat + speedMod + nitroBoost) / (slowMod * gotShotMod), acceleration * ((nitroBoost/10f) + speedMod + accelerationStat * deltaTime));
            else if (localVelocity.z > deadzone) //negative acceleration and going forwards
                localVelocity.z = Mathf.MoveTowards(localVelocity.z, 0f, -acceleration * braking * deltaTime);
            else //negative acceleration and going backwards
                localVelocity.z = Mathf.MoveTowards(localVelocity.z, -reverseSpeed, -acceleration * reverseAcceleration * deltaTime);
        }

        currentSpeed = localVelocity.z;

        //if (!isHuman)
        //{
        //    if(localVelocity.z > maxSpeedStat)
        //    {
        //        GetComponent<MLADrive2>().AddReward(-.001f);
        //    }
        //}

        rbVelocity = unrotation * rotationStream * localVelocity;

        if(!isGrounded && controlsEnabled)
        {
            Vector3 fallVelocity = rbVelocity;

            //velocity in the ship's downward direction is added to over time
            fallVelocity += -transform.up * (gravity * gravityMod) * deltaTime;

            //fallVelocity is clamped so it cannot exceed maxSpeed
            fallVelocity.y = Mathf.Clamp(fallVelocity.y, -50f, 50f);
            fallVelocity.x = Mathf.Clamp(fallVelocity.x, -50f, 50f);
            fallVelocity.z = Mathf.Clamp(fallVelocity.z, -50f, 50f);

            rbVelocity = fallVelocity;
            //rbVelocity = unrotation * rotationStream * fallVelocity;

        }
    }

    void Jump(Quaternion rotationStream)
    {
        if (isGrounded && !isJumping)
        {
            isJumping = true;

            if (!isHuman)
            {
                GetComponent<MLADrive2>().AddReward(-0.01f);
            }

            //rbVelocity += rotationStream * Vector3.up * jumpSpeed;
            rbVelocity += rb.rotation * Vector3.up * jumpSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        //{
        //    rbVelocity.z = 0f;
        //}

    }
}
