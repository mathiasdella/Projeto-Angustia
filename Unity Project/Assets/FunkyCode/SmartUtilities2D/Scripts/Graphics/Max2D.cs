using System.Collections.Generic;
using UnityEngine;

public class Max2D {
	public enum LineMode {Smooth, Glow, Default};
	
	public static Material lineMaterial;
	private static Material glowMaterial;
	public static Material defaultMaterial; 

	public static float lineWidth = 0.2f;
	public static Max2D.LineMode lineMode = Max2D.LineMode.Smooth;
	private static bool setBorder = false;
	private static Color setColor = Color.white;

	private static Material setPassMaterial = null;
	private static bool optimizeSetPassCalls = false;
	private static Color setPassColor = Color.white;
	public static float setScale = 1f;

	static public void Vertex3(Vector2D p, float z)
    {
        GL.Vertex3((float)p.x, (float)p.y, z);
	}

	static public void SetScale(float scale) {
		setScale = scale;
	}

	static public void SetPass(Material material) {
		if (optimizeSetPassCalls == false || material != setPassMaterial || setPassColor.r != setColor.r || setPassColor.g != setColor.g || setPassColor.b != setColor.b) {
			setPassColor = setColor;
			material.SetPass(0);
			
			setPassMaterial = material;
		}
	}

	static public void ResetSetPass() {
		setPassMaterial = null;
	}

	static public void SetBatching(bool set) {
		optimizeSetPassCalls = set;
	}

	static public void SetBorder(bool border)
	{
		setBorder = border;
	}

	static public void SetLineMode(LineMode mode)
	{
		lineMode = mode;
	}

	public static void SetLineWidth (float size)
	{
		lineWidth = Mathf.Max(.01f, size / 5f);
	}

	public const string shaderPath = "";

	public static void Check()
	{
		if (lineMaterial == null || glowMaterial == null) {
			lineMaterial = new Material (Shader.Find ("Legacy Shaders/Transparent/VertexLit"));
			lineMaterial.mainTexture = Resources.Load ("Textures/LineTexture") as Texture;
			glowMaterial = new Material (Shader.Find (shaderPath + "Particles/Additive"));
			glowMaterial.mainTexture = Resources.Load ("Textures/LineGlowTexture") as Texture;
			lineMaterial.SetInt("_ZWrite", 1);
			lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		}
		if (defaultMaterial == null) {
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			defaultMaterial = new Material(shader);
			defaultMaterial.hideFlags = HideFlags.HideAndDontSave;
			defaultMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			defaultMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			defaultMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			defaultMaterial.SetInt("_ZWrite", 0);
		}
	}

	static public void SetColor(Color color)
	{
		Check ();
		lineMaterial.SetColor ("_Emission", color);
		setColor = color;
	}

	
	static public void iDrawImage(Material material, Vector2D pos, Vector2D size, float z = 0f)
    {
        GL.PushMatrix ();
        material.SetPass (0); 

        GL.Begin (GL.QUADS);
        GL.TexCoord2 (0, 0);
        GL.Vertex3 ((float)(pos.x - size.x),(float)( pos.y - size.y), z);
        GL.TexCoord2 (0, 1);
        GL.Vertex3 ((float)(pos.x - size.x),(float)( pos.y + size.y), z);
        GL.TexCoord2 (1, 1);
        GL.Vertex3 ((float)(pos.x + size.x), (float)(pos.y + size.y), z);
        GL.TexCoord2 (1, 0);
        GL.Vertex3 ((float)(pos.x + size.x), (float)(pos.y - size.y), z);
        GL.End ();

        GL.PopMatrix ();
    }

    static public void iDrawImage2(Material material, Vector2 pos, Vector2 size, float rot, float z = 0f)
    {
        GL.PushMatrix ();
        material.SetPass (0); 
        
        rot = rot * Mathf.Deg2Rad + Mathf.PI;

        float rectAngle = Mathf.Atan2(size.y, size.x);
        float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

        Vector2 v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
        Vector2 v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
        Vector2 v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
        Vector2 v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
        
        GL.Begin (GL.QUADS);
        GL.TexCoord2 (0, 0);
        GL.Vertex3 (v1.x, v1.y, z);
        GL.TexCoord2 (0, 1);
        GL.Vertex3 (v2.x, v2.y, z);
        GL.TexCoord2 (1, 1);
        GL.Vertex3 (v3.x, v3.y, z);
        GL.TexCoord2 (1, 0);
        GL.Vertex3 (v4.x, v4.y, z);
        GL.End ();

        GL.PopMatrix ();
    }


    static public void iDrawMesh(Mesh mesh, Transform transform, Vector2D offset, float z) {
        if (mesh == null)
            return;
    
        GL.PushMatrix();
        defaultMaterial.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        GL.Color(setColor);

        for (int i = 0; i <  mesh.triangles.GetLength (0); i = i + 3) {
            Vector3 a = transform.TransformPoint(mesh.vertices [mesh.triangles [i]]);
			Vector3 b = transform.TransformPoint(mesh.vertices [mesh.triangles [i + 1]]);
			Vector3 c = transform.TransformPoint(mesh.vertices [mesh.triangles [i + 2]]);
            Max2DMatrix.DrawTriangle(a.x, a.y, b.x, b.y, c.x, c.y, offset, z);

        }

        GL.End();
        GL.PopMatrix();    
    }

	static public void DrawMesh(Material material, Mesh mesh, Transform transform, Vector2D offset, float z = 0f)
	{
		if (mesh == null)
			return;
		
		GL.PushMatrix ();
		SetPass(material);
		GL.Begin(GL.TRIANGLES);

		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < mesh.triangles.GetLength (0); i++ ) {
			list.Add (transform.TransformPoint(mesh.vertices [mesh.triangles [i]]));
			if (list.Count > 2) {
				Max2DMatrix.DrawTriangle (list [0].x, list [0].y, list [1].x, list [1].y, list [2].x, list [2].y, offset, z);
				list.Clear ();
			}
		}

		GL.End ();
		GL.PopMatrix ();
	}

	static public void DrawMesh(Mesh mesh, Transform transform, Vector2D offset, float z = 0f)
	{
		if (mesh == null)
			return;
		
		GL.PushMatrix ();
		SetPass(defaultMaterial);
		GL.Begin(GL.TRIANGLES);

		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < mesh.triangles.GetLength (0); i++ ) {
			list.Add (transform.TransformPoint(mesh.vertices [mesh.triangles [i]]));
			if (list.Count > 2) {
				Max2DMatrix.DrawTriangle (list [0].x, list [0].y, list [1].x, list [1].y, list [2].x, list [2].y, offset, z);
				list.Clear ();
			}
		}

		GL.End ();
		GL.PopMatrix ();
	}

	static public void DrawTriangle(Vector2D p0, Vector2D p1, Vector2D p2, Vector2D offset, float z = 0f)
	{
		DrawTrianglef ((float)p0.x, (float)p0.y, (float)p1.x, (float)p1.y, (float)p2.x, (float)p2.y, offset, z);
	}

	static public void DrawSquare(Vector2D p, float size, float z = 0f)
	{
		Vector2D p0 = new Vector2D (p.x - size, p.y - size);
		Vector2D p1 = new Vector2D (p.x + size, p.y - size);
		Vector2D p2 = new Vector2D (p.x + size, p.y + size);
		Vector2D p3 = new Vector2D (p.x - size, p.y + size);

		DrawTriangle (p0, p1, p2, new Vector2D(0, 0), z);
		DrawTriangle (p2, p3, p0, new Vector2D(0, 0), z);
	}

	static public void DrawImage(Material material, Vector2 pos, Vector2 size, float rot, float z = 0f)
    {

        GL.PushMatrix ();
        material.SetPass (0); 
		
		rot = rot * Mathf.Deg2Rad + Mathf.PI;

		float rectAngle = Mathf.Atan2(size.y, size.x);
		float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

		Vector2 v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
		Vector2 v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
		Vector2 v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
		Vector2 v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
		
        GL.Begin (GL.QUADS);
        GL.TexCoord2 (0, 0);
        GL.Vertex3 (v1.x, v1.y, z);
        GL.TexCoord2 (0, 1);
        GL.Vertex3 (v2.x, v2.y, z);
        GL.TexCoord2 (1, 1);
        GL.Vertex3 (v3.x, v3.y, z);
        GL.TexCoord2 (1, 0);
        GL.Vertex3 (v4.x, v4.y, z);
        GL.End ();

        GL.PopMatrix ();
    }

	static public void DrawImage(Transform transform, Material material, Vector2D pos, Vector2D size, float z = 0f) {
		GL.PushMatrix ();
		GL.MultMatrix(transform.localToWorldMatrix);

		SetPass(material);
		GL.Begin (GL.QUADS);

		GL.TexCoord2 (0, 0);
		GL.Vertex3 ((float)pos.x - (float)size.x, (float)pos.y - (float)size.y, z);
		GL.TexCoord2 (0, 1);
		GL.Vertex3 ((float)pos.x - (float)size.x, (float)pos.y + (float)size.y, z);
		GL.TexCoord2 (1, 1);
		GL.Vertex3 ((float)pos.x + (float)size.x, (float)pos.y + (float)size.y, z);
		GL.TexCoord2 (1, 0);
		GL.Vertex3 ((float)pos.x + (float)size.x, (float)pos.y - (float)size.y, z);

		GL.End ();
		GL.PopMatrix ();
	}


	static public void DrawImage(Material material, Vector2D pos, Vector2D size, float z = 0f)
	{
		GL.PushMatrix ();
		SetPass(material);
		GL.Begin (GL.QUADS);

		GL.TexCoord2 (0, 0);
		GL.Vertex3 ((float)pos.x - (float)size.x, (float)pos.y - (float)size.y, z);
		GL.TexCoord2 (0, 1);
		GL.Vertex3 ((float)pos.x - (float)size.x, (float)pos.y + (float)size.y, z);
		GL.TexCoord2 (1, 1);
		GL.Vertex3 ((float)pos.x + (float)size.x, (float)pos.y + (float)size.y, z);
		GL.TexCoord2 (1, 0);
		GL.Vertex3 ((float)pos.x + (float)size.x, (float)pos.y - (float)size.y, z);

		GL.End ();
		GL.PopMatrix ();
	}




	static public void DrawLineSquare(Vector2D p, float size, float z = 0f)
	{
		float width = lineWidth;
		lineWidth = lineWidth * 0.5f;
		DrawLineRectf ((float)p.x - size / 2f, (float)p.y - size / 2f, size, size, z);
		lineWidth = width;
	}

	static public void DrawLine(Vector2D p0, Vector2D p1, float z = 0f)
	{
		if (setBorder == true) {
			Color tmcColor = setColor;
			float tmpWidth = lineWidth;
			SetColor(Color.black);
			lineWidth = tmpWidth * 2f;
			DrawLinef ((float)p0.x, (float)p0.y, (float)p1.x, (float)p1.y, z);
			SetColor(tmcColor);
			lineWidth = tmpWidth;
			DrawLinef ((float)p0.x, (float)p0.y, (float)p1.x, (float)p1.y, z);
			lineWidth = tmpWidth;
		} else {
			DrawLinef((float)p0.x, (float)p0.y, (float)p1.x, (float)p1.y, z);
		}
	}
		
	static public void DrawLinef(float x0, float y0, float x1, float y1, float z = 0f)
	{
		Check ();

		if (lineMode == LineMode.Smooth)
			DrawSmoothLine (new Pair2D (new Vector2D (x0, y0), new Vector2D (x1, y1)), z);
		else {
			GL.PushMatrix();
			SetPass(defaultMaterial);
			GL.Begin(GL.LINES);
			GL.Color(setColor);

			Max2DMatrix.DrawLine (x0, y0, x1, y1, z);

			GL.End();
			GL.PopMatrix();
		}
	}
		
	static public void DrawTrianglef(float x0, float y0, float x1, float y1, float x2, float y2, Vector2D offset, float z = 0f)
	{
		GL.PushMatrix();
		SetPass(defaultMaterial);
		GL.Begin(GL.TRIANGLES);
		GL.Color(setColor);

		Max2DMatrix.DrawTriangle(x0, y0, x1, y1, x2, y2, offset, z);

		GL.End();
		GL.PopMatrix();
	}

	static public void DrawLineRectf(float x, float y, float w, float h, float z = 0f)
	{
		if (lineMode == LineMode.Smooth) {
			GL.PushMatrix ();
			SetPass(lineMaterial);
			GL.Begin (GL.QUADS);

			if (setBorder == true) {
				Color tmcColor = setColor;
				float tmpWidth = lineWidth;

				SetColor (Color.black);
				lineWidth = tmpWidth * 2f;

				Max2DMatrix.DrawLineImage (new Pair2D (new Vector2D (x, y), new Vector2D (x + w, y)), z);
				Max2DMatrix.DrawLineImage (new Pair2D (new Vector2D (x, y), new Vector2D (x, y + h)), z);
				Max2DMatrix.DrawLineImage (new Pair2D (new Vector2D (x + w, y), new Vector2D (x + w, y + h)), z);
				Max2DMatrix.DrawLineImage (new Pair2D (new Vector2D (x, y + h), new Vector2D (x + w, y + h)), z);

				SetColor (tmcColor);
				lineWidth = tmpWidth;
			}

			float tmpLine = lineWidth;
			lineWidth = tmpLine * 1f;

			SetColor (setColor);

			Max2DMatrix.DrawLineImage (new Pair2D(new Vector2D(x, y), new Vector2D(x + w, y)), z);
			Max2DMatrix.DrawLineImage (new Pair2D(new Vector2D(x, y), new Vector2D(x, y + h)), z);
			Max2DMatrix.DrawLineImage (new Pair2D(new Vector2D(x + w, y), new Vector2D(x + w, y+ h)), z);
			Max2DMatrix.DrawLineImage (new Pair2D(new Vector2D(x, y + h), new Vector2D(x + w, y+ h)), z);

			GL.End();
			GL.PopMatrix();

			lineWidth = tmpLine;

		} else {
			DrawLine (new Vector2D (x, y), new Vector2D (x + w, y), z);
			DrawLine (new Vector2D (x + w, y), new Vector2D (x + w, y + h), z);
			DrawLine (new Vector2D (x + w, y + h),	new Vector2D (x, y + h), z);
			DrawLine (new Vector2D (x, y + h), new Vector2D (x, y), z);
		}
	}

	static public void DrawSlice(List< Vector2D> slice, float z = 0f)
	{
		foreach (Pair2D p in Pair2D.GetList(slice, false)) 
			DrawLine (p.A, p.B, z);
	}

	static public void DrawPolygonList(List<Polygon2D> polyList, float z = 0f)
	{
		foreach (Polygon2D p in polyList)
			DrawPolygon (p, z);
	}

	static public void DrawStrippedLine(List<Vector2D> pointsList, float minVertsDistance, float z = 0f, bool full = false, Vector2D offset = null)
	{
		if (offset == null)
			offset = new Vector2D (0, 0);

		Vector2D vA = null, vB = null;

		if (setBorder == true) {
			Color tmcColor = setColor;
			float tmpWidth = lineWidth;

			GL.PushMatrix();
			SetColor (Color.black);
			SetPass(lineMaterial);
			GL.Begin(GL.QUADS);

			lineWidth = 2f * tmpWidth;

			foreach (Pair2D id in Pair2D.GetList(pointsList, full)) {
				vA = new Vector2D (id.A + offset);
				vB = new Vector2D (id.B + offset);

				vA.Push (Vector2D.Atan2 (id.A, id.B), -minVertsDistance / 5 * setScale);
				vB.Push (Vector2D.Atan2 (id.A, id.B), minVertsDistance / 5 * setScale);

				Max2DMatrix.DrawLineImage (new Pair2D(vA, vB), z);
			}

			GL.End();
			GL.PopMatrix();

			SetColor (tmcColor);
			lineWidth = tmpWidth;
		}

		GL.PushMatrix();
		SetPass(lineMaterial);
		GL.Begin(GL.QUADS);

		foreach (Pair2D id in Pair2D.GetList(pointsList, full)) {
			vA = new Vector2D (id.A + offset);
			vB = new Vector2D (id.B + offset);

			vA.Push (Vector2D.Atan2 (id.A, id.B), -minVertsDistance / 4 * setScale);
			vB.Push (Vector2D.Atan2 (id.A, id.B), minVertsDistance / 4 * setScale);

			Max2DMatrix.DrawLineImage (new Pair2D(vA, vB), z);
		}

		GL.End();
		GL.PopMatrix();
	}

	static public void DrawSmoothLine(Pair2D pair, float z = 0f)
	{
		GL.PushMatrix();
		SetPass(lineMaterial);
		GL.Begin(GL.QUADS);

		Max2DMatrix.DrawLineImage (pair, z);

		GL.End();
		GL.PopMatrix();
	}

	static public void DrawPolygon(Polygon2D poly, float z = 0f, bool connect = true)
	{
		Check ();

		switch (lineMode) {
			case LineMode.Smooth:
				GL.PushMatrix ();
				SetPass(lineMaterial);
				GL.Begin(GL.QUADS);

				Max2DMatrix.DrawSliceImage (poly.pointsList, z, connect);

				GL.End();
				GL.PopMatrix();

				break;

			case LineMode.Default:
				GL.PushMatrix();
				SetPass(defaultMaterial);
				GL.Begin(GL.LINES);
				GL.Color(setColor);

				Max2DMatrix.DrawSlice(poly.pointsList, z, connect);

				GL.End ();
				GL.PopMatrix();
			
				break;

			case LineMode.Glow:
				GL.PushMatrix ();
				Color color = setColor;
				color.a /= 2f;
				glowMaterial.SetColor("_TintColor", color);
				SetPass(glowMaterial);
				
				GL.Begin(GL.QUADS);

				Max2DMatrix.DrawSliceImage (poly.pointsList, z, connect);

				GL.End();
				GL.PopMatrix();

				break;
		}

		foreach (Polygon2D p in poly.holesList)
			DrawPolygon (p, z);
	}

	static public void DrawPolygon(Transform transform, Polygon2D poly, float z = 0f, bool connect = true)
	{
		Check ();

		switch (lineMode) {
			case LineMode.Smooth:
				GL.PushMatrix ();
				GL.MultMatrix(transform.localToWorldMatrix);

				SetPass(lineMaterial);
				GL.Begin(GL.QUADS);

				Max2DMatrix.DrawSliceImage (transform, poly.pointsList, z, connect);

				GL.End();
				GL.PopMatrix();

				break;

			case LineMode.Default:
				GL.PushMatrix();
				GL.MultMatrix(transform.localToWorldMatrix);

				SetPass(defaultMaterial);
				GL.Begin(GL.LINES);
				GL.Color(setColor);

				Max2DMatrix.DrawSlice(poly.pointsList, z, connect);

				GL.End ();
				GL.PopMatrix();
			
				break;

			case LineMode.Glow:
				GL.PushMatrix ();
				GL.MultMatrix(transform.localToWorldMatrix);

				Color color = setColor;
				color.a /= 2f;
				glowMaterial.SetColor("_TintColor", color);
				SetPass(glowMaterial);
				
				GL.Begin(GL.QUADS);

				Max2DMatrix.DrawSliceImage (poly.pointsList, z, connect);

				GL.End();
				GL.PopMatrix();

				break;
		}

		foreach (Polygon2D p in poly.holesList)
			DrawPolygon (transform, p, z);
	}
}