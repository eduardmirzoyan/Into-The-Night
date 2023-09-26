using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    [SerializeField] private PlantHead plantHead;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private float aggroRange = 15f;

    private void Awake()
    {
        plantHead = GetComponentInChildren<PlantHead>();
    }

    private void Start()
    {
        playerTransform = PlayerController.instance.transform;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) < aggroRange)
        {
            plantHead.SetTarget(playerTransform);
        }
        else
        {
            plantHead.SetTarget(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
