using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float lerpPosSmoothing;
    public float lerpRotSmoothing;

    public bool followPosition;

    public bool followRotation;
    //Vector3 currentEulerAngles;
    Quaternion desiredRotation;
    public bool followEulerAnglesX;
    public bool followEulerAnglesY;
    public bool followEulerAnglesZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (followPosition)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * lerpPosSmoothing);
        }

        if(followRotation)
        {
            //if(followEulerAnglesX)
            //{
            //    transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(target.transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime * lerpRotSmoothing);

            //}
            //else if(followEulerAnglesY)
            //{
            //    transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, target.transform.eulerAngles.y, transform.eulerAngles.z), Time.deltaTime * lerpRotSmoothing);

            //}
            //else if (followEulerAnglesZ)
            //{
            //    transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, target.transform.eulerAngles.z), Time.deltaTime * lerpRotSmoothing);

            //}

            //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, 0f, target.transform.eulerAngles.z), Time.deltaTime * lerpRotSmoothing);



            //transform.Rotate(0, 0, target.transform.eulerAngles.z);

            //desiredRotation = Quaternion.Euler(0f, 0f, target.transform.eulerAngles.z);



            desiredRotation = Quaternion.Euler(0f, 0f, target.transform.eulerAngles.z);

            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * lerpRotSmoothing);

            //transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * lerpRotSmoothing);


            //currentEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, target.transform.eulerAngles.z) * Time.deltaTime * lerpRotSmoothing;

            //transform.eulerAngles = currentEulerAngles;
        }
    }
}

