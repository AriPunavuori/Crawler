using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour {
    public float speed = 5f;
    private Rigidbody2D rigidBody;
    public GameObject MagicalGirlJoint;
    public GameObject boltSpawn;
    public GameObject bolt;
    public GameObject projectile;
    public float boltForce;


    void Start() {

        rigidBody = GetComponent<Rigidbody2D>();
    }
    void Update() {
        if(photonView.isMine) {
            if(Input.GetKeyDown(KeyCode.Mouse0)) {
                //Shoot();
                //photonView.RPC("Shoot", PhotonTargets.Others);
            }
        }
    }

    void FixedUpdate() {
        if(photonView.isMine) {

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            rigidBody.velocity = new Vector2(x * speed, y * speed);

        }
    }
    //[PunRPC]
    //public void Shoot() {
    //    GameObject BoltSpawnInstance = Instantiate(projectile, boltSpawn.transform.position, Quaternion.identity);
    //    Projectile proj = BoltSpawnInstance.GetComponent<Projectile>();
    //    // Set damage of the projectile
    //    proj.damage = 25;
    //    // Set speed of the projectile
    //    proj.speed = 8;
    //    // Assing shooter that shot the projectile
    //    proj.shooter = EntityType.Hero1;
    //    BoltSpawnInstance.transform.SetParent(MagicalGirlJoint.transform);
    //    //BoltSpawnInstance.GetComponent<Rigidbody2D>().AddForce(MagicalGirlJoint.transform.right * boltForce, ForceMode2D.Impulse);
    //}
}