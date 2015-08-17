using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleLayer : MonoBehaviour 
{
    public Transform container;                 //Contains the layer elements

    public GameObject coinPrefab;               //The coin particle prefab, used to instantiate more if needed
    public GameObject explosionPrefab;          //The explosion particle prefab, used to instantiate more if needed

    public float startingSpeed;                 //The starting speed of the layer

    private List<Transform> activeElements;     //Contains the active layer elements, which has not reached the spawnAt position
    private List<Transform> inactive;           //Contains the inactive layer elements

    private float speedMultiplier;              //The current speed multiplier

    private bool paused;                        //True, if the level is paused

    // Use this for initialization
    void Start()
    {
        speedMultiplier = 1;
        paused = true;

        activeElements = new List<Transform>();
        inactive = new List<Transform>();

        foreach (Transform child in container)
            inactive.Add(child);
    }
    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            //Loop through the active elemets
            foreach (Transform item in activeElements)
            {
                //Move the item to the left
                item.transform.position -= Vector3.right * startingSpeed * speedMultiplier * Time.deltaTime;
            }
        }
    }

    //Adds the given element to the layer
    private void AddElement(Transform item, Vector2 pos)
    {
        item.transform.position = pos;
        item.gameObject.SetActive(true);

        inactive.Remove(item);
        activeElements.Add(item);

        StartCoroutine(PlayParticle(item, 2f));
    }
    //Removes the given element
    private void RemoveElement(Transform item)
    {
        //Reset it's position, disable it, and remove it from the active elements
        item.transform.position = new Vector3(-12, item.transform.position.y, 0);

        //Remove it from the active elements, add it to the inactive, and disable it
        activeElements.Remove(item);
        inactive.Add(item);

        item.gameObject.SetActive(false);
    }
    //Generates a new explosion particle
    private Transform SpawnNewParticle(GameObject prefab)
    {
        //Create a new object from the given prefab, and name it
        GameObject newGo = (GameObject)Instantiate(prefab);
        newGo.name = prefab.name;

        //Put it to the system
        newGo.transform.parent = this.transform;
        inactive.Add(newGo.transform);

        return newGo.transform;
    }

    //Enables the generator
    public void StartGenerating()
    {
        //Unpause the layer
        paused = false;
    }
    //Adds an explosion particle to the given position
    public void AddExplosion(Vector2 position)
    {
        //Find an unused explosion particle
        Transform item = inactive.Find(x => x.name == "ExplosionParticle");

        //If there is none, create a new
        if (item == null)
            item = SpawnNewParticle(explosionPrefab);

        //Add it to the level
        AddElement(item, position);      
    }
    //Adds a coin particle to the given position
    public void AddCoinParticle(Vector2 position)
    {
        //Find an unused coin particle
        Transform item = inactive.Find(x => x.name == "CoinParticle");

        //If there is none, create a new
        if (item == null)
            item = SpawnNewParticle(coinPrefab);

        //Add it to the level
        AddElement(item, position); 
    }
    //Resets the layer
    public void Reset()
    {
        paused = true;

        StopAllCoroutines();

        foreach (Transform item in activeElements)
        {
            item.transform.position = new Vector3(-12, item.transform.position.y, 0);
            item.gameObject.SetActive(false);

            inactive.Add(item);
        }

        activeElements.Clear();
    }
    //Sets scrolling state
    public void SetPauseState(bool state)
    {
        paused = state;
    }
    //Updates speed multiplier
    public void UpdateSpeedMultiplier(float n)
    {
        speedMultiplier = n;
    }

    //Plays the particle then remove it
    IEnumerator PlayParticle(Transform particle, float time)
    {
        ParticleSystem p = particle.GetComponent("ParticleSystem") as ParticleSystem;
        p.Play();

        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / time;

        while (i < 1.0)
        {
            //If the game is not paused, increase t, and scale the object
            if (!paused)
                i += Time.deltaTime * rate;

            //Wait for the end of frame
            yield return 0;
        }

        p.Stop();
        p.Clear();

        RemoveElement(particle);
    }   
}
