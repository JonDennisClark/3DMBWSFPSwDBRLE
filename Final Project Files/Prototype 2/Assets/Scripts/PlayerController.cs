using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    float playerSpeed = 15.0f;
    public GameObject projectilePrefab;
    float fireInterval = 0.1f;
    float fireTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        if (transform.position.x < -12)
        {
            transform.position = new Vector3(-12, transform.position.y, transform.position.z);
        }

        if (transform.position.x > 12)
        {
            transform.position = new Vector3(12, transform.position.y, transform.position.z);
        }
        transform.Translate(Vector3.right * Time.deltaTime * horizontalInput * playerSpeed);

        //if (Input.GetMouseButtonDown(0))
        // {
        //   Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);
        //}

        if (Input.GetMouseButton(0) &&  fireTime <= 0)
        {
            Instantiate(projectilePrefab, transform.position, projectilePrefab.transform.rotation);
            fireTime = fireInterval;
        }

        if (fireTime > 0)
        {
            fireTime -= Time.deltaTime;
        }
    }
}
