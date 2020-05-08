using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPopper : MonoBehaviour {
    public RectTransform[] buttons;
    public RectTransform[] cards;

    Action button0Up;
    Action button1Up;
    Action button2Up;
    Action button3Up;
    Action card0Down;
    Action card1Down;
    Action card2Down;
    Action card3Down;

    private void Awake() {
        button0Up = MoveButton0Up;
        button1Up = MoveButton1Up;
        button2Up = MoveButton2Up;
        button3Up = MoveButton3Up;
        card0Down = MoveCard0Down;
        card1Down = MoveCard1Down;
        card2Down = MoveCard2Down;
        card3Down = MoveCard3Down;
    }

    public void OnHoverEnter0() {
        if(buttons[0].GetComponent<Button>().IsInteractable()) {
            LeanTween.cancel(cards[0]);
            LeanTween.cancel(buttons[0]);
            LeanTween.move(cards[0], new Vector3(0, -400, 0), .15f).setOnComplete(button0Up);
        }
    }
    public void OnHoverEnter1() {

        if(buttons[1].GetComponent<Button>().IsInteractable()) {
            LeanTween.cancel(cards[1]);
            LeanTween.cancel(buttons[1]);
            LeanTween.move(cards[1], new Vector3(0, -400, 0), .15f).setOnComplete(button1Up);
        }
    }
    public void OnHoverEnter2() {
        if(buttons[2].GetComponent<Button>().IsInteractable()) {
            LeanTween.cancel(cards[2]);
            LeanTween.cancel(buttons[2]);
            LeanTween.move(cards[2], new Vector3(0, -400, 0), .15f).setOnComplete(button2Up);
        }
    }
    public void OnHoverEnter3() {
        if(buttons[3].GetComponent<Button>().IsInteractable()) {
            LeanTween.cancel(cards[3]);
            LeanTween.cancel(buttons[3]);
            LeanTween.move(cards[3], new Vector3(0, -400, 0), .15f).setOnComplete(button3Up);
        }
    }

    void MoveButton0Up() {
        LeanTween.move(buttons[0], new Vector3(-450, 400, 0), .25f);
    }
    void MoveButton1Up() {
        LeanTween.move(buttons[1], new Vector3(-150, 400, 0), .25f);
    }
    void MoveButton2Up() {
        LeanTween.move(buttons[2], new Vector3(150, 400, 0), .25f);
    }
    void MoveButton3Up() {
        LeanTween.move(buttons[3], new Vector3(450, 400, 0), .25f);
    }

    public void OnHoverExit0() {
        LeanTween.cancel(cards[0]);
        LeanTween.cancel(buttons[0]);
        LeanTween.move(buttons[0], new Vector3(-450, 0, 0), .5f).setOnComplete(card0Down).setEaseInCirc();
    }
    public void OnHoverExit1() {
        LeanTween.cancel(cards[1]);
        LeanTween.cancel(buttons[1]);
        LeanTween.move(buttons[1], new Vector3(-150, 0, 0), .5f).setOnComplete(card1Down).setEaseInCirc();
    }
    public void OnHoverExit2() {
        LeanTween.cancel(cards[2]);
        LeanTween.cancel(buttons[2]);
        LeanTween.move(buttons[2], new Vector3(150, 0, 0), .5f).setOnComplete(card2Down).setEaseInCirc();
    }
    public void OnHoverExit3() {
        LeanTween.cancel(cards[3]);
        LeanTween.cancel(buttons[3]);
        LeanTween.move(buttons[3], new Vector3(450, 0, 0), .5f).setOnComplete(card3Down).setEaseInCirc();
    }

    void MoveCard0Down() {
        LeanTween.move(cards[0], new Vector3(0, -1000, 0), .1f);
    }
    void MoveCard1Down() {
        LeanTween.move(cards[1], new Vector3(0, -1000, 0), .1f);
    }
    void MoveCard2Down() {
        LeanTween.move(cards[2], new Vector3(0, -1000, 0), .1f);
    }
    void MoveCard3Down() {
        LeanTween.move(cards[3], new Vector3(0, -1000, 0), .1f);
    }



}
