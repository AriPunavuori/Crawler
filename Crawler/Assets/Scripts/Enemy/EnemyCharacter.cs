﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCharacter : Character, IDamageable<int> {

    Rigidbody2D rigidBody;
    public LayerMask layerMaskPlayer;
    public LayerMask layerMaskObstacles;
    public GameObject player;
    GameObject rotator;
    GameObject meleeIndicator;
    int prevHealth;
    public TextMeshProUGUI healthText;
    PlayerCharacter pc;
    public bool stunned;
    public bool flying;
    float flyDeactTime;
    float stunDeactTime;
    

    Vector3 target;
    bool seen;
    float proximityDistance = 1f;
    public float detectionDistance = 25f;
    public Animator animator;

    bool flipped;
    float spriteFlipCoolDown;
    void Start() {
        rotator = transform.Find("Rotator").gameObject;
        meleeIndicator = rotator.transform.Find("MeleeIndicator").gameObject;
        layerMaskPlayer = LayerMask.GetMask("Player");
        layerMaskObstacles = LayerMask.GetMask("Obstacles");
        rigidBody = GetComponent<Rigidbody2D>();
        SetCharacterAttributes();
        meleeIndicator.transform.localScale = new Vector3(attackRange, .1f, 1);
        meleeIndicator.transform.localPosition = new Vector3(attackRange / 2, 0, 0);
        meleeIndicator.SetActive(false);
        //EnemyManager.Instance.AddEnemyStats(this);
        //EnemyManager.Instance.ModifyHealth(this, health);
        healthText.text = "" + health;
        //photonView.TransferOwnership(1);
        flipped = false;
        stunned = false;
        flying = false;
        spriteFlipCoolDown = 0; // Cooldown for sprite flipping to avoid too frequent flips in some instances. Need better solution
    }
    //private void Update() {
    //    healthText.text = "" + health;
    //}
    private void FixedUpdate() {

            if (!flying)
            {
                if (!stunned)
                {
                    rigidBody.velocity = Vector2.zero;
                    if (player == null || !pc.alive)
                    {
                        SearchForPlayers(); // Search for next player
                    }
                    else
                    {
                        if (DistToPlayer() < detectionDistance)
                        {
                            if (PlayerSeen())
                            { // Function updates also target
                                if (DistToPlayer() > attackRange)
                                    Move(speed); // Moves close enough to attact
                                else
                                {
                                    // Slow down when getting closer
                                    var speedFactor = (DistToPlayer() - proximityDistance) / (attackRange - proximityDistance);
                                    Move(speed * speedFactor);
                                }
                                if (DistToPlayer() < attackRange)
                                    StartAttack();
                            }
                            else
                            {
                                Move(speed);    // If !TargetSeen(), target has been set to hit.point (Happens only once before seen again)
                            }                   // Goes to nearest obstacle on the way towards player
                        }
                        else
                        {
                            player = null; // If player out of detectionRange
                        }
                    }
                }

            
        }
        
        
    }

    public void Fly(float flyTime)
    {
        flying = true;
        flyDeactTime = Time.time + flyTime;
        rigidBody.drag = 5f;
    }

    public void Stun(float stunTime)
    {
        stunned = true;
        stunDeactTime = Time.time + stunTime;
    }

    private void Update()
    {
        
            if (Time.time >= flyDeactTime && flying)
            {
                flying = false;
                if (!stunned)
                {
                    Stun(2f);
                }
            }

            if (Time.time >= stunDeactTime && stunned)
            {
                rigidBody.drag = 0f;
                stunned = false;
            }
        
        

        // Animation
        #region Animation handling

        // Need a better solution than rigidbody velocity. For example enemies start facing the wrong direction for a brief period if they get shot
        // Added spriteFlipCooldown for a temporary "fix"
        movement.x = rigidBody.velocity.normalized.x;

        if(movement.x > 0 && spriteFlipCoolDown == 0) {
            if(!flipped) {
                // Change this value to delay sprite flips
                spriteFlipCoolDown = 0.4f;
            }
            flipped = true;
        } else if(movement.x < 0 && spriteFlipCoolDown == 0) {
            if(flipped) {
                // Change this value to delay sprite flips
                spriteFlipCoolDown = 0.4f;
            }
            flipped = false;
        }

        if(animator.enabled) {
            animator.SetFloat("Horizontal", movement.x);
        }


        if(flipped) {
            GetComponent<SpriteRenderer>().flipX = true;
        } else {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        if((spriteFlipCoolDown - Time.deltaTime) < 0) {
            spriteFlipCoolDown = 0;
        } else {
            spriteFlipCoolDown -= Time.deltaTime;
        }
        #endregion

    }

    void Move(float s) {
        if(Vector2.Distance(transform.position, target) > proximityDistance) { // Moves close towards target until in proximityDistance
            float MoveDirX = target.x - transform.position.x;
            float MoveDirY = target.y - transform.position.y;
            if(PhotonNetwork.isMasterClient)
            {
                rigidBody.velocity = new Vector2(MoveDirX, MoveDirY).normalized * s;
            }
        } else {
            if(!PlayerSeen())
                player = null;
        }
    }

    float DistToPlayer() {
        return Vector2.Distance(transform.position, player.transform.position);
    }

    bool PlayerSeen() {
        Vector2 dirVector = player.transform.position - transform.position; // Pelaajan suuntaan vihollisesta
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirVector, DistToPlayer(), layerMaskObstacles); // Castataan ray pelaajaan päin
        if(hit) {
            if(seen) { // Boolean for setting only last hit.point
                target = hit.point;
                seen = false;
            }
        } else {
            target = player.transform.position;
            seen = true;
        }
        return !hit;
    }

    void SearchForPlayers() {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, detectionDistance, layerMaskPlayer); //Etsi 2Dcollidereita detectionDistance-kokoiselta, ympyrän muotoiselta alueelta
        if(players.Length > 0) { // Jos löytyi pelaaja/pelaajia
            GameObject closest = players[0].gameObject;
            float shortestDist = Mathf.Infinity;
            for(int i = 0; i < players.Length; i++) {
                float dist = Vector2.Distance(transform.position, players[i].gameObject.transform.position);
                if(dist < shortestDist) {
                    player = players[i].gameObject;
                    shortestDist = dist;
                }
            }
            pc = player.GetComponent<PlayerCharacter>();
            //int playerID = player.GetComponent<PhotonView>().ownerId;
            //if(photonView.ownerId != playerID) {
            //    photonView.TransferOwnership(playerID);
            //}
        }
    }
    [PunRPC]
    public void TakeDamage(int damage, Vector3 v) {

        if(PhotonNetwork.isMasterClient) {
            health -= damage;
            healthText.text = "" + health;
            if(health <= 0)
                if(gameObject != null) {
                    PhotonNetwork.Destroy(gameObject);
                }
        } else {
            photonView.RPC("TakeDamage", PhotonTargets.MasterClient, damage, v);

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.isWriting) {
            stream.SendNext(health);
        } else if(stream.isReading) {
            this.health = (int)stream.ReceiveNext();
            healthText.text = "" + health;
        }
    }

    void StartAttack() {
        if(PhotonNetwork.isMasterClient) {
            if(attackTimer >= attackInterval) { // Odota attackInterval -pituinen aika
                Attack();
                attackTimer = 0;
            } else {
                attackTimer += Time.deltaTime;
            }
        }
    }

    public void Attack() {
        if(ranged) {
            photonView.RPC("Shoot", PhotonTargets.AllViaServer);
        } else {
            photonView.RPC("Melee", PhotonTargets.AllViaServer);
        }
        attackTimer = attackInterval;
    }

    [PunRPC]
    public void Shoot() {
        rotator.transform.right = target - rotator.transform.position; // Turn rotator
        Vector2 dir = (projectileSpawn.transform.position - transform.position).normalized;
        GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, rotator.transform.rotation);
        projectileClone.transform.parent = projectileSpawn.transform;
        projectileClone.transform.localPosition = new Vector3(0f, 0f, 0f);
        projectileClone.transform.parent = null;
        Projectile projectile = projectileClone.GetComponent<Projectile>();
        projectile.LaunchProjectile(damage, attackRange, projectileSpeed, dir, true);
    }

    [PunRPC]
    public void Melee() {
        rotator.transform.right = target - rotator.transform.position; // Turn rotator
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, layerMaskPlayer);
        foreach(var hit in hits) {
            IDamageable<int> iDamageable = hit.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
            if(iDamageable != null) {
                Vector3 recoilVector = new Vector3(hit.gameObject.transform.position.x - transform.position.x, hit.gameObject.transform.position.y - transform.position.y, 0f).normalized;
                iDamageable.TakeDamage(damage, recoilVector);
            }
        }
        // Play animation
        meleeIndicator.SetActive(true);
        StartCoroutine(RotateMe(Vector3.forward * 85, attackInterval * .3f));
    }
    IEnumerator RotateMe(Vector3 byAngles, float inTime) {
        //print("Melee animation");
        var fromAngle = Quaternion.Euler(rotator.transform.eulerAngles - byAngles);
        var toAngle = Quaternion.Euler(rotator.transform.eulerAngles + byAngles);
        for(var t = 0f; t < 1; t += Time.deltaTime / inTime) {
            rotator.transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            if(t >= .9f)
                meleeIndicator.SetActive(false);
            yield return null;
        }
    }
}

