using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private string areaSoundName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ServiceLocator.GetService<IAudioManager>().PlaySFX(areaSoundName, null);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ServiceLocator.GetService<IAudioManager>().StopSFXWithTime(areaSoundName);
        }
    }
}
