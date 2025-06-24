using System;
using System.Collections.Generic;
using UnityEngine;

// IK_toolkit: UR16e �κ� ���� ���ⱸ�� ��Ŷ
[ExecuteInEditMode]
public class IK_toolkit : MonoBehaviour
{
    public Transform ik; // IK ����� ���� Transform
    public int solutionID; // ���õ� �ַ���� ID
    [SerializeField]private List<string> IK_Solutions = new List<string>(); // ������ IK �ַ�� ���
    public List<double> goodSolution = new List<double>(); // ��ȿ�� �ַ�� ����
    public List<Transform> robot = new List<Transform>(); // �κ� ���� Transform ���

    // UR16e �κ� ���� Denavit-Hartenberg �Ķ���� ���
    public static double[,] DH_matrix_UR16e = new double[6, 3] {
        { 0, Mathf.PI / 2.0, 0.1807 },
        { -0.4784, 0, 0 },
        { -0.36, 0, 0 },
        { 0, Mathf.PI / 2.0, 0.17415 },
        { 0, -Mathf.PI / 2.0, 0.11985},
        { 0, 0, 0.11655}
    };

    // �� �����Ӹ��� ȣ��Ǵ� Update �޼���
    void Update()
    {
        // IK�� ���� ��ȯ ��� ���
        Matrix4x4 transform_matrix = GetTransformMatrix(ik);

        // Y���� �������� ����� �ݻ�
        Matrix4x4 mt = Matrix4x4.identity;
        mt.m11 = -1; // Y�� �ݻ� ��� ����
        Matrix4x4 mt_inverse = mt.inverse; // �ݻ� ����� �����
        Matrix4x4 result = mt * transform_matrix * mt_inverse; // ���� ��ȯ ���

        // ���ⱸ�� �ַ�� ���
        double[,] solutions = Inverse_kinematic_solutions(result);
        IK_Solutions.Clear(); // ���� �ַ�� ��� �ʱ�ȭ
        IK_Solutions = DisplaySolutions(solutions); // ���ο� �ַ�� ��� ǥ��

        // ���õ� �ַ�ǿ� ���� �κ� �� ���� ����
        ApplyJointSolution(IK_Solutions, solutions, solutionID, robot);
        goodSolution.Clear(); // ��ȿ�� �ַ�� �ʱ�ȭ
        // ��ȿ�� �ַ���� �� ���� ���� ����
        for (int i = 0; i < 6; i++) {
            goodSolution.Add(solutions[i, 5]);
        }
    }

    // �־��� Transform�� ���� ��ȯ ����� ��� �޼���
    public static Matrix4x4 GetTransformMatrix(Transform controller)
    {
        // ��ġ �� ȸ���� ������� ��ȯ ��� ����
        return Matrix4x4.TRS(new Vector3(controller.localPosition.x, controller.localPosition.y, controller.localPosition.z), 
                              Quaternion.Euler(controller.localEulerAngles.x, controller.localEulerAngles.y, controller.localEulerAngles.z), 
                              new Vector3(1, 1, 1));
    }

    // Denavit-Hartenberg �Ķ���͸� ����Ͽ� ��ȯ ��� ���
    public static Matrix4x4 ComputeTransformMatrix(int jointIndex, double[,] jointAngles)
    {
        jointIndex--;

        // Z�� ���� ȸ��
        var rotationZ = Matrix4x4.identity;
        rotationZ.m00 = Mathf.Cos((float)jointAngles[0, jointIndex]);
        rotationZ.m01 = -Mathf.Sin((float)jointAngles[0, jointIndex]);
        rotationZ.m10 = Mathf.Sin((float)jointAngles[0, jointIndex]);
        rotationZ.m11 = Mathf.Cos((float)jointAngles[0, jointIndex]);

        // Z���� ���� �̵�
        var translationZ = Matrix4x4.identity;
        translationZ.m23 = (float)DH_matrix_UR16e[jointIndex, 2];

        // X���� ���� �̵�
        var translationX = Matrix4x4.identity;
        translationX.m03 = (float)DH_matrix_UR16e[jointIndex, 0];

        // X�� ���� ȸ��
        var rotationX = Matrix4x4.identity;
        rotationX.m11 = Mathf.Cos((float)DH_matrix_UR16e[jointIndex, 1]);
        rotationX.m12 = -Mathf.Sin((float)DH_matrix_UR16e[jointIndex, 1]);
        rotationX.m21 = Mathf.Sin((float)DH_matrix_UR16e[jointIndex, 1]);
        rotationX.m22 = Mathf.Cos((float)DH_matrix_UR16e[jointIndex, 1]);

        // ��ȯ�� ����: rotationZ, translationZ, translationX, rotationX
        return rotationZ * translationZ * translationX * rotationX;
    }

    // ���ⱸ�� �ַ���� �κ� �� ������ �����ϴ� �޼���
    public static void ApplyJointSolution(List<string> solutionStatus, double[,] jointSolutions, int solutionIndex, List<Transform> robotJoints)
    {
        // �ַ���� ��ȿ���� Ȯ��
        if (solutionStatus[solutionIndex] != "NON DISPONIBLE")
        {
            // �κ��� �� ������ ���� ���� ����
            for (int i = 0; i < robotJoints.Count; i++)
            {
                robotJoints[i].localEulerAngles = ConvertJointAngles(jointSolutions[i, solutionIndex], i);
            }
        }
        else
        {
            // �ַ���� ������ ���� �޽��� ���
            Debug.LogError("NO SOLUTION");
        }
    }

    // ���� ������ ���ȿ��� ���� ��ȯ�ϰ� �������� �����ϴ� �޼���
    private static Vector3 ConvertJointAngles(double angleRad, int jointIndex)
    {
        float angleDeg = -(float)(Mathf.Rad2Deg * angleRad); // ������ ���� ��ȯ

        // �� ������ ���� ������ ����
        switch (jointIndex)
        {
            case 1:
            case 4:
                return new Vector3(-90, 0, angleDeg);
            case 5:
                return new Vector3(90, 0, angleDeg);
            default:
                return new Vector3(0, 0, angleDeg);
        }
    }

    // ���ⱸ�� �ַ�� ���
    public static double[,] Inverse_kinematic_solutions(Matrix4x4 transform_matrix_unity)
    {
        double[,] theta = new double[6, 8]; // �ַ�� ���� �迭

        // P05 ��ġ ���
        Vector4 P05 = transform_matrix_unity * new Vector4()
        {
            x = 0,
            y = 0,
            z = -(float)DH_matrix_UR16e[5, 2],
            w = 1
        };
        
        // ���� ���
        float psi = Mathf.Atan2(P05[1], P05[0]);
        float phi = Mathf.Acos((float)((DH_matrix_UR16e[1, 2] + DH_matrix_UR16e[3, 2] + DH_matrix_UR16e[2, 2]) / Mathf.Sqrt(Mathf.Pow(P05[0], 2) + Mathf.Pow(P05[1], 2))));

        // ù ��° ���� ���� ����
        theta[0, 0] = psi + phi + Mathf.PI / 2;
        // ������ ������ ����� ������� ����
        for (int i = 1; i <= 7; i++)
        {
            theta[0, i] = psi - phi + Mathf.PI / 2; // �ݺ������� ó��
        }

        // �� ������ ��ġ�� ������� �߰� ���� ���
        for (int i = 0; i < 8; i += 4)
        {
            double t5 = (transform_matrix_unity[0, 3] * Mathf.Sin((float)theta[0, i]) - transform_matrix_unity[1, 3] * Mathf.Cos((float)theta[0, i]) - (DH_matrix_UR16e[1, 2] + DH_matrix_UR16e[3, 2] + DH_matrix_UR16e[2, 2])) / DH_matrix_UR16e[5, 2];
            float th5;
            if (1 >= t5 && t5 >= -1)
            {
                th5 = Mathf.Acos((float)t5); // ��ȿ�� ��� ��ũ�ڻ��� ���
            }
            else
            {
                th5 = 0; // ��ȿ���� ������ 0���� ����
            }

            // ���� ����
            if (i == 0)
            {
                theta[4, 0] = th5;
                theta[4, 1] = th5;
                theta[4, 2] = -th5;
                theta[4, 3] = -th5;
                }
            else
            {
                // theta �迭�� Ư�� �ε����� th5 ���� �Ҵ�
                theta[4, 4] = th5; // 4��° �ε����� th5 �Ҵ�
                theta[4, 5] = th5; // 5��° �ε����� th5 �Ҵ�
                theta[4, 6] = -th5; // 6��° �ε����� -th5 �Ҵ�
                theta[4, 7] = -th5; // 7��° �ε����� -th5 �Ҵ�
            }
        }

        // transform_matrix_unity�� ������� ���
        Matrix4x4 tmu_inverse = transform_matrix_unity.inverse;

        // theta �迭�� �� �ε����� ���� ��ũź��Ʈ�� ���
        float th0 = Mathf.Atan2((-tmu_inverse[1, 0] * Mathf.Sin((float)theta[0, 0]) + tmu_inverse[1, 1] * Mathf.Cos((float)theta[0, 0])), 
                                (tmu_inverse[0, 0] * Mathf.Sin((float)theta[0, 0]) - tmu_inverse[0, 1] * Mathf.Cos((float)theta[0, 0])));
        
        float th2 = Mathf.Atan2((-tmu_inverse[1, 0] * Mathf.Sin((float)theta[0, 2]) + tmu_inverse[1, 1] * Mathf.Cos((float)theta[0, 2])), 
                                (tmu_inverse[0, 0] * Mathf.Sin((float)theta[0, 2]) - tmu_inverse[0, 1] * Mathf.Cos((float)theta[0, 2])));
        
        float th4 = Mathf.Atan2((-tmu_inverse[1, 0] * Mathf.Sin((float)theta[0, 4]) + tmu_inverse[1, 1] * Mathf.Cos((float)theta[0, 4])), 
                                (tmu_inverse[0, 0] * Mathf.Sin((float)theta[0, 4]) - tmu_inverse[0, 1] * Mathf.Cos((float)theta[0, 4])));
        
        float th6 = Mathf.Atan2((-tmu_inverse[1, 0] * Mathf.Sin((float)theta[0, 6]) + tmu_inverse[1, 1] * Mathf.Cos((float)theta[0, 6])), 
                                (tmu_inverse[0, 0] * Mathf.Sin((float)theta[0, 6]) - tmu_inverse[0, 1] * Mathf.Cos((float)theta[0, 6])));

        // theta �迭�� ��ũź��Ʈ ����� ����
        theta[5, 0] = th0;
        theta[5, 1] = th0;
        theta[5, 2] = th2;
        theta[5, 3] = th2;
        theta[5, 4] = th4;
        theta[5, 5] = th4;
        theta[5, 6] = th6;
        theta[5, 7] = th6;

        // �� �ַ�ǿ� ���� �ݺ�
        for (int i = 0; i <= 7; i += 2)
        {
            double[,] t1 = new double[1, 6];
            // theta �迭�� ���� t1 �迭�� ����
            t1[0, 0] = theta[0, i];
            t1[0, 1] = theta[1, i];
            t1[0, 2] = theta[2, i];
            t1[0, 3] = theta[3, i];
            t1[0, 4] = theta[4, i];
            t1[0, 5] = theta[5, i];

            // ��ȯ ����� ���
            Matrix4x4 T01 = ComputeTransformMatrix(1, t1);
            Matrix4x4 T45 = ComputeTransformMatrix(5, t1);
            Matrix4x4 T56 = ComputeTransformMatrix(6, t1);
            Matrix4x4 T14 = T01.inverse * transform_matrix_unity * (T45 * T56).inverse;

            // P13 ���� ���
            Vector4 P13 = T14 * new Vector4()
            {
                x = 0,
                y = (float)-DH_matrix_UR16e[3, 2], // DH ��Ʈ������ Ư�� �� ���
                z = 0,
                w = 1
            };

            // theta[2, i]�� ���� th3 ���
            double t3 = (Mathf.Pow(P13[0], 2) + Mathf.Pow(P13[1], 2) - Mathf.Pow((float)DH_matrix_UR16e[1, 0], 2) - 
                         Mathf.Pow((float)DH_matrix_UR16e[2, 0], 2)) / 
                         (2 * DH_matrix_UR16e[1, 0] * DH_matrix_UR16e[2, 0]);
            double th3;
            // t3�� ��ȿ ���� ���� ���� ��� ��ũ�ڻ����� ���
            if (1 >= t3 && t3 >= -1)
            {
                th3 = Mathf.Acos((float)t3);
            }
            else
            {
                th3 = 0; // t3�� ������ ����� 0���� ����
            }
            theta[2, i] = th3; // theta �迭�� th3 ����
            theta[2, i + 1] = -th3; // ��Ī�� ����
        }

        // ��� �ַ�ǿ� ���� �ݺ�
        for (int i = 0; i < 8; i++)
        {
            double[,] t1 = new double[1, 6];
            // theta �迭�� ���� t1 �迭�� ����
            t1[0, 0] = theta[0, i];
            t1[0, 1] = theta[1, i];
            t1[0, 2] = theta[2, i];
            t1[0, 3] = theta[3, i];
            t1[0, 4] = theta[4, i];
            t1[0, 5] = theta[5, i];

            // ��ȯ ����� ���
            Matrix4x4 T01 = ComputeTransformMatrix(1, t1);
            Matrix4x4 T45 = ComputeTransformMatrix(5, t1);
            Matrix4x4 T56 = ComputeTransformMatrix(6, t1);
            Matrix4x4 T14 = T01.inverse * transform_matrix_unity * (T45 * T56).inverse;

            // P13 ���� ���
            Vector4 P13 = T14 * new Vector4()
            {
                x = 0,
                y = (float)-DH_matrix_UR16e[3, 2],
                z = 0,
                w = 1
            };

            // theta[1, i] ���
            theta[1, i] = Mathf.Atan2(-P13[1], -P13[0]) - 
                           Mathf.Asin((float)(-DH_matrix_UR16e[2, 0] * Mathf.Sin((float)theta[2, i]) / 
                           Mathf.Sqrt(Mathf.Pow(P13[0], 2) + Mathf.Pow(P13[1], 2))));

            double[,] t2 = new double[1, 6];
            // theta �迭�� ���� t2 �迭�� ����
            t2[0, 0] = theta[0, i];
            t2[0, 1] = theta[1, i];
            t2[0, 2] = theta[2, i];
            t2[0, 3] = theta[3, i];
            t2[0, 4] = theta[4, i];
            t2[0, 5] = theta[5, i];

            // ��ȯ ����� ���
            Matrix4x4 T32 = ComputeTransformMatrix(3, t2).inverse;
            Matrix4x4 T21 = ComputeTransformMatrix(2, t2).inverse;
            Matrix4x4 T34 = T32 * T21 * T14;

            // theta[3, i] ���
            theta[3, i] = Mathf.Atan2(T34[1, 0], T34[0, 0]);
        }

        return theta; // ���� theta �迭 ��ȯ
    }

    // �ַ���� ǥ���ϱ� ���� �޼���
    public static List<string> DisplaySolutions(double[,] solutions)
    {
        List<string> info = new List<string>();

        // 8���� ������ �ַ���� �ݺ�
        for (int column = 0; column < 8; column++)
        {
            // ��� ����Ʈ ������ ��ȿ���� Ȯ��
            bool isValidSolution = true;
			for (int row = 0; row < 6; row++)
			{
				// �� �ַ���� ��(row)�� ���� NaN ���� Ȯ��
				if (double.IsNaN(solutions[row, column]))
				{
				    isValidSolution = false; // NaN�� �߰ߵǸ� ��ȿ���� ���� �ַ������ ����
				    break; // �ݺ��� ����
				}
			}
				
			// �ַ���� ��ȿ�� ���, ���� ������ �����Ͽ� info ����Ʈ�� �߰�
			if (isValidSolution)
			{
				string solutionInfo = ""; // �ַ�� ������ ������ ���ڿ� �ʱ�ȭ
				for (int row = 0; row < 6; row++)
				{
				    // ���� ���� ��(degree) ������ ��ȯ�ϰ� �Ҽ��� ��° �ڸ����� �ݿø�
				    double angleInDegrees = Math.Round(Mathf.Rad2Deg * solutions[row, column], 2);
				    solutionInfo += $"{angleInDegrees}"; // ������ ���ڿ��� �߰�
				
				    // ������ ���� �ƴ� ��� ������ �߰�
				    if (row < 5)
				    {
				        solutionInfo += " | ";
				    }
				}
				info.Add(solutionInfo); // �ϼ��� �ַ�� ������ info ����Ʈ�� �߰�
			}
			// �ַ���� ��ȿ���� ���� ��� "NON DISPONIBLE"�� info ����Ʈ�� �߰�
			else
			{
				info.Add("NON DISPONIBLE"); // ��ȿ���� ���� �ַ�ǿ� ���� �޽��� �߰�
			}
		}
				
		return info; // info ����Ʈ ��ȯ
	}
}
