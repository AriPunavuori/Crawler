using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPopper : MonoBehaviour {
    public RectTransform[] buttons;
    public RectTransform[] cards;

    Action buttonUp;
    Action cardDown;

    private void Awake() {
        buttonUp = MoveButtonUp;
        cardDown = MoveCardDown;
    }

    public void OnHoverEnter(int i) {
        LeanTween.cancel(cards[i]);
        LeanTween.cancel(buttons[i]);
        LeanTween.move(cards[i], new Vector3(0, -400, 0), .1f).setOnComplete(buttonUp);
    }

    void MoveButtonUp() {
        LeanTween.move(buttons[0], new Vector3(-450, 400, 0), .1f);
    }

    public void OnHoverExit(int i) {
        LeanTween.cancel(cards[i]);
        LeanTween.cancel(buttons[i]);
        LeanTween.move(buttons[i], new Vector3(-450, 0, 0), .1f).setOnComplete(MoveCardDown);
    }

    void MoveCardDown() {
        LeanTween.move(cards[0], new Vector3(0, -1000, 0), .1f);
    }



}
