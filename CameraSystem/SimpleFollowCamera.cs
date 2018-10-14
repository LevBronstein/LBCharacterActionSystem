using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
	public GameObject Target;
	//public Vector3 Offset = Vector3.forward;

	protected GameObject target;
	protected Vector3 offset;

	// Use this for initialization
	void Start ()
	{
		offset = transform.position - Target.transform.position;
		target = Target;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (target != Target)
		{
			offset = transform.position - Target.transform.position;
			target = Target;
		}

		if (Target!=null && Target.transform != null)
		{
			transform.position = Target.transform.position + offset;
			//transform.rotation = Quaternion.LookRotation (transform.position - Target.transform.position);
		}
	}
}
