using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class TapToPlace : MonoBehaviour
{
    [SerializeField] Camera _camera;

    Transform hitTransform;

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

    void TapScreen(Vector2 screenPosition)
    {
        if (Physics.Raycast(_camera.ScreenPointToRay(screenPosition), out RaycastHit hit, Mathf.Infinity))
        {
            hitTransform = hit.transform;
            switch (hitTransform.tag)
            {
                case "Selectable":
                    selectedTransform = hitTransform;
                    if (selectedTransform == hitTransform) // If the tapped object is already selected destroy it
                    {
                        placedObjects.Remove(selectedTransform.gameObject);
                        Destroy(selectedTransform.gameObject);
                        selectedTransform = null;
                    }
                    else // Otherwise select the tapped object
                    {
                        if (selectedTransform != null)
                        {
                            selectedTransform.GetComponent<MeshRenderer>().material = previousMaterial;
                        }

                        selectedTransform = hitTransform;
                        MeshRenderer meshRenderer = selectedTransform.GetComponent<MeshRenderer>();
                        previousMaterial = meshRenderer.material;
                        meshRenderer.material = selectedMaterial;
                    }
                    Debug.DrawLine(_camera.transform.position, hit.point, Color.green, 1); // Draw a green line in the scene view
                    break;
                case "Block":
                    hitTransform.GetComponent<Block>().OnPickUp();
                    break;
                case "Wheel":
                    break;
                default:
                    GameObject clone = Instantiate(prefabs[prefabIndex], hit.point, prefabs[prefabIndex].transform.rotation);
                    Bounds colliderBounds = clone.GetComponent<Collider>().bounds;
                    clone.transform.position += hit.normal * colliderBounds.extents.y;
                    placedObjects.Add(clone);
                    if (prefabIndex == 2)
                    {
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
        if (Physics.Raycast(_camera.ScreenPointToRay(screenPosition), out RaycastHit hit, Mathf.Infinity))
        {
            hitTransform = hit.transform;
            switch (hitTransform.tag)
            {
                case "Wheel":
                    deltaHitPosition = rotateSpeed * (hit.point - previousHitPosition);
                    Transform wheel = hitTransform;
                    Transform wheelOrigin = wheel.parent;
                    Vector3 delta = (wheelOrigin.position - hit.point).normalized;
                    Vector3 crossForward = Vector3.Cross(delta, wheelOrigin.forward);

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

                    if (hit.point.y > wheelOrigin.position.y)
                    {
                        // Target is above
                        wheel.Rotate(wheelOrigin.forward, Vector3.Dot(-deltaHitPosition, wheelOrigin.right), Space.World);
                    }
                    else if (hit.point.y < wheelOrigin.position.y)
                    {
                        // Target is below
                        wheel.Rotate(wheelOrigin.forward, Vector3.Dot(deltaHitPosition, wheelOrigin.right), Space.World);
                    }
                    break;
            }
            previousHitPosition = hit.point;
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
            if(hitTransform != null && hitTransform.CompareTag("Block"))
            {
                Debug.Log("Here: " + hitTransform.name + _camera.ScreenToWorldPoint(Input.mousePosition));
                hitTransform.position = _camera.ScreenToWorldPoint(Input.mousePosition) + _camera.transform.forward;
            }
        }
        else
        {
            previousDistance = -1f; // When fingers are lifted from the screen reset the distance to stop the scale from jumping around
            if (hitTransform != null)
            {
                switch (hitTransform.tag)
                {
                    case "Block":
                        hitTransform.GetComponent<Block>().OnRelease();
                        break;
                }
                hitTransform = null;
            }
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
