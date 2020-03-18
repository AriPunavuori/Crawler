using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    public Transform myParentObject;
    private float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        transform.localRotation = Quaternion.identity;
        myParentObject.parent = null;

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 0.5)
        {
            Destroy(this.gameObject);
        }
    }
}
