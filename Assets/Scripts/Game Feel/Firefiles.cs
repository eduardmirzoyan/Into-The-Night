using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefiles : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Vector3 position;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        FollowCamera();
    }

    private void FollowCamera()
    {
        position = cam.transform.position;
        position.z = 0f;
        transform.position = position;
    }
}
