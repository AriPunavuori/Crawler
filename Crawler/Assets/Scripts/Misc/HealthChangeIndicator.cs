using System;
using UnityEngine;
using TMPro;

public class HealthChangeIndicator : MonoBehaviour {
    public TextMeshProUGUI healthText;
    float lifeTime = 1f;
    Action doDestroy;
    
    void Start() {
        doDestroy += DoDestroy;
        LeanTween.move(gameObject, transform.position + Vector3.up, lifeTime).setEaseOutCirc().setOnComplete(doDestroy);
    }

    void DoDestroy() {
        Destroy(gameObject);
    }

    public void SetHealthChangeText(int change) {
        healthText.text = "" + change;
    }
}
