using UnityEngine;
using System.Collections;

public class BeanCollision : MonoBehaviour {

    private BeanLife currentBean;

    void Start()
    {
        currentBean = transform.root.gameObject.GetComponent<BeanLife>();
    }

    // Entering a collision with ANY other game object in the world.
    void OnCollisionEnter2D(Collision2D col)
    {
        // check if the collision object is a stone block material.
        if (col.gameObject.CompareTag("Building"))
        {
            currentBean.colliding = true;
            if (!col.gameObject.GetComponent<StoneBlock>().partOfHouse)
            {
                Destroy(col.gameObject);
                currentBean.blockMaterial++;
            }
        }
        if (col.gameObject.CompareTag("World"))
        {
            currentBean.isGrounded = true;
        }
    }

    // Not sure if this works like I want it to... but check for continued collision with an object.
    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Bean"))
        {
            if (col.gameObject.GetComponent<BeanLife>().isMale == currentBean.isMale)
                currentBean.chanceToTurn += 0.5F;
        }
    }

    // Collision exited.
    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Building"))
        {
            currentBean.colliding = false;
        }
        if (col.gameObject.CompareTag("World"))
        {
            currentBean.isGrounded = false;
        }
    }
}
