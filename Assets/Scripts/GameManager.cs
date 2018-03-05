using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private int totalPoints;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Text versionText;

	public static float defaultCameraSize;
	public static Vector3 defaultCameraPosition;

	public void RestartCurrentLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}


	void Start() {
		totalPoints = 0;
		TotalBlockPoints();
		UpdateVersionText();
		defaultCameraSize = Camera.main.orthographicSize;
		defaultCameraPosition = Camera.main.transform.position;
	}

	public void AddPoints(int points) {
		totalPoints += points;
		UpdateScoreText(totalPoints);
	}

	public void DeductPoints(int points) {
		totalPoints -= points;
		UpdateScoreText(totalPoints);
	}

	private void TotalBlockPoints() {
		GameObject[] blocksGameObject = GameObject.FindGameObjectsWithTag("Block");
		int size = blocksGameObject.Length;
		if (size > 0) {
			Block[] blocks = new Block[size];
			for (int i = 0; i < blocks.Length; i++) {
				blocks[i] = blocksGameObject[i].GetComponent<Block>();
				totalPoints += blocks[i].points;
				UpdateScoreText(totalPoints);
			}	
		}
 	}

	private void UpdateScoreText(int newScore) {
		if (scoreText == null) {
			return;
		}
		scoreText.text = "Score: " + newScore;
	}

	private void UpdateVersionText() {
		if (versionText != null) {
			versionText.text = Application.version;
		}
	}

	
}
