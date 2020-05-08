﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_OverlayBG = null;
	[SerializeField]
	private GameObject m_RandomizingText = null;
	[SerializeField]
	private Button m_RollButton = null;
	[SerializeField]
	private Button m_TakeOverButton = null;
	[SerializeField]
	private GameObject m_HideDuringStartup = null;

	[Header("Score")]
	[SerializeField]
	private Text m_PlayerHeader = null;
	[SerializeField]
	private Text m_PlayerScore = null;
	[SerializeField]
	private Text m_PlayerRerolls = null;
	[SerializeField]
	private Text m_OpponentHeader = null;
	[SerializeField]
	private Text m_OpponentScore = null;
	[SerializeField]
	private Text m_OpponentRerolls = null;

	[Header("Reroll")]
	[SerializeField]
	private GameObject m_RerollParent = null;
	[SerializeField]
	private Text m_PlayerScoreThisRound = null;
	[SerializeField]
	private Text m_OpponentScoreThisRound = null;
	[SerializeField]
	private Button m_PlayerReroll = null;
	[SerializeField]
	private Button m_PlayerKeep = null;


	public delegate void OnRollDelegate();
	public static OnRollDelegate RollRequested;
	public delegate void OnTakeOverDelegate();
	public static OnTakeOverDelegate TakeOverRequested;
	public delegate void OnRerollDelegate(bool isReroll);
	public static OnRerollDelegate RerollDecision;

	private void OnEnable()
	{
		m_RollButton.onClick.AddListener(OnRollButtonClicked);
		m_TakeOverButton.onClick.AddListener(OnTakeOverButtonClicked);
		m_PlayerReroll.onClick.AddListener(OnRerollClicked);
		m_PlayerKeep.onClick.AddListener(OnKeepClicked);
	}

	private void OnDisable()
	{
		m_RollButton.onClick.RemoveAllListeners();
		m_TakeOverButton.onClick.RemoveAllListeners();
		m_PlayerReroll.onClick.RemoveAllListeners();
		m_PlayerKeep.onClick.RemoveAllListeners();
	}

	private void Start()
	{
		m_PlayerScore.text = "0";
		m_OpponentScore.text = "0";
		m_PlayerRerolls.text = "0";
		m_OpponentRerolls.text = "0";
	}

	public void GameOverUI()
	{
		m_OverlayBG.SetActive(true);
	}

	public void RandomizingUI()
	{
		m_RerollParent.SetActive(false);
		m_OverlayBG.SetActive(true);
		m_RandomizingText.SetActive(true);
		m_RollButton.interactable = false;
		m_HideDuringStartup.gameObject.SetActive(false);
	}

	public void ClearAll()
	{
		m_OverlayBG.SetActive(false);
		m_RandomizingText.SetActive(false);
	}

	public void PlayerInput()
	{
		m_OverlayBG.SetActive(false);
		m_RandomizingText.SetActive(false);
		m_HideDuringStartup.gameObject.SetActive(true);
		m_RollButton.interactable = true;
	}

	private void OnRollButtonClicked()
	{
		m_RollButton.interactable = false;
		RollRequested?.Invoke();
	}

	private void OnTakeOverButtonClicked()
	{
		TakeOverRequested?.Invoke();
	}

	public void SetPlayerScoreUI(int score)
	{
		m_PlayerScore.text = score.ToString();
	}
	public void SetOpponentScoreUI(int score)
	{
		m_OpponentScore.text = score.ToString();
	}

	public void RerollUI(int playerScore, int opponentScore)
	{
		m_RerollParent.SetActive(true);
		m_PlayerScoreThisRound.text = string.Format("You rolled {0}",playerScore);
		m_OpponentScoreThisRound.text = string.Format("Your opponent rolled {0}", opponentScore);
	}

	public void HideRerollUI()
	{
		m_RerollParent.SetActive(false);
	}

	private void OnRerollClicked()
	{
		RerollDecision?.Invoke(true);
	}

	private void OnKeepClicked()
	{
		RerollDecision?.Invoke(false);
	}
}
