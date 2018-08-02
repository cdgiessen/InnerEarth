using UnityEngine;

public static class IcosohedronGenerator
{
	public static Mesh Create(int subdivisions, float radius)
	{
		int resolution = 1 << subdivisions;
		//Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1) * 4 - (resolution * 2 - 1) * 3];
		/*Vector3[] vertices = {
			Vector3.down,
			Vector3.forward,
			Vector3.left,
			Vector3.back,
			Vector3.right,
			Vector3.up
		};*/

		float phi = (1 + Mathf.Sqrt(5)) / 2;

		Vector3[] vertices =
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
		int[] triangles =
		{
			1,0,2,
			2,0,3,
			3,0,4,
			4,0,5,
			5,0,1,

			1,2,7,
			7,2,8,
			8,2,3,
			3,9,8,
			3,4,9,
			9,4,10,
			10,4,5,
			6,10,5,
			6,5,1,
			6,1,7,

			11,7,8,
			11,8,9,
			11,9,10,
			11,10,6,
			11,6,7
		};
		//int[] triangles = new int[(1 << (subdivisions * 2 + 3)) * 3];

		//vertices[0] = new Vector3(0, 0, 1);
		//vertices[1] = new Vector3(1, 0, 0);
		//vertices[2] = new Vector3(0, 1, 0);

		Vector3[] normals = new Vector3[vertices.Length];
		Vector2[] uv = new Vector2[vertices.Length];

		if (radius != 1f)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] *= radius;
			}
		}

		Mesh mesh = new Mesh();
		mesh.name = "Icosohedron";
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.triangles = triangles;
		return mesh;

	}
}
