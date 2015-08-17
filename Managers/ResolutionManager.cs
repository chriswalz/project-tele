using UnityEngine;
using System.Collections;

public class ResolutionManager : MonoBehaviour 
{
    public Transform[] toScale;                     //Elements to scale
    public Transform[] toReposition;                //Elements to reposition

    public Renderer sand;                           //The renderer of the sand

    public bool checkEveryFrame;

    private int lastWidth;
    private int lastHeight;

    private float scaleFactor;                      //The current scale factor
    private float lastScaleFactor;

    private bool scaleScreen;

    void Start()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        lastScaleFactor = 1;
        ScaleScreen();
    }
    void Update()
    {
        if (checkEveryFrame)
        {
            if (lastWidth != Screen.width || lastHeight != Screen.height)
            {
                lastWidth = Screen.width;
                lastHeight = Screen.height;

                lastScaleFactor = scaleFactor;
                scaleScreen = true;
            }
            else if (scaleScreen)
            {
                ScaleScreen();
            }
        }
    }

    void ScaleScreen()
    {
        scaleFactor = Camera.main.aspect / 1.28f;

        //Rescale elements
        foreach (Transform item in toScale)
            item.localScale = new Vector3((item.localScale.x / lastScaleFactor) * scaleFactor, item.localScale.y, item.localScale.z);

        //Reposition Elements
        foreach (Transform item in toReposition)
            item.position = new Vector3((item.position.x / lastScaleFactor) * scaleFactor, item.position.y, item.position.z);

        //Rescale sand 
        sand.material.mainTextureScale = new Vector2((sand.material.mainTextureScale.x / lastScaleFactor) * scaleFactor, 1);

        scaleScreen = false;
    }
}
