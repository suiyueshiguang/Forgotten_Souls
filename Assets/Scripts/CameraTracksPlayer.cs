using Cinemachine;
using UnityEngine;

public class CameraTracksPlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        CinemachineVirtualCamera camera = GetComponent<CinemachineVirtualCamera>();

        player = ServiceLocator.GetService<IPlayerManager>()?.GetPlayer();

        if(player != null)
        {
            camera.Follow = player.transform;
        }

        //注意：这里的由于默认为0(前面自己弄默认优先级为0)
        //这里单纯的加一是为了让当前的摄像机优先级比其他摄像机高
        camera.Priority++;
    }
}
