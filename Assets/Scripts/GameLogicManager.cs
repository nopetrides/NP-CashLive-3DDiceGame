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
        RerollingRound,
        ScoreTally,
    }
    private StateOfPlay m_State = StateOfPlay.none;

    [SerializeField]
    private int m_GameSpeedMultiplier = 1;
    [SerializeField]
    private int m_MaxGameRounds = 11;    
    [SerializeField]
    private int m_RerollsPerPlayer = 3;
    [SerializeField]
    private DieManager m_DieManager;
    [SerializeField]
    private GameUIManager m_UIManager;

    private int m_CurrentRound = 0;
    private int m_PlayerGameScore = 0;
    private int m_PlayerScoreThisRound = 0;
    private int m_OpponentGameScore = 0;
    private int m_OpponentScoreThisRound = 0;
    private RollingAI m_OpponentAI = null;
    private RollingAI m_PlayerAI = null;
    private bool m_AITakeOver = false;
    private bool m_PlayerRerollDecided = false, m_OpponentRerollDecided = false; 
    private bool m_PlayerWantsToReroll = false, m_OpponentWantsToReroll = false;

    private void OnEnable()
    {
        GameUIManager.RollRequested += PlayerDieRollCallback;
        GameUIManager.TakeOverRequested += BotTakeOverCallback;
        GameUIManager.RerollDecision += PlayerRerollDecision;

        if (m_PlayerAI == null)
        {
            m_PlayerAI = new RollingAI();
        }
        m_PlayerAI.RollRequested += PlayerDieRollCallback;
        m_PlayerAI.RerollDecided += PlayerRerollDecision;
        m_PlayerAI.SetupRerolls(m_RerollsPerPlayer);
        m_UIManager.SetPlayerRerollsRemaining(m_PlayerAI.GetRerolls());

        if (m_OpponentAI == null)
        {
            m_OpponentAI = new RollingAI();
        }
        m_OpponentAI.RollRequested += OpponentDieRollCallback;
        m_OpponentAI.RerollDecided += OpponentRerollDecision;
        m_OpponentAI.SetupRerolls(m_RerollsPerPlayer);
        m_UIManager.SetOpponentRerollsRemaining(m_OpponentAI.GetRerolls());
    }

    private void OnDisable()
    {
        GameUIManager.RollRequested -= PlayerDieRollCallback;
        GameUIManager.TakeOverRequested -= BotTakeOverCallback;
        GameUIManager.RerollDecision -= PlayerRerollDecision;

        m_OpponentAI.RollRequested -= OpponentDieRollCallback;
        m_OpponentAI.RerollDecided -= OpponentRerollDecision;

        m_PlayerAI.RollRequested -= PlayerDieRollCallback;
        m_PlayerAI.RerollDecided -= PlayerRerollDecision;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_State = StateOfPlay.RandomizingStart;
        m_UIManager.RandomizingUI();
        Time.timeScale = m_GameSpeedMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_DieManager.AllDiceStopped())
        {
            return;
        }
        switch (m_State)
        {
            case StateOfPlay.RandomizingStart:
                SetState(StateOfPlay.PlayerWaitForInput);
                break;
            case StateOfPlay.PlayerRolling:
                PlayerRollComplete();
                break;
            case StateOfPlay.OpponentRolling:
                OpponentRollComplete();
                break;
            case StateOfPlay.AskReroll:
                CheckForRerollDecision();
                break;
            case StateOfPlay.RerollingRound:
                RerollsFinished();
                break;
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
                GetOpponentDecision();
                break;
            case StateOfPlay.OpponentRolling:
                RollOpponentsDice();
                break;
            case StateOfPlay.AskReroll:
                AskReroll();
                break;
            case StateOfPlay.RerollingRound:
                DoRerolls();
                break;
            case StateOfPlay.ScoreTally:
                GameOver();
                break;
            default:
                Debug.LogError("Menu state \"" + state + "\" is not supported!");
                break;
        }
    }
	#region Player_Turn
	private void WaitingForPlayerInput()
    {
        m_UIManager.RandomizedDone();
        if (m_AITakeOver)
        {
            m_PlayerAI.DoRoll();
        }
        else
        {
            m_UIManager.PlayerInput();
        }
    }

    private void PlayerDieRollCallback()
    {
        m_UIManager.DisablePlayerButton();
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
        SetState(StateOfPlay.OpponentWaitForInput);
    }

    private void SetPlayerScore()
    {
        int[] diceNums = m_DieManager.GetPlayerScore();
        m_PlayerScoreThisRound = CalculateScore(diceNums, true);
    }
    #endregion

    #region Opponent_Turn
    private void GetOpponentDecision()
    {
        m_OpponentAI.DoRoll();
    }
    private void OpponentDieRollCallback()
    {
        SetOpponentRolling();
    }
    private void SetOpponentRolling()
    {
        SetState(StateOfPlay.OpponentRolling);
    }
    private void RollOpponentsDice()
    {
        m_DieManager.OpponentRoll();
    }
    private void OpponentRollComplete()
    {
        SetOpponentScore();
        SetState(StateOfPlay.AskReroll);
    }
    private void SetOpponentScore()
    {
        int[] diceNums = m_DieManager.GetOpponentScore();
        m_OpponentScoreThisRound = CalculateScore(diceNums, true);
    }

	#endregion
	#region Reroll
	private void AskReroll()
    {
        AskPlayerReroll();
        AskOpponentReroll();
    }

    private void AskPlayerReroll()
    {
        m_PlayerRerollDecided = false;
        if (m_AITakeOver)
        {
            m_PlayerAI.RerollDecision();
        }
        else
        {
            if (m_PlayerAI.GetRerolls() > 0)
            {
                m_UIManager.RerollUI(m_PlayerScoreThisRound, m_OpponentScoreThisRound);
            }
            else
            {
                PlayerRerollDecision(false);
            }
        }
    }

    private void AskOpponentReroll()
    {
        m_OpponentRerollDecided = false;
        if (m_OpponentAI.GetRerolls() > 0)
        {
            m_OpponentAI.RerollDecision();
        }
        else
        {
            OpponentRerollDecision(false);
        }
    }

    private void PlayerRerollDecision(bool reroll)
    {
        if (reroll && m_PlayerAI.GetRerolls() > 0)
        {
            m_PlayerWantsToReroll = true;
        }
        else
        {
            m_PlayerWantsToReroll = false;
        }
        m_PlayerRerollDecided = true;
    }

    private void UpdatePlayerScoreIU()
    {
        m_PlayerGameScore += m_PlayerScoreThisRound;
        m_PlayerScoreThisRound = 0;
        m_UIManager.SetPlayerScoreUI(m_PlayerGameScore);
    }

    private void OpponentRerollDecision(bool reroll)
    {
        if (reroll && m_OpponentAI.GetRerolls() > 0)
        {
            m_OpponentWantsToReroll = true;
        }
        else
        {
            m_OpponentWantsToReroll = false;
        }
        m_OpponentRerollDecided = true;
    }

    private void UpdateOpponentScoreIU()
    {
        m_OpponentGameScore += m_OpponentScoreThisRound;
        m_OpponentScoreThisRound = 0;
        m_UIManager.SetOpponentScoreUI(m_OpponentGameScore);
    }

    private void CheckForRerollDecision()
    {
        if (m_PlayerRerollDecided && m_OpponentRerollDecided)
        {
            m_UIManager.HideRerollUI();
            if (!m_PlayerWantsToReroll && !m_OpponentWantsToReroll)
            {
                TryStartNextRound();
                return;
            }
            SetState(StateOfPlay.RerollingRound);
        }
    }

    private void DoRerolls()
    {
        if (m_PlayerWantsToReroll)
        {
            m_PlayerAI.DecrementRerolls();
            m_UIManager.SetPlayerRerollsRemaining(m_PlayerAI.GetRerolls());
            RollPlayersDice();
        }
        if (m_OpponentWantsToReroll)
        {
            m_OpponentAI.DecrementRerolls();
            m_UIManager.SetOpponentRerollsRemaining(m_OpponentAI.GetRerolls());
            RollOpponentsDice();
        }
    }

    private void RerollsFinished()
    {
        if (m_PlayerWantsToReroll)
        {
            SetPlayerScore();
            m_PlayerWantsToReroll = false;
        }
        if (m_OpponentWantsToReroll)
        {
            SetOpponentScore();
            m_OpponentWantsToReroll = false;
        }

        TryStartNextRound();
    }

    private void TryStartNextRound()
    {
        UpdatePlayerScoreIU();
        UpdateOpponentScoreIU();
        if (m_CurrentRound < m_MaxGameRounds)
        {
            m_CurrentRound++;
            SetState(StateOfPlay.PlayerWaitForInput); // start next round
        }
        else
        {
            SetState(StateOfPlay.ScoreTally);
        }
    }

    #endregion
    private void GameOver()
    {
        m_UIManager.GameOverUI();
    }

    private void BotTakeOverCallback()
    {
        m_AITakeOver = !m_AITakeOver;
        m_UIManager.TogglePlayForMeText(m_AITakeOver);
        if (m_State == StateOfPlay.PlayerWaitForInput)
        {
            WaitingForPlayerInput();
        }
        else if (m_State == StateOfPlay.AskReroll)
        {
            AskPlayerReroll();
        }
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
