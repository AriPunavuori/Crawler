using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFacingAt : Photon.MonoBehaviour
{
    public Camera myCam;
    public AudioListener myMic;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.isMine)
        {
            myCam.enabled = true;
            myMic.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine)
        {

            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dir = Input.mousePosition - pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


            //Debug.Log(transform.rotation.eulerAngles);
            float mouseAngle = transform.rotation.eulerAngles.z;
            float angle22dot5 = 22.5f;

            if (((mouseAngle) <= (angle22dot5)) && ((mouseAngle) <= (angle22dot5))){

            }
            if (transform.rotation.eulerAngles == new Vector3(0, 0, mouseAngle))
            {

            }
            if (transform.rotation.eulerAngles == new Vector3(0, 0, mouseAngle))
            {

            }
            if (transform.rotation.eulerAngles == new Vector3(0, 0, mouseAngle))
            {

            }
            if (transform.rotation.eulerAngles == new Vector3(0, 0, mouseAngle))
            {

            }
        }
    }
}

