using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House {

	List<GameObject> houseBase = new List<GameObject>();
	List<GameObject> houseWalls = new List<GameObject>();
	List<GameObject> houseRoof = new List<GameObject>();

	bool baseDone = false;
	bool wallsDone = false;
	bool roofDone = false;

	Vector2 houseStartLoc;

	public Object stoneBlock = Resources.Load("StoneBlock");


	// Use this for initialization
	void Start () {
	
	}

	public void newHouse (Vector2 l) {
		Debug.Log ("HOUSE BUILT");
		houseStartLoc = l;
		Debug.Log ("house loc: " + houseStartLoc.ToString ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		Vector3 point = Camera.main.WorldToScreenPoint (houseStartLoc);

		GUI.Box (new Rect (point.x, point.y, 20, 20), "house");
	}

	public void addBlock() {
		if (!baseDone) {
			Debug.Log ("Count: " + houseBase.Count + " Special x: " + (houseStartLoc.x + houseBase.Count));
			houseBase.Add ((GameObject)GameObject.Instantiate (stoneBlock, new Vector2((houseStartLoc.x + (houseBase.Count * .7F)), houseStartLoc.y - 2F), Quaternion.identity));
			houseBase[houseBase.Count-1].GetComponent<StoneBlock>().partOfHouse = true;
			houseBase[houseBase.Count-1].rigidbody2D.isKinematic = true;
		}

		if (houseBase.Count >= 4)
			baseDone = true;

	}
}
