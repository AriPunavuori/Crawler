using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMover : MonoBehaviour {
    void Start() {
        LeanTween.move(gameObject.GetComponent<RectTransform>(), new Vector3(-1150, -850, 0), 125f).setEaseInOutCubic();
    }

}
