using UnityEngine;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    public GameObject tilePrefab;     // Atanacak tile prefabı
    public int tileCount = 2;         // Başlangıçta kaç tile spawnlanacak
    public float tileSpacing = 0f;    // İsteğe bağlı ekstra boşluk (0 = tam bitişik)
    [SerializeField] private float toleranceBetweenTiles = 0.5f; // Tile'lar arasındaki boşluk toleransı
    private List<GameObject> tiles = new List<GameObject>();
    private float tileWidth;
    void Start()
    {
        // Prefab’ın Renderer boyutunu al
        Renderer rend = tilePrefab.GetComponentInChildren<Renderer>();
        tileWidth = rend.bounds.size.x ;
        SpawnInitialTiles();
    }

    void Update()
    {
    toleranceBetweenTiles = FutureEscape.Tiles.GlobalTileSettings.speed / 32f;
        //Debug.Log("toleranceBetweenTiles: " + toleranceBetweenTiles);
        if (tiles.Count > 0)
        {
            GameObject lastTile = tiles[tiles.Count - 1];

            // Eğer spawner, son tile'ın son noktasını geçtiyse yeni tile ekle
            float lastTileEndX = lastTile.transform.position.x + (tileWidth / 2f);
            if (transform.position.x >= lastTileEndX)
            {
                //Debug.Log("last tile end x: " + lastTileEndX + " spawner x: " + transform.position.x);
                SpawnTileAtEnd();
            }
        }

        // tiles listesinden yok olanları temizle
        tiles.RemoveAll(t => t == null);
    }

    void SpawnInitialTiles()
    {
        Vector3 startPos = transform.position;

        for (int i = 0; i < tileCount; i++)
        {
            Vector3 spawnPos = startPos + new Vector3(i * tileWidth, 0, 0);
            GameObject tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
            tiles.Add(tile);
        }
    }

    void SpawnTileAtEnd()
    {
        if (tiles.Count == 0) return;

        GameObject lastTile = tiles[tiles.Count - 1];
        // Yeni tile tam sonuncunun bitiş noktasına koyuluyor
        Vector3 spawnPos = lastTile.transform.position + new Vector3(tileWidth - toleranceBetweenTiles, 0, 0);

        GameObject newTile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
        tiles.Add(newTile);
    }
}
