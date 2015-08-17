using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour 
{
    public Transform target;                    //The target to follow
	
	// Update is called once per frame
	void Update () 
    {
        this.transform.position = target.position;	
	}
}
