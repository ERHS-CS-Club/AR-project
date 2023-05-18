using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] GameObject[] snapPoints;
    public SnapPoint otherSnapPoint;

    public void OnPickUp()
    {
        
    }

    public void OnRelease()
    {
        Debug.Log("Release");
        if (otherSnapPoint != null)
        {
            transform.position = otherSnapPoint.transform.position;
            transform.rotation = otherSnapPoint.transform.rotation;
        }
    }
}
