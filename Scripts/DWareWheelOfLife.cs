using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DWareWheelOfLife : MonoBehaviour
{
    Vector3 previousHitPosition = Vector3.zero;
    Vector3 deltaHitPosition = Vector3.zero;

    [SerializeField] float rotateSpeed = 100;
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit wheelHit, Mathf.Infinity))
            {
                if (wheelHit.transform.CompareTag("Wheel"))
                {
                    deltaHitPosition = rotateSpeed * (wheelHit.point - previousHitPosition);
                    Transform wheel = wheelHit.transform;
                    Transform wheelOrigin = wheel.parent;
                    Vector3 delta = (wheelOrigin.position - wheelHit.point).normalized;
                    Vector3 crossForward = Vector3.Cross(delta, wheelOrigin.forward);
                    Vector3 crossRight = Vector3.Cross(delta, wheelOrigin.right);
                    Debug.Log("Right: " + crossRight);
                    Debug.Log("Forward: " + crossForward);

                    if (crossForward.y > 0)
                    {
                        // Target is to the right
                        wheel.Rotate(wheelOrigin.forward, Vector3.Dot(deltaHitPosition, wheelOrigin.up), Space.World);
                    }
                    else if (crossForward.y < 0)
                    {
                        // Target is to the left
                        wheel.Rotate(wheelOrigin.forward, Vector3.Dot(-deltaHitPosition, wheelOrigin.up), Space.World);
                    }

                    if (wheelHit.point.y > wheelOrigin.position.y)
                    {
                        // Target is above
                        wheel.Rotate(wheelOrigin.forward, Vector3.Dot(-deltaHitPosition, wheelOrigin.right), Space.World);
                    }
                    else if (wheelHit.point.y < wheelOrigin.position.y)
                    {
                        // Target is below
                        wheel.Rotate(wheelOrigin.forward, Vector3.Dot(deltaHitPosition, wheelOrigin.right), Space.World);
                    }
                }
                previousHitPosition = wheelHit.point;
            }
        }
    }
}
