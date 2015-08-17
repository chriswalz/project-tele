using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TorpedoLayer : MovingLayer
{
    public Transform indicatorContainer;                //The container, which holds the indicators

    public float minPosY;                               //The minimum position for the indicator
    public float maxPosY;                               //The maximum position for the indiator
    public float indicatorTime;                         //The indicator display time

    public int minTorpedoes;                            //The minimum ammount of torpedoes to spawn in the same time
    public int maxTorpedoes;                            //The maximum ammount of torpedoes to spawn in the same time

    private List<GameObject> activeIndicators;          //Stores the active indicators
    private List<GameObject> inactiveIndicators;        //Stores the inactive indicators

    private bool canSpawn;

    // Use this for initialization
    public override void Start()
    {
        canSpawn = true;

        activeIndicators = new List<GameObject>();
        inactiveIndicators = new List<GameObject>();

        foreach (Transform child in indicatorContainer)
            inactiveIndicators.Add(child.gameObject);

        base.Start();
    }
    
    //Spawns new torpedo elements
    public override void SpawnElement(bool inMiddle)
    {
        if (!canSpawn)
            return;

        //Calculate the number
        int toSpawn = Random.Range(minTorpedoes, maxTorpedoes + 1);

        for (int i = 0; i < toSpawn; i++)
        {
            //Get the first inactive indicator, and add it to the activate ones
            GameObject indicator = inactiveIndicators[0];
            inactiveIndicators.Remove(indicator);
            activeIndicators.Add(indicator);

            //Place it in a random Y position
            float yPos = Random.Range(minPosY, maxPosY);
            indicator.transform.position = new Vector3(indicator.transform.position.x, yPos, 0);

            //Activate it
            indicator.SetActive(true);
            StartCoroutine(ShowIndicator(indicator, yPos));
        }
    }
    //Resets the active indicators
    public override void Reset()
    {
        StopAllCoroutines();

        while (activeIndicators.Count > 0)
        {
            activeIndicators[0].SetActive(false);
            inactiveIndicators.Add(activeIndicators[0]);

            activeIndicators.RemoveAt(0);
        }

        base.Reset();
    }
    //Shows the torpedo indicator for a set ammount of time
    IEnumerator ShowIndicator(GameObject indicator, float yPos)
    {
        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / indicatorTime;

        while (i < 1.0)
        {
            //If the game is not paused, increase t, and scale the object
            if (!paused)
            {
                i += Time.deltaTime * rate;
            }

            //Wait for the end of frame
            yield return 0;
        }

        //Disable the indicator
        indicator.SetActive(false);    
        inactiveIndicators.Add(indicator);
        activeIndicators.Remove(indicator);

        //Spawn a torpedo on the Y position of the indicator
        SpawnTorpedo(yPos);
    }

    //Set the value of the canSpawn variable
    public void EnableSpawning(bool newState)
    {
        canSpawn = newState;
    }

    //Spawns a torpedo
    private void SpawnTorpedo(float yPos)
    {
        //Get the first item from the inactive elements
        Transform torpedo = inactive[0];

        //Place the torpedo on the proper y position
        torpedo.position = new Vector3(startAt, yPos, 0);

        torpedo.GetComponent<Renderer>().enabled = true;
        torpedo.GetComponent<Collider2D>().enabled = true;
        torpedo.transform.FindChild("TorpedoFire").gameObject.SetActive(true);

        //Activate it, and add it to the active elements
        inactive.Remove(torpedo);
        activeElements.Add(torpedo);
        torpedo.gameObject.SetActive(true);
    }
}
