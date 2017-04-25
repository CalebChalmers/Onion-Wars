using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotation;
    public Space space;

	void Update ()
	{
        transform.Rotate(rotation * Time.deltaTime, space);
	}
}
