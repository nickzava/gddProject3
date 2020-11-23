﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerGameObject; //Player Game Object
    public GameObject fireballPrefab; //Fireball Prefab
    public GameObject bulletPrefab;  //Bullet Prefab
    public GameObject canePrefab; //Cane Attack Prefab
    public GameObject windPrefab; //Windblast Attack Prefab
    private Stamina playerStamina; //Stamina Object
    public bool invincible = false; //Used for shield
    private bool isDashing = false; //Used for dash
    private const float gunshotSpeed = 15f; //Usedd for gunshot projectile speed
    private int health = 3;
	public bool paused = false;		//disables player input when paused

    // We create a vector which is modified by key presses so that holding A and W for example does not give you an increase in velocity.
    // This method only applies one force to the player while the original method I tried would apply one for each key press, leading to that behavior.
    Vector3 frameMovement = new Vector3(0, 0, 0);

    // Adjust the amount of force we apply
    [SerializeField]
    float speed;
    // Holds the player's rigidbody so we can apply forces to it
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
		playerStamina = GetComponent<Stamina>();
		rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
	{
		if (!paused)
		{
			CheckRoation();
			CheckAction();
		}
    }

    // We use fixedupdate because it is more reliable specifically for physics interactions
    private void FixedUpdate()
    {
        if(isDashing == false)
        {
            MovementCheck();
        }
        
    }

    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    //This will check if W A S or D are pressed and move in the corresponding direction
    void MovementCheck()
    {
        // Reset framemovement
        frameMovement = Vector3.zero;

        if(Input.GetKey(KeyCode.W) == true)
        {
            frameMovement += new Vector3(0, 1, 0);
            //MovePlayer(new Vector3(0, speed, 0));
        }
        if(Input.GetKey(KeyCode.A) == true)
        {
            frameMovement += new Vector3(-1, 0, 0);
            //MovePlayer(new Vector3(-speed, 0, 0));
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            frameMovement += new Vector3(0, -1, 0);
            //MovePlayer(new Vector3(0, -speed, 0));
        }
        if (Input.GetKey(KeyCode.D) == true)
        {
            frameMovement += new Vector3(1, 0, 0);
            //MovePlayer(new Vector3(speed, 0, 0));
        }

        // Now we normalize the temporary vector, to get our direction which will always be the same magnitude
        frameMovement.Normalize();

        // Actually apply the force here
        rb.AddForce(frameMovement * speed);
    }
    /// <summary>
    /// This will check the rotation the sprite should be facing
    /// </summary>
    void CheckRoation()
    {
        Vector3 mouse_pos = Input.mousePosition;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(playerGameObject.transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
        playerGameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));        
    }

    //This will check if the user has pressed an "Action Key" which includes firing a weapon, ability or some other use
    void CheckAction()
    {
        if (isDashing == false)
        {
            //SHIELD
            //Inital Press
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (playerStamina.Attack(15, false))
                {
                    invincible = true;
                    playerGameObject.GetComponent<SpriteRenderer>().color = Color.blue; //Test for invincible color

                    // Just a note from when Will is reading thru here, if we get a chance we should take GetComponent<> out of update cause it may cause performance issues
                    // Although this is labeled as a test so I'm guessing you already knew that lol
                }
            }
            //Shield Hold
            else if (Input.GetKey(KeyCode.Space))
            {
                //Invincible == true makes it so that this attack can not be the first press of shield.
                if (invincible == true && playerStamina.Attack(60f * Time.deltaTime, false))
                {
                }
                //This will stop the hold from mattering until shield is pressed again
                else
                {
                    invincible = false;
                    playerGameObject.GetComponent<SpriteRenderer>().color = Color.white; //Test for invincible color
                }
            }
            //Shield End
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                invincible = false;
                playerGameObject.GetComponent<SpriteRenderer>().color = Color.white;

            }
            //FIREBALL
            if (Input.GetKeyDown(KeyCode.Q) == true)
            {
                if (playerStamina.Attack(20, false))
                {
                    //Paramater for rotation
                    GameObject newObject = Instantiate(fireballPrefab,
                        playerGameObject.transform.position + transform.right * 1f,
                        playerGameObject.transform.rotation);
                    Fireball newFireball = newObject.GetComponent<Fireball>();
                }
            }
            //REVOLVER
            else if (Input.GetKeyDown(KeyCode.Mouse1) == true)
            {
                if (playerStamina.Attack(20, true))
                {
                    //Paramater for rotation
                    GameObject newObject = Instantiate(bulletPrefab,
                        playerGameObject.transform.position + transform.right * 1f,
                        playerGameObject.transform.rotation);                    
                    Gunshot newGunshot = newObject.GetComponent<Gunshot>();
                    newGunshot.speed = gunshotSpeed; //Changes the speed of the gunshot to the proper amount.
                }
            }
            //DASH 
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (playerStamina.Attack(20, true))
                {
                    StartCoroutine("Dash");
                }
            }
            //CANE
            else if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (playerStamina.Attack(15, true))
                {
                    //Paramater for rotation
                    GameObject newCane = Instantiate(canePrefab,
                        playerGameObject.transform.position + transform.right * 1f,
                        playerGameObject.transform.rotation);
                    //Instantiate New Hitbox
                    BasicAttack caneAttack = newCane.GetComponent<BasicAttack>();
                    caneAttack.init(playerGameObject, 1, .15f, new Vector3(0, 0, 0), .1f);
                }
            }
            //Wind Attack
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (playerStamina.Attack(15, false))
                {
                    //Paramater for rotation
                    GameObject newWind = Instantiate(windPrefab,
                        playerGameObject.transform.position + transform.right * 1f,
                        playerGameObject.transform.rotation);
                    //Instantiate new Hitbox
                    BasicAttack caneAttack = newWind.GetComponent<BasicAttack>();
                    caneAttack.init(playerGameObject, 1, 0f, new Vector3(0, 0, 0), .2f);
                }
            }
        }
    }

    //Dash Coroutine
    IEnumerator Dash()
    {
        isDashing = true; 

        float secondsElapsed = 0;

        // Add dashing force
        rb.AddForce(frameMovement * 1000);

        //Dash loop for duration
        while(secondsElapsed < .3f)
        {
            secondsElapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        yield break;
       
    }

    // Method to apply force to the player
    private void MovePlayer(Vector3 movement)
    {
        rb.AddForce(movement);
    }
}
