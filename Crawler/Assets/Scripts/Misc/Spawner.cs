using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour, IDamageable<int> {
    public EntityType spawningType;
    Vector3 spawnPoint;
    string[] enemyType = new string[] { "NetworkEnemy0", "NetworkEnemy1", "NetworkEnemy2", "NetworkEnemy3" };
    public float spawnInterval = 5f;
    float timer;
    public static PlayerNetwork Instance;
    LayerMask layerMaskPlayer;
    float detectionDistance;
    int health = 200;
    private void Start() {
        spawnPoint = gameObject.transform.Find("SpawnPoint").transform.position;
        var ec = FindObjectOfType<EnemyCharacter>();
        if(ec != null) {
            detectionDistance = ec.detectionDistance;
        } else
            detectionDistance = 25f;
        layerMaskPlayer = LayerMask.GetMask("Player");
    }
    void Update() {
        if(PlayerNetwork.Instance.joinedGame() == true) {
            if(PhotonNetwork.isMasterClient) {
                var player = Physics2D.OverlapCircle(transform.position, detectionDistance, layerMaskPlayer); //Etsi 2Dcollidereita detectionDistance-kokoiselta, ympyrän muotoiselta alueelta
                if(player != null) { // Jos löytyi pelaaja/pelaajia
                    if(timer < 0) {
                        var enemy = PhotonNetwork.Instantiate(enemyType[(int)spawningType - 4], spawnPoint, Quaternion.identity, 0);
                        timer = spawnInterval;
                    }
                }
                timer -= Time.deltaTime;
            }
        }
    }

    public void TakeDamage(int damage) {
        health -= damage;
        if(health <= 0)
            Destroy(gameObject);
    }
}