using System.Collections;
using UnityEngine;

//Cobot�� EndEffector�� ��ǥ ��ġ�� �̵��ϴ� �ڷ�ƾ�� �����ϴ� ��ũ��Ʈ
public class AutomationController : MonoBehaviour
{
    public IK_toolkit ikToolkit; //IK_toolkit ��ũ��Ʈ
    public Transform targetToPick; //ä�� ��ǥ ��ġ
    public Transform targetToPlaceCNC; //CNC ��ǥ ��ġ
    public Transform targetHome; //Home ��ǥ ��ġ

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(MachineTendingProcess());
        }
    }

    //Cobot�� �۾� ������ �����ϴ� �ڷ�ƾ
    IEnumerator MachineTendingProcess()
    {
        while (true)
        {
            // �ڷ�ƾ�� ��������� �����ϱ� ���� StartCoroutine ���
            yield return StartCoroutine(MoveRobotTo(targetToPick, 2)); //ä�� ��ǥ ��ġ�� �̵�

            yield return StartCoroutine(MoveRobotTo(targetToPlaceCNC, 3)); //CNC ��ǥ ��ġ�� �̵�

            yield return new WaitForSeconds(5); //5�� ���

            yield return StartCoroutine(MoveRobotTo(targetHome, 2)); //Home ��ǥ ��ġ�� �̵�
        }
    }

    // �κ��� ������ �ð�(time) ���� ��ǥ ��ġ(target)�� �̵���Ű�� �ڷ�ƾ
    private IEnumerator MoveRobotTo(Transform target, int time)
    {
        Vector3 startPos = ikToolkit.ik.position; //���� ��ġ
        Quaternion startRot = ikToolkit.ik.rotation; //���� ȸ����
        // ����: elapsedTime ������ float Ÿ������ ���� �� �ʱ�ȭ
        float elapsedTime = 0; 

        while (elapsedTime < time)
        {
            ikToolkit.ik.rotation = Quaternion.Slerp(startRot, target.rotation, elapsedTime / time);
            // End_Effector�� Target ��ġ�� Lerp�� �̿��� time���� �̵�
            ikToolkit.ik.position = Vector3.Lerp(startPos, target.position, elapsedTime / time);
            
            // ����: �ð��� ������ �� ���� ���ϵ��� ����
            elapsedTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        // �̵��� �Ϸ�� �� ��Ȯ�� ��ǥ ��ġ�� ����
        ikToolkit.ik.position = target.position;
    }
}
