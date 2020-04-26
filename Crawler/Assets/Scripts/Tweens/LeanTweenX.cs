using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenX : MonoBehaviour
{
    public RectTransform rectTrans;
    public float timeOfEase;
    public float whereToMove;
    public GameObject[] objectsToEnable;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveX(rectTrans, whereToMove, timeOfEase).setEaseInOutElastic();
        Invoke("EnableObjects", timeOfEase);
    }
    void EnableObjects() {
        foreach(var o in objectsToEnable) {
            o.SetActive(true);
        }
    }
}
