using UnityEngine;
using System.Collections;

public class BeanLife : MonoBehaviour {
	SpriteRenderer sr;

	public float moveSpeed = -5; 
	public float tick = 0;
	public float chanceToTurn = 0.01F;
	public float randomChance;
	public bool isMale = true, isAdult = false, isDead = false;
	public Sprite maleSprite, femaleSprite, maleBabySprite, femaleBabySprite, maleDeadSprite, femaleDeadSprite;
	public bool givenBirth = false;
	public bool isSelected = false;
	public bool colliding = false;
	public int age;

	public GameObject bean; 
	public GameStats gameStats;

	public string beanName = "noname", motherName = "God", fatherName = "God";

	GameObject go;


	//GUI
	//Vector2 offset =  Vector2(0, 1.5);

	void Start () {
		go = GameObject.Find("_SCRIPTS_");
		gameStats = (GameStats) go.GetComponent(typeof(GameStats));
		age = 0;
		isAdult = false;
		generateName ();
		InvokeRepeating ("birthday", 0, 1);
		sr = GetComponent<SpriteRenderer>();
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
		resizeCollider ();
	

	}
	
	void FixedUpdate() {
		if (!isDead) {
			randomChance = UnityEngine.Random.Range (chanceToTurn, 1);
			if (randomChance >= .99) {
				if (moveSpeed < 0)
					moveSpeed = Random.Range (1, 10);
				else
					moveSpeed = Random.Range (-10, -1);
				chanceToTurn = 0;

			} else
				chanceToTurn += 0.01F;
			if(colliding)	
				rigidbody2D.AddForce (new Vector2 (moveSpeed, 20));
			else
				rigidbody2D.AddForce (new Vector2 (moveSpeed, 0));

			tick++;
		}
	}

	void OnGUI() {
		if (isSelected) {
			GUI.depth = 5;
			GUI.color = Color.black;
			Vector3 point = Camera.main.WorldToScreenPoint (transform.position);

			//Debug.Log ("X: " + gameObject.rigidbody2D.position.x + " Y: " + gameObject.rigidbody2D.position.y); 
			//GUI.Label (new Rect (point.x - 50, Screen.height - point.y - 50, 200, 200), "Name: " + beanName + " Age: " + age + "\nMother: " + motherName + "\nFather: " + fatherName + "\nAlive: " + !isDead);
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
			isSelected = true;
			gameStats.deselect ();
		} else {
			isSelected = false;
			gameStats.setSelected (this.gameObject);
		}
	}

	void OnMouseOver()
	{
		if (isSelected) {
			isSelected = true;
			gameStats.deselect ();
		}
	}
	void OnMouseExit()
	{
		if (!isSelected) {
			isSelected = false;
			gameStats.setSelected (this.gameObject);
		}
	}

	public void setMother(string s) { motherName = s; }
	public void setFather(string s) { fatherName = s; }

}

