using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] Camera _camera;
    [SerializeField] GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {            
            if(Physics.Raycast(_camera.ScreenPointToRay(Input.GetTouch(0).position), out RaycastHit hit, Mathf.Infinity))
            {
                Collider newCollider = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal)).GetComponent<Collider>();
                newCollider.transform.position += hit.normal * newCollider.bounds.extents.z;
                Debug.DrawLine(_camera.transform.position, hit.point, Color.red, 1);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                Collider newCollider = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal)).GetComponent<Collider>();
                newCollider.transform.position += hit.normal * newCollider.bounds.extents.z;
                Debug.DrawLine(_camera.transform.position, hit.point, Color.green, 1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(prefab, _camera.transform.position, _camera.transform.rotation).GetComponent<Rigidbody>().AddForce(_camera.transform.forward * 8, ForceMode.Impulse);
        }
    }
}
