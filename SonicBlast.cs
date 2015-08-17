using UnityEngine;
using System.Collections;

public class SonicBlast : MonoBehaviour 
{
    public LevelGenerator levelGenerator;           //A link to the level generator
    public GUIManager guiManager;                   //A link to the GUI Manager

    public Vector2 startPosition;                   //The starting position of the sonic blast
    public Vector2 endPosition;                     //The end position of the sonic blast

    public float time;                              //The traver time of the sonic wave

    private Transform lastObstacle;                 //The last obstace the blast hit
    private bool paused;                            //True, if the level is paused

    //Used for initialisation
    void Start()
    {
        paused = false;
    }
    //Called when the wave collides with a coin or with an obstacle
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Obstacle")
        {
            levelGenerator.AddExplosionParticle(other.transform.position);
            other.GetComponent<Renderer>().enabled = false;
            other.GetComponent<Collider2D>().enabled = false;

            if (other.name != "Torpedo")
                lastObstacle = other.transform;
            else
                other.transform.FindChild("TorpedoFire").gameObject.SetActive(false);
        }
    }

    //Activate the sonic wave
    public void Activate()
    {
        lastObstacle = null;
        StartCoroutine("Move");
    }
    //Resets the sonic wave
    public void Reset()
    {
        StopCoroutine("Move");
        this.transform.position = startPosition;
        this.gameObject.SetActive(false);
    }
    //Set the pause state of the level generator in time
    public void SetPauseState(bool state)
    {
        paused = state;
    }

    //Moves the sonic wave from the starting position to the end position in time
    private IEnumerator Move()
    {
        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / time;

        while (i < 1.0)
        {
            //If the game is not paused, increase t
            if (!paused)
            {
                i += Time.deltaTime * rate;
                this.transform.position = Vector3.Lerp(startPosition, endPosition, i);
            }

            //Wait for the end of frame
            yield return 0;
        }

        Disable();
    }

    //Disable the obstacles of the last obstacle group with the wave collided
    private void Disable()
    {
        if (lastObstacle)
        {
            Transform obstacleParent = lastObstacle.parent;

            foreach (Transform item in obstacleParent)
            {
                if (item.tag == "Obstacle")
                {
                    item.GetComponent<Renderer>().enabled = false;
                    item.GetComponent<Collider2D>().enabled = false;
                }
            }
        }

        guiManager.ShowAvailablePowerups();
        Reset();
    }
}
