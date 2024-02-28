    using System.Collections;
    using UnityEngine;
    using UnityEngine.AI;
    using MonsterLove.StateMachine;

    public class AIBehaviorRanged : MonoBehaviour, IAIBehavior
    {
        public enum States
        {
            Idle,
            Patrol,
            Chasing,
            Attacking,
            Dodging,
            Cooldown
        }

        [Header("Player Interaction")]
        public Transform player;

        [Header("Movement Settings")]
        public float speed = 3f;
        public float rotationSpeed = 1f;
        public float dodgeDuration = 2f;

        [Header("Patrol Settings")]
        public float minPatrolDistance = 10f;
        public float maxPatrolDistance = 20f;
        public float idleDuration = 2f;

        [Header("Attack Settings")]
        public float attackDuration = 5f;
        public float attackCooldown = 5f;
        public float attackRange = 10f;
        public float dodgingCirclingSpeed = 100f;

        public bool aggressive { get; set; }

        private NavMeshAgent navMeshAgent;
        private AISenseSystem aiSense;
        private EnemyGunController enemyGunController;

        private StateMachine<States, Driver> fsm;
        private bool isAttacking = false;
        private bool isOnAttackCooldown = false;
        private Vector3 patrolDestination;

        private void Awake()
        {
            fsm = new StateMachine<States, Driver>(this);
            fsm.ChangeState(States.Idle);
        }

        private void Start()
        {
            AISenseSystem AISense = GetComponent<AISenseSystem>();

            AISense.OnHitReceived += fsm.Driver.OnHitReceived.Invoke;
            AISense.OnPlayerVisible += fsm.Driver.OnPlayerVisible.Invoke;
            AISense.OnPlayerLost += fsm.Driver.OnPlayerVisible.Invoke;

            if (aggressive) { fsm.Driver.OnAggressive.Invoke(); }

            player = GameManager.instance.player.transform;
            navMeshAgent = GetComponent<NavMeshAgent>();
            aiSense = GetComponent<AISenseSystem>();
            enemyGunController = GetComponentInChildren<EnemyGunController>();

            navMeshAgent.speed = speed;
            //navMeshAgent.updateRotation = false; // Disable NavMeshAgent automatic rotation    }
        }


        private void Update()
        {
            fsm.Driver.Update.Invoke();
        }


        public class Driver
        {
            public StateEvent OnPlayerVisible;
            public StateEvent OnAggressive;

            public StateEvent OnHitReceived;
            public StateEvent Update;
        }

        #region StateMachineMethods
        private void Idle_OnPlayerVisible()
        {
            fsm.ChangeState(States.Chasing);
        }
        private void Idle_OnAggressive()
        {
            fsm.ChangeState(States.Chasing);
        }

        private void Idle_OnHitReceived()
        {
            fsm.ChangeState(States.Chasing);
        }
        private void Patrol_Enter()
        {
            SetNewPatrolDestination();
        }
        private void Patrol_Update()
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                SetNewPatrolDestination();
            }
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

        private void Attacking_Update()
        {
            navMeshAgent.SetDestination(this.transform.position);

            enemyGunController.AttemptShoot(); // Initiates the attack, EnemyGunController manages firing rate
            Invoke("EndAttack", attackDuration);
        }

        private void EndAttack()
        {
            fsm.ChangeState(States.Chasing);
        }

        #endregion

        private void SetNewPatrolDestination()
        {
            patrolDestination = AIUtility.GetRandomPoint(transform.position, maxPatrolDistance);
            if (patrolDestination != Vector3.zero)
            {
                navMeshAgent.SetDestination(patrolDestination);
            }
        }

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

    }
