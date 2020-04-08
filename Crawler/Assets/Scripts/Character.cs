using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, Enemy0, Enemy1, Enemy2, Enemy3 }
[RequireComponent(typeof(PhotonView))]
public class Character : Photon.MonoBehaviour {

    public EntityType characterType;

    // Attributes of characters (No need for public variables after testing)
    public bool npc;
    public bool ranged;
    public float projectileSpeed;
    public int projectilesPerAttack;
    public int weaponLevel;
    public int attackRange;
    public int damage;
    public float attackInterval;
    public float attackTimer;
    public float speed;

    [SerializeField]
    public int health;

    public GameObject projectileSpawn;
    public GameObject projectilePrefab;

    public Vector2 movement;


    [PunRPC]
    public void Destroy() {
        if(gameObject != null) {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    private void Update() {
        if(PhotonNetwork.isMasterClient) {
            if(health < 0) { // What happens here?
                photonView.TransferOwnership(1);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }




    #region Set attributes
    public void SetCharacterAttributes() {
        if(characterType == EntityType.Hero0) {
            ranged = true;
            damage = 5;
            attackRange = 20;
            weaponLevel = 0;
            projectileSpeed = 15f;
            projectilesPerAttack = 1;
            attackInterval = .1f;
            speed = 5f;
            health = 150;
        }
        if(characterType == EntityType.Hero1) {
            ranged = true;
            damage = 20;
            attackRange = 15;
            projectileSpeed = 5f;
            projectilesPerAttack = 1;
            attackInterval = 0.4f;
            speed = 4f;
            health = 200;
        }
        if(characterType == EntityType.Hero2) {
            ranged = false;
            damage = 50;
            attackRange = 3;
            projectileSpeed = 0f;
            attackInterval = 0.5f;
            speed = 4f;
            health = 250;
        }
        if(characterType == EntityType.Hero3) {
            ranged = false;
            damage = 100;
            attackRange = 5;
            projectileSpeed = 0f;
            attackInterval = 1f;
            speed = 3.5f;
            health = 300;
        }
        if(characterType == EntityType.Enemy0) {
            npc = true;
            ranged = true;
            damage = 1;
            attackRange = 3;
            projectileSpeed = 0;
            projectilesPerAttack = 0;
            attackInterval = 5f;
            speed = 3.5f;
            health = 20;
        }
        if(characterType == EntityType.Enemy1) {
            npc = true;
            ranged = true;
            damage = 1;
            attackRange = 10;
            projectileSpeed = 5f;
            projectilesPerAttack = 1;
            attackInterval = 2.5f;
            speed = 3f;
            health = 60;
        }
        if(characterType == EntityType.Enemy2) {
            npc = true;
            ranged = false;
            damage = 15;
            attackRange = 3;
            projectileSpeed = 0f;
            projectilesPerAttack = 1;
            attackInterval = 1f;
            speed = 2.5f;
            health = 250;
        }
        if(characterType == EntityType.Enemy3) {
            npc = true;
            ranged = false;
            damage = 10;
            attackRange = 5;
            projectileSpeed = 0f;
            attackInterval = 1f;
            speed = 2f;
            health = 300;
        }
    }

    #endregion
}