using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

	int score;       	
	public Text number;

	void Awake() {
		score = 0;
	}

	void Update() {
		number.text = score.ToString();
	}

	public void AddScore(int toAdd) {
		score += toAdd;
		number.GetComponent<Animation>().Stop();
		number.GetComponent<Animation>().Play();
	}

	public int GetScore() {
		return score;
	}
}