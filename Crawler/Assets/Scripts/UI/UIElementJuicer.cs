using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementJuicer : MonoBehaviour {


    public float onHoverSizeMultiplier = 1.05f;
    float onHoverEnterLength = .1f;
    float onHoverExitLength = .1f;
    public Vector2 shadowDistance = new Vector2(10f, -10f);

    public bool entryMovement = true;
    public Vector3 movementDelta;
    Vector3 destination;
    public float tweenLength;

    public GameObject[] objectsToEnableAfterEntryTween;

    RectTransform rectTransform;
    Shadow shadow;
    
    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        shadow = gameObject.AddComponent<Shadow>();
        shadow.effectDistance = shadowDistance;
    }

    void Start() {
        if(entryMovement) {
            MoveIn(false);
            if(objectsToEnableAfterEntryTween != null)
                Invoke("EnableObjects", tweenLength);
        }
    }

    public void MoveIn(bool delayed) {
        if(delayed)
        Invoke("MoveInDelayed", tweenLength);
        else
            Invoke("MoveInDelayed", 0);
    }

    void MoveInDelayed() {
        destination = (Vector3)rectTransform.anchoredPosition + movementDelta;
        LeanTween.move(rectTransform, destination, tweenLength).setEaseInOutBack();
    }

    public void MoveOut(bool delayed) {
        if(delayed)
            Invoke("MoveOutDelayed", tweenLength);
        else
            Invoke("MoveOutDelayed", 0);
    }

    void MoveOutDelayed() {
        destination = (Vector3)rectTransform.anchoredPosition - movementDelta;
        LeanTween.move(rectTransform, destination, tweenLength).setEaseInOutBack();
    }

    void EnableObjects() {
        foreach(var obj in objectsToEnableAfterEntryTween) {
            obj.SetActive(true);
        }
    }

    public void OnButtonHoverEnter() {
        LeanTween.scale(rectTransform, Vector3.one * onHoverSizeMultiplier, onHoverEnterLength);
        if(shadow != null) {
            shadow.effectDistance = shadowDistance * onHoverSizeMultiplier * 1.5f;
        }
        AudioFW.Play("ButtonSwitch");
    }

    public void OnButtonHoverExit() {
        LeanTween.scale(rectTransform, Vector3.one, onHoverExitLength);
        if(shadow != null) {
            shadow.effectDistance = shadowDistance;
        }
        AudioFW.Play("ButtonSwitch");
    }

    public void OnButtonClick() {
        AudioFW.Play("ButtonClicked");
    }
}
