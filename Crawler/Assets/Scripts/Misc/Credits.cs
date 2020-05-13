using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {
    public Button skipOutro;
    public Button goToLobby;
    public RectTransform outroText;
    Action watchedOutro;
    private void Start() {
        Cursor.visible = true;
        GameObject.Find("DynamicBG").GetComponent<Image>().enabled = true;
        GameObject.Find("DynamicBG").GetComponent<BackgroundMover>().enabled = true;
        watchedOutro += WatchOutro;
        if(PlayerPrefs.GetInt("OutroSeen") == 1) {
            WatchOutro();
        }

        LeanTween.move(outroText, Vector2.up * 1300, 43f).setOnComplete(watchedOutro).setEaseOutSine();
        AudioFW.StopAllSounds();
        AudioFW.Play("Credits");
    }

    void WatchOutro() {
        PlayerPrefs.SetInt("OutroSeen", 1);
        goToLobby.interactable = true;
        skipOutro.interactable = true;
        skipOutro.Select();
    }

    public void OnClickGoToLobby() {
        AudioFW.StopAllSounds();
        AudioFW.PlayLoop("MenuLoop");
        PhotonNetwork.LeaveRoom();
        PlayerNetwork.Instance.LoadMenu();
    }

    public void OnClickQuitGame() {
        Application.Quit();
    }
}
