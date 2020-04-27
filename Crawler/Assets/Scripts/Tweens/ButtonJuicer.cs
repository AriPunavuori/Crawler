using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonJuicer : MonoBehaviour {
    public float factor = 1.05f;
    Shadow shadow;
    Vector2 originalEffectDistance;
    private void Awake() {
        shadow = GetComponent<Shadow>();
        if(shadow != null) {
            originalEffectDistance.x = shadow.effectDistance.x;
            originalEffectDistance.y = shadow.effectDistance.y;
        }
    }

    public void OnButtonHoverEnter(RectTransform rect) {
        LeanTween.scale(rect, Vector3.one * factor, .05f);
        if(shadow != null) {
            shadow.effectDistance = originalEffectDistance * factor * 1.5f;
        }
        AudioFW.Play("ButtonSwitch");
    }

    public void OnButtonHoverExit(RectTransform rect) {
        LeanTween.scale(rect, Vector3.one, .05f);
        if(shadow != null) {
            shadow.effectDistance = originalEffectDistance;
        }
        AudioFW.Play("ButtonSwitch");
    }

    public void OnButtonClick() {
        AudioFW.Play("ButtonClicked");
    }
}
