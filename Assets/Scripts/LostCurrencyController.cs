using UnityEngine;

public class LostCurrencyController : MonoBehaviour
{
    public int currency;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ServiceLocator.GetService<IPlayerManager>().currency += currency;
            Destroy(gameObject);
        }
    }
}
