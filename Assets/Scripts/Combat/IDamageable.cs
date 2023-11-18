using UnityEngine;
public interface IDamageable : IHitable
{
    void TakeDamage(Vector2 damageSourceDir, int damageAmount, float knockBackThrust);

}