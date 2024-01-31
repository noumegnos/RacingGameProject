using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is merely a list of all checkpoints in the track
//when the level generator creates a track piece, it adds checkpoints from the track piece's list to this list

public class CheckpointManager : MonoBehaviour
{
    public List<Transform> listOfChecks = new List<Transform>();

    public Dictionary<int, Vector3> dictOfChecks = new Dictionary<int, Vector3>();
}
