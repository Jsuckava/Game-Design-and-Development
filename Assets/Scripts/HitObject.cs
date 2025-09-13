using UnityEngine;
using UnityEngine.Tilemaps;

public class HitObject : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase specialTile;

    void Update()
    {
        Vector3Int cellPos = tilemap.WorldToCell(transform.position);
        TileBase tile = tilemap.GetTile(cellPos);

        if (tile == specialTile)
        {
            Debug.Log("Player stepped on the special tile!");
        }
    }
}
