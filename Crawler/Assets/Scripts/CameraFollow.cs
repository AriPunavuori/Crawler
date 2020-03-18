using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Vector3 camOffset;
    Transform player;
    void Start() {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update() {
        var pPos = player.position;
        transform.position = new Vector3(pPos.x + camOffset.x, pPos.y + camOffset.y, pPos.z + camOffset.z);
    }
}
