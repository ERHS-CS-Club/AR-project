using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackedImage : MonoBehaviour
{
    [SerializeField] float thickness = 0.1f;

    ARTrackedImage aRTrackedImage;

    // Start is called before the first frame update
    void Start()
    {
        aRTrackedImage = GetComponent<ARTrackedImage>();
        transform.localScale = new Vector3(aRTrackedImage.size.x, thickness, aRTrackedImage.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
