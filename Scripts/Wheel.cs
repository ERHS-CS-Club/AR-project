using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Camera _camera;
    public Transform wheelAnchor;
    [SerializeField] int sections = 4;
    [SerializeField] string[] words;
    [SerializeField] Animation[] wordAnimations;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] wordAudioClips;
    int selectedSection = 0;

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
                selectedSection = i;
                float targetAngle = angleRange * i;
                while(Mathf.Abs(wheelAnchor.localEulerAngles.z - targetAngle) > 1)
                {
                    wheelAnchor.localEulerAngles = Vector3.RotateTowards(wheelAnchor.localEulerAngles, new Vector(wheelAnchor.localEulerAngles.x, wheelAnchor.localEulerAngles.y, targetAngle), Time.deltaTime * 5, 0);
                }
                break;
            }
        }
    }

    public void ShowWord()
    {
        //wordAnimations[selectedSection].Play();
        audioSource.clip = wordAudioClips[selectedSection];
        audioSource.Play();
    }
}
