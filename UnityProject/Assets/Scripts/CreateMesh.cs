using UnityEngine;
using System.Collections;

public class CreateMesh : MonoBehaviour 
{
	[SerializeField] private Material Mat;
	[SerializeField] private float Size = 1.0f;

	private MeshRenderer mMeshRenderer;
	private MeshFilter mMesh;

	public Material Material {
		get { return Mat; }
		set { Mat = value; }
	}
	
	private Vector3 [] GetVerts( float size )
	{
		Vector3 [] verts = new Vector3[8]; 
		float wide = size * 0.5f;
		float narrow = size * 0.15f;
		
		verts[0] = new Vector3(   0.0f,   wide, 0.0f );
		verts[1] = new Vector3( narrow, narrow, 0.0f );
		verts[2] = new Vector3(   wide,   0.0f, 0.0f );
		verts[3] = new Vector3( narrow,-narrow, 0.0f );
		verts[4] = new Vector3(   0.0f,  -wide, 0.0f );
		verts[5] = new Vector3(-narrow,-narrow, 0.0f );
		verts[6] = new Vector3(  -wide,   0.0f, 0.0f );
        verts[7] = new Vector3(-narrow, narrow, 0.0f );

		return verts;
	}
	
	private int [] GetTriangles()
	{
		int [] starTriangles = new int[18]; // 6 triangles

        starTriangles[0] = 0;
        starTriangles[1] = 1;
        starTriangles[2] = 7;
        
        starTriangles[3] = 1;
        starTriangles[4] = 2;
        starTriangles[5] = 3;

        starTriangles[6] = 3;
        starTriangles[7] = 4;
        starTriangles[8] = 5;

        starTriangles[9]  = 5;
        starTriangles[10] = 6;
        starTriangles[11] = 7;

        starTriangles[12] = 1;
        starTriangles[13] = 3;
        starTriangles[14] = 5;

        starTriangles[15] = 1;
        starTriangles[16] = 5;
        starTriangles[17] = 7;
        return starTriangles;
	}
	
	private Mesh DoCreateMesh()
	{
		Mesh m = new Mesh();
		m.name = "ScriptedMesh";
		m.vertices = GetVerts( Size ); 
		m.triangles = GetTriangles();
		m.RecalculateNormals();
		
		return m;
	}
	
	void Start() 
	{
		mMeshRenderer = gameObject.AddComponent<MeshRenderer>();
		mMesh = gameObject.AddComponent<MeshFilter>();
		mMesh.mesh = DoCreateMesh();
		mMeshRenderer.material = Mat;
	}
}
