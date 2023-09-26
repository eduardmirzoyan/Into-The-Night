using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantHead : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform baseTransform;

    [SerializeField] private float travelSpeed = 3f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float padding = 2f;
    [SerializeField] private float maxDistance = 6f;

    private void Update()
    {
        if (targetTransform != null)
        {
            MaintainDistance();
            FaceTarget();
        }
        else
        {
            ReturnHome();
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    private void MaintainDistance()
    {
        Vector3 direction = targetTransform.position - baseTransform.position;
        direction.Normalize();

        float distance = Vector3.Distance(baseTransform.position, targetTransform.position);
        distance = Mathf.Min(distance, maxDistance);

        Vector3 targetPosition = baseTransform.position + direction * (distance - padding);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, travelSpeed * Time.deltaTime);
    }

    private void FaceTarget()
    {
        // Rotate over time
        Vector3 direction = targetTransform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private void ReturnHome()
    {
        // Rotate over time
        Vector3 direction = Vector2.up;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        // Return to base
        transform.position = Vector3.MoveTowards(transform.position, baseTransform.position + Vector3.up * 0.5f, travelSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
