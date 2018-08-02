using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class IcosohedronTester : MonoBehaviour
{

	static float phi = (1 + Mathf.Sqrt(5)) / 2;
	private Vector3[] vertices =
			{
			new Vector3(-phi,0,1), //0
			new Vector3(-1,phi,0), //1
			new Vector3(0,1,phi), //2
			new Vector3(0,-1,phi), //3
			new Vector3(-1,-phi,0), //4
			new Vector3(-phi,0,-1), //5
			new Vector3(0,1,-phi), //6

			new Vector3(1,phi,0), //7
			new Vector3(phi,0,1), //8
			new Vector3(1,-phi,0), //9
			new Vector3(0,-1,-phi), //10
			new Vector3(phi,0,-1) //11

		};
	public int subdivisions = 0;
	public float radius = 1f;

	private void Awake()
	{
		//mesh = IcosohedronGenerator.Create(subdivisions, radius);
		GetComponent<MeshFilter>().mesh = IcosohedronGenerator.Create(subdivisions, radius);

	}
	/*
	private void OnDrawGizmos()
	{
		if (vertices == null)
		{
			return;
		}
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++)
		{
			Gizmos.color = new Color((float)i /5, (float)i /5,(float)i/5);
			Gizmos.DrawSphere(vertices[i]*2, 0.1f);
		}
	}
	*/


}

