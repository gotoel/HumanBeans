using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour 
{
	public GameObject explosion;		// Prefab of explosion effect.


	void Start () 
	{
		GetComponent<Rigidbody2D>().AddForce(transform.right * 200f);
	}


	void OnExplode()
	{
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

		// Instantiate the explosion where the rocket is with the random rotation.
		Instantiate(explosion, transform.position, randomRotation);
		for (int i = 0; i < 5; i++) {
			Debug.Log ("Creating babby at: " + transform.position);
			GameObject newBean = (GameObject)Instantiate (Resources.Load ("Bean_prefab"), transform.position, Quaternion.identity);
			newBean.GetComponent<BeanLife> ().setMother ("God");
			newBean.GetComponent<BeanLife> ().setFather ("God");
		}
	}
	
	void OnTriggerEnter2D (Collider2D col) 
	{
		if(col.gameObject.tag == "World")
		{
			OnExplode();
			Destroy (gameObject);
		}
	}
}
