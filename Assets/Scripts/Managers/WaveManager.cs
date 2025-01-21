using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {
	
	
	public PlayerHealth playerHealth;   
	public float bufferDistance = 200;
	public float timeBetweenWaves = 5f;
    public float spawnTime = 3f;
	public int startingWave = 1;
	public int startingDifficulty = 1;
	public Text number;
	public Text numberEnemies; 
	[HideInInspector]
	public int enemiesAlive = 0;

    
    [System.Serializable]
    public class Wave {
        public Entry[] entries;

        
        [System.Serializable]
        public class Entry {
            
            public GameObject enemy;
            public int count;
            
            [System.NonSerialized]
            public int spawned;
        }
    }

    
    public Wave[] waves;
    Vector3 spawnPosition = Vector3.zero;
	int waveNumber;
	float timer; 
	Wave currentWave;
	int spawnedThisWave = 0;
	int totalToSpawnForWave;
	bool shouldSpawn = false;
	int difficulty;
	int enemiesInLastFrame;
	float timeSinceNoEnemiesKilled;

	void Start() {
		waveNumber = startingWave > 0 ? startingWave - 1 : 0;
		difficulty = startingDifficulty;
		StartCoroutine("StartNextWave");
	}
	
	void Update() {
		if (!shouldSpawn) {
			return;
        }

		if (spawnedThisWave == totalToSpawnForWave && (enemiesAlive == 0 || (enemiesAlive == enemiesInLastFrame && timeSinceNoEnemiesKilled > 20))) {
			StartCoroutine("StartNextWave");
			return;
		}

		timer += Time.deltaTime;

		if (timer >= spawnTime) {            
			foreach (Wave.Entry entry in currentWave.entries) {
				if (entry.spawned < (entry.count * difficulty)) {
					Spawn(entry);
				}
			}
		}

		numberEnemies.text = enemiesAlive.ToString();

		
		if (enemiesInLastFrame == enemiesAlive) {
			timeSinceNoEnemiesKilled += Time.deltaTime;
		} else {
			enemiesInLastFrame = enemiesAlive;
			timeSinceNoEnemiesKilled = 0;
		}
	}

	public void playEnemyTextAnimation(){
		numberEnemies.GetComponent<Animation>().Play();
	}

	IEnumerator StartNextWave() {
		shouldSpawn = false;

		yield return new WaitForSeconds(timeBetweenWaves);

		if (waveNumber < waves.Length) {
			currentWave = waves[waveNumber];
		} else {
			difficulty++;

			foreach (Wave.Entry entry in waves [waves.Length - 1].entries) {
				entry.spawned = 0;
			}
			currentWave = waves [waves.Length - 1];
		}

        totalToSpawnForWave = 0;
		foreach (Wave.Entry entry in currentWave.entries) {
			totalToSpawnForWave += (entry.count * difficulty);
		}

		spawnedThisWave = 0;
		shouldSpawn = true;
		waveNumber++;
		number.text = waveNumber.ToString ();
		number.GetComponent<Animation>().Play();
	}

	void Spawn(Wave.Entry entry) {
		timer = 0f;
		
		if (playerHealth.currentHealth <= 0f) {
			return;
		}
		
		Vector3 randomPosition = Random.insideUnitSphere * 35;
		randomPosition.y = 0;
		
		UnityEngine.AI.NavMeshHit hit;
		if (!UnityEngine.AI.NavMesh.SamplePosition(randomPosition, out hit, 5, 1)) {
			return;
		}
		
		spawnPosition = hit.position;
		
		Vector3 screenPos = Camera.main.WorldToScreenPoint(spawnPosition);
		if ((screenPos.x > -bufferDistance && screenPos.x < (Screen.width + bufferDistance)) && 
		    (screenPos.y > -bufferDistance && screenPos.y < (Screen.height + bufferDistance))) 
		{
			return;
		}

		
		GameObject enemy =  Instantiate(entry.enemy, spawnPosition, Quaternion.identity) as GameObject;
		enemy.GetComponent<EnemyHealth>().startingHealth *= difficulty;
		enemy.GetComponent<EnemyHealth>().scoreValue *= difficulty;
		entry.spawned++;
		spawnedThisWave++;
		enemiesAlive++;
		numberEnemies.text = enemiesAlive.ToString();
	}
}
