using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundariesManager : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Rock") || other.gameObject.CompareTag("Paper") || other.gameObject.CompareTag("Scissors"))
        {
            if (GameManager.Instance.Boundary == BoundaryStyles.Destruction)
            {
                int index = 3;
                /*switch (other.gameObject.tag) // Different Fire Colors per unit
                {
                    case "Rock":
                        index = 3;
                        break;
                    case "Paper":
                        index = 4;
                        break;
                    case "Scissors":
                        index = 5;
                        break;
                    default:
                        break;
                }*/
                GameManager.Instance.NewTombstone(index, other.transform.position);
                other.gameObject.SetActive(false);
                GameManager.Instance.UpdateGameState(GameState.Decide);
            }
            else if (GameManager.Instance.Boundary == BoundaryStyles.WrapAround)
            {
                other.gameObject.GetComponent<IndividualAI>().WrapAround();
            }


        }
    }
}
