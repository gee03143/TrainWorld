using UnityEngine;

namespace TrainWorld
{
    [RequireComponent(typeof(CurveCreator))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Rail : MonoBehaviour
    {
        [Range(0.01f, 1.5f)]
        public float spacing = 1;
        public float railWidth = 1;
        public float tiling = 1;

        public void InitRail(Vector3Int startPosition, Vector3Int endPosition, Direction startDirection, Direction endDirection)
        {
            CurveCreator creator = GetComponent<CurveCreator>();
            creator.CreateCurve(startPosition, endPosition, startDirection, endDirection);
            Curve curve = creator.curve;
            Vector3[] points = curve.CalculateEvenlySpacedPoints(spacing);
            GetComponent<MeshFilter>().mesh = CreateRailMesh(points, startDirection, endDirection);

            int textureRepeat = Mathf.RoundToInt(tiling * points.Length * spacing * 0.5f);
            GetComponent<MeshRenderer>().sharedMaterial.mainTextureScale = new Vector2(1, textureRepeat);

        }

        Mesh CreateRailMesh(Vector3[] points, Direction placementStartDirection, Direction placementEndDirection)
        {
            Vector3[] verticies = new Vector3[points.Length * 2];   //  numVerticies = 2n
            Vector2[] uvs = new Vector2[verticies.Length];
            int numTris = 2 * (points.Length - 1);
            int[] tris = new int[numTris * 3]; // numTris = 2 * (n - 1), 3 verticies for each triangle
            int vertexIndex = 0;
            int triIndex = 0;

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 forward = Vector3.zero;
                if (i < points.Length - 1) // if current index is not last point
                {
                    forward += points[(i + 1) % points.Length] - points[i];
                }
                if (i > 0)
                {
                    forward += points[i] - points[(i - 1 + points.Length) % points.Length];
                }
                forward.Normalize();    // 맨 앞이나 뒤가 아닐 경우 이웃의 방향 벡터의 평균이 forward 값이 됨

                Vector3 left = new Vector3(-forward.z, 0, forward.x);

                verticies[vertexIndex] = points[i] + left * railWidth * 0.5f;
                verticies[vertexIndex + 1] = points[i] - left * railWidth * 0.5f;

                float completionPercent = i / (float)(points.Length - 1);
                uvs[vertexIndex] = new Vector2(0, completionPercent);   //  texture의 (왼쪽, 거리 percent에 mapping)
                uvs[vertexIndex + 1] = new Vector2(1, completionPercent);// texture의 (오른쪽, 거리 precent에 mapping)

                if (i < points.Length - 1)
                {
                    //mesh 의 tri를 구성하는 세 verticies
                    // 두 point 사이에 2개의 삼각형 존재
                    // 마지막 point에서는 길이 없으므로 연산하지 않음, (closed path일때는 연산)
                    tris[triIndex] = vertexIndex;
                    tris[triIndex + 1] = (vertexIndex + 2) % verticies.Length;
                    tris[triIndex + 2] = (vertexIndex + 1);

                    tris[triIndex + 3] = (vertexIndex + 1);
                    tris[triIndex + 4] = (vertexIndex + 2) % verticies.Length;
                    tris[triIndex + 5] = (vertexIndex + 3) % verticies.Length;
                }

                vertexIndex += 2;
                triIndex += 6;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = verticies;
            mesh.triangles = tris;
            mesh.uv = uvs;

            return mesh;
        }
    }
}
