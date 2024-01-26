using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script spawns a traffic car every few seconds
//NOTE: the timer has the index number of the car in the list added to the timer value
//this is because I attempt to add larger cars further down the list, and add 1 second to timer for each
//in theory this should give enough space for each larger car
//needs some massaging to get the values right

public class TrafficSpawner : MonoBehaviour
{
    public List<GameObject> carsToSpawn = new List<GameObject>();
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCars());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SpawnCars()
    {
        int choice = Random.Range(0, carsToSpawn.Count);

        GameObject car = Instantiate(carsToSpawn[choice], transform.position, transform.rotation);

        car.transform.SetParent(transform);

        yield return new WaitForSeconds(timer + (choice * 4) + Random.Range(0f,2f));

        StartCoroutine(SpawnCars());
    }
}
