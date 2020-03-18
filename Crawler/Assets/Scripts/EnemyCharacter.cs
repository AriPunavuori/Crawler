using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacter : Character {
    NavMeshAgent agent;
    Transform player;
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
    }

    void Update() {
        var pPos = player.position;
        agent.destination = pPos;
    }
}
