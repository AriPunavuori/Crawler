
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour {

    public PhotonPlayer PhotonPlayer;

    public Text playerName;

    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer) {
        playerName.text = photonPlayer.NickName;
    }
}
