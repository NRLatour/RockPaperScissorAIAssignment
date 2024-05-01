using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEnemies : MonoBehaviour
{
    private IndividualAI individualAI;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.transform.parent.tag);
        individualAI = this.gameObject.transform.GetComponentInParent<IndividualAI>();
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    private void OnTriggerEnter(Collider other)
    {
        string myTag = gameObject.transform.parent.tag;
        string otherTag = other.tag;

        if (gameObject.activeSelf == false || other.CompareTag("Detection")) 
            return;

        if (other.gameObject.CompareTag("Floor"))
        {
            return;
        }
        
        if (myTag == otherTag)
        {
            if (individualAI.closestEnemy != null && other.GetComponent<IndividualAI>().closestEnemy == null)
            {
                other.GetComponent<IndividualAI>().closestEnemy = individualAI.closestEnemy;
                return;
            }
        }

        if (other != null){
                   
            if (gameObject.transform.parent.CompareTag("Rock"))
            {
                if (other.CompareTag("Paper")){
                    individualAI.StartFlee(other.gameObject.transform);
                }
            }

            if (gameObject.transform.parent.CompareTag("Paper"))
            {
                if (other.CompareTag("Scissors"))
                {
                    individualAI.StartFlee(other.gameObject.transform);
                }
            }
            if (gameObject.transform.parent.CompareTag("Scissors"))
            {
                if (other.CompareTag("Rock"))
                {
                    individualAI.StartFlee(other.gameObject.transform);
                }
            }
        }
    }
}
