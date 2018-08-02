using UnityEngine;
using System.Collections.Generic;
using System.Collections;
//using UnityEditor;

namespace Icosohedron
{
	public class IcoManager : MonoBehaviour
	{
		public IcoSide[] sides = new IcoSide[20];
		public Transform player;

		public Material material;
		public Texture2D heightMap;

		public int maxDepth = 0;
		public int MeshSubdivisionAmount = 1;
		public float radius = 1;
		public bool InversePlanet = true;

		void Awake()
		{
			GenerateIcosohedron();
		}

		public void GenerateIcosohedron()
		{
			float phi = (1 + Mathf.Sqrt(5)) / 2; //~1.61803 ie golden ratio  

			Vector3[] corners = {
			new Vector3(-phi,0,1).normalized*radius, //0
			new Vector3(0,1,phi).normalized*radius, //1
			new Vector3(-1,phi,0).normalized*radius, //2
			new Vector3(-phi,0,-1).normalized*radius, //3
			new Vector3(-1,-phi,0).normalized*radius, //4
			new Vector3(0,-1,phi).normalized*radius, //5
			new Vector3(1,phi,0).normalized*radius, //6
			new Vector3(0,1,-phi).normalized*radius, //7
			new Vector3(0,-1,-phi).normalized*radius, //8
			new Vector3(1,-phi,0).normalized*radius, //9
			new Vector3(phi,0,1).normalized*radius, //10
			new Vector3(phi,0,-1).normalized*radius //11
			};

			int[] triangles = {
			0,1,2, 0,2,3, 0,3,4, 0,4,5, 0,5,1,

			1,10,6, 1,6,2, 2,6,7, 2,7,3, 3,7,8, 3,8,4, 4,8,9, 4,9,5, 5,9,10, 5,10,1,

			10,11,6, 6,11,7, 7,11,8, 8,11,9, 9,11,10 
			};

			int counter = 0;
			sides = new IcoSide[20];
			for (int i = 0; i < 20; i++) {
				IcoSide side = new GameObject("Top IcoSide " + (i + 1)).AddComponent<IcoSide>();
				side.transform.parent = transform;
				side.InitTopSide(corners[triangles[counter++]], corners[triangles[counter++]], corners[triangles[counter++]], material, heightMap, player, MeshSubdivisionAmount, maxDepth, radius, InversePlanet);
				sides[i] = side;
			}

			//sides[0].SetNeighbors(sides[4], sides[5], sides[1]);
			//sides[1].SetNeighbors(sides[0], sides[7], sides[2]);
			//sides[2].SetNeighbors(sides[1], sides[9], sides[3]);
			//sides[3].SetNeighbors(sides[2], sides[11], sides[4]);
			//sides[4].SetNeighbors(sides[3], sides[13], sides[0]);
			//sides[5].SetNeighbors(sides[0], sides[14], sides[6]);
			//sides[6].SetNeighbors(sides[5], sides[15], sides[7]);
			//sides[7].SetNeighbors(sides[6], sides[8], sides[1]);
			//sides[8].SetNeighbors(sides[9], sides[7], sides[16]);
			//sides[9].SetNeighbors(sides[2], sides[8], sides[10]);
			//sides[10].SetNeighbors(sides[9], sides[17], sides[11]);
			//sides[11].SetNeighbors(sides[10], sides[12], sides[3]);
			//sides[12].SetNeighbors(sides[18], sides[13], sides[11]);
			//sides[13].SetNeighbors(sides[12], sides[14], sides[4]);
			//sides[14].SetNeighbors(sides[13], sides[19], sides[5]);
			//sides[15].SetNeighbors(sides[19], sides[16], sides[6]);
			//sides[16].SetNeighbors(sides[15], sides[17], sides[8]);
			//sides[17].SetNeighbors(sides[16], sides[18], sides[10]);
			//sides[18].SetNeighbors(sides[17], sides[19], sides[12]);
			//sides[19].SetNeighbors(sides[18], sides[15], sides[14]);

			Vector2[] uvs = new Vector2[24];
			counter = 0;
			for (float i = 0; i <= 1; i += 1.0f/3) {
				for (float j = 0; j <= 1; j += 0.2f) {
					uvs[counter++] = new Vector2(j, 1 -i);
					Debug.Log((counter - 1) + " " + uvs[counter - 1]);
				}
			}

			sides[0].SetUVs(uvs[0], uvs[6], uvs[7]);
			sides[1].SetUVs(uvs[1], uvs[7], uvs[8]);
			sides[2].SetUVs(uvs[2], uvs[8], uvs[9]);
			sides[3].SetUVs(uvs[3], uvs[9], uvs[10]);
			sides[4].SetUVs(uvs[4], uvs[10], uvs[11]);
			sides[5].SetUVs(uvs[6], uvs[12], uvs[13]);
			sides[6].SetUVs(uvs[6], uvs[13], uvs[7]);
			sides[7].SetUVs(uvs[7], uvs[13], uvs[14]);
			sides[8].SetUVs(uvs[7], uvs[14], uvs[8]);
			sides[9].SetUVs(uvs[8], uvs[14], uvs[15]);
			sides[10].SetUVs(uvs[8], uvs[15], uvs[9]);
			sides[11].SetUVs(uvs[9], uvs[15], uvs[16]);
			sides[12].SetUVs(uvs[9], uvs[16], uvs[10]);
			sides[13].SetUVs(uvs[10], uvs[16], uvs[17]);
			sides[14].SetUVs(uvs[10], uvs[17], uvs[11]);
			sides[15].SetUVs(uvs[12], uvs[19], uvs[13]);
			sides[16].SetUVs(uvs[13], uvs[20], uvs[14]);
			sides[17].SetUVs(uvs[14], uvs[21], uvs[15]);
			sides[18].SetUVs(uvs[15], uvs[22], uvs[16]);
			sides[19].SetUVs(uvs[16], uvs[23], uvs[17]);


			foreach (IcoSide side in sides) {
				//side.UpdateNeighbors();
				side.CheckForSubdivision();
			}

			foreach (IcoSide side in sides) {
				side.StartSelfUpdate();
			}

			Mesh meh = new Mesh();

			Vector3[] v = new Vector3[3];
			Vector2[] uv = new Vector2[3];
			int[] tris = { 0, 1, 2 };
			v[0] = new Vector3(0, 1, 0);
			v[1] = new Vector3(1, 0, 0);
			v[2] = new Vector3(0, 0, 0);

			uv[0] = new Vector2(0, 1);
			uv[1] = new Vector2(0, 0.83f);
			uv[2] = new Vector2(0.1f, 0.83f);

			Vector3[] n = new Vector3[3];
			n[0] = Vector3.back;
			n[1] = Vector3.back;
			n[2] = Vector3.back;

			meh.vertices = v;
			meh.uv = uv;
			meh.normals = n;
			meh.triangles = tris;

			gameObject.AddComponent<MeshFilter>().mesh = meh;
			gameObject.AddComponent<MeshRenderer>().material = material;
		}

		public void DeleteIcosohedron()
		{
			foreach (Transform child in transform) {
				DestroyImmediate(child.gameObject);
			}
		}

		public void InitializeTopSide()
		{



		}

		private void Update()
		{
			//Debug.Log(gameObject.GetComponentsInChildren<Transform>().Length);
		}
	}

	//[CustomEditor(typeof(IcoManager))]
	//public class ObjectBuilderEditor : Editor
	//{
	//	public override void OnInspectorGUI()
	//	{
	//		DrawDefaultInspector();

	//		IcoManager myScript = (IcoManager)target;
	//		if (GUILayout.Button("Generate Icosohedron")) {
	//			myScript.GenerateIcosohedron();
	//		}
		
	//		if (GUILayout.Button("Delete Icosohedron")) {
	//			myScript.DeleteIcosohedron();
	//		}
	//	}
	//}

}
