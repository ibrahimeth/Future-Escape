using UnityEngine;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> tilePrefebs;
    public GameObject tilePrefab;
    public int tileCount = 2;         // Başlangıçta kaç tile spawnlanacak
    public float tileSpacing = 0f;    // İsteğe bağlı ekstra boşluk (0 = tam bitişik)
    [SerializeField] private float toleranceBetweenTiles = 0.5f; // Tile'lar arasındaki boşluk toleransı

    private List<GameObject> SpawnedTiles = new List<GameObject>();
    private float tileWidth;
    void Start()
    {
        // Prefab’ın Renderer boyutunu al
        Renderer rend = tilePrefab.GetComponentInChildren<Renderer>();
        tileWidth = rend.bounds.size.x;

        SpawnInitialTiles();
    }

    void Update()
    {
        toleranceBetweenTiles = FutureEscape.Tiles.GlobalTileSettings.speed / 32f;
        //Debug.Log("toleranceBetweenTiles: " + toleranceBetweenTiles);
        if (SpawnedTiles.Count > 0)
        {
            GameObject lastTile = SpawnedTiles[SpawnedTiles.Count - 1];

            // Eğer spawner, son tile'ın son noktasını geçtiyse yeni tile ekle
            tileWidth = SpawnedTiles[SpawnedTiles.Count - 1].GetComponentInChildren<Renderer>().bounds.size.x;
            float lastTileEndX = lastTile.transform.position.x + (tileWidth / 2f);
            if (transform.position.x >= lastTileEndX)
            {
                //Debug.Log("last tile end x: " + lastTileEndX + " spawner x: " + transform.position.x);
                SpawnTileAtEnd();
            }
        }

        // tiles listesinden yok olanları temizle
        SpawnedTiles.RemoveAll(t => t == null);
    }

    void SpawnInitialTiles()
    {
        Vector3 spawnPos = transform.position;
        for (int i = 0; i < tileCount; i++)
        {
            GameObject newTilePrefab = RandomSelectTilePrefab();
            float newTileWidth = newTilePrefab.GetComponentInChildren<Renderer>().bounds.size.x;
            if (i == 0)
            {
                // İlk tile, spawner pozisyonuna
                GameObject tile = Instantiate(newTilePrefab, spawnPos, Quaternion.identity);
                SpawnedTiles.Add(tile);
            }
            else
            {
                GameObject lastTile = SpawnedTiles[SpawnedTiles.Count - 1];
                float lastTileWidth = lastTile.GetComponentInChildren<Renderer>().bounds.size.x;
                spawnPos = lastTile.transform.position + new Vector3((lastTileWidth / 2.0f) + (newTileWidth / 2.0f) - toleranceBetweenTiles, 0, 0);

                Quaternion rotation = Quaternion.Euler(0, -90, 0);
                GameObject tile = Instantiate(newTilePrefab, spawnPos, rotation);
                SpawnedTiles.Add(tile);
            }
        }
    }
    float breakSpaceAll(int index)
    {
        float totalSpace = 0f;
        for (int i = 0; i < index; i++)
        {
            if (i < SpawnedTiles.Count)
            {
                float width = SpawnedTiles[i].GetComponentInChildren<Renderer>().bounds.size.x;
                totalSpace += width;
            }
            else
            {
                //totalSpace += tileWidth;
                Debug.LogWarning("Index out of range for SpawnedTiles when calculating breakSpaceAll.");
            }
            totalSpace += tileSpacing;
        }
        return totalSpace;
    }
    GameObject RandomSelectTilePrefab()
    {
        int randomIndex = Random.Range(0, tilePrefebs.Count);
        return tilePrefebs[randomIndex];
    }
    void SpawnTileAtEnd()
    {
        if (SpawnedTiles.Count == 0) return;

        GameObject lastTile = SpawnedTiles[SpawnedTiles.Count - 1];
        GameObject newTilePrefab = RandomSelectTilePrefab();

        float lastTileWidth = lastTile.GetComponentInChildren<Renderer>().bounds.size.x;
        float newTileWidth = newTilePrefab.GetComponentInChildren<Renderer>().bounds.size.x;

        Vector3 spawnPos = new Vector3(
            lastTile.transform.position.x,
            lastTileWidth != newTileWidth && lastTile.transform.position.y == 0.005001f
             ? 0f
             : 0.005001f,
            lastTile.transform.position.z)
            + new Vector3((lastTileWidth / 2.0f) + (newTileWidth / 2.0f) - toleranceBetweenTiles, 0, 0);

        // -90 derece Y ekseninde rotasyon veriyoruz
        Quaternion rotation = Quaternion.Euler(0, -90, 0);
        GameObject newTile = Instantiate(newTilePrefab, spawnPos, rotation);
        SpawnedTiles.Add(newTile);
    }
}
