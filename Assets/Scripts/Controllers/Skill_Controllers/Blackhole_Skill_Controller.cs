using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float blackholeTimer;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;
    private bool playerCanDisapea = true;

    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    //控制Collision，防止在极短时间内形成看似同一个对象进入黑洞多次
    int previousCollisionID = 0;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotkey = new List<GameObject>();

    private Player player => ServiceLocator.GetService<IPlayerManager>().GetPlayer();

    public bool playerCanExitState { get; private set; }

    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _amountOfAttacks, float _cloneAttackCooldown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttacks;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _blackholeDuration;

        if (ServiceLocator.GetService<ISkillManager>().GetClone().crystalInseadOfClone)
        {
            playerCanDisapea = false;
        }
    }

    public void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        player.SetZeroVelocity();

        if(blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;

            if(targets.Count > 0)
            {
                ReleaseCloneAttack();
            }
            else
            {
                FinishBlackHoleAbility();
            }
        }

        if (Input.GetButtonDown("Skill_BlackHole"))
        {
            ReleaseCloneAttack();
        }

        CloneAttackLogic();

        //扩展
        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        //收缩
        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void ReleaseCloneAttack()
    {
        if(targets.Count <= 0)
        {
            return;
        }

        DestroyHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;

        if(playerCanDisapea)
        {
            playerCanDisapea = false;
            player.fx.MakeTransprent(true);
        }
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased && amountOfAttacks > 0)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);
            float xOffset;

            if (Random.Range(0, 100) > 50)
            {
                xOffset = 1.25f;
            }
            else
            {
                xOffset = -1.25f;
            }

            if (ServiceLocator.GetService<ISkillManager>().GetClone().crystalInseadOfClone)
            {
                ServiceLocator.GetService<ISkillManager>().GetCrystal().CreateCrystal();
                ServiceLocator.GetService<ISkillManager>().GetCrystal().CurrentCrystalChooseRandomTarget();
            }
            else
            {
                if (targets[randomIndex] != null)
                {
                    ServiceLocator.GetService<ISkillManager>().GetClone().CreateClone(targets[randomIndex], new Vector3(xOffset, 0, 0));
                }
            }

            amountOfAttacks--;

            //黑洞结束后动作
            if (amountOfAttacks <= 0)
            {
                Invoke("FinishBlackHoleAbility", 1.5f);
            }
        }
    }

    private void FinishBlackHoleAbility()
    {
        playerCanExitState = true;
        canShrink = true;
        cloneAttackReleased = false;

        DestroyHotKeys();
    }

    private void DestroyHotKeys()
    {
        if(createdHotkey.Count <= 0)
        {
            return;
        }

        for (int index = 0; index < createdHotkey.Count; index++)
        {
            Destroy(createdHotkey[index]);
        }

        //清空
        createdHotkey.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null && previousCollisionID != collision.GetInstanceID())
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);

            previousCollisionID = collision.GetInstanceID();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.Log("没有了足够的快捷键");
            return;
        }

        if(!canCreateHotKeys)
        {
            return;
        }

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);

        createdHotkey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_HotKey_Controller newHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();

        newHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);
}
