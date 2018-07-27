using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LBActionTransitTypes
{
	Switch, // Transit only after action has ended
	Interrupt, // Transit during action exectution
}
	
public abstract class LBTransitiveAction : LBAction
{
	public LBActionTransitTypes TransferType; // only works for @TransfersFrom
	public string[] TransfersFrom;
	public string TransfersTo;

	public bool CanActivateAction (string prev_action)
	{
		if(CanTransferFrom (prev_action) == false)
			return false;
		
		if (base.CanActivateAction () == false)
			return false;

		return true;
	}

	protected virtual bool CanTransferFrom(string prev_action)
	{
		int i;

		for (i = 0; i < TransfersFrom.Length; i++) 
		{
			if (TransfersFrom [i] == prev_action)
				return true;
		}

		return false;
	}
}
