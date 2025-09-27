using UnityEngine;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> tilePrefebs;
    public GameObject tilePrefab;
    public int tileCount = 2;         // Başlangıçta kaç tile spawnlanacak
    public float tileSpacing = 0f;    // İsteğe bağlı ekstra boşluk (0 = tam bitişik)
    [SerializeField] private float toleranceBetweenTiles = 0.5f; // Tile'lar arasındaki boşluk toleransı

    [SerializeField] private GameObject sawTrapPrefab;
    [SerializeField] private GameObject HammerTrapPrefab;
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
            Quaternion rotation = Quaternion.Euler(0, -90, 0);
            if (i == 0)
            {
                // İlk tile, spawner pozisyonuna
                GameObject tile = Instantiate(newTilePrefab, spawnPos, rotation);
                SpawnedTiles.Add(tile);

            }
            else
            {
                GameObject lastTile = SpawnedTiles[SpawnedTiles.Count - 1];
                float lastTileWidth = lastTile.GetComponentInChildren<Renderer>().bounds.size.x;
                spawnPos = lastTile.transform.position + new Vector3((lastTileWidth / 2.0f) + (newTileWidth / 2.0f) - toleranceBetweenTiles, 0, 0);

                GameObject tile = Instantiate(newTilePrefab, spawnPos, rotation);
                SpawnedTiles.Add(tile);
            }
            transform.position += new Vector3(newTileWidth, 0, 0);
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

        // Yeni tile'ı instantiate et

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

        // Şimdi yeni spawnlanan tile'ın çocuklarını kontrol et ve trap ekle
        Transform[] instanceChildren = newTile.GetComponentsInChildren<Transform>();
        foreach (Transform child in instanceChildren)
        {
            if (child.CompareTag("SawTrap"))
            {
                // Instantiate prefab first (without parent) to preserve its prefab values
                GameObject trap = Instantiate(sawTrapPrefab);
                trap.transform.position = child.position;
                trap.transform.rotation = child.rotation;

                // Create wrapper under the child that neutralizes parent's lossyScale
                GameObject wrapper = new GameObject(trap.name + "_root");
                Transform w = wrapper.transform;
                w.SetParent(child, false);
                Vector3 parentLossy = child.lossyScale;
                w.localPosition = Vector3.zero;
                w.localRotation = Quaternion.identity;
                w.localScale = new Vector3(
                    Mathf.Approximately(parentLossy.x, 0f) ? 1f : 1f / parentLossy.x,
                    Mathf.Approximately(parentLossy.y, 0f) ? 1f : 1f / parentLossy.y,
                    Mathf.Approximately(parentLossy.z, 0f) ? 1f : 1f / parentLossy.z
                );

                // Parent trap under wrapper and set local transform
                trap.transform.SetParent(w, false);
                trap.transform.localPosition = Vector3.zero;
                trap.transform.localRotation = Quaternion.Euler(0, 0, 90);
                // ensure prefab local scale is retained
                trap.transform.localScale = sawTrapPrefab.transform.localScale;
            }
            else if (child.CompareTag("TrapPlace"))
            {
                if (Random.value < 0.5f)
                {
                    GameObject trap = Instantiate(HammerTrapPrefab);
                    trap.transform.position = child.position;
                    trap.transform.rotation = child.rotation;

                    GameObject wrapper = new GameObject(trap.name + "_root");
                    Transform w = wrapper.transform;
                    w.SetParent(child, false);
                    Vector3 parentLossy = child.lossyScale;
                    w.localPosition = Vector3.zero;
                    w.localRotation = Quaternion.identity;
                    w.localScale = new Vector3(
                        Mathf.Approximately(parentLossy.x, 0f) ? 1f : 1f / parentLossy.x,
                        Mathf.Approximately(parentLossy.y, 0f) ? 1f : 1f / parentLossy.y,
                        Mathf.Approximately(parentLossy.z, 0f) ? 1f : 1f / parentLossy.z
                    );

                    trap.transform.SetParent(w, false);
                    trap.transform.localPosition = Vector3.zero;
                    trap.transform.localRotation = Quaternion.identity;
                    trap.transform.localScale = HammerTrapPrefab.transform.localScale / 4f;
                }
                else
                {
                    GameObject trap = Instantiate(sawTrapPrefab);
                    trap.transform.position = child.position;
                    trap.transform.rotation = child.rotation;

                    GameObject wrapper = new GameObject(trap.name + "_root");
                    Transform w = wrapper.transform;
                    w.SetParent(child, false);
                    Vector3 parentLossy = child.lossyScale;
                    w.localPosition = Vector3.zero;
                    w.localRotation = Quaternion.identity;
                    w.localScale = new Vector3(
                        Mathf.Approximately(parentLossy.x, 0f) ? 1f : 1f / parentLossy.x,
                        Mathf.Approximately(parentLossy.y, 0f) ? 1f : 1f / parentLossy.y,
                        Mathf.Approximately(parentLossy.z, 0f) ? 1f : 1f / parentLossy.z
                    );

                    trap.transform.SetParent(w, false);
                    trap.transform.localPosition = Vector3.zero;
                    trap.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    trap.transform.localScale = new Vector3(
                        HammerTrapPrefab.transform.localScale.x,
                        HammerTrapPrefab.transform.localScale.y / 2f,
                        HammerTrapPrefab.transform.localScale.z
                    );
                }
            }

        }
    }
}
