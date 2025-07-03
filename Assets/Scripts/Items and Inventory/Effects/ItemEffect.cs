using UnityEngine;

public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    protected PlayerStats playerStats => ServiceLocator.GetService<IPlayerManager>().GetPlayer().GetComponent<PlayerStats>();

    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
    }
}
