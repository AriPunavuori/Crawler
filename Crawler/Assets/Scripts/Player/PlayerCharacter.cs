using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Character, IDamageable<int> {

	Rigidbody2D rb2D;
	CircleCollider2D col;
	SpriteRenderer spriteRenderer;
	GameObject projHead;
	GameObject[] players;
	GameObject MainCamera;
	LayerMask layerMaskEnemy;
	int camNum = 0;
	bool potion;
	bool dashing = false;
	bool camFound = false;
    private bool allPlayersFound;
    public bool alive;
	float playerCamOffset = 0.002f;
	float dashCooldown = 3.0f;
	float dashTime = 0.15f;
	float dashTimer;
	float respawnTime = 18.0f;
	float respawnTimer;
	float weaponDowngradeTime = 20f;
	public float weaponDowngradeTimer = 20f;
	public bool shooting;
	// Multiplier for base player speed when dashing
	float dashFactor = 4.0f;
	Vector2 dashVector;
	Vector2 lastDir;
	Vector3 TargetPosition;
    GameObject DirectionIndicator1;
    GameObject DirectionIndicator2;
    GameObject DirectionIndicator3;
    GameObject PlayerTarget1;
    GameObject PlayerTarget2;
    GameObject PlayerTarget3;

    int projectilesPerAttack = 1;

	Stack<int> projectilesPerAttStack = new Stack<int>();
	Stack<float> projectileSpeedsStack = new Stack<float>();
	Stack<int> meleeDamagesStack = new Stack<int>();
	Stack<float> meleeIntervalsStack = new Stack<float>();

	GameObject rotator;
	GameObject meleeIndicator;

	public Animator animator;
	public GameObject playerCam;
	public UIManager uim;

	void Start() {
		respawnTimer = respawnTime;
		rotator = transform.Find("ProjectileHeading").gameObject;
		meleeIndicator = rotator.transform.Find("MeleeIndicator").gameObject;
		meleeIndicator.SetActive(false);
		alive = true;
		rb2D = GetComponent<Rigidbody2D>();
		col = GetComponent<CircleCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		layerMaskEnemy = LayerMask.GetMask("Enemy");
		projHead = transform.Find("ProjectileHeading").gameObject;
		MainCamera = transform.Find("Main Camera").gameObject;
        DirectionIndicator1 = transform.Find("DirectionIndicator1").gameObject;
        DirectionIndicator2 = transform.Find("DirectionIndicator2").gameObject;
        DirectionIndicator3 = transform.Find("DirectionIndicator3").gameObject;
        players = GameObject.FindGameObjectsWithTag("Player");
		SetCharacterAttributes();
		meleeIndicator.transform.localScale = new Vector3(attackRange, .1f, 1);
		meleeIndicator.transform.localPosition = new Vector3(attackRange / 2, 0, 0);
		meleeIndicator.SetActive(false);
		uim = GameObject.Find("UIManager").GetComponent<UIManager>();
        var photonView = GetComponent<PhotonView>(); // Jos bugeja ni tässä.
        Invoke("FindPlayers", 1f);
        /*
        if (!PhotonNetwork.isMasterClient)
			return;
		var photonView = GetComponent<PhotonView>();
        */
		if (photonView != null) {
			PlayerManager.Instance.ModifyHealth(photonView.owner, health);
		}
		players = GameObject.FindGameObjectsWithTag("Player");
	}

    void FindPlayers()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (photonView.isMine)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (this.gameObject.GetComponent<PhotonView>().ownerId != players[i].GetComponent<PhotonView>().ownerId)
                {
                    if (PlayerTarget1 == null)
                    {
                        DirectionIndicator1.SetActive(true);
                        var SpriteRenderer = DirectionIndicator1.GetComponent<SpriteRenderer>();
                        SpriteRenderer.color = Color.red;
                        PlayerTarget1 = players[i].gameObject;
                        continue;
                    }
                    else if (PlayerTarget2 == null)
                    {
                        DirectionIndicator2.SetActive(true);
                        var SpriteRenderer = DirectionIndicator2.GetComponent<SpriteRenderer>();
                        SpriteRenderer.color = Color.blue;
                        PlayerTarget2 = players[i].gameObject;
                        continue;
                    }
                    else if (PlayerTarget3 == null)
                    {
                        DirectionIndicator3.SetActive(true);
                        var SpriteRenderer = DirectionIndicator3.GetComponent<SpriteRenderer>();
                        SpriteRenderer.color = Color.green;
                        PlayerTarget3 = players[i].gameObject;
                        continue;
                    }
                }
            }
            allPlayersFound = true;
        }
    }

    public void TakeDamage(int damage) {
		var random = Random.Range(0, 4);
		AudioFW.Play("PlayerTakesDamage" + random);
		SetHealth(-damage, this);
	}
	public void SetHealth(int amount, PlayerCharacter pc) {
		PhotonView photonView = pc.GetComponent<PhotonView>();
		if (photonView != null) {
			PlayerManager.Instance.ModifyHealth(photonView.owner, amount);
			//print(photonView.owner);
		}
	}

	[PunRPC]
	public void Die() {
		respawnTimer = respawnTime;
		Debug.Log(gameObject.name + " died");
		rb2D.isKinematic = true;
		rb2D.velocity = Vector2.zero;
		movement.x = 0;
		movement.y = 0;
		// Disable collider
		col.enabled = false;
		spriteRenderer.enabled = false;
		// Enable ProjectileHeading child GameObject
		projHead.SetActive(false);
		dashCooldown = 5.0f;
		alive = false;

		// Clear all weapon upgrade stacks (SetCharacterAttributes is run at respawn)
		projectilesPerAttStack.Clear();
		projectilesPerAttack = 1;
		projectileSpeedsStack.Clear();
		meleeDamagesStack.Clear();
		meleeIntervalsStack.Clear();

		weaponDowngradeTimer = weaponDowngradeTime;

		weaponLevel = 0;

		// Try to find a remote camera
		players = GameObject.FindGameObjectsWithTag("Player");
		if (players.Length > 1) {
			findCamera();
		} else {
			Debug.Log("Cant find camera, no alive players found");
		}

		if (photonView.isMine) {
			AudioFW.StopLoop("GameLoop");
			AudioFW.Play("PlayerDead");
			photonView.RPC("Die", PhotonTargets.Others);
		}

	}

	[PunRPC]
	void respawn() {
		Debug.Log(gameObject.name + " respawned");
		// Spawn at currently chosen remote cam/player position
		gameObject.transform.position = players[camNum].transform.position;
		// Fix camera position
		MainCamera.transform.position = gameObject.transform.position + new Vector3(0, 0, -11);
		// Reset character attributes
		SetCharacterAttributes();
		SetHealth(health, this);
		rb2D.isKinematic = false;
		// Enable collider
		col.enabled = true;
		spriteRenderer.enabled = true;
		// Enable ProjectileHeading child GameObject
		projHead.SetActive(true);
		alive = true;
	}


	void findCamera() { // Try to find a camera to "follow"
		bool cameraFound = false;
		int alivePlayers = 0;
		for (int i = 0; i < players.Length; i++) {
			if (players[i].GetComponent<PlayerCharacter>().alive) {
				alivePlayers++;
			}
		}
		//Debug.Log("Alive players" + alivePlayers);
		if (alivePlayers > 0) {
			while (!cameraFound) {
				if (camNum == (players.Length - 1)) {

					camNum = 0;
				} else {
					camNum++;
				}
				if (players[camNum].GetPhotonView().viewID != photonView.viewID && players[camNum].gameObject.GetComponent<PlayerCharacter>().alive) {
					//Debug.Log("Found Cam num " + camNum);
					//Debug.Log("Camera found");
					//Debug.Log("Found camera ID: " + players[camNum].GetPhotonView().viewID);
					MainCamera.transform.position = players[camNum].transform.Find("Main Camera").transform.position;
					cameraFound = true;
					// Global camFound variable
					camFound = true;
				}
			}
		} else {
			Debug.Log("No alive players found");
		}
	}

	IEnumerator recoil(Vector3 recoilOffset, float recoilTime)
	{
		float elapsedTime = 0;
		Vector3 startingPos = playerCam.transform.position;

		while (elapsedTime < recoilTime)
		{
			playerCam.transform.position = Vector3.Lerp(startingPos, startingPos + recoilOffset, (elapsedTime / recoilTime));
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		// FIX THIS--> (use attackInterval?)
		elapsedTime = 0;
		startingPos = playerCam.transform.position;
		while (elapsedTime < recoilTime)
		{
			playerCam.transform.position = Vector3.Lerp(startingPos, startingPos - recoilOffset, (elapsedTime / recoilTime));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	void Update() {


		if (photonView.isMine) {
            if (allPlayersFound == true)
            {
                if (players.Length > 1)
                {
                    var heading = PlayerTarget1.gameObject.transform.position - this.transform.position;
                    var distance = heading.magnitude;
                    var direction = heading / distance;
                    DirectionIndicator1.transform.localPosition = direction;
                    if (players.Length > 2)
                    {
                        heading = PlayerTarget2.gameObject.transform.position - this.transform.position;
                        distance = heading.magnitude;
                        direction = heading / distance;
                        DirectionIndicator2.transform.localPosition = direction;
                        if (players.Length > 3)
                        {
                            heading = PlayerTarget3.gameObject.transform.position - this.transform.position;
                            distance = heading.magnitude;
                            direction = heading / distance;
                            DirectionIndicator3.transform.localPosition = direction;
                        }
                    }
                }
            }
            // When the player is dead
            if (!alive) {
				respawnTimer -= Time.deltaTime;
				uim.SetInfoText("You Died!\n" + "Respawn in " + respawnTimer.ToString("f0"), 1);
				if (Input.GetMouseButtonDown(0)) {
					if (players.Length > 1) {
						findCamera();
					} else {
						Debug.Log("Cant search for a remote camera, only 1 player in the game");
					}
				}
			}

			// Respawn
			if (respawnTimer <= 0) {
				respawnTimer = respawnTime;
				uim.SetInfoText("", 1);
				respawn();
				AudioFW.PlayLoop("GameLoop");
				photonView.RPC("respawn", PhotonTargets.Others);
			}

			// When the player is alive
			if (alive) {
				attackTimer -= Time.deltaTime;
				// Health potion input
				if (Input.GetKeyDown(KeyCode.H)) {
					UsePotion();
				}
				// Attack input
				if (attackTimer < 0 && Input.GetKey(KeyCode.Mouse0)) {
					Attack();

					// Camera recoil when shooting. Kinda shit tbh
					if (ranged)
					{
						shooting = true;
						
						Vector3 camSP = Camera.main.WorldToScreenPoint(transform.position); // name misleading?
						Vector2 mouseVectorN = new Vector2(Input.mousePosition.x - camSP.x, Input.mousePosition.y - camSP.y).normalized;
						//Vector3 campos = playerCam.transform.position;
						//playerCam.transform.position = new Vector3(Mathf.Lerp(campos.x, -mouseVectorN.x, 0.5f) * 10, Mathf.Lerp(campos.y, -mouseVectorN.y, 0.5f) * 10, campos.z);
						
						// When the player is still
						if(movement.magnitude <= 0)
						{
							StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 0.05f, 0.05f));
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 0.05f, attackInterval / 2));
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 1f, 5f));
						}
						else
						{
							StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 0.05f, 0.05f));
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 0.05f, attackInterval / 2));
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 1f, 5f));
						}
						

					}
				}
				else if(attackTimer < 0)
				{
					if(ranged)
					{
						shooting = false;
					}
					
				}
				// Movement input
				movement.x = Input.GetAxisRaw("Horizontal");
				movement.y = Input.GetAxisRaw("Vertical");

				// For testing weaponupgrades
				if (Input.GetKeyDown(KeyCode.N)) {
					GetWeaponUpgrade();
				}

				if (Input.GetKeyDown(KeyCode.M)) {
					weaponDowngrade();
				}

				if (weaponLevel > 0) {
					if ((weaponDowngradeTimer - Time.deltaTime) > 0) {
						weaponDowngradeTimer -= Time.deltaTime;
					} else {
						weaponDowngradeTimer = 0;
					}
					if (weaponDowngradeTimer <= 0) {
						weaponDowngrade();
					}
				}



				if (movement.x != 0 || movement.y != 0) {
					lastDir = new Vector2(movement.x, movement.y);
				}

				// Camera movement
				Vector3 mousePos = Camera.main.WorldToScreenPoint(transform.position); // name misleading?
				playerCam.transform.position = new Vector3((Input.mousePosition.x - mousePos.x) * playerCamOffset, (Input.mousePosition.y - mousePos.y) * playerCamOffset, playerCam.transform.position.z) + transform.position;

				// Setting the correct animation/stance depending on the current mouse position and if moving or not
				Vector2 mouseVector = new Vector2(Input.mousePosition.x - mousePos.x, Input.mousePosition.y - mousePos.y);
				//Debug.Log(mouseVector);

				animator.SetFloat("Horizontal", mouseVector.x);
				animator.SetFloat("Vertical", mouseVector.y);
				animator.SetFloat("Magnitude", movement.magnitude);

				if (Input.GetKeyDown(KeyCode.Space) && dashTimer <= 0) {
					Debug.Log("Dashing");
					dashing = true;
					dashTimer = dashCooldown;

					// Initial dash direction from mouse position
					//Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
					//dashVector = new Vector2(Input.mousePosition.x - position.x, Input.mousePosition.y - position.y);

					// Dashing to the last movement direction from keyboard inputs
					dashVector = lastDir;
				}

				if (dashing) {
					//dashTimer += Time.deltaTime;
					// Updating dashing direction mid dash with mouse position.
					//Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
					//dashVector = new Vector2(Input.mousePosition.x - position.x, Input.mousePosition.y - position.y);

					// Updating dashing direction mid dash with keyboard inputs. Comment to have static direction
					//dashVector = lastDir;
					if (dashTimer <= dashCooldown - dashTime) {
						dashing = false;
						//dashTimer = dashTime;
					}
				}
				dashTimer -= Time.deltaTime;
			}
		} else {
			transform.position = Vector3.Lerp(transform.position, TargetPosition, 0.1f);
			rb2D.isKinematic = true;
		}
	}

	private void FixedUpdate() {

		// Move the PlayerCharacter of the correct player
		if (photonView.isMine) {

			// When the player is dead
			if (!alive) {
				// If remote camera is found follow a camera/player with current camNum.
				if (camFound) {
					//MainCamera.transform.position = players[camNum].transform.Find("Main Camera").transform.position;
					MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, players[camNum].transform.Find("Main Camera").transform.position, 0.1f);
				}
			}


			if (rb2D != null)
				if (dashing) {
					//rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed * dashFactor;
					rb2D.velocity = dashVector.normalized * speed * dashFactor;
				} else {
					if(!shooting)
					{
						rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed;
					}
					else
					{
						// When player is shooting slow his movement speed. Could also do for melee
						rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed * 0.7f;
					}
					
				}
			//Debug.Log(rb2D.velocity.magnitude);
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			stream.SendNext(transform.position);
		} else {
			TargetPosition = (Vector3)stream.ReceiveNext();
		}
	}

	#region Powerups
	public void UsePotion() {
		if (potion) {
			print(CheckCharacterHealt(characterType));
			if (photonView.isMine) {
				uim.UpdatePotion();
			}
			if (health + 100 > CheckCharacterHealt(characterType)) {
				SetHealth(CheckCharacterHealt(characterType) - health, this);
			} else
				SetHealth(100, this);
			potion = false;
		}
	}

	public void GetPotion() {
		potion = true;
		if (photonView.isMine) {
			uim.UpdatePotion();
		}
	}

	public void GetSpeed() {
		speed *= 1.25f;
	}

	public void GetWeaponUpgrade() {
		uim.setPowerupUITimer(weaponDowngradeTime, weaponLevel + 1);
		if (photonView.isMine)
			uim.SetInfoText("Picked up a weapon upgrade!", 2);
		Debug.Log("Weapon level upgraded");
		if (ranged) {
			if (weaponLevel % 2 == 0) {
				projectilesPerAttStack.Push(projectilesPerAttack);
				projectilesPerAttack++;
				Debug.Log("Projectiles per attack: " + projectilesPerAttack);
			} else {
				projectileSpeedsStack.Push(projectileSpeed);
				projectileSpeed *= 2f;
				Debug.Log("Projectile speed: " + projectileSpeed);
			}
		} else {
			if (weaponLevel % 2 == 0) {
				meleeDamagesStack.Push(damage);
				damage *= (3 / 2);
				Debug.Log("Damage: " + damage);
			} else {
				meleeIntervalsStack.Push(attackInterval);
				attackInterval *= .75f;
				Debug.Log("AttackInterval: " + attackInterval);
			}
		}
		weaponDowngradeTimer = weaponDowngradeTime;
		weaponLevel++;
	}

	public void weaponDowngrade() {
		if (weaponLevel > 0) {
			Debug.Log("Weapon level downgraded");
			if (weaponLevel > 1) {
				uim.setPowerupUITimer(weaponDowngradeTime, weaponLevel - 1);
			} else {
				uim.setPowerupUITimer(0, 0);
			}

			if (ranged) {
				if (weaponLevel % 2 == 0) {
					projectileSpeed = projectileSpeedsStack.Pop();
					Debug.Log("Projectile speed reduced");
					Debug.Log("Projectile speed: " + projectileSpeed);
				} else {
					projectilesPerAttack = projectilesPerAttStack.Pop();
					Debug.Log("Projectiles per attack decremented");
					Debug.Log("Projectiles per attack: " + projectilesPerAttack);
				}
			} else {
				if (weaponLevel % 2 == 0) {
					attackInterval = meleeIntervalsStack.Pop();
					Debug.Log("Attack interval increased");
					Debug.Log("AttackInterval: " + attackInterval);
				} else {
					damage = meleeDamagesStack.Pop();
					Debug.Log("Damage reduced");
					Debug.Log("Damage: " + damage);
				}
			}
			weaponDowngradeTimer = weaponDowngradeTime;
			weaponLevel--;
		} else {
			Debug.Log("Cannot downgrade weapon: weapon level is less than 1");
		}
	}

	#endregion

	public void Attack() {
		if (ranged) {
			Shoot(projectilesPerAttack, projectileSpeed);
			photonView.RPC("Shoot", PhotonTargets.Others, projectilesPerAttack, projectileSpeed);
		} else {
			Melee();
			photonView.RPC("Melee", PhotonTargets.Others);
		}
		attackTimer = attackInterval;
	}



	[PunRPC]
	public void Shoot(int amount, float projSpeed) {
		float gap = .5f;
		var offset = (amount - 1f) / 2 * gap;

		for (int i = 0; i < amount; i++) {
			GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
			projectileClone.transform.parent = projectileSpawn.transform;
			projectileClone.transform.localPosition = new Vector3(-(offset - i * (gap / 2)), offset - i * gap, 0f);
			projectileClone.transform.parent = null;
			Projectile projectile = projectileClone.GetComponent<Projectile>();
			projectile.LaunchProjectile(damage, attackRange, projSpeed, (projectileSpawn.transform.position - transform.position).normalized, false);
		}
	}

	[PunRPC]
	public void Melee() {
		if (PhotonNetwork.isMasterClient) {
			Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, layerMaskEnemy);
			foreach (var hit in hits) {
				IDamageable<int> iDamageable = hit.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
				if (iDamageable != null) {
					iDamageable.TakeDamage(damage);
				}
			}
		}
		// Play animation
		meleeIndicator.SetActive(true);
		StartCoroutine(RotateMe(Vector3.forward * 85, attackInterval * .3f));
	}
	IEnumerator RotateMe(Vector3 byAngles, float inTime) {
		print("Melee animation");
		var fromAngle = Quaternion.Euler(rotator.transform.eulerAngles - byAngles);
		var toAngle = Quaternion.Euler(rotator.transform.eulerAngles + byAngles);
		for (var t = 0f; t < 1; t += Time.deltaTime / inTime) {
			rotator.transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
			if (t >= .9f)
				meleeIndicator.SetActive(false);
			yield return null;
		}
	}
}