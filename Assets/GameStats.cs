using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStats : MonoBehaviour {

	int beans = 0, males = 0, females = 0, deceased = 0;

	List<GameObject> beansList = new List<GameObject>();
	GameObject selectedBean;


	// Use this for initialization
	void Start () {
	
	}

	void OnGUI() {
		GUI.depth = 5;
		GUI.color = Color.black;
		Vector3 point = Camera.main.WorldToScreenPoint (transform.position);
			
		//Debug.Log ("X: " + gameObject.rigidbody2D.position.x + " Y: " + gameObject.rigidbody2D.position.y); 
		GUI.Box (new Rect (50, 50, 120, 120), "Beans: " + beans + "\nMales: " + males + "\nFemales: " + females + "\nDeceased: " + deceased + "\nbeansList count: " + beansList.Count);

		if (selectedBean != null) {
			point = Camera.main.WorldToScreenPoint (selectedBean.transform.position);
			GUI.Label (new Rect (point.x - 50, Screen.height - point.y - 50, 200, 200), "Name: " + selectedBean.GetComponent<BeanLife> ().beanName + " Age: " + selectedBean.GetComponent<BeanLife> ().age + "\nMother: " + selectedBean.GetComponent<BeanLife> ().motherName + "\nFather: " + selectedBean.GetComponent<BeanLife> ().fatherName + "\nAlive: " + !selectedBean.GetComponent<BeanLife> ().isDead);
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void newMale(GameObject g) {
		beansList.Add (g);
		beans++;
		males++;
	}
	public void newFemale(GameObject g) {
		beansList.Add (g);
		beans++;
		females++;
	}

	public void dead(bool isMale, GameObject g) {
		deceased++;
		beans--;
		if (isMale)
			males--;
		else
			females--;
		foreach(GameObject b in beansList) {
			if(b.Equals (g))
				beansList.Remove (b);
		}
	}

	public void setSelected(GameObject b) {
		selectedBean = b;
	}

	public void deselect() {
		selectedBean = null;
	}


}
