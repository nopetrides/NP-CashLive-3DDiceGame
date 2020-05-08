using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If done right this should be able to drive both the bot opponent
/// and the player when the player wants the computer to take over
/// </summary>
public class RollingAI : MonoBehaviour
{
	public delegate void OnRollDelegate();
	public OnRollDelegate RollRequested;
	public delegate void RerollDelegate(bool reroll);
	public RerollDelegate RerollDecided;

	public void DoRoll()
	{
		// there should never be a reason to not roll
		RollRequested?.Invoke();
	}

	public void RerollDecision()
	{
		// bulk of the logic to decide if the player/AI will reroll
		RerollDecided?.Invoke(false);
	}
}
