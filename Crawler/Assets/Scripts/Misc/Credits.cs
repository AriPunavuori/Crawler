using System;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour {
    public Button skipOutro;
    public RectTransform outroText;
    Action watchedOutro;
    private void Start() {
        watchedOutro += WatchOutro;
        if(PlayerPrefs.GetInt("OutroSeen") == 1)
            skipOutro.interactable = true;
        LeanTween.move(outroText, Vector2.up * 1300, 43f).setOnComplete(watchedOutro);
        AudioFW.StopAllSounds();
        AudioFW.Play("Credits");
    }

    void WatchOutro() {
        PlayerPrefs.SetInt("OutroSeen", 1);
        skipOutro.interactable = true;
    }

    public void OnClickQuitGame() {
        Application.Quit();
    }
}
