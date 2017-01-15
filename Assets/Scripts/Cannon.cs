using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cannon : MonoBehaviour
{

	private List<Vector3> poblacion;
	private List<int> fitness;
	private List<Vector3> seleccionados;
	private const int PoblacionMaxima = 10;
	private const int DistanciaMaxima = 20;
	private Transform druidaObjetivo;
	private bool disparar;
	private bool repoblar;

	public GameObject bala;
	public Transform spawnPoint;
	public Transform pibote;

	#region Monobehaviour Callback

	void Start()
	{
		poblacion = new List<Vector3>();
		for (int i = 0; i < PoblacionMaxima; i++)
		{
			Vector3 individuo = Random.insideUnitSphere * DistanciaMaxima;
			poblacion.Add(individuo);
		}
		disparar = true;
		repoblar = false;
	}

	void Update()
	{
		if (druidaObjetivo == null)
		{
			GameObject.Find("Spawner").GetComponent<Spawner>().SpawnEntity();
			GameObject druida = GameObject.FindGameObjectWithTag("Enemy");
			if (druida != null)
				druidaObjetivo = druida.transform;
			else
				return;
		}

		if (repoblar)
		{
			for (int i = 0; i < 5; i++) //5 Generaciones
			{
				CalcularFitness();
				Seleccionar();
				Reproducir();
				Cruzar();
				Mutar();
			}
			repoblar = false;
			disparar = true;
		}

		if (disparar)
		{
			//StartCoroutine(DispararTodos());
			StartCoroutine(DispararMejor());
		}
	}

	#endregion

	#region Private Methods

	private void CalcularFitness()
	{
		fitness = new List<int>();
		foreach (Vector3 individuo in poblacion)
		{
			//Vector3 direccionCannonIndividuo = transform.position - individuo;
			//Vector3 direccionIndividuoDruida = druidaObjetivo.position - individuo;
			Vector3 direccionPiboteDruida = druidaObjetivo.position - pibote.position;
			Vector3 direccionPiboteIndividuo = individuo - pibote.position;
			float angulo = Vector3.Angle(direccionPiboteIndividuo, direccionPiboteDruida);
			fitness.Add((int) angulo);
		}
	}

	private void Seleccionar()
	{
		seleccionados = new List<Vector3>();
		IEnumerable<int> fitnessOrdenado = fitness.OrderBy(x => x);

		for (int i = 0; i < 4; i++)
		{
			int fitns = fitnessOrdenado.ElementAt(i);
			for (int j = 0; j < fitness.Count; j++)
			{
				if (fitns == fitness[j])
				{
					seleccionados.Add(poblacion[j]);
					break;
				}
			}
		}
	}

	private void Reproducir()
	{
		poblacion = new List<Vector3>();
		foreach (Vector3 seleccionado in seleccionados)
		{
			poblacion.Add(seleccionado);
		}
	}

	private void Cruzar()
	{
		while (poblacion.Count < PoblacionMaxima)
		{
			int[][] padres = { new []{0,1}, new []{0,2}, new []{0,3}, new []{1,2}, new []{1,3}, new []{2,3} };
			foreach (int[] padre in padres)
			{
				int padre1 = padre[0];
				int padre2 = padre[1];
				bool[] escogerPadre1 = new bool[3];
				for (int i = 0; i < 3; i++)
				{
					escogerPadre1[i] = Random.Range(0, 1) == 0;
				}
				Vector3 hijo = new Vector3( poblacion[escogerPadre1[0] ? padre1 : padre2].x,
											poblacion[escogerPadre1[1] ? padre1 : padre2].y,
											poblacion[escogerPadre1[2] ? padre1 : padre2].z); //(poblacion[padre1] + poblacion[padre2]) / 2;
				poblacion.Add(hijo);
			}
		}
	}

	private void Mutar()
	{
		for (int i = 0; i < poblacion.Count; i++)
		{
			if (Random.Range(0, 100) < 25 && fitness[i] > 2) //Probabilidad de mutacion del 15% y con fitneess bajo
			{
				poblacion[i] = new Vector3(Random.Range(-180, 180), poblacion[i].y, poblacion[i].z);
			}
			if (Random.Range(0, 100) < 25 && fitness[i] > 2) //Probabilidad de mutacion del 15%
			{
				poblacion[i] = new Vector3(poblacion[i].x, Random.Range(-180, 180), poblacion[i].z);
			}
			if (Random.Range(0, 100) < 25 && fitness[i] > 2) //Probabilidad de mutacion del 15%
			{
				poblacion[i] = new Vector3(poblacion[i].x, poblacion[i].y, Random.Range(-180, 180));
			}
		}
	}

	private Vector3 SeleccionarMejor()
	{
		if (fitness != null)
		{
			IEnumerable<int> fitnessOrdenado = fitness.OrderBy(x => x);
			int fitns = fitnessOrdenado.ElementAt(0);
			for (int j = 0; j < fitness.Count; j++)
			{
				if (fitns == fitness[j])
				{
					Debug.Log(fitness[j]);
					return poblacion[j];
				}
			}
		}
		return poblacion[0];
	}

	private IEnumerator DispararTodos()
	{
		disparar = false;
		foreach (Vector3 individuo in poblacion)
		{
			Disparar(individuo);
			yield return new WaitForSeconds(0.3f);
		}
		repoblar = true;
	}

	private IEnumerator DispararMejor()
	{
		Disparar(SeleccionarMejor());
		disparar = false;
		yield return new WaitForSeconds(0.5f);
		repoblar = true;
	}

	private void Disparar(Vector3 destino)
	{
		pibote.LookAt(destino);
		GameObject instance = Instantiate(bala, spawnPoint.position, spawnPoint.rotation);
		instance.GetComponent<Rigidbody>().velocity = spawnPoint.forward * 100;
	}

	#endregion

}
