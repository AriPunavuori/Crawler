using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossEnemyScript : Photon.MonoBehaviour, IDamageable<int>
{
    public int health = 30000;
    int bossStage;
    LayerMask layerMaskPlayer;
    LayerMask layerMaskEnemy;
    public Collider2D[] meleerangeColliders;
    float meleeRangeTimer;
    float maxMeleeRangeTime = 3f;
    float enemySpawnTime;
    float enemySpawnCooldown = 5f;
    bool enemiesSpawned = true;
    bool pushAttackFin = true;
    bool rotateBurstFin = true;
    float rotateBurstTime;
    float rotateBurstCooldown = 20f;
    bool fireAtTargetsFin = true;
    bool meleeAttackFin = true;
    Collider2D[] foundPlayers;
    bool fightStarted = false;
    bool immune = true;
    List<Collider2D> playerColliders = new List<Collider2D>();
    List<GameObject> playerTargets = new List<GameObject>();
    List<Collider2D> enemyColliders = new List<Collider2D>();
    List<GameObject> enemies = new List<GameObject>();
    public GameObject ProjectileRotator;
    public GameObject MeleeRotator;
    //public GameObject ProjectileSpawn;
    public GameObject EnemySpawnRotator;
    public GameObject EnemySpawn;
    string[] enemyType = new string[] { "NetworkEnemy0", "NetworkEnemy1", "NetworkEnemy2", "NetworkEnemy3" };
    Quaternion projSpawnRot;
    Quaternion enemySpawnRot;
    public GameObject bossProjectile;
    // Start is called before the first frame update
    void Start()
    {
        bossStage = 1;
        //projSpawnRot = ProjectileRotator.transform.rotation;
        layerMaskPlayer = LayerMask.GetMask("Player");
        layerMaskEnemy = LayerMask.GetMask("Enemy");
        updateTargets();
        rotateBurstTime = Time.time + rotateBurstCooldown;
        enemySpawnTime = Time.time + enemySpawnCooldown;
    }


    [PunRPC]
    public void RPC_startFight()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        immune = false;
        fightStarted = true;
    }
    public void startFight()
    {
        photonView.RPC("RPC_startFight", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    public void RPC_updateBossStage(int stage)
    {
        bossStage = stage;
        Debug.Log("Bossstage updated");
        Debug.Log("Bosstage: " + bossStage);
    }
    public void updateBossStage(int stage)
    {
        photonView.RPC("RPC_updateBossStage", PhotonTargets.AllViaServer, stage);
    }

    void updateTargets()
    {
        playerTargets.Clear();
        playerColliders.Clear();
        playerColliders = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, 10f, layerMaskPlayer));

        foreach (Collider2D c in playerColliders)
        {
            playerTargets.Add(c.gameObject);
        }

        if(playerTargets.Count > 0)
        {
            Debug.Log("Target count: " + playerTargets.Count);
            foreach (GameObject p in playerTargets)
            {
                Debug.Log(p.name);
            }
        }
        else
        {
            Debug.Log("No targets found");
        }
    }

    void updateEnemies()
    {
        enemyColliders.Clear();
        enemies.Clear();
        // Layermask includes the boss itself
        enemyColliders = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, 10f, layerMaskEnemy));

        foreach (Collider2D c in enemyColliders)
        {
            enemies.Add(c.gameObject);
        }

        if (enemies.Count > 0)
        {
            Debug.Log("Enemy count: " + enemies.Count);
            foreach (GameObject p in enemies)
            {
                Debug.Log(p.name);
            }
        }
        else
        {
            Debug.Log("No enemies found");
        }
    }


    void spawnEnemies(int type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            enemySpawnRot.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
            EnemySpawnRotator.transform.rotation = enemySpawnRot;
            PhotonNetwork.Instantiate(enemyType[type], EnemySpawn.transform.position, Quaternion.identity, 0);
        }
        enemiesSpawned = true;
    }

    /// <summary>
    /// Fires a projectile towards all found targets
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="dmg"></param>
    void fireAtTargets(float speed, int dmg, bool homing, bool onlyRangeds, float chargeTime)
    {
        photonView.RPC("RPC_fireAtTargets", PhotonTargets.AllViaServer, speed, dmg, homing, onlyRangeds, chargeTime);
    }

    void finishFAT()
    {
        fireAtTargetsFin = true;
    }

    [PunRPC]
    void RPC_fireAtTargets(float speed, int dmg, bool homing, bool onlyRangeds, float chargeTime)
    {
        //fireAtTargetsFin = false;
        updateTargets();
        for(int i = 0; i < playerTargets.Count; i++)
        {
            //bool last = false;
            GameObject target = playerTargets[i];
            //if(i == (playerTargets.Count - 1))
            //{
            //    last = true;
            //}
            if (onlyRangeds)
            {
                if (target.GetComponent<Character>().ranged)
                {
                    Vector2 dir = (target.transform.position - transform.position).normalized;
                    GameObject projectileClone = Instantiate(bossProjectile, new Vector3(transform.position.x + dir.x * 1.6f, transform.position.y + dir.y * 1.6f, transform.position.z), Quaternion.identity);
                    //projectileClone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Projectile projectile = projectileClone.GetComponent<Projectile>();
                    projectile.damage = dmg;
                    projectile.target = target;
                    projectile.homing = homing;
                    StartCoroutine(chargeProjectile(speed, dmg, chargeTime, dir, projectileClone));
                    //projectile.LaunchProjectile(dmg, 10f, speed, dir, true);
                }
            }
            else
            {
                Vector2 dir = (target.transform.position - transform.position).normalized;
                GameObject projectileClone = Instantiate(bossProjectile, new Vector3(transform.position.x + dir.x * 1.6f, transform.position.y + dir.y * 1.6f, transform.position.z), Quaternion.identity);
                //projectileClone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Projectile projectile = projectileClone.GetComponent<Projectile>();
                projectile.damage = dmg;
                projectile.target = target;
                projectile.homing = homing;
                StartCoroutine(chargeProjectile(speed, dmg, chargeTime, dir, projectileClone));
                //projectile.LaunchProjectile(dmg, 10f, speed, dir, true);
            }
        }
        // Maybe better to use this if projectile gets destroyed mid charge
        Invoke("finishFAT", chargeTime);

        //foreach(GameObject target in playerTargets)
        //{
        //    if(onlyRangeds)
        //    {
        //        if(target.GetComponent<Character>().ranged)
        //        {
        //            Vector2 dir = (target.transform.position - transform.position).normalized;
        //            GameObject projectileClone = Instantiate(bossProjectile, new Vector3(transform.position.x + dir.x * 1.6f, transform.position.y + dir.y * 1.6f, transform.position.z), Quaternion.identity);
        //            //projectileClone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //            Projectile projectile = projectileClone.GetComponent<Projectile>();
        //            projectile.target = target;
        //            projectile.homing = homing;
        //            StartCoroutine(chargeProjectile(speed, dmg, chargeTime, dir, projectileClone));
        //            //projectile.LaunchProjectile(dmg, 10f, speed, dir, true);
        //        }
        //    }
        //    else
        //    {
        //        Vector2 dir = (target.transform.position - transform.position).normalized;
        //        GameObject projectileClone = Instantiate(bossProjectile, new Vector3(transform.position.x + dir.x * 1.6f, transform.position.y + dir.y * 1.6f, transform.position.z), Quaternion.identity);
        //        //projectileClone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //        Projectile projectile = projectileClone.GetComponent<Projectile>();
        //        projectile.target = target;
        //        projectile.homing = homing;
        //        StartCoroutine(chargeProjectile(speed, dmg, chargeTime, dir, projectileClone));
        //        //projectile.LaunchProjectile(dmg, 10f, speed, dir, true);
        //    }
        //}
    }

    IEnumerator chargeProjectile(float speed, int dmg, float chargeTime, Vector2 dir, GameObject proj)
    {
        float elapsedTime = 0;
        float scalingFactor = 0;
        while(elapsedTime < chargeTime)
        {
            elapsedTime += Time.deltaTime;
            scalingFactor = elapsedTime / chargeTime;
            if(proj)
            {
                proj.GetComponent<Transform>().localScale = new Vector3(scalingFactor, scalingFactor, scalingFactor);
            }
            yield return null;
        }
        if(proj)
        {
            proj.GetComponent<Projectile>().LaunchProjectile(dmg, 3f, speed, dir, true);
        }
        // Last projectile is fired and the action has finished
        //if(last)
        //{
        //    fireAtTargetsFin = true;
        //}
    }

    void rotateBurstRoutineMult(float time, float rotation, int shotsPerSec, int spawnAmount, float distance)
    {
        photonView.RPC("RPC_rotateBurstRoutineMult", PhotonTargets.AllViaServer, time, rotation, shotsPerSec, spawnAmount, distance);
    }

    [PunRPC]
    void RPC_rotateBurstRoutineMult(float time, float rotation, int shotsPerSec, int spawnAmount, float distance)
    {
        rotateBurstFin = false;
        Quaternion startRot = new Quaternion();
        startRot.eulerAngles = new Vector3(0, 0, 0);

        for(int i = 0; i < spawnAmount; i++)
        {
            GameObject rotator = Instantiate(ProjectileRotator, transform.position, startRot, transform);
            startRot.eulerAngles += new Vector3(0, 0, distance);
            StartCoroutine(rotateProjSpawn(time, rotation, shotsPerSec, rotator));
        }
    }

    IEnumerator pushAttackRoutine(float force, float time)
    {
        transform.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(time);

        List<Collider2D> pushTargets = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, 3f, layerMaskPlayer));
        
        foreach(Collider2D c in pushTargets)
        {
            Debug.Log(c.name);
            Debug.Log(force);
            c.gameObject.GetComponent<PlayerCharacter>().Stun(0.7f);
            Vector2 pushDir = (c.gameObject.transform.position - transform.position).normalized;
            c.gameObject.GetComponent<Rigidbody2D>().AddForce(pushDir * force, ForceMode2D.Impulse);
        }
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        pushAttackFin = true;
    }

    void pushAttack(float force, float warningTime)
    {
        photonView.RPC("RPC_PushAttack", PhotonTargets.AllViaServer, force, warningTime);
    }


    [PunRPC]
    void RPC_PushAttack(float force, float warningTime)
    {
        StartCoroutine(pushAttackRoutine(force, warningTime));
    }

    IEnumerator rotateProjSpawn(float time, float rotation, int shotsPerSec, GameObject rotator)
    {
        float rotDelta = 0;
        float elapsedTime = 0;
        float shotInterval = 1f / shotsPerSec;
        float shotTime = 0;
        Quaternion startRot = new Quaternion();
        startRot.eulerAngles = rotator.transform.rotation.eulerAngles;
        Quaternion rot = new Quaternion();
        Transform spawn = rotator.transform.GetChild(0);

        while (elapsedTime < time)
        {
            rotDelta = (rotation * (elapsedTime / time));
            //rot.eulerAngles += new Vector3(0, 0, rotDelta);
            rot.eulerAngles = startRot.eulerAngles + new Vector3(0, 0, rotDelta);
            rotator.transform.rotation = rot;
            elapsedTime += Time.deltaTime;

            if (elapsedTime > shotTime)
            {
                Vector2 dir = (spawn.position - transform.position).normalized;
                GameObject projectileClone = Instantiate(bossProjectile, spawn.position, rot);
                projectileClone.transform.parent = spawn;
                projectileClone.transform.localPosition = new Vector3(0f, 0f, 0f);
                projectileClone.transform.parent = null;
                Projectile projectile = projectileClone.GetComponent<Projectile>();
                projectile.LaunchProjectile(10, 10f, 5f, dir, true);
                shotTime = elapsedTime + shotInterval;
            }
            yield return null;
        }
        Destroy(rotator);
        rotateBurstTime = Time.time + rotateBurstCooldown; 
        rotateBurstFin = true;
    }

    /// <summary>
    /// Rotates the projectile spawn for given amount in given time and fires projectiles at given rate
    /// </summary>
    /// <param name="time">Time the action lasts</param>
    /// <param name="rotation">How much the projectile spawn rotates around the Boss during the given time</param>
    /// <param name="shotsPerSec">How many projectiles are fired per second during this action</param>
    /// <returns></returns>
    //IEnumerator rotateBurstRoutine(float time, float rotation, int shotsPerSec)
    //{

    //    float rotDelta = 0;
    //    float elapsedTime = 0;
    //    float shotInterval = 1f / shotsPerSec;
    //    float shotTime = 0;

    //    while (elapsedTime < time)
    //    {
    //        rotDelta = (rotation * (elapsedTime / time));
    //        projSpawnRot.eulerAngles = new Vector3(0, 0, rotDelta);
    //        ProjectileRotator.transform.rotation = projSpawnRot;
    //        elapsedTime += Time.deltaTime;

    //        if (elapsedTime > shotTime)
    //        {
    //            Vector2 dir = (ProjectileSpawn.transform.position - transform.position).normalized;
    //            GameObject projectileClone = Instantiate(bossProjectile, ProjectileSpawn.transform.position, projSpawnRot);
    //            projectileClone.transform.parent = ProjectileSpawn.transform;
    //            projectileClone.transform.localPosition = new Vector3(0f, 0f, 0f);
    //            projectileClone.transform.parent = null;
    //            Projectile projectile = projectileClone.GetComponent<Projectile>();
    //            projectile.LaunchProjectile(10, 10f, 7.5f, dir, true);
    //            shotTime = elapsedTime + shotInterval;
    //        }
    //        yield return null;
    //    }

    //}

    //[PunRPC]
    //void RPC_rotateBurst(float time, float rotation, int shotsPerSec)
    //{
    //    StartCoroutine(rotateBurstRoutine(time, rotation, shotsPerSec));
    //}

    void rotateBurst(float time, float rotation, int shotsPerSec)
    {
        photonView.RPC("RPC_rotateBurst", PhotonTargets.AllViaServer, time, rotation, shotsPerSec);
    }

    void updateMeleeRange(float force, float warningTime, bool meleeAttack, int dmg)
    {
        meleerangeColliders = Physics2D.OverlapCircleAll(transform.position, 3f, layerMaskPlayer);
        if(meleerangeColliders.Length > 0 && pushAttackFin)
        {
            meleeRangeTimer += Time.deltaTime;
            Debug.Log("meleeRangeTimer: " + meleeRangeTimer);
        }

        if(meleerangeColliders.Length < 1)
        {
            //Debug.Log("meleeRangeTimer reset");
            meleeRangeTimer = 0;
        }

        if(meleeRangeTimer > maxMeleeRangeTime && pushAttackFin && meleeAttackFin)
        {
            // If melee attack is enabled there is 50% chance it will happen instead of push
            if(meleeAttack)
            {
                if (Random.Range(1, 3) == 1)
                {
                    pushAttackFin = false;
                    pushAttack(force, warningTime);
                    meleeRangeTimer = 0;
                }
                else
                {
                    meleeAttackFin = false;
                    MeleeAttack(1f, 360, dmg, 1, 0, 0, 1f);
                    meleeRangeTimer = 0;
                }
            }
            else
            {
                pushAttackFin = false;
                pushAttack(force, warningTime);
                meleeRangeTimer = 0;
            }
            
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.isMine)
        {

            if(PhotonNetwork.isMasterClient)
            {

                if(!fightStarted)
                {
                    foundPlayers = Physics2D.OverlapCircleAll(transform.position, 4f, layerMaskPlayer);
                    //Debug.Log(foundPlayers.Length);
                    if(foundPlayers.Length == PhotonNetwork.room.PlayerCount)
                    {
                        startFight();
                    }
                }

                //Debug.Log("FireAttargetsfin: " + fireAtTargetsFin);
                //Debug.Log("rotateBurstFin: " + rotateBurstFin);
                if (fightStarted && bossStage == 1)
                {
                    updateMeleeRange(30f, 2f, false, 0);

                    if (fireAtTargetsFin && rotateBurstFin)
                    {
                        Debug.Log("ds");
                        fireAtTargetsFin = false;
                        fireAtTargets(3.5f, 10, false, true, 1f);
                    }
                    if(Time.time > rotateBurstTime && rotateBurstFin)
                    {
                        rotateBurstFin = false;
                        rotateBurstRoutineMult(5f, 360, 2, 4, 90);
                    }
                    if(enemiesSpawned && Time.time > enemySpawnTime)
                    {
                        enemiesSpawned = false;
                        spawnEnemies(2, 4);
                        enemySpawnTime = Time.time + 15f;
                    }
                }


                if (fightStarted && bossStage == 2)
                {
                    // First enemy spawn of the stage happens after 5 seconds
                    enemySpawnTime = Time.time + 5f;

                    // Now you can only be in melee range for 2 continous seconds before melee attack is initiated
                    maxMeleeRangeTime = 2f;

                    // Melee hand attack is now enabled
                    updateMeleeRange(30f, 2f, true, 50);

                    if (fireAtTargetsFin && rotateBurstFin)
                    {
                        Debug.Log("ds");
                        fireAtTargetsFin = false;
                        fireAtTargets(3f, 20, true, true, 0.5f);
                    }
                    if (Time.time > rotateBurstTime && rotateBurstFin)
                    {
                        rotateBurstFin = false;
                        rotateBurstRoutineMult(8f, 360, 3, 6, 60);
                    }
                    if (enemiesSpawned && Time.time > enemySpawnTime)
                    {
                        enemiesSpawned = false;
                        spawnEnemies(0, 4);
                        enemySpawnTime = Time.time + 15f;
                    }
                }

                if (fightStarted && bossStage == 3)
                {
                    // First enemy spawn of the stage happens after 5 seconds
                    enemySpawnTime = Time.time + 5f;

                    updateMeleeRange(50f, 1f, true, 50);

                    if (fireAtTargetsFin && rotateBurstFin)
                    {
                        Debug.Log("ds");
                        fireAtTargetsFin = false;
                        fireAtTargets(3.5f, 20, true, true, 0.5f);
                    }
                    if (Time.time > rotateBurstTime && rotateBurstFin)
                    {
                        rotateBurstFin = false;
                        rotateBurstRoutineMult(10f, 360, 3, 8, 60);
                    }
                    if (enemiesSpawned && Time.time > enemySpawnTime)
                    {
                        enemiesSpawned = false;
                        spawnEnemies(3, 4);
                        enemySpawnTime = Time.time + 15f;
                    }
                }

                if (health <= 20000 && bossStage == 1)
                {
                    updateBossStage(2);
                }
                if(health <= 10000 && bossStage == 2)
                {
                    updateBossStage(3);
                }




                //if (Input.GetKeyDown(KeyCode.U))
                //{
                //    updateTargets();
                //    rotateBurst(10, 360, 10);
                //}
                //if (Input.GetKeyDown(KeyCode.I))
                //{
                //    updateTargets();
                //    fireAtTargets(5f, 25, false, false, 5f);
                //}
                //if (Input.GetKeyDown(KeyCode.Y))
                //{
                //    updateTargets();
                //    fireAtTargets(2f, 25, true, true, 0f);
                //}
                //if (Input.GetKeyDown(KeyCode.J))
                //{
                //    spawnEnemies(Random.Range(0, 3), Random.Range(1, 6));
                //}
                //if(Input.GetKeyDown(KeyCode.O))
                //{
                //    rotateBurstRoutineMult(10, 720, 5, 8, 45);
                //}
                //if (Input.GetKeyDown(KeyCode.P))
                //{
                //    pushAttack(50f, 1f);
                //}
                //if (Input.GetKeyDown(KeyCode.L))
                //{
                //    MeleeAttack(10, 360, 10, 4, 0, 90);
                //}
                //if (Input.GetKeyDown(KeyCode.L))
                //{
                //    MeleeAttack(1, 360, 50, 1, 0, 0, 1f);
                //}
            }
        }
    }



    IEnumerator MeleeAttackRotator(float time, float rotation, int dmg, GameObject rotator, float warningTime)
    {
        transform.GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(warningTime);
        float rotDelta = 0;
        float elapsedTime = 0;
        Quaternion startRot = new Quaternion();
        startRot.eulerAngles = rotator.transform.rotation.eulerAngles;
        Quaternion rot = new Quaternion();
        MeleeHand hand = rotator.transform.GetChild(0).gameObject.GetComponent<MeleeHand>();
        hand.damage = dmg;

        while (elapsedTime < time)
        {
            rotDelta = (rotation * (elapsedTime / time));
            rot.eulerAngles = startRot.eulerAngles + new Vector3(0, 0, rotDelta);
            rotator.transform.rotation = rot;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(rotator);
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        meleeAttackFin = true;
    }

    [PunRPC]
    /// <summary>
    /// 
    /// </summary>
    /// <param name="time">How long the action lasts</param>
    /// <param name="rotationAmount">How much the melee hands rotate druing this action</param>
    /// <param name="dmg">Damage dealt by the melee hands</param>
    /// <param name="spawnAmount">How many melee hands are spawned</param>
    /// <param name="startRotZ">Where the first hands is spawned</param>
    /// <param name="angle">angle between hands</param>
    /// <param name="meleeRotator">The prefab</param>
    void MeleeAttackSpawner(float time, float rotationAmount, int dmg, int spawnAmount, float startRotZ, float angle, float warningTime)
    {
        Quaternion startRot = new Quaternion();
        startRot.eulerAngles = new Vector3(0, 0, startRotZ);

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject rotator = Instantiate(MeleeRotator, transform.position, startRot, transform);
            startRot.eulerAngles += new Vector3(0, 0, angle);
            StartCoroutine(MeleeAttackRotator(time, rotationAmount, dmg, rotator, warningTime));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time">How long the action lasts</param>
    /// <param name="rotationAmount">How much the melee hands rotate druing this action</param>
    /// <param name="dmg">Damage dealt by the melee hands</param>
    /// <param name="spawnAmount">How many melee hands are spawned</param>
    /// <param name="startRotZ">Where the first hands is spawned</param>
    /// <param name="angle">angle between hands</param>
    void MeleeAttack(float time, float rotationAmount, int dmg, int spawnAmount, float startRotZ, float angle, float warningTime)
    {
        photonView.RPC("MeleeAttackSpawner", PhotonTargets.AllViaServer, time, rotationAmount, dmg, spawnAmount, startRotZ, angle, warningTime);
    }

    [PunRPC]
    public void TakeDamage(int damage, Vector3 v)
    {
        if(!immune)
        {
            if (PhotonNetwork.isMasterClient)
            {
                health -= damage;
                //healthText.text = "" + health;
                if (health <= 0)
                    if (gameObject != null)
                    {
                        GameManager.Instance.gameWon = true;
                        PhotonNetwork.Destroy(gameObject);
                    }
            }
            else
            {
                photonView.RPC("TakeDamage", PhotonTargets.MasterClient, damage, v);

            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(health);
        }
        else if (stream.isReading)
        {
            this.health = (int)stream.ReceiveNext();
            //healthText.text = "" + health;
        }
    }
}
