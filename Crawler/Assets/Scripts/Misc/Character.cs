using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Hero0, Hero1, Hero2, Hero3, Enemy0, Enemy1, Enemy2, Enemy3 }

public class Character : Photon.MonoBehaviour {

	public EntityType characterType;

	// Four first of the values are for heroes the rest for enemies
	string[] names = new string[] {				"Magical Girl", "Light Oni",	"Dark Magi",	"Dark Oni" }; // Names
	string[] abilities  = new string[] {		"Dash", "Reign of Love","StunBun",	"Reign Of Terror" }; // Names
	bool[] rangeds = new bool[] {				true,	false,	true,	false,	true,	true,	false,	false }; // Ranger or melee
	float[] projectileSpeeds = new float[] {	15f,	0f,		7.5f,	0f,		6.5f,	6.5f,	10f,	10f }; // Speed of projectile
	float[] attackRanges = new float[] {		7.5f,	1.5f,	8.5f,	1.5f,	6.5f,	6.5f,	1.5f,	1.5f }; // Range of attack
	int[] damages = new int[] {					10,		15,		15,		25,		5,		15,		5,		50 };  // Amount of damage
	float[] attackIntervals = new float[] {		0.2f,	0.2f,	0.4f,	0.4f,	.5f,	.75f,	.5f,	1.5f }; // Attack interval
	float[] speeds = new float[] {				4.5f,	4.5f,	4.5f,	4.5f,	3.5f,	0.5f,	5f,		4.5f }; // Movement speed
	int[] healths = new int[] {					150,	200,	150,	250,	20,		40,		10,		80 }; // Health
	float[] specialCooldowns = new float[] {	3.0f,	10.0f,	5.0f,	10.0f,	0f,		0f,		0f,		0f }; // Special cooldowns

	public bool ranged;
	public float projectileSpeed;
	public float attackRange;
	public int damage;
	public float attackInterval;
	public float speed;
	public int health;
	public float specialCooldown;

	public float attackTime;
	public int speedLevel;
	public int weaponLevel;

	public GameObject projectileSpawn;
	public GameObject projectilePrefab;

	public Vector2 movement;

	public int CheckCharacterHealt(EntityType et) {
		return healths[(int)et];
	}

	public string GetCharacterData(int index, int hero) {
		if(index == 0)
			return names[hero];
		if(index == 1) {
			if(rangeds[hero] == true)
				return "Ranged";
			else
				return "Melee";
		}
		if(index == 2)
			return "" + damages[hero];
		if(index == 3)
			return "" + healths[hero];
		if(index == 4)
			return abilities[hero];

		return "";
	}

	public void SetCharacterAttributes() {
		ranged = rangeds[(int)characterType];
		projectileSpeed = projectileSpeeds[(int)characterType];
		attackRange = attackRanges[(int)characterType];
		damage = damages[(int)characterType];
		attackInterval = attackIntervals[(int)characterType];
		speed = speeds[(int)characterType];
		health = healths[(int)characterType];
		specialCooldown = specialCooldowns[(int)characterType];
	}

}