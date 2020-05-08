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
        StartCoroutine(OnPlayerRoll());
    }

    IEnumerator OnPlayerRoll()
    {
        for (int i = 0; i < m_PlayerDice.Length; i++)
        {
            m_PlayerDice[i].Roll();
            yield return null;
        }
        yield return null;
    }


    public void OpponentRoll()
    {
        OnOpponentRoll();
    }

    private void OnOpponentRoll()
    {
        for (int i = 0; i < m_OpponentDice.Length; i++)
        {
            m_OpponentDice[i].Roll();
        }
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

    public void GetOpponentScore()
    {

    }
}
