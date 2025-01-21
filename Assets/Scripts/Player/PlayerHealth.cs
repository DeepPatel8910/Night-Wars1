using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PlayerHealth : MonoBehaviour {

	public Text playerHealthText;	
	public int startingHealth = 100;  
	public int currentHealth;
	public float invulnerabilityTime = 1f;
	public float timeAfterWeLastTookDamage = 1f;  
	public Slider healthSlider;
	public Image damageImage;      
	public AudioClip deathClip;         
	public float flashSpeed = 5f;     
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
	public Color minHealthColor = new Color (1.0f, 0.326f, 0.326f, 1.0f);
	public Color maxHealthColor = new Color (0.26f, 0.47f, 0.965f, 1.0f);
	public Image healthFillImage;
	Animator anim;            
	AudioSource playerAudio;      
	PlayerMovement playerMovement; 
	PlayerShooting playerShooting;  
	bool isDead;            
	bool damaged;          
	float timer;
	SkinnedMeshRenderer myRenderer;
    Color rimColor;
    float rimPower;

    void Awake() {
		anim = GetComponent<Animator>();
		playerAudio = GetComponent<AudioSource>();
		playerMovement = GetComponent<PlayerMovement>();
		playerShooting = GetComponentInChildren<PlayerShooting>();
		
		currentHealth = startingHealth;
		playerHealthText.text = currentHealth.ToString();

		SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach (SkinnedMeshRenderer meshRenderer in meshRenderers) {
			if (meshRenderer.gameObject.name == "Player") {
				myRenderer = meshRenderer;
				break;
			}
		}
	}

	void Start() {
        rimColor = myRenderer.materials[0].GetColor("_RimColor");
        rimPower = myRenderer.materials[0].GetFloat("_RimPower");
		healthSlider.value = startingHealth;
		healthFillImage.color = maxHealthColor;
    }
	
	void Update() {
		if (damaged) {
			
			damageImage.color = flashColour;
		}
		
		else {
			
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}

		timer += Time.deltaTime;
		damaged = false;
	}
	
	
	public void TakeDamage(int amount) {
		if (timer < invulnerabilityTime) {
			return;
		}
		StopCoroutine("IsHit");
		StartCoroutine("IsHit");
		
		damaged = true;
		currentHealth -= amount;
		int health = currentHealth >= 0 ? currentHealth : 0;
		playerHealthText.text = health.ToString();

		if (currentHealth > startingHealth) {
			currentHealth = startingHealth;
		}

		healthSlider.value = currentHealth;
		
		healthFillImage.color = maxHealthColor;
		if (currentHealth <= startingHealth * 0.3)
			healthFillImage.color = minHealthColor;

		
		timer = 0;
		
		playerAudio.Play ();
		
		if (currentHealth <= 0 && !isDead) {
			Death();
		}
	}

    IEnumerator IsHit() {
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

    public void AddHealth(int amount) {
		currentHealth += amount;
		
		if (currentHealth > startingHealth) {
			currentHealth = startingHealth;
		}
		
		healthSlider.value = currentHealth;
		
		healthFillImage.color = maxHealthColor;
		if (currentHealth <= startingHealth * 0.3)
			healthFillImage.color = minHealthColor;

		playerHealthText.text = currentHealth.ToString ();
	}
	
	
	void Death() {
		isDead = true;
		playerShooting.DisableEffects ();
		anim.SetTrigger ("Die");
		
		playerAudio.clip = deathClip;
		playerAudio.Play ();
		
		playerMovement.enabled = false;
		playerShooting.enabled = false;
	}
}
