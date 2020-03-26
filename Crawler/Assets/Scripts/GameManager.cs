using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Photon.MonoBehaviour {
    int keys = 0;

    [PunRPC]
    public bool UseKeyRPC() {
        if(keys > 0) {
            keys -= 1;
            return true;
        }
        return false;
    }

    [PunRPC]
    public void FoundKeyRPC() {
        keys += 1;
    }
    public void FoundKey() {
        photonView.RPC("FoundKeyRPC", PhotonTargets.All);
    }
    public bool UseKey() {
        photonView.RPC("UseKeyRPC", PhotonTargets.Others);
        return UseKeyRPC();
    }
}
