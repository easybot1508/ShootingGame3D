using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State {Idle,Chasing,Attacking};//trang thai' cua? ke? thu`
    State currentState;

    NavMeshAgent pathfinder;
    //lay vi. tri' cua? nguoi` choi de? theo doi~
    Transform target;
    Material skinMaterial;
    Color originalColor;

    //luu tru~ hieu. ung' tu? vong
    public ParticleSystem deathEffect;

    LivingEntity targetEntity;

    float attackDistanceThreshold = 0.5f;
    float timeBetweenAttack = 1f;
    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;
    float damage = 1f;

   bool hasTarget;//kiem tra xem muc. tieu con` ton` tai. hay khong

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        //Neu muc. tieu co the? la` Player con` ton` tai. thi` chay. ma~ ben trong
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetEntity = target.GetComponent<LivingEntity>();

        }
    }

    // Start is called before the first frame update
    protected override void  Start()
    {
        base.Start();
       
        //Neu muc. tieu co the? la` Player con` ton` tai. thi` chay. ma~ ben trong
        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;
            StartCoroutine(UpdatePath());
        }
        
    }
    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;
        //Neu ke? thu` tim` thay muc. tieu la` nguoi` choi 
        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
        }
        startingHealth = enemyHealth;
        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColor;
        originalColor = skinMaterial.color;


    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        //tao. hieu. ung' tu? vong
        if(damage >= health)
        {
            //sau khi khoi? tao. hieu. ung' chung ta se~ pha huy? foi' tuong. hieu. ung' sau 2s
           Destroy( Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                //xem khoang? cach giua~ player va` enemy co' nho? hon nguong~ khoang? cach' hay khong
                //khoang? cach den' muc. tieu
                float sqrDistanceToTaget = (target.position - transform.position).sqrMagnitude;
                //so sanh' khoang? cach den' muc. tieu < nguong~ khoang? cach' tan cong
                //Mathf.Pow(attackDistanceThreshold,2)) : return attackDistanceThreshold luy~ thua` 2
                //Neu Khoang? cach du? gan` 
                if (sqrDistanceToTaget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttack;
                    StartCoroutine(Attack());
                }
            }
        }
       
    }

    IEnumerator Attack()
    {
       
        //khi cuoc. tan' cong bat' dau`
        currentState = State.Attacking;
        pathfinder.enabled = false;
        //tao. hoat. anh? dong. cho cu' dam cua? enemy
        //luc' dau` ta se~ o vi. tri' hien. tai. (originalPosition),sau do se~ chuyen? den' vi. tri' tan' cong, sau do' quay lai. vi. tri'
        //ban dau`, gia tri. se~ chuyen? tu` 0 den' 1 va` tu` 1 ve` 0
        Vector3 originalPosition = transform.position;
        //Vecto huong den muc. tieu
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        //xac' dinh. vi. tri' cua? muc. tieu, tinh' toan' ban' kinh' pham. vi va cham.
        Vector3 attackPosition = target.position - directionToTarget * (myCollisionRadius);
        
        float attackSpeed = 3;//o? day gia' tri. cang` cao thi` hoat. anh? cang` nhanh  
        float percent = 0;
         
        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;
        //neu dieu` kien. nay` dung' thi` se~ thuc. hien. hanh` dong. dam
        while (percent <= 1)
        {
            //neu nhu % lon' gon 1/2 va` chua chiu. thie. hai.
            if(percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            //gia' tri. noi. suy , tim` hieu?
            float interpolation = (Mathf.Pow(-percent,2) + percent) * 4;//gia' tri. noi. suy
            //Vector3.Lerp: tra? ve` diem? phan chia ben trong giua~ 2 vector duoi dang. gia' 0 va` 1 
            //neu gia tri. noi. suy la` 0, chung' toi se~ o vi. tri' ban dau`(originalPosition)
            //neu gia tri. noi. suy la` 1, chung' toi se~ o vi. tri' ban tan cong(attackPosition)
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;//dieu` nay` bo? qua cac' khung hinh` giua~ moi~ lan` xu? ly' cua? vong` lap. while 
        }

        //khi cuoc. tan cong ket' thuc'
        currentState = State.Chasing;
        skinMaterial.color = originalColor;
        //khong muon' enemy tiep' tuc. duoi? theo nguoi` choi
        pathfinder.enabled = true;
    }

    //SetDestination: kich' hoat. tinh' toan' cho tuyen' duong` di moi' cho moi~ khung nhung dieu` nay` se~ doi` hoi? viec. xu? ly' rat' ton' kem'
    // vi` se~ co' nhieu` enemy  va` chuong' ngai. vat.
    //thay vi` cap. nhat. moi. khung hinh` thi` ta se~ tao. ra 1 bo. dem time 
    IEnumerator UpdatePath()
    {
        //khi UpdatePath duoc goi. 1 lan`
        //khong? time lam` moi' = 0.25s , o? day no se~ tinh' toan' lai. duong` di 4 lan` trong 1s
        float refreshRate = 0.25f;

        
        //vong` lap. while nay` se~ lap. di lap. lai. sau 1s
        //neu nhu muc. tieu van~ con` ton` tai.
        while(hasTarget)
        {
            if (currentState == State.Chasing)
            {
                //Vecto huong den muc. tieu
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                //xac' dinh. vi. tri' cua? muc. tieu, tinh' toan' ban' kinh' pham. vi va cham.
                Vector3 targetPosition = target.position - directionToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);
                if (!dead)
                {
                    //target theo vi. tri' cua? player
                    pathfinder.SetDestination(targetPosition);
                }
            }
           
            yield return new WaitForSeconds(refreshRate);
        }        
    }
}
