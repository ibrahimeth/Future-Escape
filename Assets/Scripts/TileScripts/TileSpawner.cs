using UnityEngine;

using System.Collections.Generic;
public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab; // Atanacak tile prefabı
    public int tileCount = 4;     // Başlangıçta kaç tile spawnlanacak
    public float tileSpacing = 10f; // Tile'lar arası mesafe

    private List<GameObject> tiles = new List<GameObject>();

    void Start()
    {
        SpawnInitialTiles();
    }

    void Update()
    {
        if (tiles.Count > 0)
        {
            GameObject lastTile = tiles[tiles.Count - 1];
            // Son tile'ın x pozisyonu, kendi uzunluğunun sonuna ulaştıysa yeni tile ekle
            if (lastTile.transform.position.x < transform.position.x - tileSpacing * (tileCount - 1))
            {
                SpawnTileAtEnd();
            }
        }
        // tiles listesinden yok olan tile'ları çıkar
        tiles.RemoveAll(t => t == null);
    }

    void SpawnInitialTiles()
    {
        Vector3 startPos = transform.position;
        for (int i = 0; i < tileCount; i++)
        {
            Vector3 spawnPos = startPos + new Vector3(i * tileSpacing, 0, 0);
            GameObject tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
            tiles.Add(tile);
        }
    }

    void SpawnTileAtEnd()
    {
        if (tiles.Count == 0) return;
        GameObject lastTile = tiles[tiles.Count - 1];
        Vector3 spawnPos = lastTile.transform.position + new Vector3(tileSpacing, 0, 0);
        GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
        tiles.Add(newTile);
    }
}
