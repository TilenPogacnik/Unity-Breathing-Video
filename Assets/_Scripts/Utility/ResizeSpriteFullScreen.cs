using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ResizeSpriteFullScreen : MonoBehaviour {

	private SpriteRenderer sRend;

	// Use this for initialization
	void Start () {
		sRend = this.GetComponent<SpriteRenderer> ();
		Resize ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Resize(){

		if (sRend == null) {
			sRend = this.GetComponent<SpriteRenderer>();
			if (sRend == null){
				Debug.LogError("Can not find component of type SpriteRenderer on GameObject " + gameObject.name);
			}
		} 
		float width = sRend.sprite.bounds.size.x;
		float height = sRend.sprite.bounds.size.y;

		float spriteAspectRatio = width / height;
		float cameraAspectRatio = Camera.main.aspect;
		
		float yScale;
		float xScale;
		
		//If sprite aspect ratio is wider than screen aspect ratio-> set sprite width to screen width and calculate correct height
		if (spriteAspectRatio > cameraAspectRatio) {
			xScale = cameraAspectRatio * 2.0f * Camera.main.orthographicSize;
			yScale = xScale / spriteAspectRatio;
			
		//If sprite aspect ratio is taller than screen aspect ratio -> set sprite height to screen height and calculate correct width
		} else {
			xScale = spriteAspectRatio * 2.0f * Camera.main.orthographicSize / width;
			yScale = 2.0f*Camera.main.orthographicSize / height;
		}
		
		gameObject.transform.localScale = new Vector3 (xScale, yScale, 1.0f);
		//gameObject.transform.position = Camera.main.transform.position;
	}
}
