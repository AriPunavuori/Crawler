using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {
    public Button skipOutro;
    public Button goToLobby;
    public RectTransform outroText;
    Action watchedOutro;
    Action outroDone;
    private void Start() {
        Cursor.visible = true;
        GameObject.Find("DynamicBG").GetComponent<Image>().enabled = true;
        GameObject.Find("DynamicBG").GetComponent<BackgroundMover>().enabled = true;
        watchedOutro += WatchOutro;
        outroDone += OutroDone;
        if(PlayerPrefs.GetInt("OutroSeen") == 1) {
            ActivateButtons();
        }

        LeanTween.move(outroText, Vector2.up * 1300, 42.2f).setOnComplete(watchedOutro).setEaseOutSine();
        AudioFW.StopAllSounds();
        AudioFW.Play("Credits");
    }

    void ActivateButtons() {
        goToLobby.interactable = true;
        skipOutro.interactable = true;
        skipOutro.Select();
    }

    void WatchOutro() {
        ActivateButtons();
        PlayerPrefs.SetInt("OutroSeen", 1);
        LeanTween.move(outroText, Vector3.up * 2300, .15f).setEaseInBack().setOnComplete(outroDone);
    }

    public void OnClickGoToLobby() {
        AudioFW.StopAllSounds();
        AudioFW.PlayLoop("MenuLoop");
        PhotonNetwork.LeaveRoom();
        PlayerNetwork.Instance.LoadMenu();
    }

    void OutroDone() {
        OnClickGoToLobby();
    }

    public void OnClickQuitGame() {
        Application.Quit();
    }
}
