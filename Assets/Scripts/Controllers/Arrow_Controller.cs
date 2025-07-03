using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    private string targetLayerName = "Player";

    private float xVelocity;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private bool canMove;
    [SerializeField] private bool flipped;

    private CharacterStats myStats;

    private void Update()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(xVelocity, rb.velocity.y);
        }
    }

    public void SetupArrow(float _speed, CharacterStats _myStats)
    {
        if (_speed < 0)
        {
            transform.Rotate(0, 180, 0);
        }

        xVelocity = _speed;
        myStats = _myStats;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            myStats.DoDamage(collision.GetComponent<CharacterStats>());
            StuckInto(collision);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StuckInto(collision);
        }

        Destroy(gameObject, Random.Range(6, 9));
    }

    private void StuckInto(Collider2D collision)
    {
        //行为不会再更新
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponentInChildren<ParticleSystem>().Stop();

        canMove = false;
        //启用 isKinematic ,使 Forces、collision 或 joints 将不再影响刚体。
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.parent = collision.transform;
    }

    public void FlipArrow()
    {
        if (flipped)
        {
            return;
        }

        xVelocity = xVelocity * -1;
        flipped = true;
        transform.Rotate(0, 180, 0);
        targetLayerName = "Enemy";
    }
}
