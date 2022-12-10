using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    public LayerMask collisionMark;
    public Color trailColor;
    [SerializeField] float damage = 1f;
    float lifeTimeBullet = 3f;
    float skinWidth = 0.1f;

    private void Start()
    {
        Destroy(gameObject, lifeTimeBullet);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMark);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0],transform.position);
        }
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //tao. 1 chu' nang phat' hien. va cham.
        //b1: tinh' toan' khoang? cach' di chuyen?
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        //lam` cho duong` dan. bay ve` phia' truoc'
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance)
    {
        //tao. 1 tia ray theo vi. tri' cua? duong` dan. va` huong' cua? duong` dan.
        Ray ray = new Ray(transform.position, transform.forward);
        //Lay' thong tin cua? doi' tuong. va cham.
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,moveDistance + skinWidth,collisionMark,QueryTriggerInteraction.Collide))
        {
            //neu' va cham. 1 cai' gi` do' thi` goi.
            OnHitObject(hit.collider, hit.point);
        }
    }

 

    void OnHitObject(Collider c,Vector3 hitPoint)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage,hitPoint,transform.forward);
        }
        //pha' huy? duong` dan. khi no' va cham.
        GameObject.Destroy(gameObject);
    }
}
