using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnStart : MonoBehaviour
{
    public GameObject prefab;

	void Start ()
	{
        GameObject clone = Instantiate(prefab, transform.parent);
        clone.transform.SetParent(transform.parent);
        clone.transform.position = transform.position;
        clone.transform.rotation = transform.rotation;
        clone.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
}
