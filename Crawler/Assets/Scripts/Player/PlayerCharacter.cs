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
	bool alive;
	float playerCamOffset = 0.002f;
	float dashCooldown = 3.0f;
	float dashTime = 0.15f;
	float dashTimer = 0.15f;
	float respawnTime = 10.0f;
	float respawnTimer = 10.0f;
	// Multiplier for base player speed when dashing
	float dashFactor = 4.0f;
	Vector2 dashVector;
	Vector2 lastDir;
	Vector3 TargetPosition;

	int projectilesPerAttack = 1;

	public Animator animator;
	public GameObject playerCam;
	public GameObject myUIBox;

	void Start() {
		alive = true;
		rb2D = GetComponent<Rigidbody2D>();
		col = GetComponent<CircleCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		layerMaskEnemy = LayerMask.GetMask("Enemy");
		projHead = transform.Find("ProjectileHeading").gameObject;
		MainCamera = transform.Find("Main Camera").gameObject;
		dashCooldown = 0.0f;
		SetCharacterAttributes();
		if (!PhotonNetwork.isMasterClient)
			return;
		var photonView = GetComponent<PhotonView>();
		if (photonView != null) {
			print(health);
			PlayerManager.Instance.ModifyHealth(photonView.owner, health);
		}
		players = GameObject.FindGameObjectsWithTag("Player");
	}

	public void TakeDamage(int damage) {
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

		// Try to find a remote camera
		players = GameObject.FindGameObjectsWithTag("Player");
		if (players.Length > 1)
		{
			findCamera();
		}
		else
		{
			Debug.Log("Cant find camera, no alive players found");
		}
		
		

		if(photonView.isMine)
		{
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

	void Update() {

		if (photonView.isMine) {

			players = GameObject.FindGameObjectsWithTag("Player");

			// When the player is dead
			if (!alive) {
				respawnTimer -= Time.deltaTime;

				if (Input.GetMouseButtonDown(0)) {
					if (players.Length > 1) {
						findCamera();
					} else {
						Debug.Log("Cant search for a remote camera, only 1 player in the game");
					}
				}
			}

			//// Death
			//if (health <= 0 && alive)
			//{
			//	Die();
			//	photonView.RPC("Die", PhotonTargets.Others);
			//}

			// Respawn
			if (respawnTimer <= 0) {
				respawnTimer = respawnTime;
				respawn();
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
				}
				// Movement input
				movement.x = Input.GetAxisRaw("Horizontal");
				movement.y = Input.GetAxisRaw("Vertical");


				if (movement.x != 0 || movement.y != 0) {
					lastDir = new Vector2(movement.x, movement.y);
				}

				// Camera movement
				Vector3 mousePos = Camera.main.WorldToScreenPoint(transform.position);
				playerCam.transform.position = new Vector3((Input.mousePosition.x - mousePos.x) * playerCamOffset, (Input.mousePosition.y - mousePos.y) * playerCamOffset, playerCam.transform.position.z) + transform.position;


				// Setting the correct animation/stance depending on the current mouse position and if moving or not
				Vector2 mouseVector = new Vector2(Input.mousePosition.x - mousePos.x, Input.mousePosition.y - mousePos.y);
				//Debug.Log(mouseVector);

				animator.SetFloat("Horizontal", mouseVector.x);
				animator.SetFloat("Vertical", mouseVector.y);
				animator.SetFloat("Magnitude", movement.magnitude);



				if (Input.GetKeyDown(KeyCode.Space) && dashCooldown <= 0) {
					Debug.Log("Dashing");
					dashing = true;
					dashCooldown = 5.0f;

					// Initial dash direction from mouse position
					//Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
					//dashVector = new Vector2(Input.mousePosition.x - position.x, Input.mousePosition.y - position.y);

					// Dashing to the last movement direction from keyboard inputs
					dashVector = lastDir;
				}

				if (dashing) {
					dashTimer -= Time.deltaTime;
					// Updating dashing direction mid dash with mouse position.
					//Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
					//dashVector = new Vector2(Input.mousePosition.x - position.x, Input.mousePosition.y - position.y);

					// Updating dashing direction mid dash with keyboard inputs. Comment to have static direction
					//dashVector = lastDir;
				}

				if (dashTimer <= 0) {
					dashing = false;
					dashTimer = dashTime;
				}

				if (!dashing && dashCooldown - Time.deltaTime > 0) {
					dashCooldown -= Time.deltaTime;
				} else if (!dashing && dashCooldown - Time.deltaTime < 0) {
					dashCooldown = 0f;
				}

			}
			//if (myUIBox != null)
			//	myUIBox.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120); // Make my UIBox Bigger than others.

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
					rb2D.velocity = new Vector2(movement.x * speed, movement.y * speed).normalized * speed;
				}
			//Debug.Log(rb2D.velocity.magnitude);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("WeaponUpgrade")) {
			upgradeWeapon();
			Destroy(collision.gameObject);
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
			print(healths[(int)characterType]);
			if (health + 100 > healths[(int)characterType]) {
				SetHealth(healths[(int)characterType] - health, this);
			} else
				SetHealth(100, this);
			potion = false;
		}
	}

	public void GetPotion() {
		potion = true;
	}

	public void GetSpeed() {
		speed += 10;
	}

	void upgradeWeapon() {
		if(characterType == EntityType.Hero0) {
			if(weaponLevel == 0) {
				projectilesPerAttack++;
			} else if(weaponLevel == 1) {
				attackInterval = 0.2f;
			}

		} else if(characterType == EntityType.Hero1) {
			if(weaponLevel == 0) {
				attackInterval = 1f;
			} else if(weaponLevel == 1) {
				projectileSpeed = 20f;
			}

		}
		weaponLevel++;
	}
	#endregion



	public void Attack() {
		if (ranged) {
			Shoot(projectilesPerAttack, damage);
			photonView.RPC("Shoot", PhotonTargets.Others, projectilesPerAttack, 0);
		} else {
			Melee(damage);
			photonView.RPC("Melee", PhotonTargets.Others, 0);
		}
		attackTimer = attackInterval;
	}



	[PunRPC]
	public void Shoot(int amount, int dmg) {
		float gap = .5f;
		var offset = (amount - 1f) / 2 * gap;

		for(int i = 0; i < amount; i++) {
			GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
			projectileClone.transform.parent = projectileSpawn.transform;
			projectileClone.transform.localPosition = new Vector3(0f, offset - i * gap, 0f);
			projectileClone.transform.parent = null;
			Projectile projectile = projectileClone.GetComponent<Projectile>();
			projectile.LaunchProjectile(dmg, attackRange, projectileSpeed, npc, (projectileSpawn.transform.position - transform.position).normalized);
		}
	}



	[PunRPC]
	public void Melee(int dmg) {

		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, layerMaskEnemy);
		foreach(var hit in hits) {
			IDamageable<int> iDamageable = hit.gameObject.GetComponent(typeof(IDamageable<int>)) as IDamageable<int>;
			if(iDamageable != null) {
				iDamageable.TakeDamage(damage);
			}
		}
	}
}
