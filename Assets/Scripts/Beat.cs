using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/



    private void OnCollisionEnter(Collision collision)
    {
        // Paper beats Rock
        if (this.gameObject.CompareTag("Paper"))
        {
            if (collision.gameObject.CompareTag("Rock"))
            {
                GameManager.Instance.NewTombstone(0, collision.transform.position);
                collision.gameObject.SetActive(false);
                GameManager.Instance.UpdateGameState(GameState.Decide);
            }

        }
        // Rock beats Scissors
        if (this.gameObject.CompareTag("Rock"))
        {
           if (collision.gameObject.CompareTag("Scissors"))
            {
                GameManager.Instance.NewTombstone(2, collision.transform.position);
                collision.gameObject.SetActive(false);
                GameManager.Instance.UpdateGameState(GameState.Decide);
            }
        }
        // Scissors beats Paper
        if (this.gameObject.CompareTag("Scissors"))
        {
            if (collision.gameObject.CompareTag("Paper"))
            {
                GameManager.Instance.NewTombstone(1, collision.transform.position);
                collision.gameObject.SetActive(false);
                GameManager.Instance.UpdateGameState(GameState.Decide);
            }
        }

    }


}
