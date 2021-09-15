using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoubleSize()
    {
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}
