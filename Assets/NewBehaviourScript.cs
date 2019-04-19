using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public GameObject player;       //Public variable to store a reference to the player game object


    private Vector3 offset = new Vector3(16f,1.5f,1.5f); //Private variable to store the offset distance between the player and camera

    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = player.transform.position + offset;
    }
}


//    float speed = 5.0f;
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//        if (Input.GetKey(KeyCode.RightArrow))
//        {
//            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
//        }
//        if (Input.GetKey(KeyCode.LeftArrow))
//        {
//            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
//        }
//        if (Input.GetKey(KeyCode.S))
//        {
//            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
//        }
//        if (Input.GetKey(KeyCode.W))
//        {
//            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
//        }
//        if (Input.GetKey(KeyCode.W))
//        {
//            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
//        }
//        if (Input.GetKey(KeyCode.UpArrow))
//        {
//            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
//        }
//        if (Input.GetKey(KeyCode.DownArrow))
//        {
//            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
//        }
//    }
//}
