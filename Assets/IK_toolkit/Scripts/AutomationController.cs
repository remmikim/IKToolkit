using System.Collections;
using UnityEngine;

//Cobot의 EndEffector를 목표 위치로 이동하는 코루틴을 실행하는 스크립트
public class AutomationController : MonoBehaviour
{
    public IK_toolkit ikToolkit; //IK_toolkit 스크립트
    public Transform targetToPick; //채취 목표 위치
    public Transform targetToPlaceCNC; //CNC 목표 위치
    public Transform targetHome; //Home 목표 위치

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(MachineTendingProcess());
        }
    }

    //Cobot의 작업 과정을 실행하는 코루틴
    IEnumerator MachineTendingProcess()
    {
        while (true)
        {
            // 코루틴을 명시적으로 시작하기 위해 StartCoroutine 사용
            yield return StartCoroutine(MoveRobotTo(targetToPick, 2)); //채취 목표 위치로 이동

            yield return StartCoroutine(MoveRobotTo(targetToPlaceCNC, 3)); //CNC 목표 위치로 이동

            yield return new WaitForSeconds(5); //5초 대기

            yield return StartCoroutine(MoveRobotTo(targetHome, 2)); //Home 목표 위치로 이동
        }
    }

    // 로봇을 지정된 시간(time) 동안 목표 위치(target)로 이동시키는 코루틴
    private IEnumerator MoveRobotTo(Transform target, int time)
    {
        Vector3 startPos = ikToolkit.ik.position; //현재 위치
        Quaternion startRot = ikToolkit.ik.rotation; //현재 회전값
        // 수정: elapsedTime 변수를 float 타입으로 선언 및 초기화
        float elapsedTime = 0; 

        while (elapsedTime < time)
        {
            ikToolkit.ik.rotation = Quaternion.Slerp(startRot, target.rotation, elapsedTime / time);
            // End_Effector를 Target 위치로 Lerp를 이용해 time동안 이동
            ikToolkit.ik.position = Vector3.Lerp(startPos, target.position, elapsedTime / time);
            
            // 수정: 시간을 루프당 한 번만 더하도록 수정
            elapsedTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        // 이동이 완료된 후 정확한 목표 위치로 설정
        ikToolkit.ik.position = target.position;
    }
}
