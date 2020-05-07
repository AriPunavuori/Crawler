using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InfoManager : MonoBehaviour {
    public RectTransform[] characterRectTransforms;
    public RectTransform[] infoCardRectTransforms;
    public Vector2 imagePosition;
    public Vector2 cardPosition;
    public Vector2[] originalCardPositions;
    public Vector2[] originalImagePositions;
    bool selected;

    public void Show(int i) {
        if(!selected) {
            LeanTween.cancel(characterRectTransforms[i]);
            LeanTween.cancel(infoCardRectTransforms[i]);
            LeanTween.move(characterRectTransforms[i], imagePosition, .5f).setEaseOutCirc();
            LeanTween.move(infoCardRectTransforms[i], cardPosition, .5f).setEaseOutCirc();
        }
    }
    
    public void Hide(int i) {
        LeanTween.cancel(characterRectTransforms[i]);
        LeanTween.cancel(infoCardRectTransforms[i]);
        LeanTween.move(characterRectTransforms[i], originalImagePositions[i], .15f);
        LeanTween.move(infoCardRectTransforms[i], originalCardPositions[i], .15f);
    }

    public void SelectedCharacter() {
        selected = true;
        for(int i = 0; i < characterRectTransforms.Length; i++) {
            Hide(i);
        }
    }
}
