using UnityEngine;
using System.Collections;

public class HellephantAttack : MonoBehaviour {

	public float timeBetweenAttacks = 3f;  
	public int bulletsPerVolley = 5;
	public float timeBetweenBullets = 0.1f;
	public int numberOfBullets = 36;
	public float angleBetweenBullets = 10f;
	public GameObject bullet;
	public int attacksPerSpecialAttack = 3;
	public AudioClip shootClip;
	public AudioClip specialClip;	
	GameObject player;           
	EnemyHealth enemyHealth; 
	bool playerInRange;   
	Animator anim;
	float attackTimer;
	float bulletTimer;
	int attackCount;
	int bulletCount;
	HellephantMovement helleMovement;
	float floatHeight = 3f;
	float landingTime;
	bool usedSpecial = false;
	bool landed = false;
	
	AudioSource enemyAudio;  
	
	void Awake() {
		player = GameObject.FindGameObjectWithTag("Player");
		enemyHealth = GetComponent<EnemyHealth>();
		enemyAudio = GetComponent<AudioSource>();
		anim = GetComponent<Animator>();
		helleMovement = GetComponent<HellephantMovement>();
	}

	void Start() {
		transform.position = new Vector3(transform.position.x, floatHeight, transform.position.z);
	}

	void Update() {
		attackTimer += Time.deltaTime;
		if (attackTimer > timeBetweenAttacks && enemyHealth.currentHealth > 0) {
			Attack();
		}
	}
	
	void Attack() {
		bulletTimer += Time.deltaTime;

		if (attackCount < attacksPerSpecialAttack) {
			if (bulletTimer > timeBetweenBullets && bulletCount < bulletsPerVolley) {
				Vector3 relativePos = player.transform.position - transform.position;
				Quaternion rotation = Quaternion.LookRotation(relativePos);
				Quaternion rot = rotation * Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), Vector3.up) * Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), Vector3.right);
				Instantiate(bullet, transform.position + new Vector3(0, 0.5f, 0), rot);

				enemyAudio.clip = shootClip;
				enemyAudio.Play();
				
				bulletTimer = 0f;
				bulletCount++;

				if (bulletCount == (bulletsPerVolley)) {
					bulletCount = 0;
					attackTimer = 0;
					attackCount++;
				}
			}
		}
		else {
			SpecialAttack();
		}
	}

	void SpecialAttack() {
		
		if (!landed) {
			anim.SetBool("Landing", true);
			helleMovement.shouldMove = false;
			landingTime += Time.deltaTime * 5f;
			transform.position = new Vector3(transform.position.x, Mathf.Lerp(floatHeight, 0, landingTime), transform.position.z);
		}
		
		else {
			anim.SetBool("Landing", false);
			helleMovement.shouldMove = true;
			landingTime += Time.deltaTime * 2f;
			transform.position = new Vector3(transform.position.x, Mathf.Lerp(0, floatHeight, landingTime), transform.position.z);
		}

		
		if (transform.position.y == 0) {
			landed = true;
			if (!usedSpecial) {
				for (int i = 0; i < numberOfBullets; i++) {
					
					float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (numberOfBullets - 1));
					Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
					Instantiate(bullet, transform.position + new Vector3(0, 0.5f, 0), rot);
				}

				enemyAudio.clip = specialClip;
				enemyAudio.Play();
				usedSpecial = true;
				attackTimer = 0;
				landingTime = 0;
			}
		}

		if (transform.position.y == floatHeight) {
			attackCount = 0;
			landed = false;
			usedSpecial = false;
			landingTime = 0;
		}
	}
}