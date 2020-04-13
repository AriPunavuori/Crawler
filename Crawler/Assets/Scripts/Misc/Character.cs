using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, Enemy0, Enemy1, Enemy2, Enemy3 }
[RequireComponent(typeof(PhotonView))]
public class Character : Photon.MonoBehaviour {

    public EntityType characterType;

    bool[] npcs = new bool[] { false, false, false, false, true, true, true, true }; // Player or enemy
    bool[] rangeds = new bool[] { true, true, false, false, true, true, false, false }; // Ranger or melee
    float[] projectileSpeeds = new float[] { 10, 10, 10, 10, 10, 10, 10, 10 }; // Speed of projectile
    float[] attackRanges = new float[] { 10f, 10f, 2f, 2f, 8f, 8f, 1.5f, 1.5f }; // Range of attack
    int[] damages = new int[] { 10, 10, 15, 15, 5, 5, 10, 10 };  // Amount of damage
    float[] attackIntervals = new float[] { 0.2f, 0.2f, 0.5f, 0.5f, .5f, .5f, .5f, .5f }; // Attack interval
    float[] speeds = new float[] { 4.5f, 4.5f, 4.5f, 4.5f, 3.5f, 3.5f, 3.5f, 3.5f }; // Movement speed
    int[] healths = new int[] { 200, 200, 200, 200, 20 , 30, 50, 60 }; // Health

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

    public int CheckCharacterHealt(EntityType et) {
        return healths[(int)et];
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