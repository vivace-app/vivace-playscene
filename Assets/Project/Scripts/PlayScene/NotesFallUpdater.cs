using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesFallUpdater : MonoBehaviour
{
    public static float speed = 6f; // 1秒間に3m移動
    //public static bool isPose = true;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, -speed, 0);
    }

    void Update()
    {
        if (transform.position.y <= -5f)
            Destroy(this);
    }

    public float notesSpeed
    {
        set => speed = value;
    }

}