using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterSelection : MonoBehaviour {

    public Button[] buttons;
    public GameObject chooseText;

    private void Start() {
        PlayerNetwork.Instance.numberOfPlayers = PhotonNetwork.playerList.Length;
        AudioFW.StopAllSounds();
        AudioFW.PlayLoop("CharaterSelectionLoop");
        Invoke("PumpText", 2.25f);
    }

    void PumpText() {
        foreach(var button in buttons) {
            button.interactable = true;
        }
        LeanTween.scale(chooseText, Vector3.one * 1.2f, 2f).setLoopPingPong().setEaseInOutSine();
    }

    public void OnClickPickCharacter(int c) {
        AudioFW.StopLoop("CharaterSelectionLoop");
        LeanTween.cancel(chooseText);
        LeanTween.scale(chooseText, Vector3.one, 0f);
        chooseText.GetComponent<TextMeshProUGUI>().text = "Get Ready!";
        LeanTween.scale(chooseText, Vector3.one * 1.2f, .2f).setLoopPingPong().setEaseInExpo();
        Invoke("PlayAudioDelayed", .25f);
        PlayerNetwork.Instance.selectedCharacter = c;
        PlayerNetwork.Instance.PickedCharacter(c);
    }

    void PlayAudioDelayed() {
        AudioFW.Play("CharacterSelected");
    }
}
