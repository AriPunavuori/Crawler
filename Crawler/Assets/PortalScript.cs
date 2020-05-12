using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : Photon.MonoBehaviour
{
    public int playersInPortal = 0;
    List<int> playerIDs = new List<int>();
    //float timer = 0;
    bool portalReady = false;
    public ParticleSystem PortalEffect;
    public bool teleportFinished = false;


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        PortalEffect.transform.localScale = new Vector3(0, 0, 0);
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
                    collision.gameObject.GetComponent<PlayerCharacter>().inPortal = true;
                    StartCoroutine(teleportPlayer(10f, collision.gameObject.transform));

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
            transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor) * 0.4f;
            PortalEffect.transform.localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
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
        GameObject characterEffect = new List<GameObject>(GameObject.FindGameObjectsWithTag("CharacterEffect")).Find(g => g.transform.IsChildOf(playerTransform));
        float cameraTime1 = 0.9f * inTime;
        float cameraTime2 = 0.1f * inTime;
        float camElapsedTime1 = 0;
        float camElapsedTime2 = 0;
        float cam2Size = 0;
        bool cam2SizeSet = false;

        while (elapsedTime < inTime)
        {
            playerTransform.position = Vector3.Lerp(fromPos, toPos, elapsedTime / inTime);
            rot.eulerAngles = Vector3.Lerp(fromRot, toRot, elapsedTime / inTime);
            playerTransform.rotation = rot;
            elapsedTime += Time.deltaTime;
            camElapsedTime1 += Time.deltaTime;
            scalingFactor = 1f - (elapsedTime / inTime);
            if(characterEffect)
            {
                characterEffect.transform.localScale = new Vector3(scalingFactor, scalingFactor, 1f);
            }
            playerTransform.localScale = new Vector3(scalingFactor, scalingFactor, 1f);
            playerCam.orthographicSize = Mathf.Lerp(5f, 3f, camElapsedTime1 / cameraTime1);
            if (elapsedTime >= (0.9f * inTime))
            {
                if(!cam2SizeSet)
                {
                    cam2Size = playerCam.orthographicSize;
                    cam2SizeSet = true;
                }
                camElapsedTime2 += Time.deltaTime;
                playerCam.orthographicSize = Mathf.Lerp(cam2Size, 0.3f, camElapsedTime2 / cameraTime2);
            }
            yield return null;
        }
        teleportFinished = true;
        //playerTransform.position = transform.position;
    }
}
