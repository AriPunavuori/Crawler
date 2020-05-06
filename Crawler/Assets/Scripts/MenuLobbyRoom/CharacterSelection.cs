using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CharacterSelection : MonoBehaviour {

    public Button[] buttons;
    public GameObject chooseText;
    public RectTransform controlInfo;
    public RectTransform girl;
    public RectTransform title;
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
        Invoke("AfterSelection", .25f);
        PlayerNetwork.Instance.selectedCharacter = c;
        PlayerNetwork.Instance.PickedCharacter(c);
        LeanTween.move(girl, Vector2.right * 2250, .25f).setEaseInBack();
        LeanTween.move(title, Vector2.left * 2250, .25f).setEaseInBack();
    }

    void AfterSelection() {
        LeanTween.move(controlInfo, Vector2.zero, .25f).setEaseOutBack();
        AudioFW.Play("CharacterSelected");
    }
}
