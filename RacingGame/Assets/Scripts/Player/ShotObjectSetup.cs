using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotObjectSetup : MonoBehaviour
{
    public float timeout;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, timeout);
    }

    public void Setup(float length, Quaternion rotation)
    {
        //length, rotation

        Vector3 scales = new Vector3(1, 1, length);

        transform.rotation = rotation;

        //transform.eulerAngles = rotation;

        //rotation.eulerAngles = 

        transform.localScale = scales;
    }
}
