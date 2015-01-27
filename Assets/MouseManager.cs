using UnityEngine;
using System.Collections;

public class MouseManager : MonoBehaviour {
	
	public bool useSpring = false;
	
	//public LineRenderer dragLine;
	
	Rigidbody2D grabbedObject = null;
	SpringJoint2D springJoint = null;
	
	float velocityRatio = 4f; 	// If we aren't using a spring

	public GameObject bean;

	GameObject go;
	public GameStats gameStats;

	void Start() {
		go = GameObject.Find("_SCRIPTS_");
		gameStats = (GameStats) go.GetComponent(typeof(GameStats));
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
				if(hit.collider.rigidbody2D != null) {
					if(hit.collider.gameObject.name.Equals ("blue_land"))
						gameStats.deselect ();
					grabbedObject = hit.collider.rigidbody2D;
					
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
						springJoint.collideConnected = true;
						
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
				gameStats.deselect ();
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

		if (Input.GetMouseButton (1)) {
			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);
			
			Vector2 dir = Vector2.zero;
			Object newBean = Instantiate (bean, mousePos2D, Quaternion.identity);
		}
		

		
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
	
}
