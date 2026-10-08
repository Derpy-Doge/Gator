using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

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

    public float attackRadius = .5f;
    public float attackRange = .3f;
    public float yOffset;
    private bool isAttacking;
    private bool canAttack = true;
    public float attackCooldown = 3f;

    [Space(5f)]
    public int pointsOnDeath;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

        if(hitInfo.collider && canAttack)
        {
            StartCoroutine(Attack());
        }

        #region movement
        playrPos = player.transform.position;
        pos = transform.position;

        Vector2 dir = (playrPos - pos).normalized;
        if (isAttacking)
        {
            rb.linearVelocityX = 0;
            //Debug.Log("gurt yo");
        }
        else
        {
            rb.linearVelocityX = dir.x * speed;
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

    IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;

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
        yield return new WaitForSeconds(.5f); // replace this with wait for anim logic
        isAttacking = false;
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 offset = new Vector3(0, yOffset);
        Gizmos.DrawWireSphere((transform.position + offset) + -transform.right * attackRange, attackRadius);
    }
}
