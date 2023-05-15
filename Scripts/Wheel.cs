using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Camera _camera;
    public Transform wheelAnchor;
    [SerializeField] int sections = 4;

    private void Start()
    {
        
    }

    void Update()
    {
        Vector3 lookDirection = _camera.transform.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void OnRelease()
    {
        float angleRange = 360f / sections;
        for(int i = 0; i < sections; i++)
        {
            if(wheelAnchor.localEulerAngles.z > angleRange * i && wheelAnchor.localEulerAngles.z < angleRange * (i+1))
            {
                wheelAnchor.localEulerAngles = new Vector3(wheelAnchor.localEulerAngles.x, wheelAnchor.localEulerAngles.y, angleRange * i);
                break;
            }
        }
    }
}
