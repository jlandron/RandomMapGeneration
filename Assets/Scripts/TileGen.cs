using UnityEngine;


public class TileGen : MonoBehaviour
{
    [SerializeField]
    NoiseMapGen _noiseMapGeneration;

    [SerializeField]
    MeshRenderer _tileRenderer;

    [SerializeField]
    MeshFilter _meshFilter;

    [SerializeField]
    MeshCollider _meshCollider;

    [SerializeField]
    public WorldGen settings;

    void Start()
    {
        GenerateTile();
    }



    void GenerateTile()
    {
        // calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = _meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        // calculate the offsets based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // calculate the offsets based on the tile position
        float[,] heightMap = _noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, settings.mapScale, offsetX, offsetZ, settings.waves);
        float[,] treeMap = _noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, settings.mapScale / 2, offsetX, offsetZ, settings.treeWaves);

        // generate a heightMap using noise
        Texture2D tileTexture = BuildTexture(heightMap, treeMap);
        _tileRenderer.material.mainTexture = tileTexture;
        UpdateMeshVertices(heightMap);
    }

    Texture2D BuildTexture(float[,] heightMap, float[,] treeMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];
                float treeVal = treeMap[zIndex, xIndex];
                // choose a terrain type according to the height value
                TerrainType terrainType = ChooseTerrainType(height);
                // assign the color according to the terrain type
                colorMap[colorIndex] = terrainType.color;
            }
        }

        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }
    TerrainType ChooseTerrainType(float height)
    {
        // for each terrain type, check if the height is lower than the one for the terrain type
        foreach (TerrainType terrainType in settings.terrainTypes)
        {
            // return the first terrain type whose height is higher than the generated one
            if (height < terrainType.height)
            {
                return terrainType;
            }
        }
        return settings.terrainTypes[settings.terrainTypes.Count - 1];
    }

    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this._meshFilter.mesh.vertices;

        // iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];

                Vector3 vertex = meshVertices[vertexIndex];
                // change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, settings.heightCurve.Evaluate(height) * settings.heightMultiplier, vertex.z);

                vertexIndex++;
            }
        }

        // update the vertices in the mesh and update its properties
        _meshFilter.mesh.vertices = meshVertices;
        _meshFilter.mesh.RecalculateBounds();
        _meshFilter.mesh.RecalculateNormals();
        // update the mesh collider
        _meshCollider.sharedMesh = _meshFilter.mesh;
    }
}
