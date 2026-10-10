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
    public float damage;
    public float damageMult; // punches do base and everything else does base * damageMult
    public float knockback;
    [Space(7f)]
    public int pointsOnHit;
}

public class SpawnHitbox : MonoBehaviour
{
    public LayerMask attackLayer;
    public bool isAttacking;
    public float punchCooldown = 2f;
    public bool canPunch = true;

    [Tooltip("SAME ORDER AS THE LIST IN INPUT READER I BEG")]public List<Attack> attacks = new List<Attack>();

    [Space(7f)]
    public AudioManager audioManager;
    public AudioClip hit;

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
        RaycastHit2D hitInfo = Physics2D.CircleCast((transform.position + offset), attack.radius, transform.right, attack.range, attackLayer);

        audioManager.PlaySFX(hit);

        yield return WaitForAnimEnd(attack);

        if(hitInfo.collider != null)
        {
            if (hitInfo.collider.TryGetComponent(out Stats enemy))
            {
                enemy.gameObject.GetComponent<Enemy>().StopAllCoroutines();

                float stunTime = enemy.gameObject.GetComponent<Enemy>().hitstunTime;

                if (enemy.gameObject.GetComponent<Enemy>().stunned)
                {
                    enemy.gameObject.GetComponent<Enemy>().StopAllCoroutines();
                    enemy.gameObject.GetComponent<Enemy>().stunned = true;

                    if (attack.attackName == "punch")
                    {
                        stunTime = enemy.gameObject.GetComponent<Enemy>().hitstunTime * 1f;
                    }
                    else
                    {
                        stunTime = enemy.gameObject.GetComponent<Enemy>().hitstunTime * 1.2f;

                        float points = enemy.gameObject.GetComponent<Enemy>().pointsOnDeath;
                        points *= 1.2f;
                        float calcedPoints = Mathf.Round(points);
                        enemy.gameObject.GetComponent<Enemy>().pointsOnDeath = (int)calcedPoints;
                    }
                }

                float calculatedDamage = attack.damage - enemy.defense;
                enemy.healthCurrent -= calculatedDamage;               

                enemy.gameObject.GetComponent<Enemy>().StartCoroutine(enemy.gameObject.GetComponent<Enemy>().Hitstun(stunTime));

                Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                float knockbackForce = attack.knockback;
                hitInfo.collider.GetComponent<Rigidbody2D>().AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                Gurt score = GetComponent<Gurt>();
                score.AddScore(attack.pointsOnHit);
            }
        }



        isAttacking = false;
        displayHitbox = false;

        animator.SetBool("isAttacking", false);
        playr.controls.FindAction("Move").Enable();

        float cooldown = 0f;
        while (cooldown < punchCooldown)
        {
            canPunch = false;
            cooldown += Time.deltaTime;
            yield return null;
        }

        canPunch = true;
    }
    public IEnumerator WaitForAnimEnd(Attack attack)
    {
        float timeout;

        InputReader player = this.GetComponent<InputReader>();
        if (player?.animator == null)
            yield break;

        if(attack.attackName == "punch")
        {
            timeout = .5f;
        }
        else
        {
            timeout = .35f;
        }

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
            //var current = player.animator.GetCurrentAnimatorStateInfo(0);

            //if (current.shortNameHash != hash && !player.animator.IsInTransition(0))
            //    break;

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
