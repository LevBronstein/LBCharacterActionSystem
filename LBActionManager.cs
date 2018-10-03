using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBActionSystem
{
	public class LBActionManager : MonoBehaviour
	{
		public LBAction[] Actions;

		bool b1, b2;

		void Start ()
		{
			InitActions ();

			ActivateAction ("Default");
		}

		void Update ()
		{
			GeneralTick ();
			PhysicsTick ();

			DebugTest ();
		}

		protected void DebugTest ()
		{
			if (Input.GetKey (KeyCode.Alpha1))
			{
				if (!b1)
				{
					b1 = ActivateAction ("Stand");
				}
			}
			else
			{
				if (b1)
				{
					DeactivateAction ("Stand");
					b1 = false;
				}
			}

			if (Input.GetKey (KeyCode.Alpha2))
			{
				if (!b2)
				{
					b2 = ActivateAction ("Walk");
				}
			}
			else
			{
				if (b2)
				{
					DeactivateAction ("Walk");
					b2 = false;
				}
			}

		}

		protected virtual void InitActions ()
		{
			int i, k = 0;

			for (i = 0; i < Actions.Length; i++)
			{
				if (Actions [i] != null)
				{
					Actions [i] = Actions [i].Duplicate (); 
				}
				else
				{
					Actions [i] = LBDummyAction.Default ();
					Actions [i].ActionName = "Dummy Action " + k.ToString ();
					Actions [i].Init (gameObject, this);
					k++;
				}
			}
				
			for (i = 0; i < Actions.Length; i++)
			{
				Actions [i].Init (gameObject, this);
			}
		}

		protected virtual void GeneralTick ()
		{
			int i;

			for (i = 0; i < Actions.Length; i++)
			{
				if (Actions[i].ActionTick == LBActionTickTypes.GeneralTick)
					Actions [i].Tick ();
			}
		}

		protected virtual void PhysicsTick ()
		{
			int i;

			for (i = 0; i < Actions.Length; i++)
			{
				if (Actions[i].ActionTick == LBActionTickTypes.PhysicsTick)
					Actions [i].Tick ();
			}
		}

		//		protected bool CheckActivationTransitive (LBTransitiveAction action)
		//		{
		//
		//			LBAction act;
		//
		//			act = FindSwitchableAction (action);
		//
		//			if (act == null)
		//				return false;
		//
		//			if (action.CanActivateAction (act.ActionName))
		//				return true;
		//			else
		//				return false;
		//		}
			
		//		protected void ActivateTransitive (LBTransitiveAction action)
		//		{
		//			LBAction act;
		//
		//			act = FindSwitchableAction (action);
		//
		//			if (act == null)
		//				return;
		//
		//			action.ActivateAction ();
		//
		//			act.DeactivateAction ();
		//		}

		//		public virtual bool ManualActivateAction(string action)
		//		{
		//			LBAction a;
		//
		//			a = FindAction (action);
		//
		//			if (a == null)
		//				return false;
		//
		//			bool b = a.GetType() == typeof(LBAction);////!!!!
		//
		//			if (a.ActionActivation == LBActionActivationTypes.Manual || a.ActionActivation == LBActionActivationTypes.ConditionalManual)
		//			{
		//				if (a.GetType ().IsSubclassOf(typeof(LBTransitiveAction)) || a.GetType () == typeof(LBTransitiveAction))
		//				{
		//					if (CheckActivationTransitive ((LBTransitiveAction) a))
		//						ActivateTransitive ((LBTransitiveAction) a);
		//				}
		//
		//				if (a.GetType () == typeof(LBAction))
		//				{
		////					if (CheckActivationTransitive (a))
		////						ActivateTransitive (a);
		//				}
		//			}
		//
		//			return false;
		//		}


		public bool ActivateAction (string _action_name)
		{
			LBAction a;

			a = FindAction (_action_name);

			if (a != null)
			{
				return a.ActivateAction ();
			}

			return false;
		}

		public bool DeactivateAction (string _action_name)
		{
			LBAction a;

			a = FindAction (_action_name);

			if (a != null)
			{
				return a.DeactivateAction ();
			}

			return false;
		}

		public LBAction[] AllActions
		{
			get {
				return Actions;
			}
		}

		public LBAction[] ActiveActions
		{
			get {
				LBAction[] acts = new LBAction[0];
				int i, k;

				k = 0;

				for (i = 0; i < Actions.Length; i++)
				{
					if (Actions [i].ActionState == LBActionStates.Active)
					{
						k++;
						Array.Resize (ref acts, k); 
						acts [k - 1] = Actions [i];
					}
				}

				return acts;
			}
		}

		public string[] AllActionNames
		{
			get {
				string[] str;
				int i;

				str = new string[Actions.Length];

				for (i = 0; i < Actions.Length; i++)
				{
					str [i] = Actions [i].ActionName;
				}

				return str;
			}
		}

		public string[] ActiveActionNames
		{
			get {
				string[] str = new string[0];
				int i, k;

				k = 0;

				for (i = 0; i < Actions.Length; i++)
				{
					if (Actions [i].ActionState == LBActionStates.Active)
					{
						k++;
						Array.Resize (ref str, k);
						str [k - 1] = Actions [i].ActionName;
					}
				}

				return str;
			}
		}

		//		LBAction FindSwitchableAction(LBTransitiveAction act)
		//		{
		//			int i, j;
		//
		//			LBAction[] active;
		//
		//			active = ActiveActions;
		//
		//			for (i = 0; i < active.Length; i++)
		//			{
		//				for (j = 0; j < act.TransfersFrom.Length; j++)
		//				{
		//					if (act.TransfersFrom [j] == active [i].ActionName)
		//					{
		//						if (act.TransferType == LBActionTransitTypes.Interrupt)
		//							return active [i];
		//
		//						if (act.TransferType == LBActionTransitTypes.Switch)
		//						{
		//							if (active [i].CanDeactivateAction ())
		//								return active [i];
		//						}
		//					}
		//				}
		//			}
		//
		//			return null;
		//		}
			
		LBAction FindAction (string action)
		{
			int i;

			for (i = 0; i < Actions.Length; i++)
			{
				if (Actions [i].ActionName == action)
					return Actions [i];
			}

			return null;
		}
	}

}