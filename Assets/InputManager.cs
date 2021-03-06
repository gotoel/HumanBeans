﻿using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	
	public bool useSpring = false;
	
	//public LineRenderer dragLine;
	
	Rigidbody2D grabbedObject = null;
	SpringJoint2D springJoint = null;
	
	float velocityRatio = 4f; 	// If we aren't using a spring

	public string spawnType;


	public bool musicOn = true, sfxOn = true;
	public bool settings = false, debug = false;

	public bool hudOn = true;

	public static InputManager Instance { get; private set; }


	void Awake() {
		if (Instance != null && Instance != this) {
			Destroy (gameObject);
		}
		Instance = this;
		DontDestroyOnLoad (gameObject);
	}
		
	void OnGUI() {
		

		if (GUI.Button (new Rect (Screen.width-90, Screen.height-30, 80, 20), "Toggle HUD")) {
			hudOn = !hudOn;
		}
		if (hudOn) {
			GUI.Box (new Rect (Screen.width-110, 20, 100, 90), "Spawn Menu");
			if (GUI.Button (new Rect (Screen.width - 100, 50, 80, 20), "Beans")) {
				spawnType = "beans";
			}
			if (GUI.Button (new Rect (Screen.width - 100, 80, 80, 20), "Blocks")) {
				spawnType = "blocks";
			}

			GUI.Box (new Rect (Screen.width - 110, 115, 100, 190), "Settings");
			if (GUI.Button (new Rect (Screen.width - 100, 145, 80, 20), "Music")) {
				musicOn = !musicOn;
				if(musicOn) 
					Camera.main.GetComponent<AudioSource>().Play ();
				else
					Camera.main.GetComponent<AudioSource>().Pause();
			}
			if (GUI.Button (new Rect (Screen.width - 100, 175, 80, 20), "SFX")) {
				sfxOn = !sfxOn;
			}
			if (GUI.Button (new Rect (Screen.width - 100, 205, 80, 20), "Debug")) {
				debug = !debug;
			}
		}


	}

	void Update() {
		if( Input.GetMouseButtonDown(0) ) {
			// We clicked, but on what?
			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);
			
			Vector2 dir = Vector2.zero;
			
			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, dir);
			if(hit.collider!=null) {
				// We clicked on SOMETHING that has a collider
				if(hit.collider.transform.root.GetComponent<Rigidbody2D>() != null) {
					if(hit.collider.gameObject.name.Equals ("blue_land"))
                    GameStats.Instance.deselect();
                    grabbedObject = hit.collider.transform.root.GetComponent<Rigidbody2D>();
					
					if(useSpring) {
						springJoint = grabbedObject.gameObject.AddComponent<SpringJoint2D>();
						// Set the anchor to the spot on the object that we clicked.
						Vector3 localHitPoint = grabbedObject.transform.InverseTransformPoint(hit.point);
						springJoint.anchor = localHitPoint;
						springJoint.connectedAnchor = mouseWorldPos3D;
						springJoint.distance = 0.25f;
						springJoint.dampingRatio = 1;
						springJoint.frequency = 5;
						
						// Enable this if you want to collide with objects still (and you probably do)
						// This will also WAKE UP the spring.
                        springJoint.enableCollision = true;
						
						// This will also WAKE UP the spring, even if it's a totally
						// redundant line because the connectedBody should already be null
						springJoint.connectedBody = null;
					}
					else {
						// We're using velocity instead
						grabbedObject.gravityScale=0;
					}
					
					//dragLine.enabled = true;
				}
			}
			else
				GameStats.Instance.deselect ();
		}
		
		if( Input.GetMouseButtonUp(0) && grabbedObject!=null ) {
			if(useSpring) {
				Destroy(springJoint);
				springJoint = null;
			}
			else {
				grabbedObject.gravityScale=1;
			}
			grabbedObject = null;
			//dragLine.enabled = false;
		}
		if (Input.touchCount == 1) {

		}

        	if (Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.Space))
        	{
			if(GameStats.Instance.beansList.Count < GameStats.Instance.getMaxBeans ()) {
				Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);
				
				Vector2 dir = Vector2.zero;
				if(spawnType.Equals ("beans"))
					Instantiate (Resources.Load ("Bean_prefab"), mousePos2D, Quaternion.identity);
				else if(spawnType.Equals ("blocks"))
					Instantiate (Resources.Load ("StoneBlock"), mousePos2D, Quaternion.identity);
			}
		
		}

		int nbTouches = Input.touchCount;
		
		//if(nbTouches > 0)
		//{
		//	if(GameStats.Instance.beansList.Count < GameStats.Instance.getMaxBeans ()) {
		//		//print(nbTouches + " touch(es) detected");
		//		
		//		for (int i = 0; i < nbTouches; i++)
		//		{
		//			Touch touch = Input.GetTouch(i);
		//			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(touch.position);
		//			Vector2 mousePos2D = new Vector2(touch.position.x, touch.position.y);
		//			
		//			Vector2 dir = Vector2.zero;
		//			if(spawnType.Equals ("beans"))
		//				Instantiate (bean, touch.position, Quaternion.identity);
		//			else if(spawnType.Equals ("blocks"))
		//				Instantiate (stoneBlock, touch.position, Quaternion.identity);
		//					
		//			print("Touch index " + touch.fingerId + " detected at position " + touch.position);
		//		}
		//	}
		//}

		
	}
	
	
	void FixedUpdate () {
		if(grabbedObject != null) {
			Vector2 mouseWorldPos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(useSpring) {
				springJoint.connectedAnchor = mouseWorldPos2D;
			}
			else {
				grabbedObject.velocity = (mouseWorldPos2D - grabbedObject.position) * velocityRatio;
			}
		}
	}

	void LateUpdate() {
		if(grabbedObject != null) {
			if(useSpring) {
				Vector3 worldAnchor = grabbedObject.transform.TransformPoint(springJoint.anchor);
				//dragLine.SetPosition(0, new Vector3(worldAnchor.x, worldAnchor.y, -1));
				//dragLine.SetPosition(1, new Vector3(springJoint.connectedAnchor.x, springJoint.connectedAnchor.y, -1));
			}
			else {
				Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//dragLine.SetPosition(0, new Vector3(grabbedObject.position.x, grabbedObject.position.y, -1));
				//dragLine.SetPosition(1, new Vector3(mouseWorldPos3D.x, mouseWorldPos3D.y, -1));
			}
		}
	}

	void toggleMusic() {

	}
	
}
