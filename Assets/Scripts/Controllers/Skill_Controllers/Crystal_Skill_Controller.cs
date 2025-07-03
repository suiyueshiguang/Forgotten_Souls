using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExiststTimer;
    private bool canExplode;
    private bool canMove;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5f;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget)
    {
        crystalExiststTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    public void ChooseRandomEnemy()
    {
        float radius = ServiceLocator.GetService<ISkillManager>().GetBlackhole().GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if(colliders.Length > 0)
        {
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
        }
    }

    private void Update()
    {
        crystalExiststTimer -= Time.deltaTime;

        if(crystalExiststTimer < 0)
        {
            FinishCrystal();
        }

        if(canMove && closestTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);
        
            if(Vector2.Distance(transform.position, closestTarget.position) < 0.875f)
            {
                FinishCrystal();

                canMove = false;
            }
            
        }

        if(canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                hit.GetComponent<Entity>().SetupKnowbackDir(transform);

                ServiceLocator.GetService<IPlayerManager>().GetPlayer().stats.DoMagicDamage(hit.GetComponent<CharacterStats>());

                ItemData_Equipment equipeAmulet = ServiceLocator.GetService<IInventory>().GetEquipment(EquipmentType.Amulet);

                if(equipeAmulet != null)
                {
                    equipeAmulet.Effect(hit.transform);
                }
            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestroy();
        }
    }

    public void SelfDestroy() => Destroy(gameObject);
}
