using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Camera _camera;
    Transform wheel;
    [SerializeField] int segments = 4;

    private void Start()
    {
        wheel = transform.GetChild(0);
    }

    void Update()
    {
        Vector3 lookDirection = _camera.transform.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void OnRelease()
    {
        float angleRange = 360f / segments;
        Debug.Log(angleRange);
        for(int i = 0; i < segments; i++)
        {
            if(wheel.eulerAngles.x > angleRange * i && wheel.eulerAngles.x < angleRange * (i+1))
            {
                Debug.Log(angleRange * i + ": " + wheel.eulerAngles.x);
                wheel.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(angleRange * i, transform.forward) * transform.forward);
                break;
            }
        }
    }
}
