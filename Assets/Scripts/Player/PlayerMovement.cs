using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 12f;
    private bool canAttack = true;

    [Header("Damage Settings")]
    public float baseDamage = 20f;
    private float damageMultiplier = 1f;
    private bool isBoosted = false;

    [Header("Potion Buff Settings")]
    public float damageBoostAmount = 1.5f; // 50% more damage
    public float boostDuration = 10f;

    private CharacterController controller;
    private Animator animator;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 velocity;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Attack.performed += ctx => TryAttack();
    }

    void OnEnable() => controls.Player.Enable();
    void OnDisable() => controls.Player.Disable();

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MoveAndRotate();
        ApplyGravity();
    }

    void MoveAndRotate()
    {
        // Optional: lock movement during attack animation
        if (!canAttack) return;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (animator)
        {
            float speedPercent = move.magnitude;
            animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void TryAttack()
    {
        if (!canAttack) return;

        canAttack = false;
        animator.SetTrigger("Attack");
        SoundManager.Instance?.PlayShoot();

        // Fire bullet slightly after animation trigger (sync with animation)
        Invoke(nameof(FireBullet), 0.15f);
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void FireBullet()
    {
        if (firePoint == null || bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Set projectile damage before firing
        PlayerProjectile proj = bullet.GetComponent<PlayerProjectile>();
        if (proj != null)
            proj.damage = baseDamage * damageMultiplier;

        if (rb != null)
            rb.velocity = firePoint.forward * bulletSpeed;
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    // ===== POTION BUFF LOGIC =====
    public void ApplyDamageBoost()
    {
        if (isBoosted) return;
        StartCoroutine(DamageBoostRoutine());
    }

    private System.Collections.IEnumerator DamageBoostRoutine()
    {
        isBoosted = true;
        damageMultiplier = damageBoostAmount;
        Debug.Log("Damage boost active!");

        yield return new WaitForSeconds(boostDuration);

        damageMultiplier = 1f;
        isBoosted = false;
        Debug.Log("Damage boost ended!");
    }
}
