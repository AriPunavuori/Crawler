using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShieldScript : MonoBehaviour
{
    Collider2D[] foundPlayers;
    LayerMask layerMaskPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        layerMaskPlayer = LayerMask.GetMask("Player");
        Physics2D.IgnoreCollision(transform.parent.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
    }

    // Update is called once per frame
    void Update()
    {
        foundPlayers = Physics2D.OverlapCircleAll(transform.position, 3f, layerMaskPlayer);
        //Debug.Log("Playercount: " + PhotonNetwork.room.PlayerCount);
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCharacter>().Stun(0.5f);
            Vector2 pushDir = (collision.gameObject.transform.position - transform.position).normalized;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(pushDir * 30f, ForceMode2D.Impulse);
        }
    }



}
