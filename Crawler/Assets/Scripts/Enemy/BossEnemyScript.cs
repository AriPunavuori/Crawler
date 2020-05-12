using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossEnemyScript : Photon.MonoBehaviour, IDamageable<int>
{
    int baseHealth;
    public int health;
    int stage2Health;
    int bossStage;
    LayerMask layerMaskPlayer;
    LayerMask layerMaskEnemy;
    public Collider2D[] meleerangeColliders;
    float meleeRangeTimer;
    float maxMeleeRangeTime = 2f;
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
    bool shaking = false;
    SpriteRenderer sr;
    private Material matWhite;
    private Material SpriteLightingMaterial;
    private UnityEngine.Object explosion;
    List<Collider2D> playerColliders = new List<Collider2D>();
    List<GameObject> playerTargets = new List<GameObject>();
    List<Collider2D> enemyColliders = new List<Collider2D>();
    List<GameObject> enemies = new List<GameObject>();
    public GameObject ProjectileRotator;
    public GameObject MeleeRotator;
    public GameObject MeleeRotatorLong;
    public GameObject potion;
    public GameObject weaponUpgrade;
    ParticleSystem BossPushEffect;
    int potionCount;
    int weaponUpgradeCount;
    //public GameObject ProjectileSpawn;
    public GameObject EnemySpawnRotator;
    public GameObject EnemySpawn;
    string[] enemyType = new string[] { "NetworkEnemy0", "NetworkEnemy1", "NetworkEnemy2", "NetworkEnemy3" };
    Quaternion projSpawnRot;
    Quaternion enemySpawnRot;
    public GameObject bossProjectile;
    int playerCount;

    // Start is called before the first frame update
    void Start()
    {
        potionCount = 0;
        weaponUpgradeCount = 0;
        playerCount = PlayerNetwork.Instance.numberOfPlayers;
        baseHealth = 5000 * playerCount;
        health = baseHealth;
        stage2Health = baseHealth / 2;
        bossStage = 1;
        //projSpawnRot = ProjectileRotator.transform.rotation;
        layerMaskPlayer = LayerMask.GetMask("Player");
        layerMaskEnemy = LayerMask.GetMask("Enemy");
        updateTargets();
        rotateBurstTime = Time.time + rotateBurstCooldown;
        enemySpawnTime = Time.time + enemySpawnCooldown;
        sr = GetComponent<SpriteRenderer>();
        BossPushEffect = Resources.Load("BossPushEffect", typeof(ParticleSystem)) as ParticleSystem;
        matWhite = Resources.Load("White", typeof(Material)) as Material;
        explosion = Resources.Load("Explosion");
        SpriteLightingMaterial = sr.material;
    }



    
    [PunRPC]
    private void explosionRPC()
    {
        GameObject explode = (GameObject)Instantiate(explosion);
        explode.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void ResetMaterial()
    {
        sr.material = SpriteLightingMaterial;
    }

    [PunRPC]
    public void disableBar()
    {
        UIManager.Instance.BossHealthCanvas.SetActive(false);
    }

    [PunRPC]
    public void setBossDefeated()
    {
        GameManager.Instance.bossDefeated = true;
    }

    [PunRPC]
    public void RPC_BossDefeated()
    {
        GameManager.Instance.bossDefeated = true;
        UIManager.Instance.BossHealthCanvas.SetActive(false);
        photonView.RPC("disableBar", PhotonTargets.AllViaServer);
        photonView.RPC("setBossDefeated", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    public void RPC_startFight()
    {
        Debug.Log("Fight started");
        UIManager.Instance.BossHealthCanvas.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        GameManager.Instance.bossFightStarted = true;
        immune = false;
        fightStarted = true;
    }
    public void startFight()
    {
        photonView.RPC("RPC_startFight", PhotonTargets.AllViaServer);
    }


    void spawnPotion()
    {
        Debug.Log("Potion spawned");
        potionCount++;
        int x = Random.Range(3, 7);
        int y = Random.Range(3, 7);
        if(Random.Range(0,2) == 1)
        {
            x *= -1;
        }

        if (Random.Range(0, 2) == 1)
        {
            y *= -1;
        }

        photonView.RPC("RPC_spawnPotion", PhotonTargets.AllViaServer, x, y);
    }

    [PunRPC]
    void RPC_spawnPotion(int x, int y)
    {
        Instantiate(potion, transform.position + new Vector3(x, y, 0), Quaternion.identity);
    }

    [PunRPC]
    void RPC_spawnWeaponUpgrade(int x, int y)
    {
        Instantiate(weaponUpgrade, transform.position + new Vector3(x, y, 0), Quaternion.identity);
    }
    void spawnWeaponUpgrade()
    {
        Debug.Log("weapon upgrade spawned");
        weaponUpgradeCount++;
        int x = Random.Range(3, 7);
        int y = Random.Range(3, 7);
        if (Random.Range(0, 2) == 1)
        {
            x *= -1;
        }

        if (Random.Range(0, 2) == 1)
        {
            y *= -1;
        }

        photonView.RPC("RPC_spawnWeaponUpgrade", PhotonTargets.AllViaServer, x, y);
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
            //Debug.Log("Target count: " + playerTargets.Count);
            foreach (GameObject p in playerTargets)
            {
                //Debug.Log(p.name);
            }
        }
        else
        {
            //Debug.Log("No targets found");
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
            //Debug.Log("Enemy count: " + enemies.Count);
            foreach (GameObject p in enemies)
            {
                //Debug.Log(p.name);
            }
        }
        else
        {
            //Debug.Log("No enemies found");
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
            proj.GetComponent<Projectile>().LaunchProjectile(dmg, 10f, speed, dir, true);
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
        Debug.Log("Attack start");
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
        shakeGameObject(this.gameObject, time, time - 0.1f, true);
        //transform.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(time);

        List<Collider2D> pushTargets = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, 3f, layerMaskPlayer));
        List<Collider2D> EnemyPushTargets = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, 3f, layerMaskEnemy));

        Instantiate(BossPushEffect, transform.position + new Vector3(0.07f, -0.05f, 0), Quaternion.identity);
        // For player pushing
        foreach(Collider2D c in pushTargets)
        {
            //Debug.Log(c.name);
            //Debug.Log(force);
            c.gameObject.GetComponent<PlayerCharacter>().Stun(0.7f);
            
            Vector2 pushDir = (c.gameObject.transform.position - transform.position).normalized;
            //c.gameObject.GetComponent<Rigidbody2D>().AddForce(pushDir * force, ForceMode2D.Impulse);
        }
        // For enemy pushing
        //foreach (Collider2D e in EnemyPushTargets)
        //{
        //    //Debug.Log(e.name);
        //    //Debug.Log(force);
        //    
        //    if (e.gameObject.GetComponent<EnemyCharacter>())
        //    {
        //        e.gameObject.GetComponent<EnemyCharacter>().Fly(0.05f);
        //    }
        //    Vector2 pushDir = (e.gameObject.transform.position - transform.position).normalized;
        //    //c.gameObject.GetComponent<Rigidbody2D>().AddForce(pushDir * force, ForceMode2D.Impulse);
        //}

        //transform.GetComponent<SpriteRenderer>().color = Color.white;
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
        yield return new WaitForSeconds(3.5f);
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
            //Debug.Log("meleeRangeTimer: " + meleeRangeTimer);
        }

        if(meleerangeColliders.Length < 1)
        {
            //Debug.Log("meleeRangeTimer reset");
            meleeRangeTimer = 0;
        }

        if(meleeRangeTimer > maxMeleeRangeTime && pushAttackFin && meleeAttackFin && rotateBurstFin)
        {
            // If melee attack is enabled there is 50% chance it will happen instead of push
            if(meleeAttack)
            {
                if (Random.Range(1, 3) == 1)
                {
                    pushAttackFin = false;
                    pushAttack(force, warningTime / 1.5f);
                    meleeRangeTimer = 0;
                }
                else
                {
                    // Long rotating attack, 1 in 3 chance
                    if(Random.Range(1, 4) == 1)
                    {
                        meleeAttackFin = false;
                        MeleeAttack(6f, 220, dmg, 4, Random.Range(0, 360), 90, warningTime, true);
                        meleeRangeTimer = 0;
                    }
                    // Fast swipe attack, 2 in 3 chance
                    else
                    {
                        meleeAttackFin = false;
                        MeleeAttack(1f, 360, dmg, 1, Random.Range(0, 360), 0, warningTime, false);
                        meleeRangeTimer = 0;
                    }
                }
            }
            else
            {
                pushAttackFin = false;
                pushAttack(force, warningTime / 1.5f);
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
                    foundPlayers = Physics2D.OverlapCircleAll(transform.position, 7f, layerMaskPlayer);
                    //Debug.Log(foundPlayers.Length);
                    if(foundPlayers.Length == PhotonNetwork.room.PlayerCount)
                    {
                        startFight();
                    }
                }

                if (fightStarted && bossStage == 1)
                {
                    updateMeleeRange(50f, 1.5f, false, 0);

                    if (fireAtTargetsFin && rotateBurstFin)
                    {
                        fireAtTargetsFin = false;
                        fireAtTargets(4.5f, 10, false, true, 0.5f);
                    }
                    if (Time.time > rotateBurstTime && rotateBurstFin)
                    {
                        rotateBurstFin = false;
                        rotateBurstRoutineMult(8f, 360, 2, 4, 90);
                    }
                    if (enemiesSpawned && Time.time > enemySpawnTime)
                    {
                        enemiesSpawned = false;
                        if (Random.Range(1, 3) == 1)
                        {
                            spawnEnemies(2, (int)(16 * ((float)playerCount / 4)));
                        }
                        else
                        {
                            spawnEnemies(0, (int)(8 * ((float)playerCount / 4)));
                        }

                        enemySpawnTime = Time.time + 12f;
                    }
                }


                if (fightStarted && bossStage == 2)
                {
                    // Now you can only be in melee range for 1.5 continous seconds before melee attack is initiated
                    maxMeleeRangeTime = 1.5f;

                    // Melee hand attack is now enabled
                    updateMeleeRange(80f, 1.5f, true, 50);

                    if (fireAtTargetsFin && rotateBurstFin)
                    {
                        fireAtTargetsFin = false;
                        fireAtTargets(4.2f, 20, true, true, 0.8f);
                    }
                    if (Time.time > rotateBurstTime && rotateBurstFin && meleeAttackFin)
                    {
                        rotateBurstFin = false;
                        rotateBurstRoutineMult(8f, 360, 3, 6, 60);
                    }
                    if (enemiesSpawned && Time.time > enemySpawnTime)
                    {
                        enemiesSpawned = false;

                        if (Random.Range(1, 3) == 1)
                        {
                            spawnEnemies(2, (int)(16 * ((float)playerCount / 4)));
                        }
                        else
                        {
                            spawnEnemies(0, (int)(4 * ((float)playerCount / 4)));
                        }

                        enemySpawnTime = Time.time + 12f;
                    }
                }

                // Potion spawning
                if (potionCount == 0 && health < (baseHealth * 0.75))
                {
                    spawnPotion();
                }
                else if (potionCount == 1 && health < (baseHealth * 0.50))
                {
                    spawnPotion();
                }
                else if (potionCount == 2 && health < (baseHealth * 0.25))
                {
                    spawnPotion();
                }

                // Weaponupgrade spawning
                if (weaponUpgradeCount == 0 && health < (baseHealth * 0.85))
                {
                    spawnWeaponUpgrade();
                }
                else if (weaponUpgradeCount == 1 && health < (baseHealth * 0.35))
                {
                    spawnWeaponUpgrade();
                }
                



                //Debug.Log(enemiesSpawned);
                //Debug.Log(enemySpawnTime);
                //if (fightStarted && bossStage == 3)
                //{
                //    // First enemy spawn of the stage happens after 5 seconds
                //    enemySpawnTime = Time.time + 5f;

                //    updateMeleeRange(50f, 1f, true, 50);

                //    if (fireAtTargetsFin && rotateBurstFin)
                //    {
                //        Debug.Log("ds");
                //        fireAtTargetsFin = false;
                //        fireAtTargets(3.5f, 20, true, true, 0.5f);
                //    }
                //    if (Time.time > rotateBurstTime && rotateBurstFin)
                //    {
                //        rotateBurstFin = false;
                //        rotateBurstRoutineMult(10f, 360, 3, 8, 60);
                //    }
                //    if (enemiesSpawned && Time.time > enemySpawnTime)
                //    {
                //        enemiesSpawned = false;
                //        spawnEnemies(3, 4);
                //        enemySpawnTime = Time.time + 15f;
                //    }
                //}

                // Stage 2 now beings at half health
                if (health <= stage2Health && bossStage == 1)
                {
                    updateBossStage(2);

                    // First enemy spawn of the stage happens after 5 seconds
                    enemySpawnTime = Time.time + 5f;
                }
                //if(health <= (baseHealth * ((float)1 /3)) && bossStage == 2)
                //{
                //    updateBossStage(3);
                //}

                #region CheatCodes
                //if(Input.GetKeyDown(KeyCode.U))
                //{
                //    updateBossStage(2);
                //}

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
                #endregion
            }
        }
    }



    IEnumerator MeleeAttackRotator(float time, float rotation, int dmg, GameObject rotator, float warningTime)
    {
        //transform.GetComponent<SpriteRenderer>().color = Color.yellow;
        yield return new WaitForSeconds(warningTime);
        rotator.gameObject.transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
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
        //transform.GetComponent<SpriteRenderer>().color = Color.white;
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
    void MeleeAttackSpawner(float time, float rotationAmount, int dmg, int spawnAmount, float startRotZ, float angle, float warningTime, bool extended)
    {
        Quaternion startRot = new Quaternion();
        startRot.eulerAngles = new Vector3(0, 0, startRotZ);

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject rotator;
            // Long melee hand
            if (extended)
            {
                rotator = Instantiate(MeleeRotatorLong, transform.position, startRot, transform);
            }
            else
            {
                rotator = Instantiate(MeleeRotator, transform.position, startRot, transform);
            }
            
            //rotator.GetComponent<Collider2D>().enabled = false; // Done in prefab
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
    void MeleeAttack(float time, float rotationAmount, int dmg, int spawnAmount, float startRotZ, float angle, float warningTime, bool extended)
    {
        photonView.RPC("MeleeAttackSpawner", PhotonTargets.AllViaServer, time, rotationAmount, dmg, spawnAmount, startRotZ, angle, warningTime, extended);
    }

    [PunRPC]
    public void TakeDamage(int damage, Vector3 v)
    {

        if(!immune)
        {
            if(health > 0)
            {
                UIManager.Instance.updateBossHealthBar(baseHealth, health, (health -= damage), 0.1f);
            }
            
            sr.material = matWhite;
            if (PhotonNetwork.isMasterClient)
            {
                health -= damage;
                //healthText.text = "" + health;
                if (health <= 0)
                {
                    if (gameObject != null)
                    {
                        photonView.RPC("RPC_BossDefeated", PhotonTargets.MasterClient);
                        
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
                else
                {
                    Invoke("ResetMaterial", 0.1f);
                }
                    
            }
            else
            {
                if (health > 0)
                {
                    Invoke("ResetMaterial", 0.1f);
                }
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

    void shakeGameObject(GameObject objectToShake, float shakeDuration, float decreasePoint, bool objectIs2D = false)
    {
        if (shaking)
        {
            return;
        }
        shaking = true;
        StartCoroutine(shakeGameObjectCOR(objectToShake, shakeDuration, decreasePoint, objectIs2D));
    }

    IEnumerator shakeGameObjectCOR(GameObject objectToShake, float totalShakeDuration, float decreasePoint, bool objectIs2D = false)
    {
        if (decreasePoint >= totalShakeDuration)
        {
            //Debug.LogError("decreasePoint must be less than totalShakeDuration...Exiting");
            yield break; //Exit!
        }

        //Get Original Pos and rot
        Transform objTransform = objectToShake.transform;
        Vector3 defaultPos = objTransform.position;
        Quaternion defaultRot = objTransform.rotation;

        float counter = 0f;

        //Shake Speed
        const float speed = 0.05f;

        //Angle Rotation(Optional)
        const float angleRot = 4;

        //Do the actual shaking
        while (counter < totalShakeDuration)
        {
            counter += Time.deltaTime;
            float decreaseSpeed = speed;
            float decreaseAngle = angleRot;

            //Shake GameObject
            if (objectIs2D)
            {
                //Don't Translate the Z Axis if 2D Object
                Vector3 tempPos = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                tempPos.z = defaultPos.z;
                objTransform.position = tempPos;

                //Only Rotate the Z axis if 2D
                objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRot, angleRot), new Vector3(0f, 0f, 1f));
            }
            else
            {
                objTransform.position = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRot, angleRot), new Vector3(1f, 1f, 1f));
            }
            yield return null;


            //Check if we have reached the decreasePoint then start decreasing  decreaseSpeed value
            if (counter >= decreasePoint)
            {
                //Debug.Log("Decreasing shake");

                //Reset counter to 0 
                counter = 0f;
                while (counter <= decreasePoint)
                {
                    counter += Time.deltaTime;
                    decreaseSpeed = Mathf.Lerp(speed, 0, counter / decreasePoint);
                    decreaseAngle = Mathf.Lerp(angleRot, 0, counter / decreasePoint);

                    //Debug.Log("Decrease Value: " + decreaseSpeed);

                    //Shake GameObject
                    if (objectIs2D)
                    {
                        //Don't Translate the Z Axis if 2D Object
                        Vector3 tempPos = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                        tempPos.z = defaultPos.z;
                        objTransform.position = tempPos;

                        //Only Rotate the Z axis if 2D
                        objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-decreaseAngle, decreaseAngle), new Vector3(0f, 0f, 1f));
                    }
                    else
                    {
                        objTransform.position = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                        objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-decreaseAngle, decreaseAngle), new Vector3(1f, 1f, 1f));
                    }
                    yield return null;
                }

                //Break from the outer loop
                break;
            }
        }
        objTransform.position = defaultPos; //Reset to original postion
        objTransform.rotation = defaultRot;//Reset to original rotation

        shaking = false; //So that we can call this function next time
        //Debug.Log("Done!");
    }

}
