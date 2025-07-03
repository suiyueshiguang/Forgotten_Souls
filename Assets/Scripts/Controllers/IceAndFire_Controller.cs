using UnityEngine;

//个人认为这里是无意义或意义不明的继承，属于bug
public class IceAndFire_Controller : ThunderStrike_Controller
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
