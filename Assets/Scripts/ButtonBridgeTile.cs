using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collider2D))]
public class ButtonBridgeTile : MonoBehaviour
{
    public Tilemap pitTilemap;
    public Tilemap groundTilemap;
    public Tile emptyTile;
    public GameObject bridgeTile;
    public Transform bridgeSpawnPoint;
    public Sprite usedButton;
    public bool oneShot = true;
    public string[] allowedTags = new string[] { "Blue", "Pink", "Green" };
    private bool activated = false;

    // Ensure the collider is a trigger by default when adding the component in editor
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    // Called when something enters the button trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) { return; }

        // Only react to allowed cube tags
        bool ok = false;
        foreach (var t in allowedTags)
        {
            if (other.CompareTag(t)) { ok = true; break; }
        }
        if (!ok) { return; }

        // Validate inspector assignments
        if (pitTilemap == null || groundTilemap == null || bridgeTile == null || bridgeSpawnPoint == null)
        {
            Debug.LogWarning("TilemapButtonBridge: assign pitTilemap, groundTilemap, bridgeTile and bridgeSpawnPoint in inspector.");
            return;
        }

        // Compute the cell position from the spawn point world position
        Vector3Int cell = pitTilemap.WorldToCell(bridgeSpawnPoint.position);

        // Remove the pit tile from the pitTilemap (make the cell empty)
        pitTilemap.SetTile(cell, null);
        pitTilemap.RefreshTile(cell);

        // Place the bridge tile into the groundTilemap at the same cell
        groundTilemap.SetTile(cell, emptyTile);
        groundTilemap.RefreshTile(cell);
        

        // Force rebuild of TilemapCollider2D
        RebuildCollider(pitTilemap);
        RebuildCollider(groundTilemap);

        Instantiate(bridgeTile, bridgeSpawnPoint.position, Quaternion.identity);

        // Disable the button collider so it won't trigger again
        var myCol = GetComponent<Collider2D>();
        if (myCol != null) myCol.enabled = false;

        // adjust sprite rendering so it doesn't draw above cubes
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = usedButton;
        if (sr != null)
        {
            //lower sorting order so cubes render above (choose one)
            sr.sortingOrder = -1;
        }

        if (oneShot) { activated = true; }
        
    }

    // Helper: toggle TilemapCollider2D to force physics rebuild
    private void RebuildCollider(Tilemap tm)
    {
        if (tm == null) return;
        var tilemapCollider = tm.GetComponent<TilemapCollider2D>();
        if (tilemapCollider != null)
        {
            tilemapCollider.enabled = false;
            tilemapCollider.enabled = true;
        }
    }
}
