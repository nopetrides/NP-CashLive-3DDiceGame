using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieManager : MonoBehaviour
{
    [SerializeField]
    private Vector3 StaticRollForce = Vector3.zero;
    public Vector3 GetStaticRollForce() { return StaticRollForce; }

    [SerializeField]
    private float MinimumMovement = 0.001f;
    public float GetMinimumMovement() { return MinimumMovement; }

    [SerializeField]
    private int MaxNoMoveFrameCount = 4;
    public int GetMaxNoMoveFrameCount() { return MaxNoMoveFrameCount; }

    [SerializeField]
    private float MaxDieRotationSpeed = 1f;
    public float GetMaxDieRotationSpeed() { return MaxDieRotationSpeed; }

    [SerializeField]
    private float MinDieRotationSpeed = 1f;
    public float GetMinDieRotationSpeed() { return MinDieRotationSpeed; }

    [SerializeField]
    private DieBehaviour[] m_PlayerDice = null;
    [SerializeField]
    private DieBehaviour[] m_OpponentDice = null;

    private bool m_StartingRolls = false;
    private void Start()
    {
        StartCoroutine(SetRandomRotations());
    }

    IEnumerator SetRandomRotations()
    {
        for (int i = 0; i < m_PlayerDice.Length; i++)
        {
            m_PlayerDice[i].SetRandomStartingRotation();
            yield return null;
        }
        for (int i = 0; i < m_OpponentDice.Length; i++)
        {
            m_OpponentDice[i].SetRandomStartingRotation();
            yield return null;
        }
        yield return null;
    }

    public bool AllDiceStopped()
    {
        if (m_StartingRolls)
        {
            return false;
        }
        for (int i = 0; i < m_PlayerDice.Length; i++)
        {
            if (m_PlayerDice[i].IsMoving())
            {
                return false;
            }
        }
        for (int i = 0; i < m_OpponentDice.Length; i++)
        {
            if (m_OpponentDice[i].IsMoving())
            {
                return false;
            }
        }
        return true;
    }

    public void PlayerRollButton()
    {
        m_StartingRolls = true;
        StartCoroutine(OnPlayerRoll());
    }

    IEnumerator OnPlayerRoll()
    {
        for (int i = 0; i < m_PlayerDice.Length; i++)
        {
            m_PlayerDice[i].Roll();
            yield return null;
        }
        m_StartingRolls = false;
        yield return null;
    }


    public void OpponentRoll()
    {
        m_StartingRolls = true;
        StartCoroutine(OnOpponentRoll());
    }

    IEnumerator OnOpponentRoll()
    {
        for (int i = 0; i < m_OpponentDice.Length; i++)
        {
            m_OpponentDice[i].Roll();
            yield return null;
        }
        m_StartingRolls = false;
        yield return null;
    }

    public int[] GetPlayerScore()
    {
        int[] diceNums = new int[m_PlayerDice.Length];
        for (int i = 0; i < m_PlayerDice.Length; i++)
        {
            diceNums[i] = m_PlayerDice[i].GetScore();
        }
        return diceNums;
    }

    public int[] GetOpponentScore()
    {
        int[] diceNums = new int[m_OpponentDice.Length];
        for (int i = 0; i < m_OpponentDice.Length; i++)
        {
            diceNums[i] = m_OpponentDice[i].GetScore();
        }
        return diceNums;
    }
}
