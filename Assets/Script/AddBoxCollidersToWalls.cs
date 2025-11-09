using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AddBoxCollidersToWalls : MonoBehaviour
{
    public Sprite wallSprite;
    public float colliderSize = 1.5f; // The size for the BoxCollider2D
    private int wallCollidersLayer;

    private void Awake()
    {
        wallCollidersLayer = LayerMask.NameToLayer("WallColliders");
    }

    void Start()
    {
        if (wallSprite == null)
        {
            Debug.LogError("WallSprite reference not set! Please assign the WallSprite in the Inspector.");
            return;
        }

        AddCollidersToWallTiles();
    }

    private void AddCollidersToWallTiles()
    {
        List<List<Vector2Int>> wallTileRegions = FindWallTileRegions();

        foreach (List<Vector2Int> region in wallTileRegions)
        {
            Vector3 colliderCenter = GetColliderCenter(region);
            Vector2 colliderSize = GetColliderSize(region);

            if (!TileHasCollider(colliderCenter))
            {
                CreateColliderComponent(colliderCenter, colliderSize);
            }
        }
    }

    private bool TileHasCollider(Vector3 center)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(center.x, center.y), 1 << wallCollidersLayer);
        return colliders.Length > 0;
    }

    private List<List<Vector2Int>> FindWallTileRegions()
    {
        List<List<Vector2Int>> wallTileRegions = new List<List<Vector2Int>>();
        Texture2D texture = wallSprite.texture;
        int width = texture.width;
        int height = texture.height;
        Color[] pixels = texture.GetPixels();

        bool[,] visited = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int pixelIndex = y * width + x;

                if (pixels[pixelIndex].a == 0 || visited[x, y]) // Skip transparent pixels or already visited pixels
                    continue;

                List<Vector2Int> region = new List<Vector2Int>();
                FloodFill(x, y, pixels, width, height, ref visited, ref region);
                wallTileRegions.Add(region);
            }
        }

        return MergeOverlappingRegions(wallTileRegions);
    }

    private List<List<Vector2Int>> MergeOverlappingRegions(List<List<Vector2Int>> regions)
    {
        List<List<Vector2Int>> mergedRegions = new List<List<Vector2Int>>();
        List<Vector2Int> currentRegion = new List<Vector2Int>(regions[0]);

        for (int i = 1; i < regions.Count; i++)
        {
            List<Vector2Int> region = regions[i];
            bool isOverlapping = false;

            foreach (Vector2Int tile in region)
            {
                if (currentRegion.Contains(tile))
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (isOverlapping)
            {
                currentRegion.AddRange(region);
            }
            else
            {
                mergedRegions.Add(currentRegion);
                currentRegion = new List<Vector2Int>(region);
            }
        }

        mergedRegions.Add(currentRegion);
        return mergedRegions;
    }

    private void FloodFill(int x, int y, Color[] pixels, int width, int height, ref bool[,] visited, ref List<Vector2Int> region)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || visited[x, y] || pixels[y * width + x].a == 0)
            return;

        visited[x, y] = true;
        region.Add(new Vector2Int(x, y));

        FloodFill(x + 1, y, pixels, width, height, ref visited, ref region);
        FloodFill(x - 1, y, pixels, width, height, ref visited, ref region);
        FloodFill(x, y + 1, pixels, width, height, ref visited, ref region);
        FloodFill(x, y - 1, pixels, width, height, ref visited, ref region);
    }

    private Vector3 GetColliderCenter(List<Vector2Int> region)
    {
        float sumX = 0f;
        float sumY = 0f;

        foreach (Vector2Int tilePosition in region)
        {
            sumX += tilePosition.x;
            sumY += tilePosition.y;
        }

        float centerX = sumX / region.Count;
        float centerY = sumY / region.Count;

        return new Vector3(centerX / wallSprite.pixelsPerUnit, centerY / wallSprite.pixelsPerUnit, 0f);
    }

    private Vector2 GetColliderSize(List<Vector2Int> region)
    {
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Vector2Int tilePosition in region)
        {
            minX = Mathf.Min(minX, tilePosition.x);
            minY = Mathf.Min(minY, tilePosition.y);
            maxX = Mathf.Max(maxX, tilePosition.x);
            maxY = Mathf.Max(maxY, tilePosition.y);
        }

        float tileSizeX = maxX - minX + 1;
        float tileSizeY = maxY - minY + 1;

        // Calculate the gap between tiles and adjust the size accordingly
        float gapX = Mathf.Max(0f, (tileSizeX - 1) * (1f - colliderSize));
        float gapY = Mathf.Max(0f, (tileSizeY - 1) * (1f - colliderSize));

        return new Vector2((tileSizeX + gapX) / wallSprite.pixelsPerUnit, (tileSizeY + gapY) / wallSprite.pixelsPerUnit);
    }

    private void CreateColliderComponent(Vector3 center, Vector2 size)
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.size = size * colliderSize;
        collider.offset = center;
        collider.gameObject.layer = wallCollidersLayer;
    }
}