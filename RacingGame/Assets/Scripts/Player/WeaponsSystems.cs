using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class WeaponsSystems : MonoBehaviour
{
    public bool laser;
    public bool machineGun;
    public bool missiles;

    public ShipController ship;

    public Transform shotObject;
    public Transform flashObject;
    public Transform hitObject;

    public Transform missilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(GameObject target)
    {
        if (ship.ammo - ship.ammoConsumption > 0f)
        {
            ship.ammo -= ship.ammoConsumption;

            if (missiles)
            {
                FireMissile(target);

                //print("firing at " + target);
            }

            if(ship.GetComponent<ShipSounds>()  != null)
            {
                if (laser)
                {
                    ship.GetComponent<ShipSounds>().PlayLaserSound();
                }

                if(machineGun)
                {
                    ship.GetComponent<ShipSounds>().PlayGunSound();
                }

                if (missiles)
                {
                    ship.GetComponent<ShipSounds>().PlayMissileSound();
                }
            }



            Vector3 dir = transform.forward;

            if(target != null)
            {
                dir = (target.transform.position - transform.position).normalized;

            }

            if(machineGun)
            {
                float accuracy = GetComponent<WeaponStats>().weaponAccuracy;

                dir = new Vector3(dir.x + Random.Range(-accuracy, accuracy), dir.y + Random.Range(-accuracy, accuracy), dir.z + Random.Range(-accuracy, accuracy));
            }

            RaycastHit hit;

            Ray ray = new Ray(transform.position, dir);

            if (Physics.Raycast(ray, out hit, ship.shootRange, ship.allCollidingLayers))
            {
                //Debug.DrawRay(origin, direction * hit.distance, Color.yellow, 0.3f);

                if(laser || machineGun)
                {
                    DoShot(transform.position, dir, hit.distance);

                    DoShotHit(hit.point);
                }


                if (hit.collider.GetComponent<DamageManager>() != null)
                {
                    if(laser ||machineGun)
                    {
                        if (hit.collider.GetComponent<DamageManager>().TakeDamage(ship.weaponDamage))
                        {
                            //killed the hit thing
                            if(ship.GetComponentInChildren<CharacterSheet>() != null)
                            {
                                ship.GetComponentInChildren<CharacterSheet>().money += 100f;
                            }

                            if (!ship.isHuman)
                            {
                                ship.GetComponent<MLADrive2>().AddReward(0.05f);
                            }
                        }

                    }

                    if (hit.collider.gameObject.GetComponent<ShipController>() != null)
                    {
                        if(laser || machineGun)
                        {
                            hit.collider.gameObject.GetComponent<ShipController>().gotShotMod = ship.weaponSlowMod;

                        }

                        if (!ship.isHuman)
                        {
                            ship.GetComponent<MLADrive2>().AddReward(0.002f);


                        }
                    }
                    else
                    {
                        if (!ship.isHuman)
                        {
                            ship.GetComponent<MLADrive2>().AddReward(0.001f);


                        }
                    }


                }
                else
                {
                    //shot something indestructible
                    if (!ship.isHuman)
                    {
                        ship.GetComponent<MLADrive2>().AddReward(-0.01f);
                    }
                }
            }
            else
            {
                //shot missed

                //Debug.DrawRay(transform.position, direction * ship.shootRange, Color.yellow, 0.3f);

                if(laser || machineGun)
                {
                    DoShot(transform.position, transform.forward, ship.shootRange);

                }

                if (!ship.isHuman)
                {
                    ship.GetComponent<MLADrive2>().AddReward(-0.01f);
                }
            }
        }
        else
        {
            //no ammo
            if (!ship.isHuman)
            {
                ship.GetComponent<MLADrive2>().AddReward(-0.01f);
            }
        }
    }

    public void Fire2(GameObject target)
    {
        if (ship.ammo - ship.ammoConsumption > 0f)
        {
            ship.ammo -= ship.ammoConsumption;

            if (missiles)
            {
                FireMissile(target);

                //print("firing at " + target);
            }


            Vector3 dir = transform.forward;

            if (target != null)
            {
                dir = (target.transform.position - transform.position).normalized;

            }

            RaycastHit[] hits;

            hits = Physics.RaycastAll(transform.position, dir, ship.shootRange, ship.allCollidingLayers);

            Transform closestHit;
            float closestDist = 99f;

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < closestDist)
                {
                    closestHit = hits[i].transform;
                    closestDist = hits[i].distance;
                }
            }

            RaycastHit hit;

            Ray ray = new Ray(transform.position, dir);

            if (Physics.Raycast(ray, out hit, ship.shootRange, ship.allCollidingLayers))
            {
                //Debug.DrawRay(origin, direction * hit.distance, Color.yellow, 0.3f);

                if (laser || machineGun)
                {
                    DoShot(transform.position, dir, hit.distance);

                    DoShotHit(hit.point);
                }


                if (hit.collider.GetComponent<DamageManager>() != null)
                {
                    if (laser || machineGun)
                    {
                        hit.collider.GetComponent<DamageManager>().TakeDamage(ship.weaponDamage);

                    }

                    if (hit.collider.gameObject.GetComponent<ShipController>() != null)
                    {
                        if (laser || machineGun)
                        {
                            hit.collider.gameObject.GetComponent<ShipController>().gotShotMod = ship.weaponSlowMod;

                        }

                        if (!ship.isHuman)
                        {
                            ship.GetComponent<MLADrive2>().AddReward(0.002f);


                        }
                    }
                    else
                    {
                        if (!ship.isHuman)
                        {
                            ship.GetComponent<MLADrive2>().AddReward(0.001f);


                        }
                    }


                }
                else
                {
                    //shot something indestructible
                    if (!ship.isHuman)
                    {
                        ship.GetComponent<MLADrive2>().AddReward(-0.01f);
                    }
                }
            }
            else
            {
                //shot missed

                //Debug.DrawRay(transform.position, direction * ship.shootRange, Color.yellow, 0.3f);

                if (laser || machineGun)
                {
                    DoShot(transform.position, transform.forward, ship.shootRange);

                }

                if (!ship.isHuman)
                {
                    ship.GetComponent<MLADrive2>().AddReward(-0.01f);
                }
            }
        }
        else
        {
            //no ammo
            if (!ship.isHuman)
            {
                ship.GetComponent<MLADrive2>().AddReward(-0.01f);
            }
        }
    }

    public void FireMissile(GameObject target)
    {
        Transform missile = Instantiate(missilePrefab, transform.position, transform.rotation);

        //if(target != null)
        //{
        //    print("here!");
        //}

        missile.GetComponent<MissileBehaviour>().target = target;
        missile.GetComponent<MissileBehaviour>().owner = ship.gameObject;
        missile.GetComponent<MissileBehaviour>().damage = ship.weaponDamage;
        missile.GetComponent<MissileBehaviour>().hitSlow = ship.weaponSlowMod;
    }

    public void DoShot(Vector3 from, Vector3 to, float distance)
    {
        Quaternion rotation = Quaternion.LookRotation(to, transform.up);

        //Vector3 roter = transform.rotation * to;

        Transform traced = Instantiate(shotObject, from, transform.rotation);
        //traced.parent = transform;
        //traced.GetComponent<ShotObjectSetup>().Setup(Vector3.Distance(from, to * distance), rotation);
        traced.GetComponent<ShotObjectSetup>().Setup(distance, rotation);

        Vector3 flashRotation = new Vector3(0, 0, Random.Range(-180f, 180f));

        //Quaternion rot = transform.eulerAngles(flashRotation);

        Transform flash = Instantiate(flashObject, from, transform.rotation);

        flash.parent = transform;

        flash.eulerAngles = flashRotation;

    }

    public void DoShotHit(Vector3 pos)
    {
        Transform hit = Instantiate(hitObject, pos, transform.rotation);

        Vector3 hitRotation = new Vector3(0, 0, Random.Range(-180f, 180f));

        hit.eulerAngles = hitRotation;

    }
}
