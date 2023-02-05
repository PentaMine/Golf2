using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public Rigidbody rb;
    public GameObject cam;
    public GameObject player;
    public GameObject launchIndicator;
    public MeshRenderer indicatorRenderer;
    public bool prevClick;
    public float launchForce;
    public Vector3 forceToBeApplied;
    public bool isStationary;
    public Canvas canvas;
    public GameObject hole;
    public GameObject parent;
    public GameManager gameManager;
    public List<GameObject> launchStripSegments;

    public delegate void OnPlayerShoot(Vector3 force);

    public static event OnPlayerShoot onPlayerShoot;

    void Start()
    {
        speed = 10;
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        launchIndicator = GameObject.FindGameObjectWithTag("LaunchIndicator");
        indicatorRenderer = launchIndicator.GetComponent<MeshRenderer>();
        launchForce = 4.6f;
        canvas = GameObject.FindGameObjectWithTag("Overlay").GetComponent<Canvas>();
        hole = GameObject.FindGameObjectWithTag("Hole");
        parent = transform.parent.gameObject;
        gameManager = GameManager.instance;
        launchStripSegments = new List<GameObject>(GameObject.FindGameObjectsWithTag("LaunchStrip"));
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // set the pos of the player object responsible for the camera, it is not a child of the object because
        // children inherit the rotation and at that point setting the rotation every frame is the same as setting the pos
        player.transform.position = transform.position;
        
        // unallow player input if the game ended
        if (gameManager.state == GameManager.GameState.END || gameManager.isPaused)
        {
            return;
        }

        // render the launch indicator if the ball is stationary
        indicatorRenderer.enabled = isStationary;
        // unlock y axis movement if the player is near the start or the hole, else lock it
        Vector3 vel = rb.velocity;
        vel.y = Vector3.Distance(transform.position, hole.transform.position) < 2.5 ||
                Vector3.Distance(transform.position, parent.transform.position) < 2.5
            ? vel.y
            : 0;
        rb.velocity = vel;
        
        // mark the object as stationary if its velocity in any direction does not exceed 0.1
        isStationary = VectorUtil.IsWithinTolerance(rb.velocity, new Vector3(.1f, .1f, .1f));
        
        // I ShouldDie()
        // respawn the player if it is too low or high
        if (ShouldDie())
        {
            transform.position =
                GameObject.FindGameObjectWithTag("Respawn").transform.position + new Vector3(0, .5f, 0);
            rb.velocity = Vector3.zero;
        }

        // this chunk of code moves the launch strip to the location between the clicked position and the player if the
        // left mouse button is pressed and shoots the golf ball when the button is released 
        if (Input.GetMouseButton(0) && isStationary)
        {
            prevClick = true;
            if (CameraUtil.GetCursorInWorldPos(out Vector3 point, LayerMask.GetMask("Floor")) && isStationary)
            {
                indicatorRenderer.enabled = false;
                
                // clamp the length of the launch strip to 10 units
                Vector3 myPos = transform.position;
                point = Vector3.MoveTowards(myPos, point, 10);

                // calculate the number of segments needed and the distance between them
                int segments = (int)(Vector3.Distance(point, myPos) + .1f);
                float distanceUnit = Vector3.Distance(point, myPos) / segments;
                
                // calculate the rotation of the segments
                Vector3 posDelta = new Vector3(point.x - myPos.x, 0, point.z - myPos.z);
                Vector3 newEuler = new Vector3(
                    0,
                    MathUtil.radiansToDegrees(Mathf.Atan2(posDelta.x, posDelta.z)),
                    0
                );
                
                // cancel the launch if the user is clicking near the ball
                if (segments <= 1)
                {
                    posDelta = Vector3.zero;
                }
                
                for (int i = 1; i < segments + 1; i++)
                {   
                    // calculate the position of the segment
                    Vector3 pos = Vector3.MoveTowards(myPos, point, i * distanceUnit);

                    // apply all of the calculated values to the segment
                    GameObject stripSegment = launchStripSegments[i - 1];
                    stripSegment.transform.position = pos - new Vector3(0, pos.y - 0.01f, 0);
                    stripSegment.transform.rotation = Quaternion.Euler(newEuler);
                    stripSegment.transform.localScale = new Vector3(1, 1, distanceUnit);
                    stripSegment.GetComponentInChildren<MeshRenderer>().enabled = true; // make the segment visible
                }
                
                for (int i = segments; i is < 10 and >= 0; i++)
                {
                    // make unneeded segments invisible
                    launchStripSegments[i].GetComponentInChildren<MeshRenderer>().enabled = false;
                }
                
                forceToBeApplied = posDelta;
            }
        }
        else if (prevClick)
        {
            if (forceToBeApplied != Vector3.zero)
            {
                rb.AddForce(forceToBeApplied * -100 * launchForce);
                prevClick = false;
                onPlayerShoot(forceToBeApplied * -100 * launchForce);
            }
        }
        else
        {
            Vector3 posDelta = new Vector3(transform.position.x - cam.transform.position.x, 0,
                transform.position.z - cam.transform.position.z);

            launchIndicator.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(posDelta.x, posDelta.z), 0));
            launchIndicator.transform.position = new Vector3(transform.position.x, 0.02f, transform.position.z);
            launchIndicator.transform.localScale = new Vector3(.1f, 1, .1f);
            for (int i = 0; i < 10; i++)
            {
                launchStripSegments[i].GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
    }

    private bool ShouldDie()
    {
        return transform.position.y < -10 || transform.position.y > 100;
    }
}