using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour {

    private void Start() {
        GetComponent<Button>().Select();
    }

    public void OnClickStartButton() {
        PhotonNetwork.LoadLevel(1);
    }
}
