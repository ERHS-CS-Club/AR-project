using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    Block block;
    bool occupied = false;

    void Start()
    {
        block = transform.parent.GetComponent<Block>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!occupied && other.transform.CompareTag("Block") && other.transform != block.transform)
        {
            Block otherBlock = other.GetComponent<Block>();
            if(!otherBlock.otherSnapPoint || Vector3.Distance(otherBlock.otherSnapPoint.transform.position, otherBlock.transform.position) > Vector3.Distance(transform.position, otherBlock.transform.position))
            {
                otherBlock.otherSnapPoint = this;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Block"))
        {
            Block otherBlock = other.GetComponent<Block>();
            if (otherBlock.otherSnapPoint == this)
            {
                block.otherSnapPoint = null;
                occupied = false;
            }
        }
    }

    public void SnapBlock()
    {
        occupied = true;
    }
}
