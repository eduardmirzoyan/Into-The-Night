using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineConfiner2D confiner;

    public static CameraHandler instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        // Set follow target to player
        virtualCamera.m_Follow = PlayerController.instance.transform;

        // Set boundary based on tilemap
        confiner.m_BoundingShape2D = LeverToggleTilemap.instance.GetBoundingCollider();
    }
}
