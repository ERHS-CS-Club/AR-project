using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    Block block;

    void Start()
    {
        block = transform.parent.GetComponent<Block>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Block") && other.transform != block.transform)
        {
            other.GetComponent<Block>().otherSnapPoint = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Block"))
        {
            Block otherBlock = other.GetComponent<Block>();
            if (otherBlock.otherSnapPoint == transform)
            {
                block.otherSnapPoint = null;
            }
        }
    }
}
