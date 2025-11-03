using UnityEngine;

public class AutoShootAnimation : MonoBehaviour
{
    public Animator animator;
    public float shootIntervalMin = 8f;
    public float shootIntervalMax = 12f;
    private float timer;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        timer = Random.Range(shootIntervalMin, shootIntervalMax);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            animator.SetTrigger("Shoot");
            
            timer = Random.Range(shootIntervalMin, shootIntervalMax);
        }
    }
}
