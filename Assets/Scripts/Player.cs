using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.5f;
    

    // Start is called before the first frame update
    void Start()
    {
        // Take the current position = a new position (0, 0, 0)
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);

        //if player position on y greater than 0 
        //y position = 0
        //else if player position on y less than -3.8f
        //y position = -3.8f
        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        //if player on the x > 11
        //x = -11
        //else if player on the x < -11
        //x = -11
        if (transform.position.x > 11.3)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }
}
