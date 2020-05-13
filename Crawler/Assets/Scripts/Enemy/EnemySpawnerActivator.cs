using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerActivator : MonoBehaviour
{
    GameObject[] players;
    public GameObject EnemySpawnerOnlyPlayers1DarkMagiMelee;
    public GameObject EnemySpawnerOnlyPlayers1LightMagi;
    public GameObject EnemySpawnerPlayers2;
    public GameObject EnemySpawnerPlayers3;
    public GameObject EnemySpawnerPlayers4;

    void Start()
    {
        Invoke("ActivateSpawners", 2f);
        print("Spawner Activation");
    }

    void ActivateSpawners()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        var charType = players[0].GetComponent<Character>().characterType;
        if (players.Length == 1)
        {
            if(charType == EntityType.Hero1 || charType == EntityType.Hero2 || charType == EntityType.Hero3)
            {
                //Variksia & Kasveja
                EnemySpawnerOnlyPlayers1DarkMagiMelee.gameObject.SetActive(true);
            }
            else
            {
                // Paljon Hämiksii
                EnemySpawnerOnlyPlayers1LightMagi.gameObject.SetActive(true);
            }
        }
        if (players.Length > 1)
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
