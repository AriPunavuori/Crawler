using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerActivator : MonoBehaviour
{
    GameObject[] players;
    public GameObject EnemySpawnerPlayers2;
    public GameObject EnemySpawnerPlayers3;
    public GameObject EnemySpawnerPlayers4;

    void Start()
    {
        Invoke("ActivateSpawners", 2f);
    }

    void ActivateSpawners()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(players.Length);
        if(players.Length > 1)
        {
            EnemySpawnerPlayers2.gameObject.SetActive(true);
            if (players.Length > 2)
            {
                Debug.Log("a");

                EnemySpawnerPlayers3.gameObject.SetActive(true);

                if (players.Length > 3)
                {
                    EnemySpawnerPlayers4.gameObject.SetActive(true);
                }
            }
        }
    }
}
