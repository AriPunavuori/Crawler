using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFacingAt : Photon.MonoBehaviour {
    public Texture2D crosshair;
    public GameObject controllerPointer;
    public Camera myCam;
    public AudioListener myMic;
    Vector2 mousePos;
    Vector2 preMousePos;

    void Start() {
        if(photonView.isMine) {
            var cursorCenter = new Vector2(crosshair.width / 2, crosshair.height / 2);
            Cursor.SetCursor(crosshair, cursorCenter, CursorMode.Auto);
            myCam.enabled = true;
            myMic.enabled = true;
        }
    }

    void Update() {
        if(photonView.isMine) {
            mousePos = Input.mousePosition;
            if(mousePos != preMousePos) {
                Cursor.visible = true;
                controllerPointer.SetActive(false);
                Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
                Vector2 dir = (Vector2)Input.mousePosition - pos;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            Vector2 input = new Vector2(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"));
            if(input.magnitude > 0.2f) {
                Cursor.visible = false;
                controllerPointer.SetActive(true);
                transform.right = input;
            }
            preMousePos = mousePos;
        }
    }
}

