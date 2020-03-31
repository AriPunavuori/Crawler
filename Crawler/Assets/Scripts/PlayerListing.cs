
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour {

    public PhotonPlayer PhotonPlayer;

    public Text playerName;

    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer) {
        print(photonPlayer.NickName);
        playerName.text = photonPlayer.NickName;
    }
}
