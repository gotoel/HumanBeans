using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BeanLife : MonoBehaviour {

	// movement
	private float moveSpeed = -5; 
	private float tick = 0;
	private float chanceToTurn = 0.01F;
	private float randomChance;
	private bool colliding = false;
	private bool buildingHouse = false;
	public bool isGrounded = false;


	// life
	public bool isMale = true, isAdult = false, isDead = false;
	private bool givenBirth = false;
	public int age;
	public string emotion;

	// family
	public string beanName = "noname", motherName = "God", fatherName = "God";
	public bool partOfFamily;
	private int curChildren, maxChildren;

	// player interaction
	private bool isSelected = false;


	// appearence
	SpriteRenderer sr;
	public Sprite maleSprite, femaleSprite, maleDeadSprite, femaleDeadSprite;
	public Vector3 adultSprite = new Vector3 (1f, 1f);
	public Vector3 childSprite = new Vector3(0.5f,0.5f);

	// objects
	public GameObject bean; 
	public GameStats gameStats;
	public InputManager inputManager;
	GameObject go;

	// inventory
	public int blockMaterial;

	// house
	public Vector2 houseLoc;
	public House house;
	public bool hasHouse;

	// sound
	public AudioClip introSoundMale, deathSoundMale, introSoundFemale, deathSoundFemale;

	// On creation of bean object
	void Start () {

		// bean spawns homeless, will add setting of a home so that
		// a baby bean's parents home gets assigned to it.
		house = null;
		hasHouse = false;

		// Add to game stats list
		gameStats = GameStats.Instance;
		inputManager = InputManager.Instance;

		// initilize sprite renderer so that we can change sprite
		sr = GetComponent<SpriteRenderer>();

		// 50/50 chance of becoming male/female -- could change later to make game easier
		if (UnityEngine.Random.value < .5) {
			isMale = true;
			this.name = "Bean_Male";
			sr.sprite = maleSprite; 
			gameStats.newMale (this.gameObject);
		} else { 
			isMale = false;
			this.name = "Bean_Female";
			sr.sprite = femaleSprite;
			gameStats.newFemale (this.gameObject);
		}
		CircleCollider2D c = this.gameObject.GetComponent<CircleCollider2D> ();
		SpriteRenderer s = this.gameObject.GetComponent<SpriteRenderer> ();
		s.transform.localScale = childSprite;
		c.transform.localScale = s.transform.localScale;


        	// set life specs
        	age = 0;
        	isAdult = false;
        	generateName();
        	partOfFamily = false;
		emotion = "happy";
		curChildren = 0;
		maxChildren = Random.Range (0, 5);

        	// Invoke repeating of the ageIncrase method
        	InvokeRepeating("ageIncrease", 0, 1);

		// resize the 2d collidor to fit the baby sprite
		//resizeCollider ();
		
		//blockMaterial = 13;
		if(isMale)
			AudioSource.PlayClipAtPoint (introSoundMale, this.transform.position);
		else
			AudioSource.PlayClipAtPoint (introSoundFemale, this.transform.position);
	}


	// called every phyisics engine update
	void FixedUpdate() {
		if (!isDead && !buildingHouse) {

			// if bean has more than one building material in inventory, add to its house
			// will need to check if the bean's current house is build already. Then material will
			// not be picked up.
			if(blockMaterial > 0 && isGrounded) {
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

			 
		}

		//physics engine update tick
		tick++;
	}


	// performs the beans side of building a house
	void buildHouse() {
		buildingHouse = true;

		//Debug.Log ("Building house...");
		// if the current house does not exist, create a new house at the bean's current world location.
		// The house object will be created using the House class, which will handle the physical construction of the house.
		// Place the first block that triggered this method call.
		// And if the house does exist, add a block to it.
		if (house == null && !GameStats.Instance.locIsTaken(this.gameObject.transform.position)) {
			houseLoc = new Vector2 (this.transform.position.x, this.transform.position.y);
			house = new House ();
			house.newHouse (houseLoc);
			//GameStats.Instance.houseLocs.Add (houseLoc);
			GameStats.Instance.houses.Add (house); 

			rigidbody2D.MovePosition (houseLoc);
			house.addBlock ();
			hasHouse = true;
			blockMaterial--;
		} else if(house != null) {
			rigidbody2D.AddForce(houseLoc);
			house.addBlock ();
			blockMaterial--;
		}
		buildingHouse = false;
	}

	// GUI drawing method.
	void OnGUI() {

		// draw a label for the bean's house location
		if (hasHouse) {
			// draw house

			GUI.depth = 5;
			GUI.color = Color.black;
			Vector3 point = Camera.main.WorldToScreenPoint (new Vector2(houseLoc.x-2, houseLoc.y-2));
			Vector3 pointTwo = Camera.main.WorldToScreenPoint (new Vector2(houseLoc.x+2, houseLoc.y+2));
			//Debug.Log ("House screen coord - x: " + point.x + " y: " + point.y);
			// draw house area.
			if(inputManager.debug) {
			GUI.Box (new Rect (point.x, point.y, pointTwo.x, pointTwo.y), beanName + "'s house");
			GUI.Label (new Rect (point.x, point.y, 200, 120), ". X: " + (houseLoc.x-2) + " Y: " + (houseLoc.y-2));
			GUI.Label (new Rect (pointTwo.x, pointTwo.y, 200, 120), ". X: " + (houseLoc.x+2) + " Y: " + (houseLoc.y+2));
			}
		}
	}

	// Update is called once per frame
	void Update () {

	}

	// method to handle the age increase of the bean.
	// Also handles the chance of death of the bean.
	void ageIncrease() {

		if (!isDead) {
			age++;
			resizeBean ();
		}
		if (!isAdult && age >= 18) {
			isAdult = true;
			if(isMale)
				sr.sprite = maleSprite; 
			else 
				sr.sprite = femaleSprite; 
		}

		// death algorithm by Armageddon. THX MAN
		if(age >= 18 && !isDead) {
			float deathChance = Random.Range(0f, 100f);
			if ((deathChance + Mathf.Min(Mathf.Exp(1 / 99 * Mathf.Log(1599/20) * age), 79.95f)) >= 99.95f) {
				isDead = true;
				if(isMale)
					sr.sprite = maleDeadSprite;
				else
					sr.sprite = femaleDeadSprite;
				gameStats.dead (isMale, this.gameObject);
				Invoke("destroy", 10);
				if(isMale)
					AudioSource.PlayClipAtPoint (deathSoundMale, this.transform.position);
				else
					AudioSource.PlayClipAtPoint (deathSoundFemale, this.transform.position);
			}
		}

		// chance to get block material
		float materialChance = Random.Range(0f, 100f);
		if ((materialChance + Mathf.Min(Mathf.Exp(1 / 99 * Mathf.Log(1599/20) * age), 79.95f)) >= 99.95f) {
			blockMaterial++;
		}
		
	}


	// Deletes the game object from the game world, and I THINK deletes the entire object itself, will have to find out.
	void destroy() {
		Destroy (this.gameObject);
	}

	// Entering a collision with ANY other game object in the world.
	void OnCollisionEnter2D(Collision2D col) {

		// check if the collision object is another bean
		if (col.gameObject.name.Contains ("Bean")) {
			colliding = true;

			// check pregnancy requirements, if all is fine, have a create a new bean.
			if (col.gameObject.GetComponent<BeanLife> ().isMale && col.gameObject.GetComponent<BeanLife> ().isAdult && !isMale && isAdult) {
				if(!col.gameObject.GetComponent<BeanLife> ().beanName.Equals (fatherName)) {
					if(GameStats.Instance.beansList.Count < GameStats.Instance.getMaxBeans ()) {
						if(curChildren < maxChildren) {
							for(int i = 0; i < maxChildren; i++) {
								Debug.Log ("Creating babby at: " + col.contacts [0].point);
								GameObject newBean = (GameObject)Instantiate (bean, col.contacts [0].point, Quaternion.identity);
								newBean.GetComponent<BeanLife>().setMother (beanName);
								newBean.GetComponent<BeanLife>().setFather (col.gameObject.GetComponent<BeanLife> ().beanName);
								givenBirth = true;
								curChildren++;
							}
						}
					}
				}
			}
		}

		// check if the collision object is a stone block material.
		if (col.gameObject.name.Contains ("StoneBlock")) {
			if(!col.gameObject.GetComponent<StoneBlock>().partOfHouse) {
				Destroy(col.gameObject);
				blockMaterial++;
			}
		}
		if (col.gameObject.name.Contains ("blue_land")) {
			isGrounded = true;
		}
	}


	// Not sure if this works like I want it to... but check for continued collision with an object.
	void OnCollisionStay2D(Collision2D col) {
		if (col.gameObject.name.Contains ("Bean")) {
			if (col.gameObject.GetComponent<BeanLife> ().isMale == isMale)
				chanceToTurn += 0.5F;

			//Debug.Log (this.gameObject.GetComponent<Collider2D>().transform.position.z);
		}
	}

	// Collision exited.
	void OnCollisionExit2D(Collision2D col) {
		if (col.gameObject.name.Contains ("Bean")) {
			colliding = false;
		}
		if (col.gameObject.name.Contains ("land")) {
			isGrounded = false;
		}
	}

	// resize the 2D collider to fit the bean's current sprite size.
	[System.Obsolete("Incorrect way of resizing sprite/collider, set the localScale vector of the sprite to the collidor.", true)]
	void resizeCollider() {
		CircleCollider2D c = this.gameObject.GetComponent<CircleCollider2D> ();
		c.radius = this.GetComponent<SpriteRenderer> ().bounds.size.x / 2;
	}

	void resizeBean() {
		float resizeScale;
		if (age < 18)
			resizeScale = 0.01f;
		else
			resizeScale = 0.03f;
		CircleCollider2D c = this.gameObject.GetComponent<CircleCollider2D> ();
		SpriteRenderer s = this.gameObject.GetComponent<SpriteRenderer> ();
		
		s.transform.localScale = new Vector3(s.transform.localScale.x + resizeScale, s.transform.localScale.y + resizeScale, s.transform.localScale.z);
		c.transform.localScale = s.transform.localScale;
	}


	// grabs a name from one of two text files, depending on bean gender, and assings it to the bean.
	// A BEAN CURRENTLY WILL BE ASSIGNED A 100% unique name, no two beans will have the same name.
	// this will prob change if last names are added.
	// The method itself is straight forward, will not explain it.
	private void generateName() {
		string[] words = null;
		if (isMale)
			words = GameStats.Instance.maleNames;
		else
			words = GameStats.Instance.femaleNames;
		bool nameFound = false;
		while (!nameFound) {
			int nameInt = Random.Range (1,300);
			string text = words[nameInt];

			bool dupe = false;
			foreach (GameObject g in GameStats.Instance.beansList)
			{
				if(g.GetComponent<BeanLife>().name.Equals (text))
					dupe = true;
			}
			if(!dupe) {
				nameFound = true;
				beanName = text;
				Debug.Log (beanName + " has been born!");
			}
		}
	}


	// Object clicked on by mouse.
	void OnMouseDown() {

		// Select and deselect bean, sends a selected object to gameStats in order to display bean debug info over bean.
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

    void playSound(int clip)
    {
        //audio.clip = audioClip[clip];
    }

	public void setMother(string s) { motherName = s; }
	public void setFather(string s) { fatherName = s; }

}

