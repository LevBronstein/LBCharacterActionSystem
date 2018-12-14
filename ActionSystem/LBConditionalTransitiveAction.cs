using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Jsut testing for now...

namespace LBActionSystem
{
	[System.Serializable]
	public struct LBBasicTransferOutCondition
	{
		public string ActionName;
		public bool Toggle1;
		public bool bHasToBeFinished; // only transfer if action has successfully finished
	}

	[System.Serializable]
	public struct LBBasicTransferInCondition
	{
		public string ActionName;
		public bool Toggle1;
		public bool bHasToBeFinished; // only transfer if action has successfully finished
	}


	public class LBConditionalTransitiveAction : LBTransitiveAction 
	{


	}
}
