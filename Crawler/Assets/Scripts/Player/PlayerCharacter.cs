using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Character, IDamageable<int> {

	Rigidbody2D rb2D;
	CircleCollider2D col;
	SpriteRenderer spriteRenderer;
	SpriteRenderer PlayerTarget1Renderer;
	SpriteRenderer PlayerTarget2Renderer;
	SpriteRenderer PlayerTarget3Renderer;
	Sprite lightmagiLeft;
	Sprite lightOnileft;
	Sprite darkMagiLeft;
	Sprite darkOniLeft;
	GameObject projHead;
	GameObject[] players;
	GameObject MainCamera;
	GameObject IndicatorColliderObj;
	GameObject DirectionIndicator1;
	GameObject DirectionIndicator2;
	GameObject DirectionIndicator3;
	GameObject PlayerTarget1;
	GameObject PlayerTarget2;
	GameObject PlayerTarget3;
	public GameObject healthChangeIndicator;
	public GameObject healEffectParticles;
	public GameObject damageEffectParticles;
	LayerMask layerMaskEnemy;
	LayerMask layerMaskPlayer;
	LayerMask layerMaskIndicator;
	int camNum = 0;
	bool potion;
	bool dashing = false;
	bool camFound = false;
	private bool allPlayersFound;
	public bool alive;
	float playerCamOffset = 0.002f;
	//float specialCooldown = 3.0f;
	float dashLength = 0.15f;
	float specialTime;
	float respawnTime = 18.0f;
	float respawnTimer;
	float weaponDowngradeTime = 20f;
	public float weaponDowngradeTimer = 20f;
	public bool shooting;

	float damageCooldownLength = .5f;
	float timeDamageTaken;

	// Specials

	float specialEffectArea = 7.5f;
	int specialAmount = 10;
	float pushForce = 30f;

	// Multiplier for base player speed when dashing
	float dashFactor = 4.0f;
	Vector2 dashVector;
	Vector2 lastDir;
	Vector3 TargetPosition;
	Vector2 heading;
	Vector2 direction;
	float distance;


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
		layerMaskPlayer = LayerMask.GetMask("Player");
		layerMaskIndicator = LayerMask.GetMask("DirectionIndicator");
		projHead = transform.Find("ProjectileHeading").gameObject;
		MainCamera = transform.Find("Main Camera").gameObject;
		DirectionIndicator1 = transform.Find("DirectionIndicator1").gameObject;
		DirectionIndicator2 = transform.Find("DirectionIndicator2").gameObject;
		DirectionIndicator3 = transform.Find("DirectionIndicator3").gameObject;
		IndicatorColliderObj = MainCamera.transform.Find("IndicatorCollider").gameObject;
		lightmagiLeft = Resources.Load<Sprite>("Sprites/32magi_run_left 2");
		lightOnileft = Resources.Load<Sprite>("Sprites/32oni_run_left");
		darkMagiLeft = Resources.Load<Sprite>("Sprites/dark_magi_run_left");
		darkOniLeft = Resources.Load<Sprite>("Sprites/Dark_Oni_run_left");
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

	void FindPlayers() {
		players = GameObject.FindGameObjectsWithTag("Player");
		if (photonView.isMine) {
			IndicatorColliderObj.SetActive(true);
			for (int i = 0; i < players.Length; i++) {
				if (this.gameObject.GetComponent<PhotonView>().ownerId != players[i].GetComponent<PhotonView>().ownerId) {
					if (PlayerTarget1 == null) {
						DirectionIndicator1.SetActive(true);
						var SpriteRenderer = DirectionIndicator1.GetComponent<SpriteRenderer>();
						PlayerTarget1 = players[i].gameObject;
						PlayerTarget1Renderer = PlayerTarget1.GetComponent<SpriteRenderer>();
						var charType = PlayerTarget1.GetComponent<Character>().characterType;
						if (charType == EntityType.Hero0) {
							SpriteRenderer.sprite = lightmagiLeft;
						} else if (charType == EntityType.Hero1) {
							SpriteRenderer.sprite = lightOnileft;
						} else if (charType == EntityType.Hero2) {
							SpriteRenderer.sprite = darkMagiLeft;
						} else if (charType == EntityType.Hero3) {
							SpriteRenderer.sprite = darkOniLeft;
						}
						continue;
					} else if (PlayerTarget2 == null) {
						DirectionIndicator2.SetActive(true);
						var SpriteRenderer = DirectionIndicator2.GetComponent<SpriteRenderer>();
						PlayerTarget2 = players[i].gameObject;
						PlayerTarget2Renderer = PlayerTarget2.GetComponent<SpriteRenderer>();
						var charType = PlayerTarget2.GetComponent<Character>().characterType; ;
						if (charType == EntityType.Hero0) {
							SpriteRenderer.sprite = lightmagiLeft;
						} else if (charType == EntityType.Hero1) {
							SpriteRenderer.sprite = lightOnileft;
						} else if (charType == EntityType.Hero2) {
							SpriteRenderer.sprite = darkMagiLeft;
						} else if (charType == EntityType.Hero3) {
							SpriteRenderer.sprite = darkOniLeft;
						}
						continue;

					} else if (PlayerTarget3 == null) {
						DirectionIndicator3.SetActive(true);
						var SpriteRenderer = DirectionIndicator3.GetComponent<SpriteRenderer>();
						PlayerTarget3 = players[i].gameObject;
						PlayerTarget3Renderer = PlayerTarget3.GetComponent<SpriteRenderer>();
						var charType = PlayerTarget3.GetComponent<Character>().characterType; ;
						if (charType == EntityType.Hero0) {
							SpriteRenderer.sprite = lightmagiLeft;
						} else if (charType == EntityType.Hero1) {
							SpriteRenderer.sprite = lightOnileft;
						} else if (charType == EntityType.Hero2) {
							SpriteRenderer.sprite = darkMagiLeft;
						} else if (charType == EntityType.Hero3) {
							SpriteRenderer.sprite = darkOniLeft;
						}
						continue;
					}
				}
			}
			allPlayersFound = true;
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

	IEnumerator recoil(Vector3 recoilOffset, float recoilTime) {
		float elapsedTime = 0;
		Vector3 startingPos = playerCam.transform.position;
		Vector3 offsetPos = Vector3.zero;

		while (elapsedTime < recoilTime) {
			playerCam.transform.position = Vector3.Lerp(startingPos, startingPos + recoilOffset, (elapsedTime / recoilTime));
			elapsedTime += Time.deltaTime;
			offsetPos = Vector3.Lerp(startingPos, startingPos + recoilOffset, (elapsedTime / recoilTime));
			yield return null;
		}

		// FIX THIS--> (use attackInterval?)
		elapsedTime = 0;
		startingPos = offsetPos;
		playerCam.transform.position = startingPos;
		while (elapsedTime < recoilTime) {
			Vector3 mousePos = Camera.main.WorldToScreenPoint(transform.position); // name misleading?
																				   //playerCam.transform.position = new Vector3((Input.mousePosition.x - mousePos.x) * playerCamOffset, (Input.mousePosition.y - mousePos.y) * playerCamOffset, playerCam.transform.position.z) + transform.position;

			//playerCam.transform.position = Vector3.Lerp(startingPos, startingPos - recoilOffset, (elapsedTime / recoilTime));

			// Now recoils forward to the mouse direction when the backwards recoil ends, not back to the start direction. (forward recoil direction is updated every frame depeding on the mouse position)
			playerCam.transform.position = Vector3.Lerp(startingPos, new Vector3((Input.mousePosition.x - mousePos.x) * playerCamOffset, (Input.mousePosition.y - mousePos.y) * playerCamOffset, playerCam.transform.position.z) + transform.position, (elapsedTime / recoilTime));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}
	void IndicatePlayers() {
		if (players.Length > 1) {
			if (PlayerTarget1Renderer != null) {
				if (!PlayerTarget1Renderer.isVisible) {
					DirectionIndicator1.SetActive(true);
					heading = PlayerTarget1.gameObject.transform.position - this.transform.position;
					distance = heading.magnitude;
					direction = heading / distance;
					DirectionIndicator1.transform.position = Physics2D.Raycast(MainCamera.transform.position, direction, Mathf.Infinity, layerMaskIndicator, Mathf.Infinity, Mathf.Infinity).point;
				} else {
					DirectionIndicator1.SetActive(false);
				}
			}
			if (players.Length > 2) {
				if (PlayerTarget2Renderer != null) {
					if (!PlayerTarget2Renderer.isVisible) {
						DirectionIndicator2.SetActive(true);
						heading = PlayerTarget2.gameObject.transform.position - this.transform.position;
						distance = heading.magnitude;
						direction = heading / distance;
						DirectionIndicator2.transform.position = Physics2D.Raycast(MainCamera.transform.position, direction, Mathf.Infinity, layerMaskIndicator, Mathf.Infinity, Mathf.Infinity).point;
					} else {
						DirectionIndicator2.SetActive(false);
					}
				}
				if (players.Length > 3) {
					if (PlayerTarget3Renderer != null) {
						if (!PlayerTarget3Renderer.isVisible) {
							DirectionIndicator3.SetActive(true);
							heading = PlayerTarget3.gameObject.transform.position - this.transform.position;
							distance = heading.magnitude;
							direction = heading / distance;
							DirectionIndicator3.transform.position = Physics2D.Raycast(MainCamera.transform.position, direction, Mathf.Infinity, layerMaskIndicator, Mathf.Infinity, Mathf.Infinity).point;
						} else {
							DirectionIndicator3.SetActive(false);
						}
					}
				}
			}
		}
	}
	void Update() {
		if (photonView.isMine) {
			// Player Direction Indicator
			if (allPlayersFound == true) {
				IndicatePlayers();
			}


			// Respawn
			if (respawnTimer <= 0 || Input.GetKeyDown(KeyCode.R)) {
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
				if (Input.GetKeyDown(KeyCode.H)|| Input.GetAxis("Fire3") > 0.5f) {
					UsePotion();
				}
				// Attack input
				if (attackTimer < 0 && Input.GetAxis("Fire1") > 0.5f) {
					Attack();

					// Camera recoil when shooting. Kinda shit tbh
					if (ranged) {
						shooting = true;

						Vector3 camSP = Camera.main.WorldToScreenPoint(transform.position); // name misleading?
						Vector2 mouseVectorN = new Vector2(Input.mousePosition.x - camSP.x, Input.mousePosition.y - camSP.y).normalized;
						//Vector3 campos = playerCam.transform.position;
						//playerCam.transform.position = new Vector3(Mathf.Lerp(campos.x, -mouseVectorN.x, 0.5f) * 10, Mathf.Lerp(campos.y, -mouseVectorN.y, 0.5f) * 10, campos.z);

						// When the player is still
						if (movement.magnitude <= 0) {
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 0.05f, 0.05f));
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 0.05f, attackInterval / 2));
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 1f, 1f, false));
						} else {
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 0.05f, 0.05f));
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 0.05f, attackInterval / 2));
							//StartCoroutine(recoil(new Vector3(-mouseVectorN.x, -mouseVectorN.y, 0f) * 1f, 5f, true));
						}
					}
				} else if (attackTimer < 0) {
					if (ranged) {
						shooting = false;
					}

				}
				// Movement input
				movement.x = Input.GetAxisRaw("Horizontal");
				movement.y = Input.GetAxisRaw("Vertical");


				Vector3 camPosition = Camera.main.WorldToScreenPoint(transform.position); // name misleading?

				if (animator.GetCurrentAnimatorStateInfo(0).IsName("LightOniAttack")) {

					if (Input.mousePosition.x - camPosition.x < 0) {
						spriteRenderer.flipX = true;
					} else {
						spriteRenderer.flipX = false;
					}
				} else {
					spriteRenderer.flipX = false;
				}

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



				animator.SetFloat("Horizontal", projHead.transform.right.x);
				animator.SetFloat("Vertical", projHead.transform.right.y);
				animator.SetFloat("Magnitude", movement.magnitude);


				if ((Input.GetKeyDown(KeyCode.Space)||(Input.GetAxis("Fire2") > 0.5f)) && specialTime + specialCooldown <= Time.time) {
					specialTime = Time.time;
					Debug.Log("Special");
					if (characterType == EntityType.Hero0) {
						// Light MaGi - Dash
						Dash();
					} else if (characterType == EntityType.Hero1) {
						// Light Oni - Area Heal
						AreaHeal();
					} else if (characterType == EntityType.Hero2) {
						// Dark MaGi - Push
						Push();
					} else if (characterType == EntityType.Hero3) {
						// Dark Oni - Area Damage
						AreaDamage();
					}
					uim.setSpecialCooldownTimer(specialTime + specialCooldown, specialCooldown);
				}

				if (dashing) {
					//dashTimer += Time.deltaTime;
					// Updating dashing direction mid dash with mouse position.
					//Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
					//dashVector = new Vector2(Input.mousePosition.x - position.x, Input.mousePosition.y - position.y);
					// Updating dashing direction mid dash with keyboard inputs. Comment to have static direction
					//dashVector = lastDir;
					if (specialTime + dashLength <= Time.time) {
						dashing = false;
					}
				}

			} else { // Camera switching when the player is dead
				respawnTimer -= Time.deltaTime;
				uim.SetInfoText("You Died\n" + "Respawn in " + respawnTimer.ToString("f0"), 1);
				if (Input.GetMouseButtonDown(0)) {
					if (players.Length > 1) {
						findCamera();
					} else {
						Debug.Log("Cant search for a remote camera, only 1 player in the game");
					}
				}
				// If remote camera is found follow a camera/player with current camNum.
				if (camFound) {
					//MainCamera.transform.position = players[camNum].transform.Find("Main Camera").transform.position;
					MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, players[camNum].transform.Find("Main Camera").transform.position, 0.1f);
				}
			}
		} else {
			transform.position = Vector3.Lerp(transform.position, TargetPosition, 0.1f);
			rb2D.isKinematic = true;
		}
	}

	public void TakeDamage(int damage, Vector3 recoilOffset) {
		if (Time.time > timeDamageTaken + damageCooldownLength) {
			timeDamageTaken = Time.time;
			// If the player took more than 10 damage start recoil
			if (damage > 10)
			{
				// Scale recoil based on damage 
				float recoilMultiplier = Mathf.Pow((float)damage, 2f) / 200f; // Nonlinear scaling, recoil difference between 10 and 20 dmg is 4x. Could also reverse this?
				//float timeMult = (float)damage / 100;
				StartCoroutine(recoil(recoilOffset * recoilMultiplier, 0.05f));
			}
			var random = Random.Range(0, 4);
			AudioFW.Play("PlayerTakesDamage" + random);
			SetHealth(-damage, this);
		}
	}
	public void GetHealed(int heal) {
		AudioFW.Play("Heal");
		SetHealth(heal, this);
	}
	public void SetHealth(int amount, PlayerCharacter pc) {
		var hpIndicator = Instantiate(healthChangeIndicator, transform).GetComponent<HealthChangeIndicator>();
		hpIndicator.SetHealthChangeText(amount);
		hpIndicator.transform.SetParent(null);
		PhotonView photonView = pc.GetComponent<PhotonView>();
		if (photonView != null && PhotonNetwork.isMasterClient) {
			PlayerManager.Instance.ModifyHealth(photonView.owner, amount);
		}
	}

	#region Specials
	void AreaHeal() {
		photonView.RPC("RPC_AreaHeal", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void RPC_AreaHeal() {
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, specialEffectArea, layerMaskPlayer);
		Instantiate(healEffectParticles, transform.position, Quaternion.identity, transform);
		foreach (var hit in hits) {
			var pc = hit.GetComponent<PlayerCharacter>();
			pc.GetHealed(specialAmount);
		}
	}

	void Dash() {
		dashing = true;
		dashVector = lastDir;
		// Initial dash direction from mouse position
		//Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
		//dashVector = new Vector2(Input.mousePosition.x - position.x, Input.mousePosition.y - position.y);
		// Dashing to the last movement direction from keyboard inputs
	}
	void AreaDamage() {
		photonView.RPC("RPC_AreaDamage", PhotonTargets.AllViaServer);
	}
	[PunRPC]
	void RPC_AreaDamage() {
		Instantiate(damageEffectParticles, transform.position, Quaternion.identity);
		if (PhotonNetwork.isMasterClient) {
			Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, specialEffectArea, layerMaskEnemy);
			foreach (var hit in hits) {
				print(hit);
				IDamageable<int> iDamageable = hit.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
				if (iDamageable != null) {
					iDamageable.TakeDamage(specialAmount, new Vector3(0, 0, 0));
				}
			}
		}
	}


	void Push() {
		photonView.RPC("RPC_Push", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void RPC_Push() {
		Debug.Log("Push");
		if (PhotonNetwork.isMasterClient) {
			Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, specialEffectArea * 0.5f, layerMaskEnemy);
			foreach (Collider2D col in hits) {
				if (col.GetComponent<EnemyCharacter>()) {
					if (!col.GetComponent<EnemyCharacter>().stunned && !col.GetComponent<EnemyCharacter>().flying) {
						AudioFW.Play("Boom");
						Rigidbody2D enemyRB = col.GetComponent<Rigidbody2D>();
						Vector3 enemyLoc = col.GetComponent<Transform>().position;
						Vector2 pushVector = new Vector2(enemyLoc.x - transform.position.x, enemyLoc.y - transform.position.y).normalized;
						col.GetComponent<EnemyCharacter>().Fly(0.5f);
						enemyRB.AddForce(pushVector * pushForce, ForceMode2D.Impulse);
					}
				}
			}
		}
	}

	#endregion
	private void FixedUpdate() {

		// Move the PlayerCharacter of the correct player
		if (photonView.isMine) {
			if (rb2D != null)
				if (dashing) {
					//rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed * dashFactor;
					rb2D.velocity = dashVector.normalized * speed * dashFactor;
				} else {
					if (!shooting) {
						rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed;
					} else {
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
			if (photonView.isMine) {
				uim.UpdatePotion();
				AudioFW.Play("PotionDrink");
				SetHealth(100, this);
				potion = false;
			}
		}
	}

	public void GetPotion() {
		potion = true;
		if (photonView.isMine) {
			uim.UpdatePotion();
			AudioFW.Play("PotionPickup");
		}
	}

	public void GetSpeed() {
		speed *= 1.25f;
		if (photonView.isMine) {
			uim.UpdateSpeedBoost();
		}
	}

	public void GetWeaponUpgrade() {
		if (photonView.isMine) {
			uim.setPowerupUITimer(weaponDowngradeTime, weaponLevel + 1);
			uim.SetInfoText("Picked up a weapon upgrade", 2);
		}
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
					iDamageable.TakeDamage(damage, new Vector3(0, 0, 0));
				}
			}
		}
		// Play animation
		if (characterType == EntityType.Hero1) {
			animator.Play("LightOniAttack");
		}
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