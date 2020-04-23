using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFacingAt : Photon.MonoBehaviour {
    public Camera myCam;
    public AudioListener myMic;
    Vector2 mousePos;
    Vector2 preMousePos;
    // Start is called before the first frame update
    void Start() {
        if(photonView.isMine) {
            myCam.enabled = true;
            myMic.enabled = true;
        }
    }

    // Update is called once per frame
    void Update() {
        if(photonView.isMine) {
            mousePos = Input.mousePosition;
            if(mousePos != preMousePos) {
                Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
                Vector2 dir = (Vector2)Input.mousePosition - pos;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            Vector2 input = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
            if(input.magnitude > 0.2f) {
                transform.right = input;
            }
            preMousePos = mousePos;
        }
    }
}

