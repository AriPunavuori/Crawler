using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    GameManager gm;
    BoxCollider2D bc;
    Vector3 startPosition;
    Vector3 endPosition;
    float speed = .5f;
    public bool opened;

    float openingSpeed;
    void Start() {
        gm = FindObjectOfType<GameManager>();
        bc = GetComponent<BoxCollider2D>();
        startPosition = transform.position;
        endPosition = transform.position + Vector3.down;
        print(endPosition);
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player") && !opened) {
            if(gm.UseKey()) {
                bc.enabled = false;
                opened = true;
                StartCoroutine(RotateMe(Vector3.forward * 90, 5));
            }
        }
    }
    IEnumerator RotateMe(Vector3 byAngles, float inTime) {
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for(var t = 0f; t < 1; t += Time.deltaTime / inTime) {
            transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
            yield return null;
        }
    }
}
