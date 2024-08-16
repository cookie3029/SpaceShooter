using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum State
    {
        IDLE, TRACE, ATTACK, DIE
    }

    // 현재 몬스터의 상태
    public State state = State.IDLE;

    // 추적 사정거리
    [SerializeField]
    private float traceDist = 10.0f;

    // 공격 사정거리
    [SerializeField]
    private float attackDist = 2.0f;

    private Transform playerTr;
    private Transform monsterTr;

    private NavMeshAgent agent;

    private Animator animator;
    private readonly int hashIsTrace = Animator.StringToHash("IsTrace");

    public bool isDie = false;

    void Start()
    {
        // GameObject playerObj = GameObject.Find("Player");
        GameObject playerObj = GameObject.FindGameObjectWithTag("PLAYER");

        playerTr = playerObj?.GetComponent<Transform>();
        monsterTr = transform; // monsterTr = GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>();

        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    IEnumerator CheckMonsterState()
    {
        while (isDie == false)
        {
            // 상태값을 측정
            float distance = Vector3.Distance(monsterTr.position, playerTr.position);

            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    // 아이들링일 경우 로직처리
                    agent.isStopped = true;
                    animator.SetBool(hashIsTrace, false);
                    break;
                case State.TRACE:
                    // 추적 상태일때 로직처리
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    animator.SetBool(hashIsTrace, true);
                    break;
                case State.ATTACK:
                    // 공격 상태일때 로직처리
                    Debug.Log("공격");
                    break;
                case State.DIE:
                    // 죽은 상태일때 로직처리
                    break;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
}
