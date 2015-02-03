using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStats : MonoBehaviour {

	int beans = 0, males = 0, females = 0, deceased = 0;

	public const int MAX_BEANS = 250;

	public List<GameObject> beansList = new List<GameObject>();
	//public List<Vector2> houseLocs = new List<Vector2>();
	public List<House> houses = new List<House>();

	GameObject selectedBean;

	public List<List<string>> families = new List<List<string>>();

	public static GameStats Instance { get; private set; } // Singleton ty arma



	public TextAsset maleNamesAsset, femaleNamesAsset;
	public string[] maleNames, femaleNames;

	void Awake() {
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
		maleNamesAsset = Resources.Load("male-names") as TextAsset;
		femaleNamesAsset = Resources.Load("female-names") as TextAsset;
		maleNames = maleNamesAsset.text.Split('\n');
		femaleNames = femaleNamesAsset.text.Split('\n');

	}
	
	
	// Use this for initialization
	void Start () {

	}

	void OnGUI() {
		GUI.depth = 5;
		GUI.color = Color.black;
		Vector3 point = Camera.main.WorldToScreenPoint (transform.position);
			
		//Debug.Log ("X: " + gameObject.rigidbody2D.position.x + " Y: " + gameObject.rigidbody2D.position.y); 
		GUI.Box (new Rect (10, 10, 130, 90), "Beans: " + beans + "\nMales: " + males + "\nFemales: " + females + "\nDeceased: " + deceased + "\nbeansList count: " + beansList.Count);

		if (selectedBean != null) {
			point = Camera.main.WorldToScreenPoint (selectedBean.transform.position);
			GUI.Label (new Rect (point.x - 50, Screen.height - point.y - 50, 200, 200), "Name: " + selectedBean.GetComponent<BeanLife> ().beanName + " Age: " + selectedBean.GetComponent<BeanLife> ().age + "\nMother: " + selectedBean.GetComponent<BeanLife> ().motherName + "\nFather: " + selectedBean.GetComponent<BeanLife> ().fatherName + "\nAlive: " + !selectedBean.GetComponent<BeanLife> ().isDead + "\nInventory: " + selectedBean.GetComponent<BeanLife> ().blockMaterial);
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
		beansList.Remove (g);
	}

	public void setSelected(GameObject b) {
		selectedBean = b;
	}

	public void deselect() {
		selectedBean = null;
	}

	public int getMaxBeans() {
		return MAX_BEANS;
	}

	public bool locIsTaken(Vector2 loc) {
		bool taken = false;
		foreach(House h in houses) {
			if(loc.x > (h.houseStartLoc.x - 2.8F) && loc.x < (h.houseStartLoc.x + 2.8F)) {
				//Debug.Log (loc.x + " is greater than " + (v.x - 2.8F) + " and " + loc.x + " is less than " + (v.x + 2.8F));
				taken = true;
			}
		}
		return taken;
	}


}
