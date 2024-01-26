using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowObject : MonoBehaviour
{
    //this script causes the attached object to smoothly follow its target
    //primary use is for making the player object's model and the camera follow the actual ship object in a smooth and pleasing manner
    //see also CameraFollow script

    public Transform target;

    public float lerpPosSmoothing;
    public float lerpRotSmoothing;

    public bool destroyIfNoTarget = false;

    private void FixedUpdate()
    {
        //make sure target still exists
        if(target != null)
        {

            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * lerpPosSmoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * lerpRotSmoothing);

        }
        else
        {
            //sometimes, it can be useful to delete this object if it has no target
            if (destroyIfNoTarget)
            {
                Destroy(this.gameObject);
            }
            else
            {
                //if no target can be found, it will set itself as target, which stops it from moving
                target = this.gameObject.transform;
            }
        }
    }
}
