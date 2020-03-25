using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Character {

    Rigidbody2D rb2D;
    bool potion;
    bool dashing = false;
    float dashCooldown = 5.0f;
    float dashTime = 0.15f;
    float dashTimer = 0.1f;
    float dashFactor = 4.0f;
    Vector2 dashVector;
    Vector2 lastDir;
    public Animator animator;

    void Start() {
        
        rb2D = GetComponent<Rigidbody2D>();
        dashCooldown = 0.0f;

        if(characterType == EntityType.Hero0) {
            ranged = true;
            damage = 50;
            attackRange = 20;
            projectileSpeed = 20f;
            attackAngle = 90;
            attackInterval = .2f;
            speed = 10;
            health = 100;

        }
        if(characterType == EntityType.Hero1) {
            ranged = false;
            damage = 50;
            attackRange = 3;
            projectileSpeed = 0f;
            attackAngle = 90;
            attackInterval = 0.5f;
            speed = 10;
            health = 70;
        }
    }

    void Update() {
        if(photonView.isMine) {
            attackTimer -= Time.deltaTime;
            // Health potion input
            if(Input.GetKeyDown(KeyCode.H)) {
                UsePotion();
            }
            // Attack input
            if(attackTimer < 0 && Input.GetKeyDown(KeyCode.Mouse0)) {
                if(ranged) {
                    Shoot();
                    photonView.RPC("Shoot", PhotonTargets.Others);
                } else {
                    Melee();
                    photonView.RPC("Melee", PhotonTargets.Others);
                }
                attackTimer = attackInterval;
            }
            // Movement input
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            
            if(movement.x != 0 || movement.y != 0)
            {
                lastDir = new Vector2(movement.x, movement.y);
            }


            // Setting the correct animation/stance depending on the current mouse position and if moving or not
            Vector3 mousePos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 mouseVector = new Vector2(Input.mousePosition.x - mousePos.x, Input.mousePosition.y - mousePos.y);
            
            animator.SetFloat("Horizontal", mouseVector.x);
            animator.SetFloat("Vertical", mouseVector.y);
            animator.SetFloat("Magnitude", movement.magnitude);



            if (Input.GetKeyDown(KeyCode.Space) && dashCooldown <= 0)
            {
                Debug.Log("Dashing");
                dashing = true;
                dashCooldown = 5.0f;

                // Initial dash direction from mouse position
                //Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
                //dashVector = new Vector2(Input.mousePosition.x - position.x, Input.mousePosition.y - position.y);

                // Dashing to the last movement direction from keyboard inputs
                dashVector = lastDir;
            }

            if (dashing)
            {
                dashTimer -= Time.deltaTime;


                // Updating dashing direction mid dash with mouse position.
                //Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
                //dashVector = new Vector2(Input.mousePosition.x - position.x, Input.mousePosition.y - position.y);

                // Updating dashing direction mid dash with keyboard inputs. Comment to have static direction
                //dashVector = lastDir;

            }

            if (dashTimer <= 0)
            {
                dashing = false;
                dashTimer = dashTime;
            }

            if (!dashing && dashCooldown - Time.deltaTime > 0)
            {
                dashCooldown -= Time.deltaTime;
            }
            else if(!dashing && dashCooldown - Time.deltaTime < 0)
            {
                dashCooldown = 0f;
            }
        }
        
        if (health <= 0)
        {
            Destroy(gameObject); // Kuole
        }

    }

    private void FixedUpdate() {
        // Move the PlayerCharacter of the correct player
        if(photonView.isMine) {
            if(rb2D != null)
                if(dashing)
                {
                    //rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed * dashFactor;
                    rb2D.velocity = dashVector.normalized * speed * dashFactor;
                }
                else
                {
                    rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed;
                }
                //Debug.Log(rb2D.velocity.magnitude);
        }
    }

    public void UsePotion() {
        if(potion) {
            health += 100;
            potion = false;
        }
    }

    public void GetPotion() {
        potion = true;
    }

    public void GetSpeed() {
        speed += 10;
    }
}
