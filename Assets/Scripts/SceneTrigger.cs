using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ServiceLocator.GetService<ISceneLoaderManager>().LoadScene(gameObject.name);
        }
    }
}
