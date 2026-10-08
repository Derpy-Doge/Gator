using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Attack
{
    public string attackName;
    [Space(7f)]
    public float radius;
    public float range;
    [Space(5f)]
    public float yOffset;
    [Space(7f)]
    public float damageMult; // punches do base and everything else does base * damageMult
    public float knockback;
    [Space(7f)]
    public int pointsOnHit;
}

public class SpawnHitbox : MonoBehaviour
{
    public LayerMask attackLayer;
    public bool isAttacking;


    [Tooltip("SAME ORDER AS THE LIST IN INPUT READER I BEG")]public List<Attack> attacks = new List<Attack>();
    Stats player;

    private bool displayHitbox = false;

    void Start()
    {
        player = GetComponent<Stats>();
    }

    
    void Update()
    {
        
    }

    public IEnumerator Hitbox(Attack attack)
    {
        isAttacking = true;

        displayHitbox = true;

        Animator animator = player.gameObject.GetComponent<Animator>();
        InputReader playr = player.gameObject.GetComponent<InputReader>();

        animator.SetBool("isAttacking", true);
        playr.controls.FindAction("Move").Disable();

        Vector3 offset = new Vector3(0, attack.yOffset);
        yield return WaitForAnimEnd(attack);
        RaycastHit2D hitInfo = Physics2D.CircleCast((transform.position + offset), attack.radius, transform.right, attack.range, attackLayer);
        if(hitInfo.collider != null)
        {
            if (hitInfo.collider.TryGetComponent(out Stats enemy))
            {
                float calculatedDamage = (player.attackPower * attack.damageMult) - enemy.defense;
                enemy.healthCurrent -= calculatedDamage;

                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                float knockbackForce = attack.knockback;
                hitInfo.collider.GetComponent<Rigidbody2D>().AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                Debug.Log("Hit " + hitInfo.collider.name + " with " + attack.attackName);

                Gurt score = GetComponent<Gurt>();
                score.AddScore(attack.pointsOnHit);
            }
        }



        isAttacking = false;
        displayHitbox = false;

        animator.SetBool("isAttacking", false);
        playr.controls.FindAction("Move").Enable();
        yield return null;
    }
    public IEnumerator WaitForAnimEnd(Attack attack, float timeout = .5f)
    {
        InputReader player = this.GetComponent<InputReader>();
        if (player?.animator == null)
            yield break;

        player.animator.SetTrigger(attack.attackName);      
        player.animator.Update(0f);

        int hash = Animator.StringToHash(attack.attackName);

        // wait for anim to start (or transition to it) up to timeout

        player.animator.CrossFade(attack.attackName, 0f, 0);

        //wait for anim to finish
        float timer = 0f;   
        while (timer < timeout)
        {
            yield return new WaitForEndOfFrame();
            var current = player.animator.GetCurrentAnimatorStateInfo(0);

            if (current.shortNameHash != hash && !player.animator.IsInTransition(0))
                break;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = new Vector3(0, attacks[0].yOffset);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((transform.position + offset) + transform.right * attacks[0].range, attacks[0].radius);


        if (!displayHitbox) return;

        for(int i = 0; i < attacks.Count; i++)
        {
            if(isAttacking)
            {
                Vector3 offset2 = new Vector3(0, attacks[i].yOffset);
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere((transform.position + offset2) + transform.right * attacks[i].range, attacks[i].radius);
            }
        }       
    }
}
