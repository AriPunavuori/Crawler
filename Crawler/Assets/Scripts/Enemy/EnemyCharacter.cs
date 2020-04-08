using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character {

    Rigidbody2D rigidBody;
    public LayerMask layerMaskPlayer;
    public LayerMask layerMaskObstacles;
    public GameObject player;
    GameObject rotator;

    Vector3 target;
    bool seen;
    float proximityDistance = 1f;
    public float detectionDistance = 25f;

    void Start() {
        rotator = transform.Find("Rotator").gameObject;
        layerMaskPlayer = LayerMask.GetMask("Player");
        layerMaskObstacles = LayerMask.GetMask("Obstacles");
        rigidBody = GetComponent<Rigidbody2D>();
        SetCharacterAttributes();
        EnemyManager.Instance.AddEnemyStats(this);
        EnemyManager.Instance.ModifyHealth(this, health);
        photonView.TransferOwnership(1);
    }

    private void FixedUpdate() {
        rigidBody.velocity = Vector2.zero;
        if(player == null) {
            SearchForPlayers(); // Search for next player
        } else {
            if(DistToPlayer() < detectionDistance) {
                if(TargetSeen()) { // Function updates also target
                    if(DistToPlayer() > attackRange)
                        Move(speed); // Moves close enough to attact
                    else {
                        // Slow down when getting closer
                        var speedFactor = (DistToPlayer() - proximityDistance) / (attackRange - proximityDistance);
                        Move(speed * speedFactor);
                    }
                    if(DistToPlayer() < attackRange)
                        StartAttack();
                } else {
                    Move(speed);    // If !TargetSeen(), target has been set to hit.point (Happens only once before seen again)
                }                   // Goes to nearest obstacle on the way towards player
            } else {
                player = null; // If player out of detectionRange
            }
        }
    }
    public void TakeDamage(int dmg) {

        EnemyManager.Instance.ModifyHealth(this, -dmg);


    }

    void StartAttack() {
        if (PhotonNetwork.isMasterClient)
        {
            if (attackTimer >= attackInterval) { // Odota attackInterval -pituinen aika
            rotator.transform.right = target - rotator.transform.position; // Turn rotator with projectileSpawn
            Attack();
            attackTimer = 0;
        } else {
            attackTimer += Time.deltaTime;
        }

        }
    }

    void Move(float s) {
        if(Vector2.Distance(transform.position, target) > proximityDistance) { // Moves close towards target until in proximityDistance
            float MoveDirX = target.x - transform.position.x;
            float MoveDirY = target.y - transform.position.y;
            rigidBody.velocity = new Vector2(MoveDirX, MoveDirY).normalized * s;
        }
    }

    float DistToPlayer() {
        return Vector2.Distance(transform.position, player.transform.position);
    }

    bool TargetSeen() {
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

            int playerID = player.GetComponent<PhotonView>().ownerId;
            if (photonView.ownerId != playerID)
            {
                photonView.TransferOwnership(playerID);
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.isWriting) {
            stream.SendNext(health);
        } else {
            this.health = (int)stream.ReceiveNext();
        }
    }

    // _____________________________Sort these out BELOW!!!____________________________________
    private void OnCollisionEnter2D(Collision2D collision) {
        // Check if collision is projectile and type of shooter
        if(collision.gameObject.CompareTag("Projectile")) {
            var projectile = collision.gameObject.GetComponent<Projectile>();
            if(npc != projectile.shotByNPC) {
                TakeDamage(projectile.damage);
                print("Damage goes to " + this.gameObject);
            }

            //photonView.RPC("TakeDamage", PhotonTargets.Others, projectile.damage);
        }
    }

    public void Attack() {
        if(ranged) {
            //Shoot(projectilesPerAttack);
            photonView.RPC("Shoot", PhotonTargets.All, projectilesPerAttack, (projectileSpawn.transform.position - transform.position).normalized, projectileSpawn.transform.rotation);

        } else {
            Melee();
            photonView.RPC("Melee", PhotonTargets.Others);
        }
        attackTimer = attackInterval;
    }



    [PunRPC]
    public void Shoot(int amount, Vector3 dir, Quaternion rot) {


        if(amount % 2 == 0) {
            float startOffset = 0.25f * (amount - 1);
            for(int i = 0; i < amount; i++) {
                GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, rot);
                projectileClone.transform.parent = projectileSpawn.transform;
                projectileClone.transform.localPosition = new Vector3(0f, startOffset - (i * 0.50f), 0f);
                projectileClone.transform.parent = null;
                Projectile projectile = projectileClone.GetComponent<Projectile>();
                projectile.LaunchProjectile(damage, attackRange, projectileSpeed, npc, dir);
            }
        } else {
            float leftOffset = 0.50f;
            float rightOffset = 0.50f;
            for(int i = 0; i < amount; i++) {
                if(i == 0) {
                    GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, rot);
                    projectileClone.transform.parent = projectileSpawn.transform;
                    projectileClone.transform.localPosition = new Vector3(0f, 0f, 0f);
                    projectileClone.transform.parent = null;
                    Projectile projectile = projectileClone.GetComponent<Projectile>();
                    projectile.LaunchProjectile(damage, attackRange, projectileSpeed, npc, dir);
                } else if(i % 2 == 0) {
                    GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, rot);
                    projectileClone.transform.parent = projectileSpawn.transform;
                    projectileClone.transform.localPosition = new Vector3(0f, leftOffset, 0f);
                    projectileClone.transform.parent = null;
                    Projectile projectile = projectileClone.GetComponent<Projectile>();
                    projectile.LaunchProjectile(damage, attackRange, projectileSpeed, npc, dir);
                    leftOffset += 0.50f;
                } else {
                    GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, rot);
                    projectileClone.transform.parent = projectileSpawn.transform;
                    projectileClone.transform.localPosition = new Vector3(0f, -rightOffset, 0f);
                    projectileClone.transform.parent = null;
                    Projectile projectile = projectileClone.GetComponent<Projectile>();
                    projectile.LaunchProjectile(damage, attackRange, projectileSpeed, npc, dir);
                    rightOffset += 0.50f;
                }
            }
        }
    }



    [PunRPC]
    public void Melee() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach(var hit in hits) {
            var pc = hit.gameObject.GetComponent<PlayerCharacter>();                
            if(pc != null) {
                Debug.Log(hit.gameObject);
                pc.TakeDamage(damage, pc);
            }
        }
    }
}

