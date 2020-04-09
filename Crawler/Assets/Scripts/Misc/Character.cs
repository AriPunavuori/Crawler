using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, Enemy0, Enemy1, Enemy2, Enemy3 }
[RequireComponent(typeof(PhotonView))]
public class Character : Photon.MonoBehaviour {

    public EntityType characterType;

    public bool[] npcs = new bool[] { false, false, false, false, true, true, true, true }; // Player or enemy
    public bool[] rangeds = new bool[] { true, true, false, false, true, true, false, false }; // Ranger or melee
    public float[] projectileSpeeds = new float[] { 10, 10, 10, 10, 10, 10, 10, 10 }; // Speed of projectile
    public float[] attackRanges = new float[] { 10, 10, 3, 3, 10, 10, 3, 3 }; // Range of attack
    public int[] damages = new int[] { 2, 10, 10, 10, 5, 5, 10, 10 };  // Amount of damage
    public float[] attackIntervals = new float[] { 0.1f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, }; // Attack interval
    public float[] speeds = new float[] { 3.5f, 3.5f, 3.5f, 3.5f, 3.5f, 3.5f, 3.5f, 3.5f }; // Movement speed
    public int[] healths = new int[] { 100, 150, 200, 250, 10, 20, 50, 100 }; // Health

    #region Variables
    // Attributes of characters (No need for public variables after testing)?
    public bool npc;
    public bool ranged;
    public float projectileSpeed;
    public float attackRange;
    public int damage;
    public float attackInterval;
    public float speed;
    public int health;
   
    public float attackTimer;
    public int weaponLevel;

    public GameObject projectileSpawn;
    public GameObject projectilePrefab;

    public Vector2 movement;

    #endregion
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
    public void SetCharacterAttributes() {
        npc = npcs[(int)characterType];
        ranged = rangeds[(int)characterType];
        projectileSpeed = projectileSpeeds[(int)characterType];
        attackRange = attackRanges[(int)characterType];
        damage = damages[(int)characterType];
        attackInterval = attackIntervals[(int)characterType];
        speed = speeds[(int)characterType];
        health = healths[(int)characterType];
    }
}