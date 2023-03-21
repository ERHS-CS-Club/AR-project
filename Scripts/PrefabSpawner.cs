using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField] float scaleSpeed = 0.01f;
    float previousDistance = -1f;
    Transform selectedTransform;
    [SerializeField] Material selectedMaterial;
    Material previousMaterial;

    [SerializeField] Camera _camera;
    [SerializeField] GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    void TestScreenPosition(Vector2 screenPosition)
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(screenPosition), out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Selectable"))
            {
                if (selectedTransform == hit.transform)
                {
                    selectedTransform.GetComponent<MeshRenderer>().material = previousMaterial;
                    selectedTransform = null;
                }
                else
                {
                    if (selectedTransform != null)
                    {
                        selectedTransform.GetComponent<MeshRenderer>().material = previousMaterial;
                    }

                    selectedTransform = hit.transform;
                    MeshRenderer meshRenderer = selectedTransform.GetComponent<MeshRenderer>();
                    previousMaterial = meshRenderer.material;
                    meshRenderer.material = selectedMaterial;
                }
                //Debug.DrawLine(_camera.transform.position, hit.point, Color.green, 1);
            }
            else
            {
                Instantiate(prefab, hit.point + hit.normal * prefab.transform.localScale.z, prefab.transform.rotation);
                //Debug.DrawLine(_camera.transform.position, hit.point, Color.red, 1);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 1)
        {
            if (selectedTransform != null)
            {
                float distance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                if (previousDistance != -1)
                {
                    float deltaDistance = distance - previousDistance;
                    float scale = scaleSpeed * deltaDistance;
                    selectedTransform.localScale += new Vector3(scale, scale, scale);
                    if (selectedTransform.localScale.x < 0 || selectedTransform.localScale.y < 0 || selectedTransform.localScale.z < 0)
                        selectedTransform.localScale = Vector3.zero;
                }

                previousDistance = distance;
            }
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                TestScreenPosition(touch.position);

                if (Input.touchCount > 0)
                {
                    if (Physics.Raycast(_camera.ScreenPointToRay(Input.GetTouch(0).position), out RaycastHit hit, Mathf.Infinity))
                    {
                        Collider newCollider = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal)).GetComponent<Collider>();
                        newCollider.transform.position += hit.normal * newCollider.bounds.extents.z;
                        Debug.DrawLine(_camera.transform.position, hit.point, Color.red, 1);
                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    TestScreenPosition(Input.mousePosition);

                    if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity))
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
                else
                {
                    previousDistance = -1f;
                }
            }
        }
    }
}
