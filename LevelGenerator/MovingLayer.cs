using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingLayer : MonoBehaviour 
{
    public Transform container;                 //Contains the layer elements

    public float startingSpeed;                 //The starting speed of the layer

    public float startAt;                       //The spawned layer elements starts on this X coordinate
    public float spawningRate;                  //Spawn a new layer element at this rate
    public float resetAt;                       //Reset a layer element, when it reaches this global X coordinates

    public float delayBeforeFirst;              //How much delay should be applied before the spawn of the first element

    public bool childCheck;                     //Enable it, if the child elements of a layer can be disabled by outside event (like when a player collides with an obstacle)
    public bool generateInMiddle;               //Enable it, if you want an element to be generated in zero X pos at the beginning of the level

    protected List<Transform> activeElements;   //Contains the active layer elements, which has not reached the spawnAt position
    protected List<Transform> inactive;         //Contains the inactive layer elements

    protected float speedMultiplier;            //The current speed multiplier

    protected bool paused;                      //True, if the level is paused
    protected bool removeLast;                  //True, if the first element has passed the reset position

	// Use this for initialization
	public virtual void Start () 
    {
        speedMultiplier = 1;
        paused = true;

        activeElements = new List<Transform>();
        inactive = new List<Transform>();

        foreach (Transform child in container)
            inactive.Add(child);

        if (generateInMiddle)
            SpawnElement(true);
	}
	// Update is called once per frame
	void Update () 
    {
        if (!paused)
        {
            //Loop through the active elemets
            foreach (Transform item in activeElements)
            {
                //Move the item to the left
                item.transform.position -= Vector3.right * startingSpeed * speedMultiplier * Time.deltaTime;

                //If the item has reached the reset position
                if (item.transform.position.x < resetAt)
                    removeLast = true;
            }

            //Remove the first element, if needed
            if (removeLast)
            {
                removeLast = false;
                RemoveElement(activeElements[0]);
            }
        }
	}

    //Removes the first element
    private void RemoveElement(Transform item)
    {
        //If this is a special layer, like obstacles, check and activate every child
        if (childCheck)
            EnableChild(item);

        //Remove it from the active elements, add it to the inactive, and disable it
        activeElements.Remove(item);
        inactive.Add(item);

        item.gameObject.SetActive(false);

        //Reset it's position
        item.transform.position = new Vector3(startAt, item.transform.position.y, 0);
    }
    //Loops trough the children of a layer element, and enables them
    private void EnableChild(Transform element)
    {
        //Loop trough the childrens, and activate their renderer and collider
        foreach (Transform item in element)
        {
            item.GetComponent<Renderer>().enabled = true;
            item.GetComponent<Collider2D>().enabled = true;
        }
    }
    //Spawns a new layer element with a delay
    private IEnumerator SpawnDelayedElement(float time)
    {
        //Declare starting variables
        float i = 0.0f;
        float rate = 1.0f / time;

        //Wait for time
        while (i < 1.0)
        {
            if (!paused)
                i += Time.deltaTime * rate;

            yield return 0;
        }

        //Spawn the element
        StartCoroutine("Generator");
    }
    //Spawn new layer elements at the given rate
    private IEnumerator Generator()
    {
        //Spawn a new element
        SpawnElement(false);

        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / spawningRate;

        while (i < 1.0)
        {
            //If the game is not paused, increase t, and scale the object
            if (!paused)
            {
                i += (Time.deltaTime * rate) * speedMultiplier;
            }

            //Wait for the end of frame
            yield return 0;
        }

        //Restart the generator
        StartCoroutine("Generator");
    }

    //Starts the generation of this layer
    public virtual void StartGenerating()
    {
        //Unpause the generator, and spawn the first element
        paused = false;
        StartCoroutine(SpawnDelayedElement(delayBeforeFirst));
    }
    //Spawns a new layer element
    public virtual void SpawnElement(bool inMiddle)
    {
        //Get a random item from the inactive elements
        Transform item = inactive[Random.Range(0, inactive.Count)];

        //If the item is needed at zero X position
        if (inMiddle)
        {
            //Place it
            item.transform.position = new Vector3(0, item.transform.position.y, 0);
        }

        //Activate it, and add it to the active elements
        item.gameObject.SetActive(true);
        inactive.Remove(item);
        activeElements.Add(item);
    }
    //Sets the pause state
    public virtual void SetPauseState(bool newState)
    {
        paused = newState;
    }
    //Removes every item from the level
    public virtual void Reset()
    {
        paused = true;

        StopAllCoroutines();

        foreach (Transform item in activeElements)
        {
            item.transform.position = new Vector3(startAt, item.transform.position.y, 0);

            if (childCheck)
                EnableChild(item);

            item.gameObject.SetActive(false);
            inactive.Add(item);
        }

        activeElements.Clear();

        if (generateInMiddle)
            SpawnElement(true);
    }
  
    //Updates the speed multiplier
    public void UpdateSpeedMultiplier(float n)
    {
        speedMultiplier = n;
    }
}
