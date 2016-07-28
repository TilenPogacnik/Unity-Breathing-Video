using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler {
	
	private Vector2 pointerOffset;
	private RectTransform canvasRectTransform;
	private RectTransform panelRectTransform;
	private Vector3[] canvasCorners;

	private Vector3[] panelCorners;
	private Vector2 worldPanelDimensions;
	private Vector2 worldPointerToPanelCenter;
	
	void Awake () {
		Canvas canvas = GetComponentInParent <Canvas>();
		if (canvas != null) {
			canvasRectTransform = canvas.transform as RectTransform;
			panelRectTransform = transform as RectTransform;

		} else {
			Debug.LogError("DragPanel script should be attached to a child of Canvas");
		}

	}

	void Start(){
		canvasCorners = new Vector3[4];
		canvasRectTransform.GetWorldCorners(canvasCorners);

		panelCorners = new Vector3[4];
		panelRectTransform.GetWorldCorners (panelCorners);
		worldPanelDimensions = panelCorners [2] - panelCorners [0];
	}

	//Calculates pointerOffset - position of mouse position in panel and WorldPointerToPanelCenter (distance from mouse to panel center)
	public void OnPointerDown (PointerEventData pointer) {
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, pointer.position, pointer.pressEventCamera, out pointerOffset);

		worldPointerToPanelCenter = pointer.position - new Vector2(panelRectTransform.position.x, panelRectTransform.position.y);
	}
	
	public void OnDrag (PointerEventData pointer) {
		if (panelRectTransform == null)
			return;
		
		Vector2 pointerPostion = ClampPointerToScreen (pointer);
		
		Vector2 localPointerPosition;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (
			canvasRectTransform, pointerPostion, pointer.pressEventCamera, out localPointerPosition
			)) {
			panelRectTransform.localPosition = localPointerPosition - pointerOffset;
		}
	}


	Vector2 ClampPointerToScreen (PointerEventData data) {
		return new Vector2 (Mathf.Clamp (data.position.x, canvasCorners[0].x + worldPointerToPanelCenter.x, canvasCorners[2].x - worldPanelDimensions.x + worldPointerToPanelCenter.x),
		                    Mathf.Clamp (data.position.y, canvasCorners[0].y + worldPointerToPanelCenter.y + worldPanelDimensions.y, canvasCorners[2].y + worldPointerToPanelCenter.y));
	}
}