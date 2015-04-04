using UnityEngine;
using System.Collections;

public class BeanReproduction : MonoBehaviour {

    private BeanLife currentBean;

    void Start()
    {
        currentBean = transform.root.gameObject.GetComponent<BeanLife>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Bean"))
        {
            BeanLife otherbean = col.transform.root.gameObject.GetComponent<BeanLife>();
            if (otherbean.isMale && otherbean.isAdult && !currentBean.isMale && currentBean.isAdult)
            {
                if (otherbean.beanName != currentBean.fatherName)
                {
                    if (GameStats.Instance.beansList.Count < GameStats.Instance.getMaxBeans())
                    {
                        if (currentBean.curChildren < currentBean.maxChildren)
                        {
                            for (int i = 0; i < currentBean.maxChildren; i++)
                            {
                                Debug.Log("Creating babby at: " + col.transform.position);
                                GameObject newBean = (GameObject)Instantiate(Resources.Load("Bean_prefab"), col.transform.position, Quaternion.identity);
                                newBean.GetComponent<BeanLife>().setMother(currentBean.beanName);
                                newBean.GetComponent<BeanLife>().setFather(otherbean.beanName);
                                currentBean.givenBirth = true;
                                currentBean.curChildren++;
                            }
                        }
                    }
                }
            }
        }
    }
}
