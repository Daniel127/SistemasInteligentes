using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour {

	void Start () {	
		Destroy(this.gameObject, 3f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == "Enemy")
		{
			Destroy(collision.collider.gameObject);
			Destroy(gameObject);
		}
	}
}
