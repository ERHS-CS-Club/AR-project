using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class TapToPlace : MonoBehaviour
{
    [SerializeField] Camera _camera;

    // TapToPlace
    [SerializeField] float scaleSpeed = 0.01f;
    float previousDistance = -1f;
    Transform selectedTransform;
    [SerializeField] Material selectedMaterial;
    Material previousMaterial;
    [SerializeField] int prefabIndex;
    [SerializeField] GameObject[] prefabs;
    List<GameObject> placedObjects = new List<GameObject>();

    // Wheel
    Vector3 previousHitPosition = Vector3.zero;
    Vector3 deltaHitPosition = Vector3.zero;
    [SerializeField] float rotateSpeed = 100;
    Wheel selectedWheel;

    // Block
    Block selectedBlock;

    [SerializeField] LayerMask ignoreLayers;

    [SerializeField] GameObject tapHint;
    [SerializeField] GameObject spinHint;

    private void Start()
    {
        tapHint.SetActive(true);
    }

    void TapScreen(Vector2 screenPosition)
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(screenPosition), out RaycastHit hit, Mathf.Infinity, ~ignoreLayers))
        {
            tapHint.SetActive(false);
            switch (hit.transform.tag)
            {
                case "Selectable":
                    if(selectedTransform != null)
                    {
                        if (selectedTransform == hit.transform) // If the tapped object is already selected destroy it
                        {
                            placedObjects.Remove(selectedTransform.gameObject);
                            Destroy(selectedTransform.gameObject);
                            selectedTransform = null;
                        }
                        else
                        {
                            selectedTransform.GetComponent<MeshRenderer>().material = previousMaterial;
                        }
                    }
                    
                    MeshRenderer meshRenderer = hit.transform.GetComponent<MeshRenderer>();
                    previousMaterial = meshRenderer.material;
                    meshRenderer.material = selectedMaterial;
                    Debug.DrawLine(_camera.transform.position, hit.point, Color.green, 1); // Draw a green line in the scene view
                    selectedTransform = hit.transform;
                    break;
                case "Block":
                    selectedBlock = hit.transform.GetComponent<Block>();
                    selectedBlock.OnPickUp();
                    break;
                case "Wheel":
                    selectedWheel = hit.transform.root.GetComponent<Wheel>();
                    spinHint.SetActive(false);
                    break;
                case "WheelButton":
                    hit.transform.GetComponent<Animation>().Play();
                    break;
                default:
                    GameObject clone = Instantiate(prefabs[prefabIndex], hit.point, prefabs[prefabIndex].transform.rotation);
                    if(clone.TryGetComponent<Collider>(out var cloneCollider))
                    {
                        Bounds colliderBounds = cloneCollider.bounds;
                        clone.transform.position += hit.normal * colliderBounds.extents.y;
                    }
                    placedObjects.Add(clone);
                    if (clone.CompareTag("Wheel"))
                    {
                        spinHint.SetActive(true);
                        Wheel wheelScript = clone.GetComponent<Wheel>();
                        wheelScript._camera = _camera;
                    }
                    Debug.DrawLine(_camera.transform.position, hit.point, Color.red, 1); // Draw a red line in the scene view
                    break;
            }
        }
    }

    void HoldScreen(Vector2 screenPosition)
    {
        if (selectedBlock != null)
        {
            selectedBlock.transform.position = _camera.transform.position + _camera.ScreenPointToRay(Input.mousePosition).direction;
            Debug.DrawLine(_camera.transform.position, _camera.transform.position + _camera.ScreenPointToRay(Input.mousePosition).direction, Color.cyan, 0.1f);
        }
        else if (selectedWheel != null)
        {
            if (Physics.Raycast(_camera.ScreenPointToRay(screenPosition), out RaycastHit hit, Mathf.Infinity, ~ignoreLayers))
            {
                if (hit.transform.CompareTag("Wheel"))
                {
                    deltaHitPosition = rotateSpeed * (hit.point - previousHitPosition);
                    Wheel wheel = hit.transform.root.GetComponent<Wheel>();
                    Vector3 delta = (wheel.wheelAnchor.position - hit.point).normalized;
                    Vector3 crossForward = Vector3.Cross(delta, wheel.wheelAnchor.forward);
                    

                    if (crossForward.y > 0)
                    {
                        // Target is to the right
                        wheel.wheelAnchor.localEulerAngles = new Vector3(wheel.wheelAnchor.localEulerAngles.x, wheel.wheelAnchor.localEulerAngles.y, wheel.wheelAnchor.localEulerAngles.z + Vector3.Dot(deltaHitPosition, wheel.transform.up));
                    }
                    else if (crossForward.y < 0)
                    {
                        // Target is to the left
                        wheel.wheelAnchor.localEulerAngles = new Vector3(wheel.wheelAnchor.localEulerAngles.x, wheel.wheelAnchor.localEulerAngles.y, wheel.wheelAnchor.localEulerAngles.z + Vector3.Dot(-deltaHitPosition, wheel.transform.up));
                    }

                    if (hit.point.y > wheel.wheelAnchor.position.y)
                    {
                        // Target is above
                        wheel.wheelAnchor.localEulerAngles = new Vector3(wheel.wheelAnchor.localEulerAngles.x, wheel.wheelAnchor.localEulerAngles.y, wheel.wheelAnchor.localEulerAngles.z + Vector3.Dot(-deltaHitPosition, wheel.transform.right));
                    }
                    else if (hit.point.y < wheel.wheelAnchor.position.y)
                    {
                        // Target is below
                        wheel.wheelAnchor.localEulerAngles = new Vector3(wheel.wheelAnchor.localEulerAngles.x, wheel.wheelAnchor.localEulerAngles.y, wheel.wheelAnchor.localEulerAngles.z + Vector3.Dot(deltaHitPosition, wheel.transform.right));
                    }
                }

                previousHitPosition = hit.point;
            }
        }
    }

    void Update()
    {
        if (Input.touchCount > 1) // When 2 or more fingers are on the screen we scale the selected object
        {
            if (selectedTransform != null)
            {
                float distance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                if (previousDistance != -1) // If we have a previous distance then scale the object
                {
                    float deltaDistance = distance - previousDistance;
                    float scale = scaleSpeed * deltaDistance;
                    selectedTransform.localScale += new Vector3(scale, scale, scale);
                    if (selectedTransform.localScale.x < 0 || selectedTransform.localScale.y < 0 || selectedTransform.localScale.z < 0) 
                        selectedTransform.localScale = Vector3.zero; // Stop the scale from going negative
                }

                previousDistance = distance;
            }
        }
        else if (Input.touchCount == 1) // Single tap to send a ray and select or spawn a object
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                TapScreen(touch.position);
            }
            else if(touch.phase == TouchPhase.Moved)
            {
                HoldScreen(touch.position);
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            TapScreen(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            HoldScreen(Input.mousePosition);
        }
        else
        {
            if(selectedBlock != null)
            {
                selectedBlock.OnRelease();
                selectedBlock = null;
            }
            if (selectedWheel != null)
            {
                selectedWheel.OnRelease();
                selectedWheel = null;
            }
            previousDistance = -1f; // When fingers are lifted from the screen reset the distance to stop the scale from jumping around
        }
    }

    public void ResetObjects()
    {
        foreach(GameObject GO in placedObjects)
        {
            Destroy(GO);
        }
        placedObjects.Clear();
    }

    public void ChangePrefab(TMP_Dropdown dropdown)
    {
        prefabIndex = dropdown.value;
    }
}
