using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	[CreateAssetMenu (fileName = "NewDummyAction", menuName = "LBActionSystem/DummyAction")]
	public class LBDummyAction : LBAction
	{
		//		public LBDummyAction (string _name) :
		//		base (_name)
		//		{}
		//
		//		public LBDummyAction (string _name, LBActionActivationTypes _activation, LBActionActivationTypes _deactivation) :
		//		base (_name, _activation, _deactivation)
		//		{
		//			action_state = LBActionStates.Inactive;
		//		}

		public override LBAction Duplicate ()
		{
			LBDummyAction dup;

			dup = (LBDummyAction)CreateInstance(this.GetType());
			DuplicateProperties (dup);

			return dup;
		}

		public new static LBAction Default ()
		{
			LBDummyAction def;

			def = (LBDummyAction)CreateInstance(typeof(LBDummyAction));

			return def;
		}
	}
}
