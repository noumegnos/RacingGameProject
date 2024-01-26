using UnityEngine;

//stores information about the ground under the ship
public struct GroundInfo
{
    public bool isCapsuleTouching;
    public bool isGrounded;
    public bool isCloseToGround;
    public Vector3 normal;
}
