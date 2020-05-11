using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : Photon.MonoBehaviour
{
    public int playersInPortal = 0;
    List<int> playerIDs = new List<int>();
    //float timer = 0;
    bool portalReady = false;



    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(scalePortal(3f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (portalReady)
        {
            Debug.Log("Collision with player");
            if (collision.gameObject.CompareTag("Player"))
            {
                if (!playerIDs.Contains(collision.gameObject.GetComponent<PhotonView>().viewID))
                {
                    Debug.Log("Player added");
                    playersInPortal++;
                    playerIDs.Add(collision.gameObject.GetComponent<PhotonView>().viewID);

                    StartCoroutine(teleportPlayer(9.9f, collision.gameObject.transform));

                    if (PhotonNetwork.isMasterClient)
                    {
                        if (playersInPortal == PhotonNetwork.room.PlayerCount)
                        {
                            GameManager.Instance.gameWon = true;
                        }
                    }
                }
            }
        }

    }


    //public void teleportPlayer()
    //{
    //    photonView.RPC("RPC_teleportPlayer", PhotonTargets.AllViaServer);
    //}

    //[PunRPC]
    //public void RPC_teleportPlayer()
    //{
    //    GameObject player 
    //    StartCoroutine(teleportPlayer(9.9f, ))
    //}

    IEnumerator scalePortal(float inTime)
    {
        float elapsedTime = 0;
        float scalingFactor = 0;
        while (elapsedTime < inTime)
        {
            elapsedTime += Time.deltaTime;
            scalingFactor = elapsedTime / inTime;
            transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
            yield return null;
        }
        GetComponent<Collider2D>().isTrigger = true;
        portalReady = true;
    }

    IEnumerator teleportPlayer(float inTime, Transform playerTransform)
    {
        float elapsedTime = 0;
        float scalingFactor = 0;
        Vector3 fromPos = playerTransform.position;
        Vector3 toPos = transform.position;
        Vector3 fromRot = playerTransform.rotation.eulerAngles;
        Vector3 toRot = new Vector3(0, 0, 360f);
        Quaternion rot = new Quaternion();
        Camera playerCam = playerTransform.GetChild(0).GetComponent<Camera>();
        float cameraTime = 0.1f * inTime;
        float camElapsedTime = 0;

        while (elapsedTime < inTime)
        {
            playerTransform.position = Vector3.Lerp(fromPos, toPos, elapsedTime / inTime);
            rot.eulerAngles = Vector3.Lerp(fromRot, toRot, elapsedTime / inTime);
            playerTransform.rotation = rot;
            elapsedTime += Time.deltaTime;
            scalingFactor = 1f - (elapsedTime / inTime);
            playerTransform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
            if (elapsedTime >= (0.9f * inTime))
            {
                camElapsedTime += Time.deltaTime;
                playerCam.orthographicSize = Mathf.Lerp(5f, 0.05f, camElapsedTime / cameraTime);
            }

            yield return null;
        }
    }
}
