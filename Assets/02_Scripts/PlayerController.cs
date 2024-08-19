#pragma warning disable CS0108

using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    // 전역변수 선언
    private float v;
    private float h;
    private float r;

    [SerializeField]
    private float moveSpeed = 3.0f;

    [SerializeField]
    private float turnSpeed = 200.0f;

    [SerializeField]
    private Transform firePos;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private AudioClip fireSfx;

    [SerializeField]
    private MeshRenderer muzzleFlash;

    // Animator 컴포넌트를 저장할 변수 선언
    // [NonSerialized] => 밑의 속성과 같은 동작을 수행함
    [HideInInspector]
    public Animator animator;

    // private new AudioSource audio;
    private AudioSource audio;

    // Animator Hash 추출
    private readonly int hashForward = Animator.StringToHash("forward");
    private readonly int hashStrafe = Animator.StringToHash("strafe");

    private float initHp = 100.0f;
    private float currHp = 100.0f;

    // User Define Event 사용자 정의 이벤트
    // Delegate 델리게이트 - 대리자
    public delegate void PlayerDieHandler();
    // 이벤트 선언
    public static event PlayerDieHandler OnPlayerDie;

    // 1. 호출, 제일 먼저 호출
    void Start()
    {
        // animator = this.gameObject.GetComponent<Animator>();
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();

        // MuzzleFlash의 MeshRenderer 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
    }

    // 매 프레임마다 호출, 60FPS, 불규칙한 주기, 렌더링 주기와 동일
    void Update()
    {
        InputAxis();
        Locomotion();
        Animation();
        Fire();
    }

    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 총알 프리팹을 이용해서 런타임에서 동적으로 생성
            // Instantiate(생성할 객체, 위치, 각도);
            Instantiate(bulletPrefab, firePos.position, firePos.rotation);

            // 총소리 발생
            // audio.clip = fireSfx;
            // audio.Play();
            audio.PlayOneShot(fireSfx, 0.8f);

            // 총구 화염 효과
            StartCoroutine(ShowMuzzleFlash());
        }
    }

    // 코루틴(Co-routine)
    private IEnumerator ShowMuzzleFlash()
    {
        // MuzzleFlash 활성화
        muzzleFlash.enabled = true;

        // Texture Offset 변경 (0,0) (0.5, 0) (0.5, 0.5) (0, 0.5)
        // Random.Range(0, 2) = (0, 1) * 0.5
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.mainTextureOffset = offset;

        // Scale 변경
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1.0f, 3.0f);

        // Z축 회전
        muzzleFlash.transform.localRotation = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));

        // Waiting ...
        yield return new WaitForSeconds(0.2f);

        // MuzzleFlash 비활성화
        muzzleFlash.enabled = false;
    }

    private void Animation()
    {
        // 애니메이션 파라미터 전달
        animator.SetFloat(hashForward, v);
        animator.SetFloat(hashStrafe, h);
    }

    private void Locomotion()
    {
        // Vector 덧셈 연산
        // 이동처리 로직
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        // Debug.Log("비정규화 " + moveDir.magnitude);
        // Debug.Log("정규화 " + moveDir.normalized.magnitude);

        // transform.position += new Vector3(0, 0, 0.1f);
        // transform.position += Vector3.forward * 0.1f;

        // transform.Translate(Vector3.forward * v * 0.1f);
        // transform.Translate(Vector3.right * h * 0.1f);

        transform.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);

        // 회전 처리 로직
        transform.Rotate(Vector3.up * Time.deltaTime * r * turnSpeed);
    }

    private void InputAxis()
    {
        // 축(Axis) 값을 받아옴. -1.0f ~ 0.0 ~ +1.0f;
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        r = Input.GetAxis("Mouse X");
    }

    void OnTriggerEnter(Collider coll)
    {
        if (currHp > 0.0f && coll.gameObject.CompareTag("PUNCH"))
        {
            currHp -= 10.0f;

            if (currHp <= 0.0f)
            {
                // 이벤트를 발생(Raise)
                OnPlayerDie();
                GameManager.Instance.IsGameOver = true;

                // PlayerDie();
            }
        }
    }

    private void PlayerDie()
    {
        // MONSTER 태그를 달고 있는 모든 몬스터를 추출
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        foreach (var monster in monsters)
        {
            // monster.SendMessage("YouWin", SendMessageOptions.DontRequireReceiver);
            monster.GetComponent<MonsterController>().YouWin();
        }
    }
}


/*
    Vector3.forward = Vector3(0, 0, 1)
    Vector3.up      = Vector3(0, 1, 0)
    Vector3.right   = Vector3(1, 0, 0)

    Vector3.one     = Vector3(1, 1, 1)
    Vector3.zero    = Vector3(0, 0, 0)
*/

/*
    Quaternion 쿼터니언 (사 원수) x, y, z, w
    복소수 사차원 벡터

    오일러 회전 (오일러각 Euler) 0 ~ 360
    x -> y -> z

    짐벌락(Gimbal Lock)

    Quaternion.Euler(30, 45, -15)
    => 오일럭 각을 쿼터니언으로 변환

    Quaternion.LookRotation(벡터)
    => 벡터가 가리키는 방향을 쿼터니언으로 변환

    Quaternion.identity
    => 오브젝트가 바라보는 방향을 쿼터니언으로 나타냄
*/