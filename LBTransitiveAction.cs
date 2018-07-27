using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LBActionTransitTypes
{
	Switch, // Transit only after action has ended
	Interrupt, // Transit during action exectution
	ConditionalSwitch,
	ConditionalInterrupt
}
	
public abstract class LBTransitiveAction : LBAction
{
	public LBActionTransitTypes TransferType; // only works for @TransfersFrom
	public string[] TransfersFrom;
	public string TransfersTo;

}
