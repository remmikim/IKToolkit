using UnityEngine;

// 여러 대의 카메라를 관리하고 방향키로 활성 카메라를 전환하는 스크립트
public class CameraManager : MonoBehaviour
{
    // 장면에 있는 모든 카메라를 담을 배열
    public Camera[] cameras;
    
    // 현재 활성화된 카메라의 인덱스
    private int currentCameraIndex;

    void Start()
    {
        // currentCameraIndex를 0으로 초기화
        currentCameraIndex = 0;

        // 모든 카메라를 비활성화
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // 첫 번째 카메라만 활성화
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
            Debug.Log("Camera " + cameras[0].name + " activated.");
        }
    }

    void Update()
    {
        // 오른쪽 화살표 키를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 다음 카메라로 전환
            SwitchToNextCamera();
        }
        // 왼쪽 화살표 키를 눌렀을 때
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 이전 카메라로 전환
            SwitchToPreviousCamera();
        }
    }

    // 다음 카메라로 전환하는 함수
    void SwitchToNextCamera()
    {
        // 현재 카메라 비활성화
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // 인덱스를 1 증가시키고, 배열 길이를 넘어서면 0으로 리셋 (순환 구조)
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

        // 새로운 현재 인덱스의 카메라를 활성화
        cameras[currentCameraIndex].gameObject.SetActive(true);
        Debug.Log("Camera " + cameras[currentCameraIndex].name + " activated.");
    }

    // 이전 카메라로 전환하는 함수
    void SwitchToPreviousCamera()
    {
        // 현재 카메라 비활성화
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // 인덱스를 1 감소
        currentCameraIndex--;
        // 인덱스가 0보다 작아지면 마지막 카메라 인덱스로 설정 (순환 구조)
        if (currentCameraIndex < 0)
        {
            currentCameraIndex = cameras.Length - 1;
        }

        // 새로운 현재 인덱스의 카메라를 활성화
        cameras[currentCameraIndex].gameObject.SetActive(true);
        Debug.Log("Camera " + cameras[currentCameraIndex].name + " activated.");
    }
}
