using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;

    private StarterAssetsInputs input;
    private Camera mainCam;
    private Vector3 inputDirection;
    private Transform currentTarget;

    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        mainCam = Camera.main;
    }

    private void Update()
    {
        var forward = mainCam.transform.forward;
        var right = mainCam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        inputDirection = forward * input.move.y + right * input.move.x;
        inputDirection.Normalize();

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 3f, inputDirection, out hit, 10f, enemyLayer))
        {
            currentTarget = hit.collider.transform;
        }

        if (Input.GetMouseButtonDown(0) && currentTarget != null)
        {
            MoveToTarget(currentTarget, .8f);
        }
    }

    private void MoveToTarget(Transform target, float duration)
    {
        transform.DOLookAt(target.position, 0.2f, AxisConstraint.Y);
        transform.DOMove(TargetOffset(currentTarget), duration);
    }

    private Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, 0.85f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, inputDirection);

        if (currentTarget)
        {
            Gizmos.DrawSphere(currentTarget.position, 0.5f);
        }
    }
}
