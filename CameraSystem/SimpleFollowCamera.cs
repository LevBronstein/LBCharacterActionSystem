using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollowCamera : MonoBehaviour
{
	public GameObject Target;
	//public Vector3 Offset = Vector3.forward;

	public Vector3 OrbitOffset;
	public float OrbitRadius;

	protected GameObject target;
	protected float rotx, roty, rotz;

	void Start ()
	{
		target = Target;
	}

	// Update is called once per frame
	void Update ()
	{
		if (target != Target)
		{
			target = Target;
		}

		if (Target!=null && Target.transform != null)
		{
			gameObject.transform.position = Target.transform.position + OrbitOffset + Quaternion.Euler (rotx, roty, 0) * Vector3.forward * OrbitRadius;
			gameObject.transform.rotation = Quaternion.LookRotation ((Target.transform.position + OrbitOffset) - gameObject.transform.position);

		}

		ControlCamera ();
	}

	void ControlCamera()
	{
		rotx = rotx + Input.GetAxis ("ViewRotationVer");
		roty = roty + Input.GetAxis ("ViewRotationHor");
	}

}
