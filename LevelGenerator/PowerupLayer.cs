using UnityEngine;
using System.Collections;

public class PowerupLayer : MovingLayer 
{
    public float minDepth;                      //The minimum depth for the powerup to spawn
    public float maxDepth;                      //The maximum depth for the powerup to spawn

    private bool canSpawn = true;

    //Spawns a new element
    public override void SpawnElement(bool inMiddle)
    {
        if (!canSpawn)
            return;

        //Get a random item from the inactive elements
        Transform item = inactive[Random.Range(0, inactive.Count)];

        //Place it
        item.transform.position = new Vector3(startAt, Random.Range(maxDepth, minDepth), 0);

        item.GetComponent<Renderer>().enabled = true;
        item.GetComponent<Collider2D>().enabled = true;
        item.transform.FindChild("Trail").gameObject.SetActive(true);

        //Activate it, and add it to the active elements
        item.gameObject.SetActive(true);
        inactive.Remove(item);
        activeElements.Add(item);
    }

    //Set the value of the canSpawn variable
    public void EnableSpawning(bool newState)
    {
        canSpawn = newState;
    }
}
