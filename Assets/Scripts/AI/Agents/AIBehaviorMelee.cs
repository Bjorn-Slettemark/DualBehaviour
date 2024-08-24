using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviorMelee : MonoBehaviour, IAIBehavior
{
    public enum States
    {
        Spawning,
        Idle,
        Chasing,
        Attacking,
        Cooldown
    }

    [Header("Enemy Settings")]
    public Transform player;
    public float chaseSpeed = 3f;
    public float rotationSpeed = 1f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float attackDuration = 2f;

    private StateMachine<States, Driver> fsm;

    private NavMeshAgent navMeshAgent;
    //private SwordController swordController;
    public bool aggressive { get; set; }

    private void Awake()
    {
        fsm = new StateMachine<States, Driver>(this);
        fsm.ChangeState(States.Spawning);
    }

    private void Start()
    {
        AISenseSystem AISense = GetComponent<AISenseSystem>();

        AISense.OnPlayerVisible += fsm.Driver.OnPlayerVisible.Invoke;
        AISense.OnPlayerLost += fsm.Driver.OnPlayerVisible.Invoke;

        if (aggressive) { fsm.Driver.OnAggressive.Invoke();  }

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = chaseSpeed;

        //swordController = GetComponentInChildren<SwordController>();


       

        if (navMeshAgent == null)
        {
            Debug.LogError("Missing NavMeshAgent component on enemy.", this);
        }
    }
    private void Update()
    {
        fsm.Driver.Update.Invoke(); //Tap the state machine into Unity's update loop. We could choose to call this from anywhere though!
    }
    public class Driver
    {
        public StateEvent OnPlayerVisible;
        public StateEvent OnPlayerLost;
        public StateEvent OnhitReceived;
        public StateEvent OnAggressive;
        public StateEvent Update;
    }
    

    // Draw debug graphics in editor mode
    private void OnDrawGizmos()
    {
        // Draw attack range sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);


    }
    void OnGUI()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2); // Adjust the Vector3.up * 2 to position the text above the AI
        screenPosition.y = Screen.height - screenPosition.y; // Convert to GUI coordinates

        GUIStyle style = new GUIStyle();
        style.normal.textColor = aggressive ? Color.red : Color.white; // Change text color based on the aggressive state
        style.alignment = TextAnchor.MiddleCenter;

        GUI.Label(new Rect(screenPosition.x - 50, screenPosition.y - 25, 100, 50), fsm.State.ToString(), style);
    }


    #region Finite State Machine

    private void Spawning_Enter()
    {
        fsm.ChangeState(States.Idle);
    }

    private void Idle_OnAggressive()
    {
        fsm.ChangeState(States.Chasing);
    }

    private void Idle_OnPlayerVisible()
    {
        fsm.ChangeState(States.Chasing);
    }
    private void Idle_Exit()
    {
    }
    private void Chasing_Enter()
    {
    }

    private void Chasing_Update()
    {
        navMeshAgent.SetDestination(player.position);


        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            fsm.ChangeState(States.Attacking);
        }
    }

    private void Attacking_Enter()
    {
        //swordController.StartSwinging();
        Invoke("EndAttackCooldown", attackDuration);
    }


    private void EndAttackCooldown()
    {
        fsm.ChangeState(States.Chasing);
    }

    #endregion

}
