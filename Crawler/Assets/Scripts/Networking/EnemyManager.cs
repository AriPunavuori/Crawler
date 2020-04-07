using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager Instance;
    PhotonView PhotonView;
    [SerializeField]
    public List<EnemyStats> enemyStats = new List<EnemyStats>();
    void Awake() {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();
    }

    public void AddEnemyStats(EnemyCharacter ec) {
        int index = enemyStats.FindIndex(x => x.ec == ec);
        if(index == -1) {
            enemyStats.Add(new EnemyStats(ec, 0));
        }
        print(enemyStats);
    }

    public void ModifyHealth(EnemyCharacter ec, int value) {
        int index = enemyStats.FindIndex(x => x.ec == ec);
        if(index != -1) {
            EnemyStats es = enemyStats[index];
            print("Original health: " + es.Health);
            es.Health += value;
            print(ec.gameObject.name + " Enemy health changed " + value + "!");
            // Who do we let know of new health?
            // Maybe ec? Smething like ec.NewHealth();
            //EnemyNetwork.Instance.NewHealth(photonPlayer, enemyStats.Health);
        }
    }


}
public class EnemyStats {
    public EnemyStats(EnemyCharacter ec, int healt) {
        this.ec = ec;
        Health = healt;
    }
    public readonly EnemyCharacter ec;
    public int Health;
}
