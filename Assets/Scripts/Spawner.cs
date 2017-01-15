using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Spawner : MonoBehaviour
{

	public float spawnRadius;
	public List<GameObject> entities;

	#region Monobehaviour Callback

	void OnDrawGizmos(){
		#if UNITY_EDITOR
			Handles.DrawWireArc( transform.position, Vector3.up,transform.forward, 360, spawnRadius);
		#endif
	}
	#endregion

	#region Public methods
	public void SpawnEntity()
	{
		Vector2 positionCircle = (Vector3) Random.insideUnitCircle * spawnRadius;
	    Vector3 position = new Vector3(transform.position.x, 0, transform.position.z);
	    position += new Vector3(positionCircle.x, 0, positionCircle.y);
		GameObject instanceGameObject = entities[Random.Range(0, entities.Count)];
	    Instantiate(instanceGameObject, position, Quaternion.identity);
	}
	#endregion
}
