using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rigidBody;
    public GameObject MagicalGirlJoint;
    public GameObject boltSpawn;
    public GameObject bolt;
    public float boltForce;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)){
            GameObject BoltSpawnInstance = Instantiate(bolt, boltSpawn.transform.position, Quaternion.identity);
            BoltSpawnInstance.transform.SetParent(MagicalGirlJoint.transform);
            BoltSpawnInstance.GetComponent<Rigidbody2D>().AddForce(MagicalGirlJoint.transform.right * boltForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        rigidBody.velocity = new Vector2(x * speed, y * speed);

    }
}