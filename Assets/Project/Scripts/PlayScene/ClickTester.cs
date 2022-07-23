using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTester : MonoBehaviour
{
    [SerializeField] int lineNum = 0;
    public void BeginDrag()
    {
        Debug.Log("BeginDrag: " + lineNum);
    }

    public void Drop()
    {
        Debug.Log("Drop" + lineNum);
    }

    public void PointerDown()
    {
        Debug.Log("PointerDown" + lineNum);
    }
}