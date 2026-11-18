using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData;

    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private GameObject player;
    private Vector3 lastKnowPos;

    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player; }

    public PatrolPath path;

    [Header("Sight Settings")]
    public float eyeHeight;

    public Vector3 LastKnownPlayerPosition
    {
        get => lastKnowPos;
        set => lastKnowPos = value;
    }

    [SerializeField]
    private string currentState;

    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");

        if (enemyData != null)
        {
            agent.speed = enemyData.movementSpeed;
        }

        stateMachine.Initialise();
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            LastKnownPlayerPosition = player.transform.position;
        }

        currentState = stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer()
    {
        if (player != null && enemyData != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < enemyData.sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(transform.forward, targetDirection);

                if (angleToPlayer <= enemyData.fieldOfView)
                {
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);

                    if (Physics.Raycast(ray, out RaycastHit hitinfo, enemyData.sightDistance))
                    {
                        if (hitinfo.transform.gameObject == player)
                        {
                            return true;
                        }
                    }

                    Debug.DrawRay(ray.origin, ray.direction * enemyData.sightDistance);
                }
            }
        }

        return false;
    }
}