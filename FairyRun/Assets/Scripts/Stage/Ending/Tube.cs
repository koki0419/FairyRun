using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    [SerializeField]
    int angleSegments = 16;

    private void Awake()
    {
        SetTube();
    }
    public void SetTube()
    {
        var mesh = new Mesh();

        var vertices = new Vector3[angleSegments * 2];
        var uv = new Vector2[angleSegments * 2];
        var angleStep = Mathf.PI * 2.0f / angleSegments;
        int vertex = 0;
        for (int heightIndex = 0; heightIndex < 2; heightIndex++)
        {
            for (int angleIndex = 0; angleIndex < angleSegments; angleIndex++)
            {
                var angle = angleStep * angleIndex;
                vertices[vertex] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), heightIndex);
                uv[vertex] = new Vector2(angleIndex / (float)angleSegments, heightIndex);
                vertex++;
            }
        }

        var triangles = new int[angleSegments * 6];
        for (int angleIndex = 0; angleIndex < angleSegments; angleIndex++)
        {
            triangles[angleIndex * 6 + 0] = (angleIndex + 0);
            triangles[angleIndex * 6 + 1] = (angleIndex + angleSegments);
            triangles[angleIndex * 6 + 2] = (angleIndex + 1) % angleSegments;
            triangles[angleIndex * 6 + 3] = (angleIndex + 1) % angleSegments + angleSegments;
            triangles[angleIndex * 6 + 4] = (angleIndex + 1) % angleSegments;
            triangles[angleIndex * 6 + 5] = (angleIndex + angleSegments);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    [SerializeField]
    private float rotateSpeed = 0;
    private void Update()
    {
        transform.Rotate(0, 0, rotateSpeed);
    }
}
