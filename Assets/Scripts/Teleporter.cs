using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public GameObject teleporter1;
    bool tpOneReady = false;
    bool tpTwoReady = false;
    public GameObject teleporter2;
    public GameObject cube1;
    public GameObject cube2;
    public string[] allowedTags = new string[] { "Blue", "Pink", "Green" };
    public GameObject wall1;
    public GameObject wall2;
    public bool tpDisabled = false;

    // Update is called once per frame
    void Update()
    {
        if (tpOneReady && tpTwoReady)
        {
            StartCoroutine(TeleportCubes());
            teleporter1.tag = "Untagged";
            teleporter2.tag = "Untagged";
            wall1.GetComponent<BoxCollider2D>().enabled = true;
            wall2.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    public void setTPOne(GameObject cube)
    {
        cube1 = cube;
        tpOneReady = true;
    }
    public void setTPTwo(GameObject cube)
    {
        cube2 = cube;
        tpTwoReady = true;
    }

    IEnumerator TeleportCubes()
    {
        tpOneReady = false;
        tpTwoReady = false;

        yield return new WaitUntil(() =>
        cube1.GetComponent<PlayerController>().stopMoving &&
        cube2.GetComponent<PlayerController>().stopMoving);


        Vector3 tempPos = cube1.transform.position;
        Vector3 tempMovePoint = cube1.GetComponent<PlayerController>().movePoint.position;

        cube1.GetComponent<PlayerController>().movePoint.position = cube2.GetComponent<PlayerController>().movePoint.position;
        cube2.GetComponent<PlayerController>().movePoint.position = tempMovePoint;

        cube1.transform.position = cube2.transform.position;
        cube2.transform.position = tempPos;

        
        cube1.transform.position = cube1.GetComponent<PlayerController>().movePoint.position;
        cube2.transform.position = cube2.GetComponent<PlayerController>().movePoint.position;

        cube1.GetComponent<PlayerController>().teleportComplete = true;
        cube2.GetComponent<PlayerController>().teleportComplete = true;
    }
}
