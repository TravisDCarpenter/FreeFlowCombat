using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCombat : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackCooldown = 0.7f;
    [SerializeField] private float raycastDistance = 1f;
    [SerializeField] private float moveSpeed = 0.6f;
    [SerializeField] private Vector3 raycastOffset = Vector3.zero;

    [Header("Effects")]
    [SerializeField] private GameObject shockwaveEffect;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private AudioClip[] punchSounds;

    [Header("State")]
    public bool isAttacking = false;

    private ThirdPersonController playerController;
    private Animator animator;
    private StarterAssetsInputs input;
    private Camera mainCam;

    private Vector3 inputDirection;
    private Transform currentTarget;
    private string[] attacks;
    private string attackString;
    private int animationCount = 0;
    private Coroutine attackCoroutine;

    private void Start()
    {
        playerController = GetComponent<ThirdPersonController>();
        input = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
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

        if (Input.GetMouseButtonDown(0))
        {
            Attack(currentTarget);
        }
    }

    private void Attack(Transform target)
    {
        attacks = new string[] { "FlipKick", "CrossPunch", "FlyingKick", "FlyingPunch" };

        if (target == null)
        {
            AttackType("CrossPunch", 0.2f, null, 0f);
            return;
        }

        animationCount = (int)Mathf.Repeat((float)animationCount + 1, (float)attacks.Length);
        attackString = attacks[animationCount];
        AttackType(attackString, attackCooldown, target, moveSpeed);
    }

    private void AttackType(string trigger, float cooldown, Transform target, float moveDuration)
    {
        animator.SetTrigger(trigger);

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = StartCoroutine(AttackCoroutine(cooldown));

        if (target == null) return;

        MoveToTarget(target, moveDuration);

        IEnumerator AttackCoroutine(float duration)
        {
            isAttacking = true;
            playerController.canMove = false;
            yield return new WaitForSeconds(duration);
            isAttacking = false;
            yield return new WaitForSeconds(0.25f);
            playerController.canMove = true;
        }
    }

    public void OnAnimationAttack()
    {
        Ray ray = new Ray(transform.position + raycastOffset, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, raycastDistance, enemyLayer))
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                Instantiate(shockwaveEffect, hit.point, Quaternion.identity);
                Instantiate(hitParticles, hit.point, Quaternion.identity);

                AudioClip randomClip = punchSounds[Random.Range(0, punchSounds.Length)];
                AudioSource.PlayClipAtPoint(randomClip, hit.point);

                //for testing
                Invoke(nameof(PauseForAction), 0.1f);

                enemy.OnHitReaction(attackString);
            }
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

    private void PauseForAction()
    {
        Debug.Break();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + raycastOffset, transform.forward * raycastDistance);
    }
}
