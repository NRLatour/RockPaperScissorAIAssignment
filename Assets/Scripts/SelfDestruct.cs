using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float Lifespan = 3f;

    private void FixedUpdate()
    {
        Lifespan -= Time.deltaTime;

        if (Lifespan <= 0)
        {
            Destroy(gameObject);
        }
    }
}
