using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public EntityType spawningType;
    public float spawnInterval = 5f;
    float timer;
    public static PlayerNetwork Instance;
    LayerMask layerMaskPlayer;
    float detectionDistance;
    int health = 200;
    private void Start() {
        var ec = FindObjectOfType<EnemyCharacter>();
        if(ec != null) {
            detectionDistance = ec.detectionDistance;
        } else
        detectionDistance = 25f;
        layerMaskPlayer = LayerMask.GetMask("Player");
    }
    void Update() {
        if(PlayerNetwork.Instance.joinedGame() == true) {
            //Debug.Log(NetworkManager.GetComponent<NetworkManager>().playersInGame);
            if(PhotonNetwork.isMasterClient) {
                var player = Physics2D.OverlapCircle(transform.position, detectionDistance, layerMaskPlayer); //Etsi 2Dcollidereita detectionDistance-kokoiselta, ympyrän muotoiselta alueelta
                if(player != null) { // Jos löytyi pelaaja/pelaajia
                    if(timer < 0) {
                        var enemyClone = PhotonNetwork.Instantiate("NetworkEnemy", transform.position, Quaternion.identity, 0);
                        var c = enemyClone.GetComponent<Character>();
                        c.characterType = spawningType;
                        c.npc = true;
                        timer = spawnInterval;
                    }
                }
                timer -= Time.deltaTime;
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        // Check if collision is projectile
        if(collision.gameObject.CompareTag("Projectile")) {
            var projectile = collision.gameObject.GetComponent<Projectile>();
                TakeDamage(projectile.damage);
        }
    }
    void TakeDamage(int damage) {
        health -= damage;
        if(health <= 0)
            Destroy(gameObject);
    }
}