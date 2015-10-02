using UnityEngine;
using System.Collections;

enum PlayerState { Enabled, Disabled };
enum PlayerStatus { Moving, Sinking, Crashed };
enum PlayerVulnerability { Enabled, Disabled, Shielded };
enum PowerupUsage { Enabled, Disabled };

public class PlayerManager : MonoBehaviour
{
    public LevelManager levelManager;                                   //A link to the Level Manager
    public GUIManager guiManager;                                       //A link to the GUI Manager

    public Sprite[] subTextures;										//The array containing the sub and sub damaged textures
    public SpriteRenderer subRenderer;									//A link to the sub material

    public Transform extraSpeedFront;                                   //The extra speed front sprite
    public Transform extraSpeedTail;                                    //The extra speed trail sprite
    public Transform shield;                                            //The shield sprite

    public float minDepth;                                              //Minimum depth
    public float maxDepth;						                        //Maximum depth
    public float maxRotation;						                    //The maximum rotation of the submarine

    public float maxVerticalSpeed;					                    //The maximum vertical speed
    public float depthEdge;					                            //The edge fo the smoothing zone (minDepth- depthEdge and maxDepth - depthEdge)

    public ParticleSystem smoke;										//The smoke particle
    public ParticleSystem bubbles;										//The submarine bubble particle system
    public ParticleSystem reviveParticle;								//The revive particle

    public PolygonCollider2D normalCollider;                            //The normal collider of the submarine
    public CircleCollider2D shieldCollider;                             //The collider of the shield

    private PlayerStatus playerStatus;
    private PlayerState playerState;
    private PlayerVulnerability playerVulnerability;
    private PowerupUsage powerupUsage;

	private int currentSkinID;

    private float speed;                                                //The actual vertical speed of the submarine
    private float newSpeed;						                        //The new speed of the submarine, used at the edges

    private float rotationDiv;										    //A variable used to calculate rotation
    private Vector3 newRotation;	                                    //Stores the new rotation angles

    private float distanceToMax;                                        //The current distance to the maximum depth
    private float distanceToMin;                                        //The current distance to the minimum depth

    private Vector2 playerCoords;                                        //Holds the starting position of the submarine

    //Used for initialization
    void Start()
    {
        newRotation = new Vector3();
        playerCoords = this.transform.position; // starting position

        playerStatus = PlayerStatus.Moving;
        playerState = PlayerState.Disabled;
        playerVulnerability = PlayerVulnerability.Disabled;
        powerupUsage = PowerupUsage.Disabled;

        rotationDiv = maxVerticalSpeed / maxRotation;

		currentSkinID = SaveManager.currentSkinID;
		subRenderer.sprite = subTextures[currentSkinID * 2 + 1];
    }
    //Called at every frame
    void Update()
    {
        if (playerState == PlayerState.Enabled)
        {
            if (playerStatus == PlayerStatus.Moving)
            {
                //Calculate smooth zone distance
                CalculateDistances();

                //Move and rotate the submarine
                MoveAndRotate();
            }
            else if (playerStatus == PlayerStatus.Sinking)
            {
                Sink();
            }
        }
    }
    //Called when the player enters a triggerer zone
    void OnTriggerEnter2D(Collider2D other)
    {
        //If the submarine is collided with a coin
        if (other.tag == "Coin")
        {
            //Notify the level manager, and disable the coin's renderer and collider
            levelManager.CoinCollected(other.transform.position);
            other.GetComponent<Renderer>().enabled = false;
            other.GetComponent<Collider2D>().enabled = false;

			AudioManager.Instance.PlayCoinCollected();
        }
        //If the submarine is collided with an obstacle
        else if (other.tag == "Obstacle")
        {
            //Notify the level manager, and disable the obstacle's renderer and collider
            levelManager.Collision(other.name, other.transform.position);
            other.GetComponent<Renderer>().enabled = false;
            other.GetComponent<Collider2D>().enabled = false;

			AudioManager.Instance.PlayExplosion();

            //If the obstacle is a torpedo, disable it's child as well
            if (other.name == "Torpedo")
                other.transform.FindChild("TorpedoFire").gameObject.SetActive(false);

            //If the player is vulnerable
            if (playerVulnerability == PlayerVulnerability.Enabled)
            {
                //Sink the submarine
                powerupUsage = PowerupUsage.Disabled;
                playerVulnerability = PlayerVulnerability.Disabled;
                playerStatus = PlayerStatus.Sinking;

				subRenderer.sprite = subTextures[currentSkinID * 2];
                bubbles.Stop();
            }
            //If the player is shielded, collapse it
            else if (playerVulnerability == PlayerVulnerability.Shielded)
            {
                CollapseShield();
                guiManager.ShowAvailablePowerups();
            }
        }
        //If the submarine is collided with a powerup
        else if (other.tag == "Powerup")
        {
            //Notify the level manager, and disable the powerup's components
            other.GetComponent<Renderer>().enabled = false;
            other.GetComponent<Collider2D>().enabled = false;
            other.transform.FindChild("Trail").gameObject.SetActive(false);

			AudioManager.Instance.PlayPowerupCollected();
            levelManager.PowerupPickup(other.name);
        }
    }

    //Enables the submarine
	public void EnableSubmarine()
    {
        playerStatus = PlayerStatus.Moving;
        playerState = PlayerState.Enabled;
        playerVulnerability = PlayerVulnerability.Enabled;
        powerupUsage = PowerupUsage.Enabled;

		subRenderer.sprite = subTextures[currentSkinID * 2 + 1];
        bubbles.Play();

        StartCoroutine(FunctionLibrary.MoveElementBy(this.transform, new Vector2(0.4f, 0.2f), 0.5f));
	}
    //Sets the pause state of the submarine
    public void SetPauseState(bool pauseState)
    {
        if (pauseState)
        {
            playerState = PlayerState.Disabled;
            bubbles.Pause();
        }
        else
        {
            playerState = PlayerState.Enabled;
            bubbles.Play();
        }
    }
	//Changes the active skin ID
	public void ChangeSkin(int id)
	{
		currentSkinID = id;

		if (playerStatus != PlayerStatus.Crashed)
			subRenderer.sprite = subTextures[currentSkinID * 2 + 1];
	}
    //Resets the submarine
	public void Reset()
    {
        playerStatus = PlayerStatus.Moving;
        playerState = PlayerState.Disabled;
        playerVulnerability = PlayerVulnerability.Disabled;
        powerupUsage = PowerupUsage.Disabled;

        newRotation = new Vector3(0, 0, 0);

		subRenderer.sprite = subTextures[currentSkinID * 2 + 1];

        bubbles.Stop();
        bubbles.Clear();

        this.transform.position = playerCoords;
        this.transform.eulerAngles = newRotation;
	}
    //Revives the submarine
	public void Revive()
    {
        StartCoroutine("ReviveProcess");
	}
    //Updates player input
	public void UpdateInput(bool inputActive)
    {
        if (playerStatus == PlayerStatus.Sinking || playerStatus == PlayerStatus.Crashed)
            return;

        if (playerStatus == PlayerStatus.Moving || inputActive) {
			playerStatus = PlayerStatus.Moving;
		} else {
			playerStatus = PlayerStatus.Sinking;
		}
	}
	//Updates player input
	public void UpdateCoords(float mouseX, float mouseY)
	{
		if (playerStatus == PlayerStatus.Sinking || playerStatus == PlayerStatus.Crashed)
			return;
		if (playerStatus == PlayerStatus.Moving) {
			playerCoords.x = mouseX;
			playerCoords.y = mouseY;
		}
	}
    //Disables the crash smoke particle
    public void DisableSmoke()
    {
        smoke.enableEmission = false;
    }

    //Activates the extra speed submarine effects
    public void ActivateExtraSpeed()
    {
        extraSpeedFront.gameObject.SetActive(true);
        extraSpeedTail.gameObject.SetActive(true);
        RaiseShield();

        playerVulnerability = PlayerVulnerability.Disabled;
        powerupUsage = PowerupUsage.Disabled;
    }
    //Deactivates the extra speed submarine effects
    public void DisableExtraSpeed()
    {
        extraSpeedFront.gameObject.SetActive(false);
        extraSpeedTail.gameObject.SetActive(false);
        CollapseShield();

        powerupUsage = PowerupUsage.Enabled;
    }
    //Raises the shield
    public void RaiseShield()
    {
        playerVulnerability = PlayerVulnerability.Shielded;

        normalCollider.enabled = false;
        shieldCollider.enabled = true;

        StartCoroutine(FunctionLibrary.ScaleTo(shield, new Vector2(1, 1), 0.25f));
    }
    //Collapses the shield
    public void CollapseShield()
    {
        playerVulnerability = PlayerVulnerability.Disabled;

        normalCollider.enabled = true;
        shieldCollider.enabled = false;

        StartCoroutine(FunctionLibrary.ScaleTo(shield, new Vector2(0, 0), 0.25f));
        StartCoroutine(EnableVulnerability(0.3f));
    }
    //Returns true, if a powerup can be activated
    public bool CanUsePowerup()
    {
        return playerState == PlayerState.Enabled && powerupUsage == PowerupUsage.Enabled;
    }

    //Calculate distances to minDepth and maxDepth
    private void CalculateDistances()
    {
        distanceToMax = this.transform.position.y - maxDepth;
        distanceToMin = minDepth - this.transform.position.y;
    }

    //Move and rotate the submarine based on speed
    private void MoveAndRotate()
    {
        //Calculate new rotation
        newRotation.z = speed / rotationDiv;

        //Apply new rotation and position
		this.transform.eulerAngles = newRotation;
		if (playerStatus == PlayerStatus.Sinking) {
			this.transform.Translate(Vector2.down * -speed * Time.deltaTime);
		} else {
			this.transform.position = playerCoords;
		}


    }
    //Sinks the submarine until it crashes to the sand
    private void Sink()
    {
        float crashDepth = maxDepth - 0.8f;
        float crashDepthEdge = 0.5f;

        float distance = this.transform.position.y - crashDepth;

        //If the sub is too close to minDepth
        if (distanceToMin < depthEdge)
        {
            //Calculate maximum speed at this depth (without this, the sub would leave the gameplay are)
            newSpeed = maxVerticalSpeed * (minDepth - this.transform.position.y) / depthEdge;

            //If the newSpeed is greater the the current speed
            if (newSpeed < speed)
                //Make newSpeed the current speed
                speed = newSpeed;
        }
        //If the distance to the sand is greater than 0.1
        if (distance > 0.1f)
        {
            //Reduce speed
            speed -= Time.deltaTime * maxVerticalSpeed * 0.6f;

            //If the distance to the sand smaller than the crashDepthEdge
            if (distance < crashDepthEdge)
            {
                //Calculate new speed for impact
                newSpeed = maxVerticalSpeed * (crashDepth - this.transform.position.y) / crashDepthEdge;

                //If newSpeed is greater than speed
                if (newSpeed > speed)
                    //Apply new speed to speed
                    speed = newSpeed;
            }

            //Apply the above to the submarine
            MoveAndRotate();

            //If distance to sand smaller than 0.2
            if (distance < 0.25f)
                //Enable smoke emission
                smoke.enableEmission = true;
        }
        //If the distance to the sand is smaller than 0.1
        else
        {
            //Disable this function from calling, and stop the level
            playerStatus = PlayerStatus.Crashed;
            levelManager.StopLevel();

            //Disable the smoke
            StartCoroutine(FunctionLibrary.CallWithDelay(DisableSmoke, 2));
        }
    }

    //Enables player vulnerability after time
    private IEnumerator EnableVulnerability(float time)
    {
        //Declare variables, get the starting position, and move the object
        float i = 0.0f;
        float rate = 1.0f / time;

        while (i < 1.0)
        {
            //If the game is not paused, increase t
            if (playerState == PlayerState.Enabled)
                i += Time.deltaTime * rate;

            //Wait for the end of frame
            yield return 0;
        }

        playerVulnerability = PlayerVulnerability.Enabled;
    }
    //Revives the player, and moves the submarine upward
    private IEnumerator ReviveProcess()
    {
        //Change the texture to intact, and play the revive particle
		subRenderer.sprite = subTextures[currentSkinID * 2 + 1];
        reviveParticle.Play();

        //Reset rotation, and move the submarine up
        newRotation = new Vector3(0, 0, 0);
        this.transform.eulerAngles = newRotation;
        StartCoroutine(FunctionLibrary.MoveElementBy(this.transform, new Vector2(0, Mathf.Abs(this.transform.position.y - maxDepth)), 0.5f));

        yield return new WaitForSeconds(0.5f);

        //Reset states
        playerStatus = PlayerStatus.Moving;
        playerState = PlayerState.Enabled;
        playerVulnerability = PlayerVulnerability.Enabled;

        bubbles.Play();

        yield return new WaitForSeconds(0.5f);
        powerupUsage = PowerupUsage.Enabled;
        reviveParticle.Clear();
    }
}