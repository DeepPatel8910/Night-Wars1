using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	public int startingHealth = 100;  	
	[HideInInspector]
	public int currentHealth;  
	public float sinkSpeed = 2.5f;   
	public int scoreValue = 10; 
	public AudioClip deathClip;    
	public AudioClip burnClip;  
	public ParticleSystem deathParticles;  
	public Slider healthBarSlider;
	public GameObject eye;
	Slider sliderInstance;
	bool isDead;
	bool isBurning = false;
	Color rimColor;
    float rimPower;
    float cutoffValue = 0f;
	Animator anim;            
	AudioSource enemyAudio;        
	CapsuleCollider capsuleCollider;   
	SkinnedMeshRenderer myRenderer;
	GameObject enemyHealthbarManager;
	WaveManager waveManager;
	ScoreManager scoreManager;
	PickupManager pickupManager;

	void Awake() {
		anim = GetComponent<Animator>();
		enemyAudio = GetComponent<AudioSource>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		myRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		enemyHealthbarManager = GameObject.Find("EnemyHealthbarsCanvas");
		waveManager = GameObject.Find("WaveManager").GetComponent<WaveManager>();
		scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
		pickupManager = GameObject.Find("PickupManager").GetComponent<PickupManager>();
	}

	void Start() {
		currentHealth = startingHealth;
		sliderInstance = Instantiate(healthBarSlider, gameObject.transform.position, Quaternion.identity) as Slider;
		sliderInstance.gameObject.transform.SetParent(enemyHealthbarManager.transform, false);
		sliderInstance.GetComponent<Healthbar>().enemy = gameObject;
		sliderInstance.gameObject.SetActive(false);

		rimColor = myRenderer.materials[0].GetColor("_RimColor");
        rimPower = myRenderer.materials[0].GetFloat("_RimPower");
    }

	void Update() {
		if (isBurning) {
			cutoffValue = Mathf.Lerp(cutoffValue, 1f, 1.3f * Time.deltaTime);
			myRenderer.materials[0].SetFloat("_Cutoff", cutoffValue);
			myRenderer.materials[1].SetFloat("_Cutoff", 1);
		}
	}

	public void TakeDamage(int amount, Vector3 hitPoint) {
        StopCoroutine("Ishit");
        StartCoroutine("Ishit");

		if (isDead)
			return;
		GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * -300, hitPoint);
		currentHealth -= amount;

		if (currentHealth <= startingHealth) {
			sliderInstance.gameObject.SetActive(true);
		}
		int sliderValue = (int) Mathf.Round(((float)currentHealth / (float)startingHealth) * 100);
		sliderInstance.value = sliderValue;
		
		if (currentHealth <= 0) {
			Death();
		}
	}

	IEnumerator Ishit() {
		Color newColor = new Color(10, 0, 0, 0);
        float newPower = 0.5f;
		myRenderer.materials[0].SetColor("_RimColor", newColor);
        myRenderer.materials[0].SetFloat("_RimPower", newPower);
        float time = 0.25f;
		float elapsedTime = 0;
		while (elapsedTime < time) {
			newColor = Color.Lerp(newColor, rimColor, elapsedTime / time);
			myRenderer.materials[0].SetColor("_RimColor", newColor);
            newPower = Mathf.Lerp(newPower, rimPower, elapsedTime / time);
            myRenderer.materials[0].SetFloat("_RimPower", newPower);
            elapsedTime += Time.deltaTime;
			yield return null;
		}
        myRenderer.materials[0].SetColor("_RimColor", rimColor);
        myRenderer.materials[0].SetFloat("_RimPower", rimPower);
    }

	void Death() {
		isDead = true;
		anim.SetTrigger("Dead");
		enemyAudio.clip = deathClip;
		enemyAudio.Play();

		if (GetComponent<UnityEngine.AI.NavMeshAgent>()) {
			GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
		}
		
		GetComponent<Rigidbody>().isKinematic = true;
		scoreManager.AddScore(scoreValue);
		waveManager.enemiesAlive--;
		capsuleCollider.isTrigger = true;
		
		StartCoroutine(StartSinking());
		waveManager.playEnemyTextAnimation ();
		Destroy(sliderInstance.gameObject);
	}

	IEnumerator StartSinking() {
		yield return new WaitForSeconds(2);
		isBurning = true;
		deathParticles.Play();
		enemyAudio.clip = burnClip;
		enemyAudio.Play();

		for (int i = 0; i < 2; i++) {
			GameObject instantiatedEye = Instantiate(eye, transform.position + new Vector3(0, 0.3f, 0), transform.rotation) as GameObject;
			instantiatedEye.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3 (Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)));
		}

		SpawnPickup();
		Destroy(gameObject, 3f);
	}

	
	void SpawnPickup() {
		Vector3 spawnPosition = transform.position + new Vector3(0, 0.3f, 0);

		if (scoreManager.GetScore () >= pickupManager.scoreNeededForExtraBullet) {
			Instantiate (pickupManager.bulletPickup, spawnPosition, transform.rotation);
			pickupManager.scoreNeededForExtraBullet += pickupManager.extraScoreNeededAfterEachPickup;
		} else {
			float rand = Random.value;
			if (rand <= 0.2f) {
				
				if (rand <= 0.06f) {
					Instantiate (pickupManager.bouncePickup, spawnPosition, transform.rotation);
				}
			
			else if (rand > 0.06f && rand <= 0.1f) {
					Instantiate (pickupManager.piercePickup, spawnPosition, transform.rotation);
				}
			
			else {
					Instantiate (pickupManager.healthPickup, spawnPosition, transform.rotation);
				}
			}
		}
	}
}
