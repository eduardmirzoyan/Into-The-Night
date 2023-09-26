using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlantStem : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform baseTransform;
    [SerializeField] private Vector3 baseOffset;

    [Header("Settings")]
    [SerializeField] private float length;
    [SerializeField] private int numPoints;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private Vector3[] segmentPositions;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        segmentPositions = new Vector3[numPoints];
    }

    private void Update()
    {
        var control = GetIntersectionPoint(baseTransform.position, Vector3.up, headTransform.position, Vector3.right);
        control = Vector3.ClampMagnitude(control, length);

        for (int i = 0; i < segmentPositions.Length; i++)
        {
            segmentPositions[i] = GetBezierPoint(headTransform.position, control, baseTransform.position + baseOffset, (float)i / numPoints);
        }

        lineRenderer.positionCount = numPoints;
        lineRenderer.SetPositions(segmentPositions);
    }


    private Vector3 GetBezierPoint(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, float t)
    {
        Vector3 ac = Vector3.Lerp(startPoint, controlPoint, t);
        Vector3 cb = Vector3.Lerp(controlPoint, endPoint, t);
        return Vector3.Lerp(ac, cb, t);
    }

    Vector3 GetIntersectionPoint(Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2)
    {
        // Calculate the cross product of the two direction vectors
        Vector3 cross = Vector3.Cross(dir1, dir2);

        // If the cross product is close to zero, the vectors are parallel (no intersection)
        if (cross.sqrMagnitude < 0.0001f)
        {
            return Vector3.zero;
        }

        // Calculate the vector from the start of the first vector to the start of the second vector
        Vector3 delta = start2 - start1;

        // Calculate the scalar values for both vectors
        float scalar1 = Vector3.Dot(delta, Vector3.Cross(dir2, cross)) / cross.sqrMagnitude;
        float scalar2 = Vector3.Dot(delta, Vector3.Cross(dir1, cross)) / cross.sqrMagnitude;

        // Calculate the intersection point
        Vector3 intersectionPoint = start1 + dir1 * scalar1;

        return intersectionPoint;
    }


    private void OnDrawGizmosSelected()
    {
        var control = GetIntersectionPoint(baseTransform.position, Vector3.up, headTransform.position, -headTransform.right);
        control = Vector3.ClampMagnitude(control, length);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(headTransform.position, -headTransform.right);
        Gizmos.DrawRay(baseTransform.position, Vector3.up);
        Gizmos.DrawWireSphere(control, 0.2f);

        for (int i = 0; i < numPoints - 1; i++)
        {
            var point = GetBezierPoint(headTransform.position, control, baseTransform.position + baseOffset, (float)i / numPoints);
            print($"tes?{point}");
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(point, 0.2f);
        }
    }
}
