using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GUIManager guiManager;                               //A link to the GUI Manager
    public MissionManager missionManager;                       //A link to the Mission Manager

    public List<MovingLayer> movingLayers;                      //Holds the moving layer
    public List<ScrollingLayer> scrollingLayers;                //Holds the scrolling layers

    public TorpedoLayer torpedoLayer;                           //A link to the torpedo layer
    public PowerupLayer powerupLayer;                           //A link to the powerup layer
    public ParticleLayer particleLayer;                         //A link to the particle player

    public GameObject hangar;									//Holds the hangar
    public GameObject playTriggerer;                            //Holds the play triggerer

    public float speedIncreaseRate;                             //The scrolling speed increase rate per second
    public float distance;                                      //The current distance

    private float hangarStartPosX;                              //The hangar starting position

    private float speedMultiplier;                              //Holds the speed multiplier
    private float lastSpeedMultiplier;                          //Holds the last speed multiplier

    private bool paused;                                        //True, if the game is paused
    private bool canModifySpeed;                                //True, if the generator can modify the current scrolling speed

    // Used for initialization
    void Start()
    {
        speedMultiplier = 1;
        paused = true;
    }
    // Update is called once per frame
    void Update()
    {
        //If the game is not paused
        if (!paused)
        {
            //If the speed can be modified
            if (canModifySpeed)
            {
                //Increase scrolling speed
                speedMultiplier += speedIncreaseRate * Time.deltaTime;

                powerupLayer.UpdateSpeedMultiplier(speedMultiplier);
                torpedoLayer.UpdateSpeedMultiplier(speedMultiplier);
            }

            //Increase distance
            distance += 10 * speedMultiplier * Time.deltaTime;

            //Pass speed multiplier to the layers
            foreach (MovingLayer item in movingLayers)
                item.UpdateSpeedMultiplier(speedMultiplier);

            foreach (ScrollingLayer item in scrollingLayers)
                item.UpdateSpeedMultiplier(speedMultiplier);

            particleLayer.UpdateSpeedMultiplier(speedMultiplier);

            //Update GUI and Mission Manager
            guiManager.UpdateDistance((int)distance);
            missionManager.DistanceEvent((int)distance);
        }
    }

    //Moves the hangar to the left, out of the view.
    IEnumerator MoveHangar(float posX, float time)
    {
        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / time;

        Vector3 startPos = hangar.transform.position;
        Vector3 endPos = new Vector3(posX, hangar.transform.position.y, 0);

        while (i < 1.0)
        {
            //If the game is not paused, increase t, and scale the object
            if (!paused)
            {
                i += Time.deltaTime * rate * speedMultiplier;
                hangar.transform.position = Vector3.Lerp(startPos, endPos, i);
            }

            //Wait for the end of frame
            yield return 0;
        }
    }
    //Changes the speed multiplier to newValue in time
    IEnumerator ChangeScrollingMultiplier(float newValue, float time, bool enableIncrease)
    {
        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / time;

        float startValue = speedMultiplier;

        while (i < 1.0)
        {
            //If the game is not paused, increase t, and scale the object
            if (!paused)
            {
                i += Time.deltaTime * rate;
                speedMultiplier = Mathf.Lerp(startValue, newValue, i);
            }

            //Wait for the end of frame
            yield return 0;
        }

        //If we stopped the generator because of a crash
        if (newValue == 0)
        {
            //Notify the mission manager
            missionManager.CrashEvent((int)distance);
        }
    }

	//Adds a coin particle to the level
    public void AddCoinParticle(Vector2 contactPoint)
    {
        particleLayer.AddCoinParticle(contactPoint);
	}
    //Adds an explosion particle to the level
    public void AddExplosionParticle(Vector2 contactPoint)
    {
        particleLayer.AddExplosion(contactPoint);
	}
    //Resume the generator after a revive
	public void ContinueGeneration()
    {
        torpedoLayer.EnableSpawning(true);
        powerupLayer.EnableSpawning(true);

        StartCoroutine(ChangeScrollingMultiplier(lastSpeedMultiplier, 0.5f, true));
	}
    //Resets the level generator
	public void Reset()
    {
        paused = true;
        canModifySpeed = false;

        speedMultiplier = 1;
        distance = 0;

        StopAllCoroutines();

        playTriggerer.SetActive(true);

        foreach (MovingLayer item in movingLayers)
            item.Reset();

        foreach (ScrollingLayer item in scrollingLayers)
            item.SetPauseState(true);

        torpedoLayer.EnableSpawning(true);
        powerupLayer.EnableSpawning(true);

        torpedoLayer.Reset();
        powerupLayer.Reset();
        particleLayer.Reset();

        hangar.transform.position = new Vector3(hangarStartPosX, hangar.transform.position.y, 0);
	}
    //Starts the level Generator
	public void StartToGenerate()
    {
        paused = false;
        canModifySpeed = true;

        playTriggerer.SetActive(false);

        foreach (MovingLayer item in movingLayers)
            item.StartGenerating();

        foreach (ScrollingLayer item in scrollingLayers)
            item.SetPauseState(false);

        torpedoLayer.StartGenerating();
        powerupLayer.StartGenerating();
        particleLayer.StartGenerating();

        hangarStartPosX = hangar.transform.position.x;
        StartCoroutine(MoveHangar(-30, 4.75f));
	}
    //Stops the level generaton under time
	public void StopGeneration(float time)
    {
        lastSpeedMultiplier = speedMultiplier;
        canModifySpeed = false;

        torpedoLayer.EnableSpawning(false);
        powerupLayer.EnableSpawning(false);

        StartCoroutine(ChangeScrollingMultiplier(0, time, false));
	}
    //Set the pause state of the level generator in time
    public void SetPauseState(bool state)
    {
        paused = state;

        foreach (MovingLayer item in movingLayers)
            item.SetPauseState(state);

        foreach (ScrollingLayer item in scrollingLayers)
            item.SetPauseState(state);

        torpedoLayer.SetPauseState(state);
        powerupLayer.SetPauseState(state);
        particleLayer.SetPauseState(state);
    }
    //Return the current distance as an int
    public int CurrentDistance()
    {
        return (int)distance;
    }
    //Starts the extra speed powerup effect
    public void StartExtraSpeed(float newSpeed)
    {
        lastSpeedMultiplier = speedMultiplier;
        canModifySpeed = false;

        speedMultiplier = newSpeed;

        powerupLayer.UpdateSpeedMultiplier(speedMultiplier);
        torpedoLayer.UpdateSpeedMultiplier(speedMultiplier);
    }
    //Stops the extra speed powerup effect
    public void EndExtraSpeed()
    {
        speedMultiplier = lastSpeedMultiplier;
        canModifySpeed = true;
    }
}