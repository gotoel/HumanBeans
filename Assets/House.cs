﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class House {

	public List<GameObject> houseBase = new List<GameObject>();
	List<GameObject> houseWalls = new List<GameObject>();
	GameObject houseRoof = new GameObject();

	bool baseDone = false;
	bool wallsDone = false;
	bool roofDone = false;
	//bool houseBuilt = false;
	bool firstBlockDown = false;

	public Vector2 houseStartLoc;

	public Object stoneBlock = Resources.Load("StoneBlock");
	public Object stoneRoof = Resources.Load ("StoneTriangle");

	public GameObject originalBuilder;
	public GameObject owner;
	public List<GameObject> residents;


	public float blockWidth, blockHeight;
	// Use this for initialization
	void Start () {
	
	}

	public House(Vector2 l, GameObject orig) {
		houseRoof = null;
		Debug.Log ("HOUSE BUILT");
		houseStartLoc = l;
		Debug.Log ("house loc: " + houseStartLoc.ToString ());
		originalBuilder = orig;
		owner = orig;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ownerDied() {
		owner = null;
	}

	public void addBlock() {
		if (!firstBlockDown) {
			houseBase.Add ((GameObject)GameObject.Instantiate (stoneBlock, new Vector2 (houseStartLoc.x, houseStartLoc.y), Quaternion.identity));
			houseBase [houseBase.Count - 1].GetComponent<StoneBlock> ().partOfHouse = true;
			houseBase [houseBase.Count - 1].GetComponent<Rigidbody2D>().isKinematic = true;
			blockWidth = houseBase[0].GetComponent<Collider2D>().bounds.size.x;
			blockHeight = houseBase[0].GetComponent<Collider2D>().bounds.size.y;
			firstBlockDown = true;
		}
		else if (!baseDone) {
			houseBase.Add ((GameObject)GameObject.Instantiate (stoneBlock, new Vector2 (houseStartLoc.x + houseBase.Count*blockWidth, houseStartLoc.y), Quaternion.identity));
			houseBase [houseBase.Count - 1].GetComponent<StoneBlock> ().partOfHouse = true;
			houseBase [houseBase.Count - 1].GetComponent<Rigidbody2D>().isKinematic = true;
		} else if (!wallsDone) {
			// left side of house
			if(houseWalls.Count < 3) {
				houseWalls.Add ((GameObject)GameObject.Instantiate (stoneBlock, new Vector2 ((houseStartLoc.x), (houseStartLoc.y)+(houseBase.Count * blockHeight)), Quaternion.identity));
				houseWalls [houseWalls.Count - 1].GetComponent<StoneBlock> ().partOfHouse = true;
				//houseWalls [houseWalls.Count - 1].rigidbody2D.isKinematic = true;
			}
			// right side of house
			else {
				houseWalls.Add ((GameObject)GameObject.Instantiate (stoneBlock, new Vector2 ((houseStartLoc.x + ((houseBase.Count-1) * blockHeight)), (houseStartLoc.y)+(houseBase.Count-3 * blockHeight)), Quaternion.identity));
				houseWalls [houseWalls.Count - 1].GetComponent<StoneBlock> ().partOfHouse = true;
				//houseWalls [houseWalls.Count - 1].rigidbody2D.isKinematic = true;
			}
		} else if (!roofDone) {
			houseRoof = (GameObject)GameObject.Instantiate (stoneRoof, new Vector2 (houseStartLoc.x + (1.5F*blockWidth), (houseStartLoc.y)+(5 * blockHeight)), Quaternion.identity);
			houseRoof.GetComponent<StoneTriangle> ().partOfHouse = true;
			roofDone = true;
			//houseRoof.rigidbody2D.isKinematic = true;
		}
		if (houseBase.Count >= 4)
			baseDone = true;
		if (houseWalls.Count >= 6)
			wallsDone = true;
		if (houseRoof != null) {
			roofDone = true;
			//houseBuilt = true;
		}

	}
}
