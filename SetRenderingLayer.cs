using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]  
public class SetRenderingLayer : MonoBehaviour 
{
	public string sortingLayer;             //The sorting layer
	public int sortingOrder;                //The sorting order

	void Start()
	{
		this.GetComponent<Renderer>().sortingLayerName = sortingLayer;
		this.GetComponent<Renderer>().sortingOrder = sortingOrder;
	}
}
