using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockStrike_Controller : MonoBehaviour
{
    [SerializeField] private float speed;
    private CharacterStats targetStats;
    private float damage;

    private Animator animator;
    private bool triggered;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Setup(float _damage, CharacterStats _targetStats)
    {
        damage = _damage;
        targetStats = _targetStats;
    }

    // Update is called once per frame
    void Update()
    {
        if(!targetStats)
        {
            return;
        }

        if (triggered)
        {
            return;
        }

        if(targetStats.isDead)
        {
            Destroy(gameObject);
        }

        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);

        transform.right = transform.position - targetStats.transform.position;

        if (Vector2.Distance(transform.position, targetStats.transform.position) < 0.1f)
        {
            //取消旋转并加大比例
            animator.transform.localRotation = Quaternion.identity;
            animator.transform.localPosition = new Vector3(0, 0.12f);
            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);


            Invoke("DamageAndSelfDestroy", .2f);

            triggered = true;
            animator.SetTrigger("Hit");
        }
    }

    private void DamageAndSelfDestroy()
    {
        targetStats.ApplyShock(true);
        targetStats.TakeDamage(damage);
        Destroy(gameObject, 0.4f);
    }
}
