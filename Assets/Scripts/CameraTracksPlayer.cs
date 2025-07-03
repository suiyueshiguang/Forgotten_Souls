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

        //ע�⣺���������Ĭ��Ϊ0(ǰ���Լ�ŪĬ�����ȼ�Ϊ0)
        //���ﵥ���ļ�һ��Ϊ���õ�ǰ����������ȼ��������������
        camera.Priority++;
    }
}
