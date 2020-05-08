using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterSelection : MonoBehaviour {

    public Button[] buttons;
    public GameObject chooseText;
    public RectTransform controlInfo;

    private void Start() {
        PlayerNetwork.Instance.numberOfPlayers = PhotonNetwork.playerList.Length;
        AudioFW.StopAllSounds();
        AudioFW.PlayLoop("CharaterSelectionLoop");
        Invoke("PumpText", .5f);
    }

    void PumpText() {
        foreach(var button in buttons) {
            button.interactable = true;
        }
        LeanTween.scale(chooseText, Vector3.one * 1.2f, 2f).setLoopPingPong().setEaseInOutSine();
        buttons[0].Select();
    }

    public void OnClickPickCharacter(int c) {
        PlayerNetwork.Instance.selectedCharacter = c;
        PlayerNetwork.Instance.PickedCharacter(c);
    }

    public void AfterSelection() {
        LeanTween.move(controlInfo, Vector2.zero, .25f).setEaseOutBack();
        AudioFW.StopAllSounds();
        AudioFW.Play("CharacterSelected");
        LeanTween.cancel(chooseText);
        LeanTween.scale(chooseText, Vector3.one, 0f);
        chooseText.GetComponent<TextMeshProUGUI>().text = "Get Ready!";
        LeanTween.scale(chooseText, Vector3.one * 1.2f, .2f).setLoopPingPong().setEaseInExpo();
    }
}
