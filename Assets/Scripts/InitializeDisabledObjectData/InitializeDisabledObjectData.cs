using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InitializeDisabledObjectData : MonoBehaviour
{
    private List<IInitializeDisabledObjectData> disableGameObejct;

    private void Awake()
    {
        disableGameObejct = FindAllInitiaDisObjectData();

        foreach (IInitializeDisabledObjectData gameObject in disableGameObejct)
        {
            gameObject.Initialization();
        }
    }

    private List<IInitializeDisabledObjectData> FindAllInitiaDisObjectData()
    {
        IEnumerable<IInitializeDisabledObjectData> disabledData = FindObjectsByType(typeof(MonoBehaviour), FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IInitializeDisabledObjectData>();

        return new List<IInitializeDisabledObjectData>(disabledData);
    }
}
