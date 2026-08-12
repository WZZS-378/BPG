using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 3f;
    public Transform movePoint;
    private float arriveEpsilon = 0.01f;
    public bool isStopped = false;
    public bool stopMoving = false;
    public bool firstMove = false;
    private bool hasFallen = false;
    public GameObject sceneMan;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        sceneMan = GameObject.FindWithTag("SceneManager");
        hasFallen = false;
        if (movePoint == null)
        {
            Debug.LogError("MovePoint not assigned on " + gameObject.name);
            enabled = false;
            return;
        }
        movePoint.parent = null; // detach so movePoint doesn't inherit player's transform
        
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning(gameObject.name + " has no Animator component.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped)
        {
            if (movePoint != null)
            { movePoint.position = transform.position; }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
        }


        //Only accept input when we've essentially arrived
        if (!(Vector3.Distance(transform.position, movePoint.position) > arriveEpsilon) && !stopMoving)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (CanMove(Vector3.left))
                {
                    movePoint.position += Vector3.left; if (firstMove == false){firstMove = true;}
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (CanMove(Vector3.right))
                {
                    movePoint.position += Vector3.right; if (firstMove == false){firstMove = true;}
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (CanMove(Vector3.up))
                {
                    movePoint.position += Vector3.up; if (firstMove == false){firstMove = true;}
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (CanMove(Vector3.down))
                {
                    movePoint.position += Vector3.down; if (firstMove == false){firstMove = true;}
                }
            }
        }

        // start coroutine to stop movement when object is Blue and Keypad1 is pressed
        if (gameObject.CompareTag("Blue") && Input.GetKey(KeyCode.Alpha1) && !stopMoving)
        { StartCoroutine(StopMovementWhileKeyHeld(KeyCode.Alpha1)); }
        // start coroutine to stop movement when object is Pink and Keypad2 is pressed
        else if (gameObject.CompareTag("Pink") && Input.GetKey(KeyCode.Alpha2) && !stopMoving)
        { StartCoroutine(StopMovementWhileKeyHeld(KeyCode.Alpha2)); }
        // start coroutine to stop movement when object is Green and Keypad3 is pressed
        else if (gameObject.CompareTag("Green") && Input.GetKey(KeyCode.Alpha3) && !stopMoving)
        { StartCoroutine(StopMovementWhileKeyHeld(KeyCode.Alpha3)); }
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pits"))
        { 
            if (!hasFallen && !SceneMan.instance.isTransitioning)
            {
                hasFallen=true;
                Fall();
            } 
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Exits"))
        { 
            StartCoroutine(ExitGate()); 
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Spikes"))
        { 
            hasFallen=true;
            Fall();
        }

        if (other.gameObject.CompareTag("Teleporter") && !tpCooldown)
        {
            StartCoroutine(TpWhenReady());
        }

        if (other.gameObject.CompareTag("bottomCollider"))
        {
            hasFallen=true;
            Fall();
        }
    }
    public bool teleportComplete = false;
    public bool tpCooldown = false;
    private IEnumerator TpWhenReady()
    {
        tpCooldown = true;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, movePoint.position) <= arriveEpsilon);
        
        stopMoving = true;
        isStopped = true;
        teleportComplete = false;
        if (isStopped && gameObject.CompareTag("Blue"))
        { animator.SetBool("IsBlueStopped", true); }
        else if (isStopped && gameObject.CompareTag("Pink"))
        { animator.SetBool("IsPinkStopped", true); }
        else if (isStopped && gameObject.CompareTag("Green"))
        { animator.SetBool("IsGreenStopped", true); }


        yield return new WaitUntil(() => teleportComplete);
        Debug.Log("finished");
        stopMoving = false;
        isStopped = false;
        teleportComplete = false;
        if (!isStopped && gameObject.CompareTag("Blue"))
        { animator.SetBool("IsBlueStopped", false); }
        else if (!isStopped && gameObject.CompareTag("Pink"))
        { animator.SetBool("IsPinkStopped", false); }
        else if (!isStopped && gameObject.CompareTag("Green"))
        { animator.SetBool("IsGreenStopped", false); }

        yield return null;
        tpCooldown = false;
    }

    public void Fall()
    {
        if (SceneMan.instance.isTransitioning) return;
        Debug.Log("Fall triggered");
        if (SceneMan.instance != null)
            SceneMan.instance.AddDeath();
        SceneMan.instance.reloadLevel();
    }

    private IEnumerator ExitGate()
    {
        yield return new WaitUntil(() => Vector3.Distance(transform.position, movePoint.position) <= arriveEpsilon);
        
        stopMoving = true;
        isStopped = true;

        if (isStopped && gameObject.CompareTag("Blue"))
        { animator.SetBool("IsBlueStopped", true); }
        else if (isStopped && gameObject.CompareTag("Pink"))
        { animator.SetBool("IsPinkStopped", true); }
        else if (isStopped && gameObject.CompareTag("Green"))
        { animator.SetBool("IsGreenStopped", true); }
        
        Debug.Log(gameObject.name + " reached the exit!");
        
        // Tell SceneMan that this player has arrived
        if (SceneMan.instance != null)
            SceneMan.instance.RegisterPlayerAtExit();
        else
            Debug.LogError("SceneMan instance missing!");
    }

    // Coroutine: stop movement while Keypad123 is held, resume when released
    private IEnumerator StopMovementWhileKeyHeld(KeyCode k)
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        Color colour = sr.color;
        // wait until current MoveTowards finishes (player is at movePoint)
        yield return new WaitUntil(() => Vector3.Distance(transform.position, movePoint.position) <= arriveEpsilon);

        // snap both transform and movePoint to exact grid center
        Vector3 snapped = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);

        transform.position = snapped;

        if (movePoint != null)
        { movePoint.position = snapped; }

        isStopped = true;
        if (isStopped && gameObject.CompareTag("Blue"))
        { animator.SetBool("IsBlueStopped", true); }
        else if (isStopped && gameObject.CompareTag("Pink"))
        { animator.SetBool("IsPinkStopped", true); }
        else if (isStopped && gameObject.CompareTag("Green"))
        { animator.SetBool("IsGreenStopped", true); }

        // wait until key released
        yield return new WaitUntil(() => !Input.GetKey(k));

        isStopped = false;
        if (!isStopped && gameObject.CompareTag("Blue"))
        { animator.SetBool("IsBlueStopped", false); }
        else if (!isStopped && gameObject.CompareTag("Pink"))
        { animator.SetBool("IsPinkStopped", false); }
        else if (!isStopped && gameObject.CompareTag("Green"))
        { animator.SetBool("IsGreenStopped", false); }
    }

    private bool CanMove(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            movePoint.position,
            direction,
            1f,
            LayerMask.GetMask("Walls", "Doors")
        );

        return hit.collider == null;
    }
}