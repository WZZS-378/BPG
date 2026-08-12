using UnityEngine;

public class TopCollider : MonoBehaviour
{
    public CamMover camMover;
    private bool playerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!playerInside && (other.CompareTag("Blue") || other.CompareTag("Pink") || other.CompareTag("Green")))
        {
            playerInside = true;
            camMover.TriggerBoost();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Blue") || other.CompareTag("Pink") || other.CompareTag("Green"))
        {
            playerInside = false;
        }
    }
}