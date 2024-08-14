using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    void OnCollisionEnter(Collision coll)
    {
        // 충돌한 물체를 파악
        if (coll.collider.CompareTag("BULLET"))
        {
            // 충돌 정보 => 충돌한 부분의 점의 개수 [0, 1, 2.....]
            ContactPoint cp = coll.GetContact(0);

            // 충돌 좌표
            Vector3 _point = cp.point;

            // 법선 벡터
            Vector3 _normal = -cp.normal;

            // 법선 벡터가 가리키는 방향의 각도(쿼터니언)를 계산
            Quaternion rot = Quaternion.LookRotation(_normal);

            // 스파크 이펙트 생성
            GameObject obj = Instantiate(sparkEffect, _point, rot);

            Destroy(obj, 0.5f);

            Destroy(coll.gameObject);
        }

        // if (coll.collider.tag == "BULLET")  // 사용 금지
        // {
        //     Destroy(coll.gameObject);
        // }
    }

    // 충돌 콜백 함수
    /*
        - 양쪽 다 Collider를 갖고 있어야 한다.
        - 이동하는 게임 오브젝트에 Rigidbody가 있어야 한다.

        # IsTrigger가 언체크인 경우
            - OnCollisionEnter : 충돌이 발생된 순간 실행
            - OnCollisionStay : 충돌이 발생된 상태를 유지하고 있을 때 실행
            - OnCollisionExit : 충돌이 해제되었을 때 실행

        # IsTrigger가 체크인 경우
            - OnTriggerEnter : 충돌이 발생된 순간 실행
            - OnTriggerStay : 충돌이 발생된 상태를 유지하고 있을 때 실행
            - OnTriggerExit : 충돌이 해제되었을 때 실행
    */
}
