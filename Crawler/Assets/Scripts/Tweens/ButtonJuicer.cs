using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonJuicer : MonoBehaviour {
    public void OnButtonHoverEnter(RectTransform rect) {
        LeanTween.scale(rect, Vector3.one * 1.1f, .05f);
        AudioFW.Play("ButtonSwitch");
    }

    public void OnButtonHoverExit(RectTransform rect) {
        LeanTween.scale(rect, Vector3.one, .05f);
        AudioFW.Play("ButtonSwitch");
    }

    public void OnButtonClick() {
        AudioFW.Play("ButtonClicked");
    }
}
