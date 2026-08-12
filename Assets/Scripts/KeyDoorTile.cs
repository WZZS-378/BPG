using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyDoorObjects : MonoBehaviour
{
    // Tags that can pick up the key
    public string[] allowedTags = new string[] { "Blue", "Pink", "Green" };

    public GameObject doorObject;
    public Sprite floorSprite;
    public Sprite consumedKeySprite;

    private bool activated = false;

    private void Reset()
    {
        // make collider a trigger by default when adding component in editor
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        bool ok = false;
        foreach (var t in allowedTags)
        {
            if (other.CompareTag(t)) { ok = true; break; }
        }
        if (!ok) return;

        StartCoroutine(OpenDoorSequence());
    }

    private IEnumerator OpenDoorSequence()
    {
        activated = true;

        // Immediately update key visual so it no longer blocks/overlaps
        var keySr = GetComponent<SpriteRenderer>();
        var keyCol = GetComponent<Collider2D>();
        if (keySr != null)
        {
            if (consumedKeySprite != null) keySr.sprite = consumedKeySprite;
            else keySr.enabled = false; // hide if no consumed sprite provided
        }
        if (keyCol != null) keyCol.enabled = false; // disable key collider

        // If door object assigned, disable its collider and trigger animator (no waiting)
        if (doorObject != null)
        {
            var doorCol = doorObject.GetComponent<Collider2D>();
            if (doorCol != null) doorCol.enabled = false; // prevent blocking immediately

            var doorAnimator = doorObject.GetComponent<Animator>();
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("IsOpened", true);
            }

            // Immediately swap door sprite to floorSprite (if provided)
            if (floorSprite != null)
            {
                var doorSr = doorObject.GetComponent<SpriteRenderer>();
                if (doorSr != null) doorSr.sprite = floorSprite;
            }

            // Ensure door collider remains disabled
            var doorCol2 = doorObject.GetComponent<Collider2D>();
            if (doorCol2 != null) doorCol2.enabled = false;
        }

        // end coroutine immediately
        yield break;
    }

}
