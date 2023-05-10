using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOOB : MonoBehaviour
{
    // Start is called before the first frame update
    private float topBound = 1000;
    private float bottomBound = -1000;
    private float maxLife = 6;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        maxLife -= Time.deltaTime;
        if (transform.position.z > topBound || transform.position.z < bottomBound || transform.position.x > topBound || transform.position.x < bottomBound || maxLife <= 0)
        {
            Destroy(gameObject);
        }
    }
}
