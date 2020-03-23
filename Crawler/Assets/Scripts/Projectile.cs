﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public int damage;
    public float speed;
    private float timer = 1.0f;
    public bool explosive;
    public bool reflective;
    //public enum PlayerType { Hero0, Hero1, Hero2, Hero3, enemy0, enemy1, enemy2, enemy3 }
    public EntityType shooter;

    // Start is called before the first frame update
    void Start() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        transform.localRotation = Quaternion.identity;
        rb.AddForce(transform.parent.right * speed, ForceMode2D.Impulse);
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update() {
        timer -= Time.deltaTime;
        if(timer < 0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if(collision.gameObject.CompareTag("Enemy")) {
            Destroy(gameObject);
        } else {
            // Reflect from colliding object with proper angle
        }

        if(!reflective)            
        Destroy(gameObject);
    }






}