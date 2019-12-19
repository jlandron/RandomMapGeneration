using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NoiseMapGen;

public class WorldGen : MonoBehaviour
{
    [SerializeField]
    public float mapScale;
    [SerializeField]
    private int tileOffset = 10;

    [SerializeField]
    public List<TerrainType> terrainTypes;

    [SerializeField]
    public float heightMultiplier;

    [SerializeField]
    public AnimationCurve heightCurve;

    [SerializeField]
    GameObject tilePrefab;

    [SerializeField]
    public Wave[] waves;
    [SerializeField]
    public Wave[] treeWaves;


    [SerializeField]
    private bool regenerate = false;

    private void Start()
    {
        BuildMap();
    }

    private void BuildMap()
    {
        for (int i = 0; i < mapScale; i++)
        {
            for (int j = 0; j < mapScale; j++)
            {
                GameObject go = Instantiate(tilePrefab, new Vector3(i * tileOffset, 0, j * tileOffset), Quaternion.identity, this.transform);
                go.GetComponent<TileGen>().settings = this;
            }
        }
    }
    private void Update()
    {
        if (regenerate)
        {
            regenerate = false;
            TileGen[] oldTiles = FindObjectsOfType<TileGen>();
            for (int i = 0; i < oldTiles.Length; i++)
            {
                Destroy(oldTiles[i].gameObject);
            }
            BuildMap();
        }
    }
}
