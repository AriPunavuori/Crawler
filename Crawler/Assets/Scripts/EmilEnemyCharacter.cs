using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmilEnemyCharacter : Character {

    private Rigidbody2D rigidBody;
    LayerMask layerMask;
    public GameObject player;

    float detectionDistance = 10f;
    void Start() {
        layerMask = LayerMask.GetMask("Player", "Obstacles");
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {

        if(player == null) {
            player = GameObject.Find("MagicalGirl(Clone)");
        }

        if(player != null) {
            var dirVector = player.transform.position - transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirVector, detectionDistance, layerMask); ;
            Debug.DrawLine(transform.position, hit.point, Color.red);
            //Debug.Log(hit.collider);
            rigidBody.velocity = Vector3.zero;

            if(hit.collider != null && hit.collider.CompareTag("Player")) {
                Vector3 moveDir = (player.transform.position - transform.position).normalized;
                rigidBody.velocity = moveDir * speed;
            } else {
                //Debug.Log("ei näie");
            }

        }
    }
}



//float dist = Vector2.Distance(transform.position, player.transform.position);
//Debug.DrawLine(transform.position, player.transform.position, Color.red);

//Vector2 pPositionVec2 = player.transform.position;
//Vector2 ePositionVec2 = transform.position;

//Debug.Log(pPositionVec2);
//Debug.Log(ePositionVec2);
//RaycastHit2D hit = Physics2D.Linecast(ePositionVec2, pPositionVec2);
//Debug.Log(hit.collider);
//rigidBody.velocity = Vector3.zero;

////if (!Physics.Linecast(transform.position, player.transform.position) && dist < 7) { //Välissä ei esteitä ja etäisyys on < x
//if(hit.collider != null && hit.collider.tag != "Wall" && dist < 7) {
//    Vector3 moveDir = (player.transform.position - transform.position).normalized;
//    rigidBody.velocity = moveDir * speed;

//    //transform.position += moveDir * speed * Time.deltaTime;
//    //Seurataan pelaajaa (liikutaan pelaajan suuntaan)
//} else {
//    //On este, ei voi seurata.
//    Debug.Log("ei näie");
//}