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
    float disatanceToPlayer;
    private Vector2 direction;

    [Space(7f)]
    [Header("Combat")]
    [Space(3f)]
    public LayerMask attackLayer;

    public float attackRadius = .5f;
    public float attackRange = .3f;
    private bool isAttacking = false;
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
        RaycastHit2D hitInfo = Physics2D.CircleCast(transform.position, attackRadius, direction * .4f, attackRange, attackLayer);

        if(hitInfo.collider && canAttack)
        {
            StartCoroutine(Attack());
        }

        #region movement
        playrPos = player.transform.position;
        pos = transform.position;

        disatanceToPlayer = Mathf.Sqrt(Mathf.Pow(playrPos.x - pos.x, 2));
        Vector2 dir = (playrPos - pos).normalized;

        rb.linearVelocityX = dir.x * speed;
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

        RaycastHit2D hitInfo = Physics2D.CircleCast(transform.position, attackRadius, direction * .4f, attackRange, attackLayer);
        if(hitInfo.collider.TryGetComponent(out Stats player))
        {
            float calculatedDamage = GetComponent<Stats>().attackPower - player.defense;
            player.healthCurrent -= calculatedDamage;
        }
        else
        {
            Debug.Log("yo you don have stats cuh");
        }
        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + new Vector3(direction.x, 0, 0) * attackRange, attackRadius);
    }
}
