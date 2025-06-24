using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    public Transform targetTransform;
    public IK_toolkit ikToolkit;
    public TMP_InputField xInput;
    public TMP_InputField yInput;
    public TMP_InputField zInput;
    public float XPosition;
    public float YPosition;
    public float ZPosition;

    public bool isXPlusButtonDown = false;
    public bool isXMinusButtonDown = false;
    public bool isYPlusButtonDown = false;
    public bool isYMinusButtonDown = false;
    public bool isZPlusButtonDown = false;
    public bool isZMinusButtonDown = false;

    public float x;
    public float y;
    public float z;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        x = XPosition;
        y = YPosition;
        z = ZPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(isXPlusButtonDown)
        {
            x= x+0.01f;
            xInput.text = x.ToString();
        }
        if(isXMinusButtonDown)
        {
            x= x-0.01f;
            xInput.text = x.ToString();
        }
        if(isYPlusButtonDown)
        {
            y= y+0.01f;
            yInput.text = y.ToString();
        }
        if(isYMinusButtonDown)
        {
            y= y-0.01f;
            yInput.text = y.ToString();
        }
        if(isZPlusButtonDown)
        {
            z= z+0.01f;
            zInput.text = z.ToString();
        }
        if(isZMinusButtonDown)
        {
            z= z-0.01f;
            zInput.text = z.ToString();
        }
        targetTransform.localPosition = new Vector3(x, y, z);
    }

    public void OnXPlusButtonDown()
    {
        isXPlusButtonDown = true;
    }

    public void OnXPlusButtonUp()
    {
        isXPlusButtonDown = false;
    }

    public void OnXMinusButtonDown()
    {
        isXMinusButtonDown = true;
    }

    public void OnXMinusButtonUp()
    {
        isXMinusButtonDown = false;
    }

    public void OnYPlusButtonDown()
    {
        isYPlusButtonDown = true;
    }

    public void OnYPlusButtonUp()
    {
        isYPlusButtonDown = false;
    }

    public void OnYMinusButtonDown()
    {
        isYMinusButtonDown = true;
    }

    public void OnYMinusButtonUp()
    {
        isYMinusButtonDown = false;
    }

    public void OnZPlusButtonDown()
    {
        isZPlusButtonDown = true;
    }

    public void OnZPlusButtonUp()
    {
        isZPlusButtonDown = false;
    }

    public void OnZMinusButtonDown()
    {
        isZMinusButtonDown = true;
    }

    public void OnZMinusButtonUp()
    {
        isZMinusButtonDown = false;
    }
    public void UpdateEndEffector()
    {
        ikToolkit.ik.localPosition = new Vector3(ikToolkit.ik.localPosition.x + x, ikToolkit.ik.localPosition.y + y, ikToolkit.ik.localPosition.z + z);

        isXPlusButtonDown = true;
        isXMinusButtonDown = true;
        isYPlusButtonDown = true;
        isYMinusButtonDown = true;
        isZPlusButtonDown = true;
        isZMinusButtonDown = true;
    }
}
