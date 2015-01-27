using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	GameObject go;
	public GameStats gameStats;

	// Use this for initialization
	void Start () {
		go = GameObject.Find("_SCRIPTS_");
		gameStats = (GameStats) go.GetComponent(typeof(GameStats));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {

		gameStats.deselect ();

	}
}
