using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBActionSystem;

public class StatDisplay : MonoBehaviour
{
	public GameObject character;
	LBActionManager m;

	//Text t;
	// Use this for initialization

	void Start ()
	{
		m = character.GetComponent<LBActionManager> ();
		//t = gameObject.GetComponent <Text> ();
	}

	// Update is called once per frame
	void Update ()
	{
//		LBAction[] actions;
//
//		actions = m.ActiveActions;
//
//		if (m != null) {
//			LBMechanicBase mc = m.FindActiveMechanic (mechanicgroup);
//
//			if (mc != null) {
//				t.text = "Active mechanic in group " + mechanicgroup + "\n" + mc.ToString ();
//
//				if (mc is LBMovementMechanic) {
//					t.text = t.text + "\n Speed:" + (character.GetComponent <Rigidbody> ()).velocity.magnitude + " " + (character.GetComponent <Rigidbody> ()).velocity +
//						"\n Dir:" + (character.GetComponent <Rigidbody> ()).rotation;
//				}
//			} else
//				t.text = "No active mechanics in group " + mechanicgroup;
//		} else {
//			t.text = "character not found!";
//		}
	}
}
