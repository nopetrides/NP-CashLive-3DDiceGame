using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This should be the Finite State Machine and determines the current state of play
/// </summary>
public class GameLogicManager : MonoBehaviour
{
    private enum StateOfPlay
    {
        none = -1, // error state
        RandomizingStart,
        PlayerWaitForInput,
        PlayerRolling,
        OpponentWaitForInput,
        OpponentRolling,
        AskReroll,
        ScoreTally,
    }
    private StateOfPlay m_State = StateOfPlay.none;

    [SerializeField]
    private int m_MaxGameRounds = 11;
    [SerializeField]
    private DieManager m_DieManager;
    [SerializeField]
    private GameUIManager m_UIManager;

    private int m_CurrentRound = 0;
    private int m_PlayerScore = 0;
    private int m_OpponentScore = 0;

    private void OnEnable()
    {
        GameUIManager.RollRequested += PlayerDieRollCallback;
        GameUIManager.TakeOverRequested += BotTakeOverCallback;
    }

    private void OnDisable()
    {
        GameUIManager.RollRequested -= PlayerDieRollCallback;
        GameUIManager.TakeOverRequested -= BotTakeOverCallback;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_State = StateOfPlay.RandomizingStart;
        m_UIManager.RandomizingUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_DieManager.AllDiceStopped())
        {
            return;
        }
        if (m_State == StateOfPlay.RandomizingStart)
        {
            SetState(StateOfPlay.PlayerWaitForInput);
        }
        if (m_State == StateOfPlay.PlayerRolling)
        {
            PlayerRollComplete();
        }
        if (m_State == StateOfPlay.OpponentRolling)
        {

        }
    }

    private void SetState(StateOfPlay state)
    {
        m_State = state;
        Debug.Log("New state: " + state.ToString());
        switch (m_State)
        {
            case StateOfPlay.PlayerWaitForInput:
                WaitingForPlayerInput();
                break;
            case StateOfPlay.PlayerRolling:
                RollPlayersDice();
                break;
            case StateOfPlay.OpponentWaitForInput:
                GetOpponentDescision();
                break;
            case StateOfPlay.OpponentRolling:
                SetOpponentRolling();
                break;
            case StateOfPlay.AskReroll:
                AskReroll();
                break;
            case StateOfPlay.ScoreTally:
                GameOver();
                break;
            default:
                Debug.LogError("Menu state \"" + state + "\" is not supported!");
                break;
        }
    }

    private void WaitingForPlayerInput()
    {
        m_UIManager.PlayerInput();
    }

    private void PlayerDieRollCallback()
    {
        SetPlayerRolling();
    }
    private void SetPlayerRolling()
    {
        SetState(StateOfPlay.PlayerRolling);
    }
    private void RollPlayersDice()
    {
        m_DieManager.PlayerRollButton();
    }

    private void PlayerRollComplete()
    {
        SetPlayerScore();
        SetState(StateOfPlay.OpponentRolling);
    }

    private void SetPlayerScore()
    {
        int[] diceNums = m_DieManager.GetPlayerScore();
        m_PlayerScore += CalculateScore(diceNums, true);
        m_UIManager.SetPlayerScoreUI(m_PlayerScore);
    }

    private void GetOpponentDescision()
    {

    }

    private void SetOpponentRolling()
    {

    }

    private void AskReroll()
    {

    }

    private void GameOver()
    {
        m_UIManager.GameOverUI();
    }

    private void BotTakeOverCallback()
    {

    }

    private int CalculateScore(int[] nums, bool isPlayer)
    {
        bool sameNumber = false;
        int total = 0;
        for (int i = 0; i < nums.Length; i++)
        {
            for (int j = 0; j < nums.Length; j++)
            {
                if (i != j)
                {
                    if (nums[i] == nums[j])
                    {
                        sameNumber = true;
                        return 0;
                    }
                }                
            }
            total += nums[i];
        }
        return total;
    }
}
