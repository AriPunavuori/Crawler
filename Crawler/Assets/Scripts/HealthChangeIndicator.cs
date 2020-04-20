using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthChangeIndicator : MonoBehaviour {
    public TextMeshProUGUI healthText;
    float lifeTime = 1f;
    float timeToDestroy;
    void Update() {
        transform.position += Vector3.up * Time.deltaTime;
        if(Time.time > timeToDestroy)
            Destroy(gameObject);
    }
    public void SetHealthChangeText(int change) {
        timeToDestroy = Time.time + lifeTime;
        healthText.text = "" + change;
    }
}
