using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
	[SerializeField]
	private Button PlayButton;
	[SerializeField]
	private Button StatsButton;
	
	private void OnEnable()
	{
		PlayButton.onClick.AddListener(OnPlayButtonPressed);
		StatsButton.onClick.AddListener(OnStatsButtonPressed);
	}

	private void OnDisable()
	{
		PlayButton.onClick.RemoveAllListeners();
		StatsButton.onClick.RemoveAllListeners();
	}

	private void OnPlayButtonPressed()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
	}

	private void OnStatsButtonPressed()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Stats");
	}
}
