using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerSpell_Controller : MonoBehaviour
{
    [SerializeField] private Transform check;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask whatIsPlayer;

    private CharacterStats mystats;

    public void SetupSpell(CharacterStats _stats) => mystats = _stats;

    private void AnimationTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(check.position, boxSize, 0, whatIsPlayer);

        foreach(var hit in colliders)
        {
            if(hit.GetComponent<Player>() != null)
            {
                hit.GetComponent<Entity>().SetupKnowbackDir(transform);
                mystats.DoDamage(hit.GetComponent<CharacterStats>());
            }
        }
    }

    private void OnDrawGizmos() => Gizmos.DrawWireCube(check.position, boxSize);

    private void SelfDestroy() => Destroy(gameObject);

}
