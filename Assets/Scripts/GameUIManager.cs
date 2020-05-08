using System.Collections;
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
	private Text m_OpponentHeader = null;
	[SerializeField]
	private Text m_OpponentScore = null;


	public delegate void OnRollDelegate();
	public static OnRollDelegate RollRequested;
	public delegate void OnTakeOverDelegate();
	public static OnRollDelegate TakeOverRequested;

	private void OnEnable()
	{
		m_RollButton.onClick.AddListener(OnRollButtonClicked);
		m_TakeOverButton.onClick.AddListener(OnTakeOverButtonClicked);
	}
	private void OnDisable()
	{
		m_RollButton.onClick.RemoveAllListeners();
		m_TakeOverButton.onClick.RemoveAllListeners();
	}

	private void Start()
	{
		m_PlayerScore.text = "0";
		m_OpponentScore.text = "0";
	}

	public void GameOverUI()
	{
		m_OverlayBG.SetActive(true);
	}

	public void RandomizingUI()
	{
		m_OverlayBG.SetActive(true);
		m_RandomizingText.SetActive(true);
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
		m_RollButton.enabled = false;
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

	private void ToggleMainUI(bool active)
	{

	}
}
