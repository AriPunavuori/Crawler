using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMover : MonoBehaviour {
    Action DoMove;
    Vector3 target;
    float timeToMove;
    void Start() {
        DoMove += MoveNow;
        MoveNow();
    }

    void MoveNow() {
        target = new Vector3(UnityEngine.Random.Range(-1180, 1180), UnityEngine.Random.Range(-110, 110), 0);
        timeToMove = Vector3.Distance(GetComponent<RectTransform>().anchoredPosition, target)/100f;
        LeanTween.move(gameObject.GetComponent<RectTransform>(), target, 10f).setEaseInOutCubic().setOnComplete(DoMove);
    }

}
