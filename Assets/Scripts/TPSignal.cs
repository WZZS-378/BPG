using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSignal : MonoBehaviour
{
    public GameObject tpOwner;
    public string[] allowedTags = new string[] { "Blue", "Pink", "Green" };
    void Start()
    {
        tpOwner = gameObject.transform.parent.gameObject;
        
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var t in allowedTags)
        {
            if (other.CompareTag(t))
            {
                if(tpOwner.GetComponent<Teleporter>().cube1 == null)
                {
                    tpOwner.GetComponent<Teleporter>().setTPOne(other.gameObject);
                } else
                {
                    tpOwner.GetComponent<Teleporter>().setTPTwo(other.gameObject);
                }
            }
        }
    }
}
