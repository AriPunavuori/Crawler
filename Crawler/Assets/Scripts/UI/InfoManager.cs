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

    public void Show(int i) {
        LeanTween.move(characterRectTransforms[i], imagePosition, 0f);
        LeanTween.move(infoCardRectTransforms[i], cardPosition, 0f);
    }
    
    public void Hide(int i) {
        LeanTween.move(characterRectTransforms[i], originalImagePositions[i], 0f);
        LeanTween.move(infoCardRectTransforms[i], originalCardPositions[i], 0f);
    }


}
