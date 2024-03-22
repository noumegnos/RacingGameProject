using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissileBehaviour : MonoBehaviour
{

    public GameObject target;

    public GameObject owner;

    public float speed;

    public Transform explosion;

    public float timer;
    float defTimer;

    bool didBoom = false;

    public float radius;

    public float damage;
    public float hitSlow;


    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Collider>().enabled = false;

        defTimer = timer;

        //print("missile has target: " + target);
    }

    // Update is called once per frame
    void Update()
    {
        //if(target != null)
        //{
        //    print("here i am! " + target.name);
        //}
        timer -= Time.deltaTime;

        //if (timer <= defTimer - 0.1f)
        //{
        //    GetComponent<Collider>().enabled = true;
        //}

        if (timer < 0f && !didBoom)
        {
            MissileExplode();

            didBoom = true;
        }

        if(target != null)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;

            //transform.position += dir * speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed);

            transform.rotation = Quaternion.LookRotation(dir, transform.up);


            if (!didBoom )
            {
                if (Vector3.Distance(transform.position, target.transform.position) < 5f)
                {
                    MissileExplode();

                    didBoom = true;
                }
            }

        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, speed);

//            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    public void MissileExplode()
    {
        Transform boom = Instantiate(explosion, transform.position, transform.rotation);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider col in hitColliders)
        {
            if(col.GetComponent<DamageManager>() != null)
            {
                if (col.GetComponent<DamageManager>().TakeDamage(damage))
                {
                    if(owner.GetComponentInChildren<CharacterSheet>() != null)
                    {
                        owner.GetComponentInChildren<CharacterSheet>().money += 100f;
                    }
                }
            }

            if (col.GetComponent<ShipController>() != null)
            {
                col.GetComponent<ShipController>().gotShotMod = hitSlow;
            }
        }

        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!didBoom)
        {
            MissileExplode();
        }
    }
}
