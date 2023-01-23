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
            Touch touch = Input.GetTouch(0);
            
            if(Physics.Raycast(_camera.ScreenPointToRay(touch.position), out RaycastHit hit, Mathf.Infinity))
            {
                Instantiate(prefab, hit.point + hit.normal, prefab.transform.rotation);
                Debug.DrawLine(_camera.transform.position, hit.point, Color.red, 1);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if(Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
            {
                Instantiate(prefab, hit.point, prefab.transform.rotation);
                Debug.DrawLine(_camera.transform.position, hit.point, Color.green, 1);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(prefab, _camera.transform.position, _camera.transform.rotation).GetComponent<Rigidbody>().AddForce(_camera.transform.forward * 8, ForceMode.Impulse);
        }
    }
}
