using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour {
    public RectTransform[] characterRectTransforms;
    public RectTransform[] infoCardRectTransforms;

    public void Show(int i) {
        LeanTween.move(characterRectTransforms[i], Vector2.right * 550, .25f);
        LeanTween.move(infoCardRectTransforms[i], Vector2.right * 550, .25f);
    }
    
    public void Hide(int i) {
        LeanTween.move(characterRectTransforms[i], Vector2.right * 550, .25f);
        LeanTween.move(infoCardRectTransforms[i], Vector2.right * 550, .25f);
    }
}
