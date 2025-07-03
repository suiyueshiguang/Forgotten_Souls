using UnityEngine;

public class UI_PlayerUpgrade : MonoBehaviour
{
    [SerializeField] private string campfireBoxFullPath;

    private RectTransform campfireBox;

    private bool isPlayerInExtent;
    private bool displayPlayerUpgrade;
    
    private void Start()
    {
        campfireBox = ServiceLocator.GetService<IDontDestroyManager>().GetSceneData<RectTransform>(campfireBoxFullPath);
    }

    private void Update()
    {
        if (isPlayerInExtent && Input.GetButtonDown("Interactive"))
        {
            displayPlayerUpgrade = !displayPlayerUpgrade;
            campfireBox.gameObject.SetActive(displayPlayerUpgrade);

            ServiceLocator.GetService<IGameManager>().PauseGame(displayPlayerUpgrade);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            isPlayerInExtent = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            isPlayerInExtent = false;

            campfireBox.gameObject.SetActive(false);
        }
    }
}
