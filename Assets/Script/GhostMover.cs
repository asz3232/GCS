using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// 귀신(또는 다른 오브젝트)이 인스펙터에서 지정한 포인트들을
/// 순서대로 이동하게 하는 스크립트.
/// 각 구간마다 "이동에 걸리는 시간"과 "도착 후 멈춰있는 시간"을
/// 따로 설정할 수 있습니다.
///
/// 사용법:
/// 1) 씬에 빈 오브젝트들을 만들어서 귀신이 지나갈 위치에 배치합니다
///    (예: "Point_01", "Point_02" ...).
/// 2) 귀신 오브젝트에 이 스크립트를 붙입니다.
/// 3) Path 리스트에 위 포인트들을 순서대로 등록하고,
///    각 항목마다 Move Duration(이동 시간) / Wait Time(대기 시간)을 정합니다.

public class GhostMover : MonoBehaviour
{
    [System.Serializable]
    public class GhostPathPoint
    {
        [Tooltip("귀신이 이동할 목표 위치")]
        public Transform point;

        [Tooltip("이전 지점에서 이 지점까지 이동하는 데 걸리는 시간(초)")]
        public float moveDuration = 2f;

        [Tooltip("이 지점에 도착한 뒤 다음으로 출발하기 전까지 멈춰있는 시간(초)")]
        public float waitTime = 1f;
    }

    [Header("경로 설정")]
    [SerializeField] private List<GhostPathPoint> path = new List<GhostPathPoint>();

    [Header("반복 방식")]
    [Tooltip("체크하면 마지막 포인트 이후 다시 첫 포인트로 돌아갑니다 (순환). 체크 해제하면 왕복(핑퐁)합니다.")]
    [SerializeField] private bool loopBackToStart = true;

    [Header("회전")]
    [SerializeField] private bool faceMoveDirection = true;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("시작 옵션")]
    [SerializeField] private bool startFromCurrentPosition = true; // 시작 시 첫 포인트로 순간이동하지 않고 그 자리에서 첫 포인트로 이동

    private Coroutine moveRoutine;

    private void Start()
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"{name}: GhostMover의 Path가 비어있습니다.");
            return;
        }

        moveRoutine = StartCoroutine(MoveAlongPath());
    }

    private IEnumerator MoveAlongPath()
    {
        int index = 0;
        int direction = 1; // 핑퐁 모드에서 진행 방향

        if (!startFromCurrentPosition && path[0].point != null)
        {
            transform.position = path[0].point.position;
        }

        while (true)
        {
            GhostPathPoint target = path[index];
            if (target.point != null)
            {
                yield return MoveTo(target.point.position, target.moveDuration);
                if (target.waitTime > 0f)
                {
                    yield return new WaitForSeconds(target.waitTime);
                }
            }

            index = GetNextIndex(index, ref direction);
        }
    }

    private int GetNextIndex(int currentIndex, ref int direction)
    {
        if (path.Count == 1) return 0;

        if (loopBackToStart)
        {
            return (currentIndex + 1) % path.Count;
        }

        // 핑퐁(왕복) 모드: 끝에 닿으면 방향을 뒤집습니다.
        int next = currentIndex + direction;
        if (next < 0 || next >= path.Count)
        {
            direction *= -1;
            next = currentIndex + direction;
        }
        return next;
    }

  
    /// duration 시간 동안 현재 위치에서 목표 위치까지 부드럽게 이동합니다.

    private IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        // 이동 시간이 0 이하면 즉시 이동
        if (duration <= 0f)
        {
            transform.position = targetPosition;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (faceMoveDirection)
            {
                Vector3 dir = targetPosition - transform.position;
                if (dir.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
                }
            }

            yield return null;
        }

        transform.position = targetPosition;
    }


    /// 외부에서 이동을 멈추고 싶을 때 호출 (예: 플레이어에게 들켰을 때 등).

    public void StopMoving()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }


    /// 멈춘 이동을 다시 시작합니다.

    public void ResumeMoving()
    {
        if (moveRoutine == null && path.Count > 0)
        {
            moveRoutine = StartCoroutine(MoveAlongPath());
        }
    }

    // 씬 뷰에서 경로를 눈으로 확인할 수 있도록 기즈모로 표시합니다.
    private void OnDrawGizmosSelected()
    {
        if (path == null || path.Count < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count; i++)
        {
            if (path[i].point == null) continue;

            Gizmos.DrawSphere(path[i].point.position, 0.2f);

            int nextIndex = (i + 1) % path.Count;
            if (!loopBackToStart && i == path.Count - 1) continue;
            if (path[nextIndex].point == null) continue;

            Gizmos.DrawLine(path[i].point.position, path[nextIndex].point.position);
        }
    }
}