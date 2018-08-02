using UnityEngine;
using System.Collections;
//using UnityEditor;

namespace Icosohedron
{

	public class IcoSide : MonoBehaviour
	{
		private Mesh mesh;
		private Material mat;
		public Texture2D heightMap;
		private Transform player;

		private float radius = 1; // size of planet
		private float meshSubdivisionAmount = 1; //# of triangles per "side". Ie mesh density. Instead of more subdivisions, just put more triangles on each node
		private int depth = 0;  //simple rank of how deep it is
		private int maxDepth = 3; //max times this is subdivide
		private bool inversePlanet;
		private bool isSubdivided;
		public bool isSelfUpdating; //differentiates between init phase (from manager) and normal running

		private Vector3[] corners = new Vector3[3]; //outer 3 corners of the triangle
		private Vector3 center; //center of triangle
		private Vector2[] uvs = new Vector2[3];
		private float criticalDistance; //how close the player needs to be to have the triangle subdivide
		private float edgeLengthAB; //distance from a to b.
		private float edgeLengthBC; //distance from a to b.

		private float distanceToPlayer;

		//References to its root and children. essentially a quadtree but for triangles
		private IcoSide rootSide = null; //above it
		private IcoSide topSubSide; //upper child
		private IcoSide middleSubSide; //middle child
		private IcoSide leftSubSide; //left bottom child
		private IcoSide rightSubSide; // right bottom child

		//References to its surrounding 
		private IcoSide rightNeighbor; //right neighbor
		private IcoSide bottomNeighbor; // bottom neighbor
		private IcoSide leftNeighbor; // left neighbor

		public void InitTopSide(Vector3 a, Vector3 b, Vector3 c, Material mat, Texture2D heightMap, Transform player, float meshSubdivisionAmount, int maxDepth, float radius, bool inversePlanet)
		{
			this.mat = mat;
			this.heightMap = heightMap;
			this.player = player;
			this.meshSubdivisionAmount = meshSubdivisionAmount;
			this.maxDepth = maxDepth;
			this.radius = radius;
			this.inversePlanet = inversePlanet;

			corners[0] = a;
			corners[1] = b;
			corners[2] = c;

			InitializeValues();
			isSelfUpdating = false;
		}

		private IcoSide InitializeSubdividedSide(IcoSide parent, Vector3 a, Vector3 b, Vector3 c, Vector2 uva, Vector2 uvb, Vector2 uvc)
		{
			transform.parent = parent.transform;
			mat = parent.mat;
			heightMap = parent.heightMap;
			player = parent.player;
			radius = parent.radius;
			inversePlanet = parent.inversePlanet;

			rootSide = parent;
			meshSubdivisionAmount = parent.meshSubdivisionAmount * 1f;
			depth = parent.depth + 1;
			maxDepth = parent.maxDepth;

			corners[0] = a;
			corners[1] = b;
			corners[2] = c;

			uvs[0] = uva;
			uvs[1] = uvb;
			uvs[2] = uvc;

			isSelfUpdating = parent.isSelfUpdating;

			InitializeValues();
			CheckForSubdivision();
			if (isSelfUpdating)
				StartSelfUpdate();
			return this;
		}

		private void InitializeValues()
		{
			isSubdivided = false;

			edgeLengthAB = (corners[1] - corners[0]).magnitude;
			edgeLengthBC = (corners[2] - corners[1]).magnitude;

			center = ((corners[0] + corners[1] + corners[2]) / 3).normalized * radius; //center on the sphere, not the exact center of a,b,c

			float heightValue = 0;
			//float u = 0.5f + Mathf.Atan2(center.z, center.x) / (2 * Mathf.PI);
			//float v = 0.5f - Mathf.Asin(center.y / radius) / Mathf.PI;

			if (heightMap != null) {
				//heightValue = heightMap.GetPixelBilinear((u), (v)).r;
			}
			center += heightValue * 250.0f * center.normalized;

			//criticalDistance = 2 * (center - a).magnitude;
			criticalDistance = 2 * edgeLengthAB;

			distanceToPlayer = (center - player.position).magnitude;

			gameObject.AddComponent<MeshFilter>().mesh = createSubMesh(corners[1], corners[0], corners[2], (int)meshSubdivisionAmount, radius, inversePlanet);
			gameObject.AddComponent<MeshRenderer>().material = mat;
			gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			// add physics grid if at bottom subdivision. Currently makinga mesh with the 2nd to bottom one.
			if (depth == maxDepth && depth != 1) {
				gameObject.AddComponent<MeshCollider>();
			}

			//UpdateNeighbors();
		}

		public void StartSelfUpdate()
		{
			isSelfUpdating = true;
			if (isSubdivided) {
				topSubSide.StartSelfUpdate();
				middleSubSide.StartSelfUpdate();
				leftSubSide.StartSelfUpdate();
				rightSubSide.StartSelfUpdate();
			}
			StartCoroutine(CheckPlayerDistance());
		}

		public IEnumerator CheckPlayerDistance()
		{
			yield return new WaitForSeconds(Random.Range(0, 0.5f)); //makes each side have a slightly different time in which it updates. Also gives a sick "dissolve" effect when moving about
			for (;;) {
				float distanceToPlayer = (center - player.position).magnitude;
				if (!isSubdivided && depth < maxDepth && distanceToPlayer < criticalDistance) {
					Subdivide(this, corners[0], corners[1], corners[2], uvs[0], uvs[1], uvs[2]);
				}
				//checks if player is far enough away to un-subdivide
				if (isSubdivided && distanceToPlayer > criticalDistance && depth > 1)
				{
					isSubdivided = false;
					gameObject.GetComponent<MeshRenderer>().enabled = true;
					foreach(Transform child in transform)
					{
						Destroy(child.gameObject);
					}
					topSubSide = null;
					middleSubSide = null;
					leftSubSide = null;
					rightSubSide = null;
				}
				yield return new WaitForSeconds(0.5f);
			}
		}

		public void CheckForSubdivision()
		{
			float distanceToPlayer = (center - player.position).magnitude;
			if (!isSubdivided && depth < maxDepth && distanceToPlayer < criticalDistance) {
				Subdivide(this, corners[0], corners[1], corners[2], uvs[0], uvs[1], uvs[2]);
			}
		}

		private void Subdivide(IcoSide parent, Vector3 a, Vector3 b, Vector3 c, Vector2 uva, Vector2 uvb, Vector2 uvc)
		{
			isSubdivided = true;
			gameObject.GetComponent<MeshRenderer>().enabled = false;

			Vector3 midAB = ((a + b) / 2);
			Vector3 midBC = ((b + c) / 2);
			Vector3 midAC = ((a + c) / 2);

			Vector2 midUVab = ((uva + uvb) / 2);
			Vector2 midUVbc = ((uvb + uvc) / 2);
			Vector2 midUVac = ((uva + uvc) / 2);

			topSubSide = new GameObject("Child top D= " + (depth + 1)).AddComponent<IcoSide>().InitializeSubdividedSide(this, a, midAB, midAC, uva, midUVab, midUVac); //top
			leftSubSide = new GameObject("Child left D= " + (depth + 1)).AddComponent<IcoSide>().InitializeSubdividedSide(this, midAB, b, midBC, midUVab, uvb, midUVbc); //bottom left
			middleSubSide = new GameObject("Child Middle D= " + (depth + 1)).AddComponent<IcoSide>().InitializeSubdividedSide(this, midBC, midAC, midAB, midUVab, midUVbc, midUVac); //middle
			rightSubSide = new GameObject("Child Right D= " + (depth + 1)).AddComponent<IcoSide>().InitializeSubdividedSide(this, midAC, midBC, c, midUVac, midUVbc, uvc); //bottom right
		}

		//Levels starts at 1 for no subdivision, 2, for 1 level and so on
		private Mesh createSubMesh(Vector3 a, Vector3 b, Vector3 c, int levels, float radius, bool InverseSphere)
		{
			Vector3[] vertices = new Vector3[(levels + 2) * (levels + 1) / 2];
			int[] triangles = new int[3 * levels * levels];
			Vector3[] normals = new Vector3[vertices.Length];
			Vector2[] uv = new Vector2[vertices.Length];
			//Color[] colors = new Color[vertices.Length];

			Vector3 Lab = (b - a);
			Vector3 Lbc = (c - b);

			Vector2 uvAB = (uvs[1] - uvs[0]);
			Vector2 uvBC = (uvs[2] - uvs[0]);

			int counter = 0;
			for (int i = 0; i <= levels; i++)
			{
				for (int j = 0; j <= i; j++)
				{
					vertices[counter] = (a + i * Lab / levels + j * Lbc / levels).normalized*radius; //gets where in the triangle it should be. (x,y,z) between a,b,c.

					float heightValue = 0;
					//float u = 0.5f + Mathf.Atan2(vertices[counter].z, vertices[counter].x) / (2 * Mathf.PI);
					//float v = 0.5f - Mathf.Asin(vertices[counter].y/radius) / Mathf.PI;

					Vector2 uvL = uvs[0] + i * uvAB / levels + j * uvBC / levels;

					if (heightMap != null) {
						heightValue = heightMap.GetPixel((int)(uvL.x * 1500), (int)(uvL.y * 1000)).grayscale;
					}
					vertices[counter] += heightValue * 1000.0f * vertices[counter].normalized; //normalizes it so it approximates a sphere.

					if (InverseSphere)
						normals[counter] = (-vertices[counter]).normalized; //inverts normals for inverse planet
					else
						normals[counter] = vertices[counter].normalized;
					//uv[counter] = new Vector2((float)i / levels, (float)j / levels);
					uv[counter] = uvL;
					counter++;
				}
			}
			counter = 0;
			if (InverseSphere) {
				for (int i = 0; i < levels; i++) {
					for (int j = 0; j <= i; j++) {
						triangles[counter++] = (i) * (i + 1) / 2 + j;
						triangles[counter++] = (i + 1) * (i + 2) / 2 + j;
						triangles[counter++] = (i + 1) * (i + 2) / 2 + j + 1;

						if (j != i)	{
							triangles[counter++] = (i) * (i + 1) / 2 + j;
							triangles[counter++] = (i + 1) * (i + 2) / 2 + j + 1;
							triangles[counter++] = (i) * (i + 1) / 2 + j + 1;
						}
					}
				}
			} else {
				for (int i = 0; i < levels; i++) {
					for (int j = 0; j <= i; j++) {
						triangles[counter++] = (i + 1) * (i + 2) / 2 + j;
						triangles[counter++] = (i) * (i + 1) / 2 + j;
						triangles[counter++] = (i + 1) * (i + 2) / 2 + j + 1;

						if (j != i)	{
							triangles[counter++] = (i + 1) * (i + 2) / 2 + j + 1;
							triangles[counter++] = (i) * (i + 1) / 2 + j;
							triangles[counter++] = (i) * (i + 1) / 2 + j + 1;
						}
					}
				}
			}

			Mesh mesh = new Mesh();
			mesh.name = "SubdividedMesh";
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.normals = normals;
			//uv[0] = uvs[0];
			//uv[1] = uvs[1];
			//uv[2] = uvs[2];
			mesh.uv = uv;
			//mesh.colors = colors;
			return mesh;
		}

		public void SetUVs(Vector2 a, Vector2 b, Vector2 c) {
			uvs[0] = a;
			uvs[1] = b;
			uvs[2] = c;
		}

		public void UpdateNeighbors() { //update all neighbor references
			rightNeighbor = getRightNeighbor();
			leftNeighbor = getleftNeighbor();
			bottomNeighbor = getBottomNeighbor();
		}

		public void SetNeighbors(IcoSide left, IcoSide right, IcoSide bottom)
		{
			leftNeighbor = left;
			rightNeighbor = right;
			bottomNeighbor = bottom;
		}

		private IcoSide getRightNeighbor() // neighbor to the right of it
		{
			return null;
		}

		private IcoSide getleftNeighbor() // neighbor to the left  of it
		{
			return null;
		}

		private IcoSide getBottomNeighbor() // neighbor below the triangle
		{

			return null;
		}
	}

	//[CustomEditor(typeof(IcoSide))]
	//public class SubdivideButton : Editor
	//{
	//	public override void OnInspectorGUI()
	//	{
	//		DrawDefaultInspector();

	//		IcoSide myIcoScript = (IcoSide)target;
	//		if (GUILayout.Button("Subdivide"))
	//		{
	//			myIcoScript.Subdivide();
	//		}
	//	}
	//}
}
