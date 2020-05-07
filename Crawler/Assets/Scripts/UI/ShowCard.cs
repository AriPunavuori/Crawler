using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCard : MonoBehaviour {
    RectTransform rectTransform;
    public void Show() {
        LeanTween.move(rectTransform, Vector2.right * 550, .25f);
    }
    public void Hide() {
        LeanTween.move(rectTransform, Vector2.right * 550, .25f);
    }
}
