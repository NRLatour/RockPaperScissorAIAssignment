using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IndividualAI : MonoBehaviour
{
    public Transform target;
    public Transform closestEnemy;
    public bool wrapped;
    private float WrappedNotificationTimer = 0f;
    public bool chase;
    private float SearchTimer = 0f;
    private const float AttentionSpan = 2f;
    public Vector3 LastKnownPosition;

    [SerializeField]
    private GameObject StartWarpEffectPrefab;
    [SerializeField]
    private GameObject EndWarpEffectPrefab;

    private float aggresiveness;
    private string targetTagName;

    public Vector3 Velocity_V3;
    private Rigidbody rb;
    public const float safeDistance = 7.5f;

    private const float FLEEREFRESHRATE = 2f;
    private float FleeTimer = 0f;
    private const float SpeedBoost = 2f;
    private bool BoostOn = false;
    private float BoostCooldown = 0f;
    private const float BOOSTONCOOLDOWN = 3f;

    private float StateUpdateCooldown = 0f;

    public float Aggresiveness { get => aggresiveness; set => aggresiveness = value; }

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        GetTargetTagName();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        // Reduce the number of times it seaches for a new target if there are multiple calls per frame
        if (StateUpdateCooldown < 0f)
        {
            FindNewTarget();
            StateUpdateCooldown = 0.04f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Tick();

        Vector3 speedVector = new Vector3();

        if (closestEnemy != null)
        {
            if (!BoostOn && BoostCooldown <= 0f) // Boost off, Boost Available and closest enemy exists => Turn On Boost
            {
                BoostOn = true;
                BoostCooldown = FLEEREFRESHRATE; // Set for the 2 active seconds
            }
            Velocity_V3 = Vector3.Normalize(transform.position - closestEnemy.position);
            if (FleeTimer <= 0f) // Update Escape Check
            {
                if (!closestEnemy.gameObject.activeSelf) // Enemy Eliminated during chase
                {
                    closestEnemy = null;
                }
                else if (Vector3.Distance(closestEnemy.position, this.transform.position) > safeDistance) // Only check after the 2 seconds of fleeing
                {
                    closestEnemy.GetComponent<IndividualAI>().FindNewTarget(transform);
                    closestEnemy = null;
                }
                else // Still being chased
                {
                    FleeTimer = FLEEREFRESHRATE;
                }
            }
        }
        else
        {
            if (target == null || !target.gameObject.activeSelf) // No target or Target eliminated -> Find new one
            {
                FindNewTarget();
            }
            if (target != null) // Chase target
            {
                if (target.GetComponent<IndividualAI>().wrapped)
                {
                    Velocity_V3 = Vector3.Normalize(LastKnownPosition - transform.position);
                }
                else
                {
                    Velocity_V3 = Vector3.Normalize(target.position + target.GetComponent<IndividualAI>().Velocity_V3 - transform.position);
                    LastKnownPosition = target.position;
                }
            }
            else
            {
                Debug.LogError("No Enemy Found");
            }

        }

        speedVector = Velocity_V3 * (aggresiveness + ((BoostOn)? SpeedBoost: 0.0f));
        rb.velocity = speedVector;
    }

    /// <summary>
    /// Collection of timers that decrease over time and conditions they affect
    /// </summary>
    private void Tick()
    {
        FleeTimer -= Time.deltaTime;
        BoostCooldown -= Time.deltaTime;
        SearchTimer -= Time.deltaTime;
        StateUpdateCooldown -= Time.deltaTime;
        WrappedNotificationTimer -= Time.deltaTime;

        chase = SearchTimer > 0;
        wrapped = WrappedNotificationTimer > 0;

        if (BoostOn && BoostCooldown <= 0f) // Boost is active and the 2 seconds are done. => Turn off BoostOn and Start Cooldown
        {
            BoostOn = false;
            BoostCooldown = BOOSTONCOOLDOWN;
        }

    }

    public void StartFlee(Transform enemy)
    {
        if (FleeTimer < 0f) // New Flee Sequence
        {
            closestEnemy = enemy;
            FleeTimer = FLEEREFRESHRATE;
        }
        else // Currently Fleeing
        {
            if (!closestEnemy.gameObject.activeSelf || enemy != closestEnemy && Vector3.Distance(closestEnemy.position, this.transform.position) > Vector3.Distance(enemy.position, this.transform.position))
            {
                closestEnemy = enemy; // New Closest enemy
            }
        }
    }

    public void FindNewTarget(Transform previousTarget = null)
    {
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag(targetTagName))
        {
            if ((target == null || target == previousTarget || !target.gameObject.activeSelf) && unit.transform != previousTarget && unit.activeSelf)
                target = unit.transform;
            else if (Vector3.Distance(transform.position, target.position) > Vector3.Distance(unit.transform.position, transform.position) && unit.transform != previousTarget)
                    target = unit.transform;
        }
    }

    private void GetTargetTagName()
    {
        switch (tag)
        {
            case "Rock":
                targetTagName = "Scissors";
                break;
            case "Scissors":
                targetTagName = "Paper";
                break;
            case "Paper":
                targetTagName = "Rock";
                break;
            default:
                targetTagName = "Scissors";
                break;
        }
    }

    public void WrapAround()
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(-pos.x, pos.y, -pos.z) + (pos).normalized*2;
        FindNewTarget();
        closestEnemy = null;
        wrapped = true;
        WrappedNotificationTimer = AttentionSpan;
        Instantiate(StartWarpEffectPrefab, pos, Quaternion.identity);
        Instantiate(EndWarpEffectPrefab, transform.position, Quaternion.identity);
    }
}
