using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Photon.MonoBehaviour {
    PhotonView PhotonView;
    Vector3 TargetPosition;
    Quaternion TargetRotation;
    public float Health;
    private void Awake() {
        PhotonView = GetComponent<PhotonView>();
    }
    void Update() {
        if(PhotonView.isMine)
            CheckInput();
        else
            SmoothMove();
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.isWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } else {
            TargetPosition = (Vector3)stream.ReceiveNext();
            TargetRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void SmoothMove() {
        transform.position = Vector3.Lerp(transform.position, TargetPosition, 0.25f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, 500 * Time.deltaTime);
    }
    void CheckInput() {
        float moveSpeed = 10f;
        float rotateSpeed = 250f;
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        transform.position += transform.right * (vertical * moveSpeed * Time.deltaTime);
        transform.Rotate(new Vector3(0, 0, -horizontal * rotateSpeed * Time.deltaTime));
    }
}
