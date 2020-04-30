using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossStage {Stage1, Stage2, Stage3 };

public class BossEnemyScript : Photon.MonoBehaviour, IDamageable<int>
{
    public int health = 1000;
    BossStage bossStage = BossStage.Stage1;
    LayerMask layerMaskPlayer;
    LayerMask layerMaskEnemy;
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
        //projSpawnRot = ProjectileRotator.transform.rotation;
        layerMaskPlayer = LayerMask.GetMask("Player");
        layerMaskEnemy = LayerMask.GetMask("Enemy");
        updateTargets();
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

    [PunRPC]
    void RPC_SpawnEnemies(int type, int amount)
    {
        
        
    }

    void spawnEnemies(int type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            enemySpawnRot.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
            EnemySpawnRotator.transform.rotation = enemySpawnRot;
            PhotonNetwork.Instantiate(enemyType[type], EnemySpawn.transform.position, Quaternion.identity, 0);
        }
    }

    /// <summary>
    /// Fires a projectile towards all found targets
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="dmg"></param>
    void fireAtAllTargets(float speed, int dmg, bool homing)
    {
        photonView.RPC("RPC_fireAtAllTargets", PhotonTargets.AllViaServer, speed, dmg, homing);
    }

    [PunRPC]
    void RPC_fireAtAllTargets(float speed, int dmg, bool homing)
    {
        updateTargets();
        foreach(GameObject target in playerTargets)
        {
            Vector2 dir = (target.transform.position - transform.position).normalized;
            GameObject projectileClone = Instantiate(bossProjectile, new Vector3(transform.position.x + dir.x * 1.6f, transform.position.y + dir.y * 1.6f, transform.position.z), Quaternion.identity);
            Projectile projectile = projectileClone.GetComponent<Projectile>();
            projectile.target = target;
            projectile.homing = homing;
            projectile.LaunchProjectile(dmg, 10f, speed, dir, true);
        }
    }

    void rotateBurstRoutineMult(float time, float rotation, int shotsPerSec, int spawnAmount, float distance)
    {
        photonView.RPC("RPC_rotateBurstRoutineMult", PhotonTargets.AllViaServer, time, rotation, shotsPerSec, spawnAmount, distance);
    }

    [PunRPC]
    void RPC_rotateBurstRoutineMult(float time, float rotation, int shotsPerSec, int spawnAmount, float distance)
    {
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
                projectile.LaunchProjectile(10, 10f, 7.5f, dir, true);
                shotTime = elapsedTime + shotInterval;
            }
            yield return null;
        }
        Destroy(rotator);
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

    // Update is called once per frame
    void Update()
    {
        if(photonView.isMine)
        {

            if(PhotonNetwork.isMasterClient)
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    updateTargets();
                    rotateBurst(10, 360, 10);
                }
                if (Input.GetKeyDown(KeyCode.I))
                {
                    updateTargets();
                    fireAtAllTargets(5f, 25, false);
                }
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    updateTargets();
                    fireAtAllTargets(2f, 25, true);
                }
                if (Input.GetKeyDown(KeyCode.J))
                {
                    spawnEnemies(Random.Range(0, 3), Random.Range(1, 6));
                }
                if(Input.GetKeyDown(KeyCode.O))
                {
                    rotateBurstRoutineMult(10, 720, 5, 8, 45);
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    pushAttack(50f, 1f);
                }
                if (Input.GetKeyDown(KeyCode.L))
                {
                    MeleeAttack(10, 360, 10, 4, 0, 90);
                }
            }
        }
    }



    IEnumerator MeleeAttackRotator(float time, float rotation, int dmg, GameObject rotator)
    {
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
    void MeleeAttackSpawner(float time, float rotationAmount, int dmg, int spawnAmount, float startRotZ, float angle)
    {
        Quaternion startRot = new Quaternion();
        startRot.eulerAngles = new Vector3(0, 0, startRotZ);

        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject rotator = Instantiate(MeleeRotator, transform.position, startRot, transform);
            startRot.eulerAngles += new Vector3(0, 0, angle);
            StartCoroutine(MeleeAttackRotator(time, rotationAmount, dmg, rotator));
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
    void MeleeAttack(float time, float rotationAmount, int dmg, int spawnAmount, float startRotZ, float angle)
    {
        photonView.RPC("MeleeAttackSpawner", PhotonTargets.AllViaServer, time, rotationAmount, dmg, spawnAmount, startRotZ, angle);
    }

    [PunRPC]
    public void TakeDamage(int damage, Vector3 v)
    {
        if (PhotonNetwork.isMasterClient)
        {
            health -= damage;
            //healthText.text = "" + health;
            if (health <= 0)
                if (gameObject != null)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
        }
        else
        {
            photonView.RPC("TakeDamage", PhotonTargets.MasterClient, damage, v);

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
