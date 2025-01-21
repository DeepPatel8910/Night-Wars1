using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    
    public PlayerHealth playerHealth;
    
    public float restartDelay = 5f;

    
    Animator anim;
    
    float restartTimer;

    void Awake() {
        
        anim = GameObject.Find("HUDCanvas").GetComponent<Animator>();
    }

    void Update() {
        
        if (playerHealth.currentHealth <= 0) {
			
			PlayerPrefs.Save ();

            
            anim.SetTrigger("GameOver");

            
            restartTimer += Time.deltaTime;

            
            if (restartTimer >= restartDelay) {
                
				SceneManager.LoadScene("Level 01");
            }
        }
    }
}
