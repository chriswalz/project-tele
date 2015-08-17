using UnityEngine;
using System.Collections;

public class ScrollingLayer : MonoBehaviour 
{
    public Renderer mainRenderer;           //The main renderer
    public float startingSpeed;             //The starting scrolling speed

    private float speedMultiplier;          //The current speed multiplier
    private bool paused;                    //True, if the level is paused

    private Vector2 offset;

	// Use this for initialization
	void Start () 
    {
        speedMultiplier = 1;
        paused = true;
	}
	// Update is called once per frame
	void Update () 
    {
        if (!paused)
        {
            offset = mainRenderer.material.mainTextureOffset;
            offset.x += startingSpeed * speedMultiplier * Time.deltaTime;

			if (offset.x > 1)
				offset.x -= 1;

            mainRenderer.material.mainTextureOffset = offset;
        }
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
}
