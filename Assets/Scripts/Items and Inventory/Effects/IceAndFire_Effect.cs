using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice and fire effect", menuName = "����/��ƷЧ��/��������")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _responePosition)
    {
        Player player = ServiceLocator.GetService<IPlayerManager>().GetPlayer();

        //������ƣ����ص�ǰ���������public����
        PropertyInfo[] properties = player.playerStateFactory.playerState.GetType().GetProperties();

        int comboCounter = 0;
        foreach (PropertyInfo property in properties)
        {
            if (property.Name == "comboCounter")
            {
                comboCounter = (int)property.GetValue(player);
            }
        }

        bool thirdAttack = (comboCounter == 2);

        if (thirdAttack)
        {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _responePosition.position, player.transform.rotation);

            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);

            Destroy(newIceAndFire, 5f);
        }
    }
}
