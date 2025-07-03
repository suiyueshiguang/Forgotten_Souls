using UnityEngine;

public class UI_SkillsList : MonoBehaviour
{
    [SerializeField] private GameObject initialMemu;

    private void Start()
    {
        SwitchSkill(initialMemu);
    }

    public void SwitchSkill(GameObject _memu)
    {
        for (int index = 0; index < transform.childCount; index++)
        {
            transform.GetChild(index).gameObject.SetActive(false);
        }

        if (_memu != null)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX("MenuConfirm", null);
            _memu.SetActive(true);
        }
    }
}
