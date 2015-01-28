using UnityEngine;
using System.Collections;

public class BeanLife : MonoBehaviour {

	// movement
	private float moveSpeed = -5; 
	private float tick = 0;
	private float chanceToTurn = 0.01F;
	private float randomChance;
	private bool colliding = false;


	// life
	public bool isMale = true, isAdult = false, isDead = false;
	private bool givenBirth = false;
	public int age;

	// family
	public string beanName = "noname", motherName = "God", fatherName = "God";

	// player interaction
	private bool isSelected = false;


	// appearence
	SpriteRenderer sr;
	public Sprite maleSprite, femaleSprite, maleBabySprite, femaleBabySprite, maleDeadSprite, femaleDeadSprite;

	// objects
	public GameObject bean; 
	public GameStats gameStats;
	GameObject go;

	// inventory
	public int blockMaterial = 0;

	// house
	public Vector2 houseLoc;
	public House house;
	public bool hasHouse;


	//GUI
	//Vector2 offset =  Vector2(0, 1.5);

	void Start () {
		house = null;
		hasHouse = false;

		// Add to game stats list
		// Slow, as it searches the entire scene for the GameStats object, need to fix.
		go = GameObject.Find("_SCRIPTS_");
		gameStats = (GameStats) go.GetComponent(typeof(GameStats));

		// set life specs
		age = 0;
		isAdult = false;
		generateName ();

		// Invoke repeating of the birthday method
		InvokeRepeating ("birthday", 0, 1);

		// initilize sprite renderer so that we can change sprite
		sr = GetComponent<SpriteRenderer>();

		// 50/50 chance of becoming male/female -- could change later to make game easier
		if (UnityEngine.Random.value < .5) {
			isMale = true;
			this.name = "Bean_Male";
			sr.sprite = maleBabySprite; 
			gameStats.newMale (this.gameObject);
		} else { 
			isMale = false;
			this.name = "Bean_Female";
			sr.sprite = femaleBabySprite;
			gameStats.newFemale (this.gameObject);
		}

		// resize the 2d collidor to fit the baby sprite
		resizeCollider ();
	

	}
	
	void FixedUpdate() {
		if (!isDead) {

			if(blockMaterial > 0) {
				buildHouse();
			}

			// random chance to turn and walk a different direction
			randomChance = UnityEngine.Random.Range (chanceToTurn, 1);
			if (randomChance >= .99) {
				if (moveSpeed < 0)
					moveSpeed = Random.Range (1, 10);
				else
					moveSpeed = Random.Range (-10, -1);
				chanceToTurn = 0;

			} else
				chanceToTurn += 0.01F;

			// if colliding with another object jump over it
			if(colliding)	
				rigidbody2D.AddForce (new Vector2 (moveSpeed, 20));
			else
				rigidbody2D.AddForce (new Vector2 (moveSpeed, 0));

			tick++;
		}
	}

	void buildHouse() {
		Debug.Log ("Building house...");
		if (house == null) {
			houseLoc = new Vector2(this.transform.position.x, this.transform.position.y+2F);
			house = new House();
			house.newHouse (houseLoc);
			house.addBlock ();
			hasHouse = true;
		}
		else
			house.addBlock ();
		blockMaterial--;
	}

	void OnGUI() {
		if (hasHouse) {
			// draw house

			GUI.depth = 5;
			GUI.color = Color.black;
			Vector3 point = Camera.main.WorldToScreenPoint (houseLoc);
			//Debug.Log ("House screen coord - x: " + point.x + " y: " + point.y);
			GUI.Box (new Rect (point.x, point.y, 120, 20), beanName + "'s house");
		}
	}

	// Update is called once per frame
	void Update () {

	}


	void birthday() {
		if (!isAdult && age >= 18) {
			isAdult = true;
			if(isMale)
				sr.sprite = maleSprite; 
			else 
				sr.sprite = femaleSprite; 
			resizeCollider ();
		}
		if(!isDead)
			age++; 

		// FIX DEATH
		int deathInt = Random.Range (age * age, 150000);
		if (deathInt >= 149090) {
			isDead = true;
			if(isMale)
				sr.sprite = maleDeadSprite;
			else
				sr.sprite = femaleDeadSprite;
			gameStats.dead (isMale, this.gameObject);
			Invoke("destroy", 10);
		}
	}

	void destroy() {
		Destroy (this.gameObject);
	}


	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.name.Contains ("Bean")) {
			colliding = true;
			if (col.gameObject.GetComponent<BeanLife> ().isMale && col.gameObject.GetComponent<BeanLife> ().isAdult && !isMale && isAdult && !givenBirth) {
				if(!col.gameObject.GetComponent<BeanLife> ().beanName.Equals (fatherName)) {
					Debug.Log ("Creating babby at: " + col.contacts [0].point);
					GameObject newBean = (GameObject)Instantiate (bean, col.contacts [0].point, Quaternion.identity);
					newBean.GetComponent<BeanLife>().setMother (beanName);
					newBean.GetComponent<BeanLife>().setFather (col.gameObject.GetComponent<BeanLife> ().beanName);
					givenBirth = true;
				}
			}
		}
		if (col.gameObject.name.Contains ("StoneBlock")) {
			if(!col.gameObject.GetComponent<StoneBlock>().partOfHouse) {
				Destroy(col.gameObject);
				blockMaterial++;
			}
		}
	}


	void OnCollisionStay2D(Collision2D col) {
		if (col.gameObject.name.Contains ("Bean")) {
			if (col.gameObject.GetComponent<BeanLife> ().isMale == isMale)
				chanceToTurn += 0.5F;

			//Debug.Log (this.gameObject.GetComponent<Collider2D>().transform.position.z);
		}
	}


	void OnCollisionExit2D(Collision2D col) {
		if (col.gameObject.name.Contains ("Bean")) {
			colliding = false;
		}
	}

	void resizeCollider() {
		CircleCollider2D c = this.gameObject.GetComponent<CircleCollider2D> ();
		if(isAdult)
			c.radius = 0.35F;
		else
			c.radius = 0.19F;
	}

	private void generateName() {
		TextAsset txt = null;
		string content = "";
		if(isMale)
			txt = Resources.Load("male-names") as TextAsset;
		else
			txt = Resources.Load("female-names") as TextAsset;
		content = txt.text;
		bool nameFound = false;
		string[] words = content.Split('\n');
		while (!nameFound) {
			int nameInt = Random.Range (1,2900);
			string text = words[nameInt];

			object[] obj = GameObject.FindObjectsOfType(typeof (GameObject));
			bool dupe = false;
			foreach (object o in obj)
			{
				GameObject g = (GameObject) o;
				if(g.name.Contains ("Bean")) {
					if(g.GetComponent<BeanLife>().name.Equals (text))
						dupe = true;
				}
				Debug.Log(g.name);
			}
			if(!dupe) {
				nameFound = true;
				beanName = text;
				Debug.Log (beanName + " has been born!");
			}
		}
	}

	void OnMouseDown() {
		if (isSelected) {
			isSelected = false;
			gameStats.deselect ();
		} else {
			isSelected = true;
			gameStats.setSelected (this.gameObject);
		}
	}

	void OnMouseOver()
	{

	}
	void OnMouseExit()
	{

	}

	public void setMother(string s) { motherName = s; }
	public void setFather(string s) { fatherName = s; }

}

