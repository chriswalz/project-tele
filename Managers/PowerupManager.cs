using UnityEngine;
using System.Collections;

public class PowerupManager : MonoBehaviour 
{
    public LevelGenerator levelGenerator;                       //A link to the Level Generator
    public GUIManager guiManager;                               //A link to the GUI Manager
    public PlayerManager playerManager;                         //A link to the Player Manager
    public SonicBlast sonicBlast;                               //A link to the Sonic Blast

    public float extraSpeedFactor;                              //The scrolling speed during the Extra Speed powerup
    public float extraSpeedLength;                              //The duration of the extra speed powerup

    private bool powerupUsed;                                   //True, if the player used a powerup in this run
    private bool paused;                                        //True, if the level is paused

    // Used for initialization
    void Start()
    {
        powerupUsed = false;
        paused = false;
    }

    //Changes pause state to state
    public void SetPauseState(bool state)
    {
        paused = state;
        sonicBlast.SetPauseState(state);
    }
    //Return true, if a powerup can be used
    public bool CanUsePowerup()
    {
        return playerManager.CanUsePowerup();
    }
    //Returns true, if a powerup was used
    public bool PowerupUsed()
    {
        return powerupUsed;
    }
    //Resets the variables
    public void Reset()
    {
        powerupUsed = false;
        paused = false;
    }

    //Activate the extra speed powerup
    public void ExtraSpeed()
    {
        powerupUsed = true;
        StartCoroutine("ExtraSpeedEffect");
    }
    //Activate the extra speed powerup
    public void Shield()
    {
        powerupUsed = true;
        playerManager.RaiseShield();
    }
    //Activate the extra speed powerup
    public void SonicBlast()
    {
        powerupUsed = true;
        sonicBlast.gameObject.SetActive(true);
        sonicBlast.Activate();
    }
    //Revives the player, and launches a Sonic Blast
    public void Revive()
    {
        powerupUsed = true;

        sonicBlast.gameObject.SetActive(true);
        sonicBlast.Activate();
    }

    //Activates the extra speed for the given duration, then disables it
    private IEnumerator ExtraSpeedEffect()
    {
        levelGenerator.StartExtraSpeed(extraSpeedFactor);
        playerManager.ActivateExtraSpeed();

        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / extraSpeedLength;

        while (i < 1.0)
        {
            //If the game is not paused, increase t
            if (!paused)
                i += Time.deltaTime * rate;

            //Wait for the end of frame
            yield return 0;
        }

        levelGenerator.EndExtraSpeed();
        playerManager.DisableExtraSpeed();
        guiManager.ShowAvailablePowerups();
    }
}
