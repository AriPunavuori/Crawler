using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character, IDamageable<int> {

    Rigidbody2D rigidBody;
    public LayerMask layerMaskPlayer;
    public LayerMask layerMaskObstacles;
    public GameObject player;
    GameObject rotator;
    GameObject meleeIndicator;

    Vector3 target;
    bool seen;
    float proximityDistance = 1f;
    public float detectionDistance = 25f;

    void Start() {
        rotator = transform.Find("Rotator").gameObject;
        meleeIndicator = rotator.transform.Find("MeleeIndicator").gameObject;

        layerMaskPlayer = LayerMask.GetMask("Player");
        layerMaskObstacles = LayerMask.GetMask("Obstacles");
        rigidBody = GetComponent<Rigidbody2D>();
        SetCharacterAttributes();
        meleeIndicator.transform.localScale = new Vector3(attackRange, 1, 1);
        meleeIndicator.transform.localPosition = new Vector3(attackRange / 2, 0, 0);
        meleeIndicator.SetActive(false);
        EnemyManager.Instance.AddEnemyStats(this);
        EnemyManager.Instance.ModifyHealth(this, health);
        //photonView.TransferOwnership(1);
    }

    private void FixedUpdate() {
        rigidBody.velocity = Vector2.zero;
        if(player == null) {
            SearchForPlayers(); // Search for next player
        } else {
            if(DistToPlayer() < detectionDistance) {
                if(PlayerSeen()) { // Function updates also target
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
    public void TakeDamage(int damage) {
        EnemyManager.Instance.ModifyHealth(this, -damage);
    }

    void StartAttack() {
        if(PhotonNetwork.isMasterClient) {
            if(attackTimer >= attackInterval) { // Odota attackInterval -pituinen aika
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

            //int playerID = player.GetComponent<PhotonView>().ownerId;
            //if(photonView.ownerId != playerID) {
            //    photonView.TransferOwnership(playerID);
            //}
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.isWriting) {
            stream.SendNext(health);
        } else {
            this.health = (int)stream.ReceiveNext();
        }
    }

    public void Attack() {
        if(ranged) {
            Shoot((projectileSpawn.transform.position - transform.position).normalized, projectileSpawn.transform.rotation, true);
            photonView.RPC("Shoot", PhotonTargets.Others, (projectileSpawn.transform.position - transform.position).normalized, projectileSpawn.transform.rotation, false);

        } else {
            //rotator.transform.right = Vector3.right; // Turn rotator
            Melee(true);
            photonView.RPC("Melee", PhotonTargets.Others, false);
        }
        attackTimer = attackInterval;
    }

    [PunRPC]
    public void Shoot(Vector3 dir, Quaternion rot, bool owner) {
        GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, rot);
        projectileClone.transform.parent = projectileSpawn.transform;
        projectileClone.transform.localPosition = new Vector3(0f, 0f, 0f);
        projectileClone.transform.parent = null;
        Projectile projectile = projectileClone.GetComponent<Projectile>();
        projectile.LaunchProjectile(damage, attackRange, projectileSpeed, npc, dir, owner);
    }

    [PunRPC]
    public void Melee(bool owner) {
        if(owner) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, layerMaskPlayer);
            foreach(var hit in hits) {
                IDamageable<int> iDamageable = hit.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
                if(iDamageable != null) {
                    iDamageable.TakeDamage(damage);
                }
            }
        }
        // Play animation
        meleeIndicator.SetActive(true);
        StartCoroutine(RotateMe(Vector3.forward * 85, attackInterval*.9f));
    }
    IEnumerator RotateMe(Vector3 byAngles, float inTime) {
        print("Melee animation");
        var fromAngle = Quaternion.Euler(rotator.transform.eulerAngles - byAngles);
        var toAngle = Quaternion.Euler(rotator.transform.eulerAngles + byAngles);
        for(var t = 0f; t < 1; t += Time.deltaTime / inTime) {
            rotator.transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            if(t>=.975f)
                meleeIndicator.SetActive(false);
            yield return null;
        }
    }
}

