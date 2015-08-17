using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InputManager : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PlayerManager playerManager;         //A link to the Player Manager
	public void OnPointerDown(PointerEventData eventData){
		setPosition ();
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		playerManager.UpdateInput(true);
	}
	public void OnDrag(PointerEventData data)
	{
		setPosition ();
	}
	private void setPosition()
	{
		Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float mX = pos.x;
		float mY = pos.y;
		playerManager.UpdateCoords (mX, mY);
		print ("Coords updated");
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		playerManager.UpdateInput(false);
	}
}
