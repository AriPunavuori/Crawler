using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    int keys = 0;

    public bool UseKey() {
        if(keys > 0) {
            keys -= 1;
            return true;
        }
        return false;
    }

    public void FoundKey() {
        keys += 1;
    }

}
