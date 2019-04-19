using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    
    enum birdBehaviors
    {
        sing,
        preen,
        ruffle,
        peck,
        hopForward,
        hopBackward,
        hopLeft,
        hopRight,
    }

    enum birdState
    {
        Dead,
        Flying,
        Waiting
    }

    private System.Random rand = new System.Random();
    public GameObject coin;
    private Queue<GameObject> terrainQueue = new Queue<GameObject>();
    private Queue<GameObject> coinQueue = new Queue<GameObject>();
    private float coinY = 20;
    private float terrainY;
    public GameObject[] terrain;
    public AudioClip bird_song1;
    public AudioClip bird_song2;
    public AudioClip flyAway1;
    public AudioClip flyAway2;

    private float coinPos=16.0f;

    public bool fleeCrows = true;

    Animator anim;

    bool paused = false;
    bool idle = true;
    bool flying = false;
    bool landing = false;
    bool perched = false;
    bool onGround = true;
    bool dead = false;
    //BoxCollider birdCollider;
    Vector3 bColCenter;
    Vector3 bColSize;
    SphereCollider solidCollider;
    float distanceToTarget = 0.0f;
    float agitationLevel = .5f;
    float originalAnimSpeed = 1.0f;
    Vector3 originalVelocity = Vector3.zero;

    //hash variables for the animation states and animation properties
    int idleAnimationHash;
    int singAnimationHash;
    int ruffleAnimationHash;
    int preenAnimationHash;
    int peckAnimationHash;
    int hopForwardAnimationHash;
    int hopBackwardAnimationHash;
    int hopLeftAnimationHash;
    int hopRightAnimationHash;
    int worriedAnimationHash;
    int landingAnimationHash;
    int flyAnimationHash;
    int hopIntHash;
    int flyingBoolHash;
    int peckBoolHash;
    int ruffleBoolHash;
    int preenBoolHash;
    int landingBoolHash;
    int singTriggerHash;
    int flyingDirectionHash;
    int dieTriggerHash;


    
    float speed = 5.0f;

    void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>();

        idleAnimationHash = Animator.StringToHash("Base Layer.Idle");
        flyAnimationHash = Animator.StringToHash("Base Layer.fly");
        hopIntHash = Animator.StringToHash("hop");
        flyingBoolHash = Animator.StringToHash("flying");

        peckBoolHash = Animator.StringToHash("peck");
        ruffleBoolHash = Animator.StringToHash("ruffle");
        preenBoolHash = Animator.StringToHash("preen");

        landingBoolHash = Animator.StringToHash("landing");
        singTriggerHash = Animator.StringToHash("sing");
        flyingDirectionHash = Animator.StringToHash("flyingDirectionX");
        dieTriggerHash = Animator.StringToHash("die");
        anim.SetFloat("IdleAgitated", agitationLevel);
        if (dead)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        terrainY = 42;
        Instantiate(terrain[0], new Vector3(0, 0, terrainY), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        FlyForward();
        Vector3 pos = GetComponent<Rigidbody>().transform.position;
        
        int terrainNo = rand.Next(0, 2);
        if (pos.z > terrainY - 29)
        {
            terrainY = terrainY + 29;
            terrainQueue.Enqueue(Instantiate(terrain[terrainNo], new Vector3(0, 0, terrainY), Quaternion.identity));
            if (terrainQueue.Count > 4)
            {
                Destroy(terrainQueue.Dequeue());

            }
        }
        int displacment;
        if (coinPos < 11)
        {
            displacment = rand.Next(0, 2);
        }
        else if (coinPos > 17)
        {
            displacment = rand.Next(-2, 0);
        }
        else
        {
            displacment = rand.Next(-2, 2);
        }
        
        coinPos += displacment;
        if (pos.z > coinY - 8)
        {
            
            GameObject coinObj = Instantiate(coin, new Vector3(coinPos, 0.7f, coinY), Quaternion.Euler(new Vector3(90, 180, 0)));
            coinObj.AddComponent<CoinScript>();
            coinQueue.Enqueue(coinObj);
            coinY += 4;
        }
        OnGroundBehavior();
        float move = Input.GetAxis("Vertical");
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            FlyRight();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            FlyLeft();
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetTrigger(Animator.StringToHash("flying"));
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            FlyUp();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            FlyDown();
        }
    }

    void OnGroundBehavior()
    {
        idle = anim.GetCurrentAnimatorStateInfo(0).nameHash == idleAnimationHash;
        if (!GetComponent<Rigidbody>().isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
        if (idle)
        {
            Debug.Log("Text: " + "OnGround");
            //the bird is in the idle animation, lets randomly choose a behavior every 3 seconds
            if (Random.value < Time.deltaTime * .33)
            {
                //bird will display a behavior
                //in the perched state the bird can only sing, preen, or ruffle
                float rand = Random.value;
                Debug.Log("Text: " + "Calling displays");
                if (rand < .3)
                {
                    DisplayBehavior(birdBehaviors.sing);
                }
                else if (rand < .5)
                {
                    DisplayBehavior(birdBehaviors.peck);
                }
                else if (rand < .6)
                {
                    DisplayBehavior(birdBehaviors.preen);
                }
                else if (!perched && rand < .7)
                {
                    DisplayBehavior(birdBehaviors.ruffle);
                }
                else if (!perched && rand < .85)
                {
                    DisplayBehavior(birdBehaviors.hopForward);
                }
                else if (!perched && rand < .9)
                {
                    DisplayBehavior(birdBehaviors.hopLeft);
                }
                else if (!perched && rand < .95)
                {
                    DisplayBehavior(birdBehaviors.hopRight);
                }
                else if (!perched && rand <= 1)
                {
                    DisplayBehavior(birdBehaviors.hopBackward);
                }
                else
                {
                    DisplayBehavior(birdBehaviors.sing);
                }
                //lets alter the agitation level of the brid so it uses a different mix of idle animation next time
                anim.SetFloat("IdleAgitated", Random.value);
            }
            //birds should fly to a new target about every 10 seconds
            if (Random.value < Time.deltaTime * .1)
            {
               // FlyAway();
            }
        }
    }

    void DisplayBehavior(birdBehaviors behavior)
    {
        idle = false;
        Debug.Log("dText: " + "Something");
        switch (behavior)
        {
            case birdBehaviors.sing:
                anim.SetTrigger(singTriggerHash);
                break;
            case birdBehaviors.ruffle:
                anim.SetTrigger(ruffleBoolHash);
                break;
            case birdBehaviors.preen:
                anim.SetTrigger(preenBoolHash);
                break;
            case birdBehaviors.peck:
                anim.SetTrigger(peckBoolHash);
                break;
            case birdBehaviors.hopForward:
                anim.SetInteger(hopIntHash, 1);
                break;
            case birdBehaviors.hopLeft:
                anim.SetInteger(hopIntHash, -2);
                break;
            case birdBehaviors.hopRight:
                anim.SetInteger(hopIntHash, 2);
                break;
            case birdBehaviors.hopBackward:
                anim.SetInteger(hopIntHash, -1);
                break;
        }
    }

    void ResetHopInt()
    {
        anim.SetInteger(hopIntHash, 0);
    }

    void ResetFlyingLandingVariables()
    {
        if (flying || landing)
        {
            flying = false;
            landing = false;
        }
    }

    void FlyUp()
    {
        Vector3 movement;
        Vector3 pos = GetComponent<Rigidbody>().transform.position;
        anim.SetBool(flyingBoolHash, true);
        anim.SetBool(landingBoolHash, false);
        anim.SetFloat(Animator.StringToHash("flyingDirectionX"), 0.0f);
        anim.SetFloat(Animator.StringToHash("flyingDirectionX"), 0.0f);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector3(moveHorizontal, 0.0f, 8.0f);
        if (pos.y > 8.0f)
        {
            movement = new Vector3(moveHorizontal, -0.5f, 0.0f);
        }
        else if (pos.y < 7.8f)
        {
            movement = new Vector3(moveHorizontal, 2.5f, 0.0f);
        }

        GetComponent<Rigidbody>().AddForce(movement * 1.0f);
        return;
    }

    void FlyDown()
    {
        Vector3 movement;
        Vector3 pos = GetComponent<Rigidbody>().transform.position;
        anim.SetBool(flyingBoolHash, true);
        anim.SetBool(landingBoolHash, false);
        anim.SetFloat(Animator.StringToHash("flyingDirectionX"), 0.0f);
        anim.SetFloat(Animator.StringToHash("flyingDirectionX"), 0.0f);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector3(moveHorizontal, 0.0f, 8.0f);
        if (pos.y > 8.0f)
        {
            movement = new Vector3(moveHorizontal, -0.5f, 0.0f);
        }
        else if (pos.y < 7.8f)
        {
            movement = new Vector3(moveHorizontal, -2.5f, 0.0f);
        }

        GetComponent<Rigidbody>().AddForce(movement * 1.0f);
        return;
    }

    void FlyForward()
    {
        Vector3 movement;
        Vector3 pos = GetComponent<Rigidbody>().transform.position;
        Vector3 vectorDirectionToTarget = new Vector3(15.54f, 3f, 20f);
        anim.SetBool(flyingBoolHash, true);
        anim.SetBool(landingBoolHash, false);
        anim.SetFloat(Animator.StringToHash("flyingDirectionX"), 0.0f);
        anim.SetFloat(Animator.StringToHash("flyingDirectionX"), 0.0f);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector3(moveHorizontal, 0.0f, 8.0f);
        if (pos.y > 7.0f)
        {
            movement = new Vector3(moveHorizontal, -2.0f, 8.0f);
        }
        GetComponent<Rigidbody>().AddForce(movement * 30.0f);
        return;
    }

    void FlyRight()
    {
        Vector3 movement;
        Vector3 pos = GetComponent<Rigidbody>().transform.position;
        Vector3 vectorDirectionToTarget = new Vector3(15.54f, 3f, 20f);
        anim.SetBool(flyingBoolHash, true);
        anim.SetBool(landingBoolHash, false);
        anim.SetFloat(Animator.StringToHash("flyingDirectionX"), 0.4f);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector3(moveHorizontal, 0.0f, 0.0f);
        GetComponent<Rigidbody>().AddForce(movement * 30.0f);
        return;
    }

    void FlyLeft()
    {
        Vector3 movement;
        Vector3 pos = GetComponent<Rigidbody>().transform.position;
        Vector3 vectorDirectionToTarget = new Vector3(15.54f, 3f, 20f);
        anim.SetBool(flyingBoolHash, true);
        anim.SetBool(landingBoolHash, false);
        anim.SetFloat(Animator.StringToHash("flyingDirectionX"), -0.4f);
        float moveHorizontal = Input.GetAxis("Horizontal");
        movement = new Vector3(moveHorizontal, 0.0f, 0.0f);
        GetComponent<Rigidbody>().AddForce(movement * 30.0f);
        return;
    }

    void OnCollisionEnter(Collision collision)
    {
        //Output the Collider's GameObject's name
        Debug.Log(collision.collider.name);
        anim.SetBool(landingBoolHash, false);
        Debug.Log("Dead is dead dead is it dead is bird");

    }

    float FindBankingAngle(Vector3 birdForward, Vector3 dirToTarget)
    {
        Vector3 cr = Vector3.Cross(birdForward, dirToTarget);
        float ang = Vector3.Dot(cr, Vector3.up);
        return ang;
    }
}
