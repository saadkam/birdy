using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = new Vector3(0, 2.0f, 0.0f);
        transform.Rotate(Vector3.forward * 3);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Output the Collider's GameObject's name
        GameManager.score++;

        Debug.Log(collision.collider.name);
        Destroy(gameObject);
        Debug.Log(GameManager.score);
    }
}
