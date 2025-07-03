using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive_Controller : MonoBehaviour
{
    private Animator animator;
    private CharacterStats myStats;
    private float growSpeed = 15;
    private float maxSize = 6;
    private float explosionRadius;

    private bool canGrow;

    private void Update()
    {
        if(canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if(maxSize - transform.localScale.x < .5f)
        {
            canGrow = false;
        }
    }

    public void SetupExplosive(CharacterStats _myStats, float _growSpeed, float _maxSize, float _explosionRadius)
    {
        animator = GetComponent<Animator>();
        canGrow = true;

        myStats = _myStats;
        growSpeed = _growSpeed;
        maxSize = _maxSize;
        explosionRadius = _explosionRadius;
    }

    public void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<CharacterStats>() != null)
            {
                hit.GetComponent<Entity>().SetupKnowbackDir(transform);
                myStats.DoDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }
}
