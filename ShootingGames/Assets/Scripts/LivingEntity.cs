using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;
    protected virtual void Start()
    {
        health = startingHealth;
    }
    public virtual void TakeHit(float damage, Vector3 hitPoint,Vector3 hitDirection)
    {
        //lam` 1 so thu khac voi hit sau nay`
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }


    [ContextMenu("Self Destruct")]
    protected void Die()
    {
        dead = true;
        if(OnDeath != null){
            //ham` nay` se~ duoc. goi. khi nao` ke? thu` chet' thong qua phuong thuc OnEnemyDeath() ben Spawner
            OnDeath();
        }
        //pha huy doi' tuong.
        GameObject.Destroy(gameObject);
    }
}
