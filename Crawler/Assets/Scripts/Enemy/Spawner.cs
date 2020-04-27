using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Spawner : Photon.PunBehaviour, IDamageable<int>, IPunObservable {
    public TextMeshProUGUI healthText;
    public EntityType spawningType;
    Vector3 spawnPoint;
    string[] enemyType = new string[] { "NetworkEnemy0", "NetworkEnemy1", "NetworkEnemy2", "NetworkEnemy3" };
    public float spawnInterval = 3f;
    public int maxEnemiesInArea = 16;
    float timer;
    public static PlayerNetwork Instance;
    LayerMask layerMaskPlayer;
    LayerMask layerMaskEnemy;
    float detectionDistance = 15;
    public int health = 200;
    private void Start() {
        spawnPoint = gameObject.transform.Find("SpawnPoint").transform.position;
        layerMaskPlayer = LayerMask.GetMask("Player");
        layerMaskEnemy = LayerMask.GetMask("Enemy");
        healthText.text = "" + health;
    }
    void Update() {
        if(PlayerNetwork.Instance.joinedGame() == true) {
            if(PhotonNetwork.isMasterClient) {
                if(timer < 0) {
                    timer = spawnInterval;
                    var enemies = Physics2D.OverlapCircleAll(transform.position, detectionDistance, layerMaskEnemy);
                    if(enemies.Length < maxEnemiesInArea) {
                        var player = Physics2D.OverlapCircle(transform.position, detectionDistance, layerMaskPlayer); //Etsi 2Dcollidereita detectionDistance-kokoiselta, ympyrän muotoiselta alueelta
                        if(player != null) { // Jos löytyi pelaaja/pelaajia
                            LeanTween.scale(gameObject, Vector3.one * 1.2f, spawnInterval * 0.5f).setEaseInQuart();
                            Invoke("Spit", spawnInterval * 0.5f);
                        }
                    }
                }
                timer -= Time.deltaTime;
            }
        }
    }

    void Spit() {
        LeanTween.scale(gameObject, Vector3.one, spawnInterval * 0.35f).setEaseOutElastic();
        SpawnNow();
    }

    void SpawnNow() {
        var enemy = PhotonNetwork.Instantiate(enemyType[(int)spawningType - 4], spawnPoint, Quaternion.identity, 0);
    }

    [PunRPC]
    public void TakeDamage(int damage, Vector3 v) {

        if(PhotonNetwork.isMasterClient) {
            health -= damage;
            healthText.text = "" + health;
            if(health <= 0)

                PhotonNetwork.Destroy(gameObject);
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
}