using UnityEngine;

//������Ϊ����������������岻���ļ̳У�����bug
public class IceAndFire_Controller : ThunderStrike_Controller
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
