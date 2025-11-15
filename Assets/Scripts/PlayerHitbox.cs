using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("zombie"))
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
                enemy.ReceiveHit(damage);
        }
    }
}
