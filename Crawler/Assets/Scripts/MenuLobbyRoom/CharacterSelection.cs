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
        AudioFW.StopLoop("MenuLoop");
        AudioFW.PlayLoop("CharaterSelectionLoop");
        Invoke("SelectFirstCharacter", 2.25f);
    }

    void SelectFirstCharacter() {
        print("Select first");
        print(buttons[0]);
        buttons[0].Select();
        LeanTween.scale(chooseText, Vector3.one * 1.2f, .15f).setLoopPingPong().setEaseInExpo();
    }

    public void OnClickPickCharacter(int c) {
        AudioFW.StopLoop("CharaterSelectionLoop");
        LeanTween.cancel(chooseText, true);
        chooseText.GetComponent<TextMeshProUGUI>().text = "Waiting others";
        LeanTween.scale(chooseText, Vector3.one * 1.2f, 2f).setLoopPingPong().setEaseInOutSine();
        Invoke("PlayAudioDelayed", .25f);
        PlayerNetwork.Instance.selectedCharacter = c;
        PlayerNetwork.Instance.PickedCharacter(c);
    }

    void PlayAudioDelayed() {
        AudioFW.Play("CharacterSelected");
    }
}
