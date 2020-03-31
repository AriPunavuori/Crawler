
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour {

    public PhotonPlayer PhotonPlayer { get; private set; }

    public Text playerName;

    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer) {
        PhotonPlayer = photonPlayer;
        playerName.text = photonPlayer.NickName;
    }
}
