using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class S12_BallEnemy : MonoBehaviour
{
    // ── FSM States ───────────────────────────────────────────────
    private enum State { Patrol, Chase, Catching }

    [Header("Waypoints")]
    public Transform waypointA;
    public Transform waypointB;

    [Header("Speed")]
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 4f;

    [Header("Radii")]
    public float detectionRadius = 4f;   // player enters this → start chasing
    public float catchRadius = 0.6f; // player within this → apply penalty

    [Header("Penalty")]
    public float penaltyTickInterval = 1f;

    [Header("References")]
    [SerializeField] private GameObject gameTimerObject;

    [Header("Gizmos")]
    public bool showDetectionRadius = true;
    public bool showCatchRadius = true;
    public bool showPatrolPath = true;

    // ── Private ──────────────────────────────────────────────────
    private S11_GameTimer gameTimer;
    private Rigidbody2D   rb;
    private Transform player;
    private State currentState;
    private Transform currentWaypoint;
    private float penaltyTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (gameTimerObject == null)
            gameTimerObject = GameObject.Find("MGR_GameTimer");

        if (gameTimerObject != null)
            gameTimer = gameTimerObject.GetComponent<S11_GameTimer>();

        Debug.Log($"[S12] gameTimerObject: {gameTimerObject} | gameTimer: {gameTimer} | player: {player}");

        if (gameTimer == null)
            Debug.LogWarning($"[S12] {name}: MGR_GameTimer not found in scene.");
    }

    private void Start()
    {
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) player = playerGO.transform;

        currentWaypoint = waypointA;
        currentState = State.Patrol;
    }

    private void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // ── State transitions ────────────────────────────────────
        switch (currentState)
        {
            case State.Patrol:
                if (distToPlayer <= detectionRadius)
                    currentState = State.Chase;
                break;

            case State.Chase:
                if (distToPlayer <= catchRadius)
                {
                    currentState = State.Catching;
                    penaltyTimer = penaltyTickInterval; // fire immediately on first contact
                }
                else if (distToPlayer > detectionRadius)
                    currentState = State.Patrol;
                break;

            case State.Catching:
                if (distToPlayer > catchRadius)
                {
                    currentState = State.Chase;
                    penaltyTimer = 0f;
                }
                break;
        }

        // ── Penalty tick ─────────────────────────────────────────
        if (currentState == State.Catching)
        {
            penaltyTimer += Time.deltaTime;
            if (penaltyTimer >= penaltyTickInterval)
            {
                penaltyTimer = 0f;
                Debug.Log($"[S12] Penalty tick — gameTimer: {gameTimer}");
                if (gameTimer != null)
                    gameTimer.AddPenalty();
            }
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Patrol: DoPatrol(); break;
            case State.Chase: DoChase(); break;
            case State.Catching: rb.linearVelocity = Vector2.zero; break;
        }
    }

    // ── Patrol ───────────────────────────────────────────────────
    private void DoPatrol()
    {
        if (waypointA == null || waypointB == null) return;

        Vector2 dir = ((Vector2)currentWaypoint.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * patrolSpeed;

        // Close enough to waypoint — flip to the other one
        if (Vector2.Distance(transform.position, currentWaypoint.position) < 0.2f)
            currentWaypoint = currentWaypoint == waypointA ? waypointB : waypointA;
    }

    // ── Chase ────────────────────────────────────────────────────
    private void DoChase()
    {
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * chaseSpeed;
    }

    private void OnDrawGizmos()
    {
        // 1. Detection Radius
        if (showDetectionRadius)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        // 2. Catching Radius
        if (showCatchRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, catchRadius);
        }

        // 3. Patrol Path
        if (showPatrolPath && waypointA != null && waypointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(waypointA.position, waypointB.position);
            
            // Small cubes to mark the exact waypoint positions
            Gizmos.DrawCube(waypointA.position, Vector3.one * 0.2f);
            Gizmos.DrawCube(waypointB.position, Vector3.one * 0.2f);
        }
    }
}
