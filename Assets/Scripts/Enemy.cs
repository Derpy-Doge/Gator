using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [Space(3f)]
    public float speed;
    private Transform player;
    private Rigidbody2D rb;

    Vector2 pos;
    Vector2 playrPos;
    private Vector2 direction;

    [Space(7f)]
    [Header("Combat")]
    [Space(3f)]
    public LayerMask attackLayer;
    [Space(5f)]
    public float attackRadius = .5f;
    public float attackRange = .3f;
    public float yOffset;
    private bool isAttacking;
    private bool canAttack = true;
    public float attackCooldown = 3f;
    public float baseCooldown = 3f;
    [Space(5f)]
    public float hitstunTime = .25f; 
    public bool stunned = false;

    [Space(5f)]
    public int pointsOnDeath;

    [Space(7f)]
    [Header("Animator")]

    Animator animator;

    [Space(7f)]
    [Header("Healthbar")]
    public Image healthBar;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    void Start()
    {
        GameObject playr = GameObject.FindGameObjectWithTag("Player");
        if(playr != null)
        {
            player = playr.transform;
        }
    }

    
    void Update()
    {
        Vector3 offset = new Vector3(0, yOffset);
        RaycastHit2D hitInfo = Physics2D.CircleCast((transform.position + offset), attackRadius, direction * .4f, attackRange, attackLayer);

        if(hitInfo.collider && canAttack && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        #region movement
        playrPos = player.transform.position;
        pos = transform.position;

        Vector2 dir = (playrPos - pos).normalized;
        if (isAttacking || stunned)
        {
            rb.linearVelocityX = 0;
        }
        else
        {
            rb.linearVelocityX = dir.x * speed;
        }       
        #endregion

        #region healthbar
        Stats stats = GetComponent<Stats>();
        if (stats != null)
        {
            healthBar.fillAmount = stats.healthCurrent / stats.healthMax;
        }
        #endregion

        #region direction

        Debug.DrawRay(transform.position, direction * 1.3f, Color.purple);

        if (rb.linearVelocity.magnitude > 0.001f)
        {
            direction = new Vector2(rb.linearVelocity.x, 0);
        }
        #endregion
    }

    public IEnumerator Hitstun(float stunTime)
    {
        stunned = true;
        //yield return WaitForAnimEnd("hurt");
        animator.Play("hurt");
        yield return new WaitForSeconds(stunTime);
        yield return new WaitForEndOfFrame();
        Debug.Log("sodium");
        stunned = false;
        isAttacking = false;
        animator.Play("walk");
        yield return new WaitForSeconds(attackCooldown/2);
        canAttack = true;
    }

    public IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;

        PlayIdle();

        yield return new WaitForSeconds(.1f);

        yield return WaitForAnimEnd("punch");
        Vector3 offset = new Vector3(0, yOffset);
        RaycastHit2D hitInfo = Physics2D.CircleCast((transform.position + offset), attackRadius, direction * .4f, attackRange, attackLayer);
        if(hitInfo.collider != null)
        {

            if (hitInfo.collider.TryGetComponent(out Stats player))
            {
                float calculatedDamage = GetComponent<Stats>().attackPower - player.defense;
                player.healthCurrent -= calculatedDamage;
            }
            else
            {
                Debug.Log("yo you don have stats cuh");
            }
        }
        isAttacking = false;

        if (!stunned)
        {
            animator.Play("walk");
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public IEnumerator WaitForAnimEnd(string anim)
    {
        float timeout;

        if (animator == null)
        {
            yield break;
        }

        if (anim == "punch")
        {
            timeout = .7f;
        }
        else
        {
            timeout = hitstunTime;
        }

        animator.SetTrigger(anim);
        animator.Update(0f);

        int hash = Animator.StringToHash(anim);

        animator.CrossFade(anim, 0f, 0);

        //wait for anim to finish
        float timer = 0f;
        while (timer < timeout)
        {
            yield return new WaitForEndOfFrame();
            var current = animator.GetCurrentAnimatorStateInfo(0);

            if (current.shortNameHash != hash && !animator.IsInTransition(0))
                break;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void PlayIdle()
    {
        if (animator != null)
        {
            animator.Play("idle");
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 offset = new Vector3(0, yOffset);
        Gizmos.DrawWireSphere((transform.position + offset) + -transform.right * attackRange, attackRadius);
    }
}
