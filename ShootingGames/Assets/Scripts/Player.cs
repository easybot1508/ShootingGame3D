using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    [SerializeField] float moveSpeed = 5f;
    public CrossHairs crossHairs;
    Camera myCamera;
    PlayerController controller;
    GunController gunController;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        myCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Movement
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //Look input(Huong' nhin`)
        //Lya 1 tia chieu tu` camera theo huong' cua? con chuot.
        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
        //tia nay` se~ keo' dai` vo han. nen phai? chan. no bang` 1 mat. phang?
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);

        float rayDistance;
        //Neu dk o day dung thi` tia se~ bi. cat' o mat phang? san`
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            //ta se~ biet duoc khoang cach' tu` camera diem? ma` tia va cham. , rayDistance se~ tra? ve` toa. do.
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawRay(ray.origin, point, Color.red);
            controller.LookAt(point);
            crossHairs.transform.position = point;
            crossHairs.DetectTarget(ray);
            //neu khoang? cach' tu` diem? ban - cho vi. tri' cua? player ma` lon hon 1 
            if((new Vector2(point.x , point.z) - new Vector2(transform.position.x,transform.position.z)).sqrMagnitude < 1)
            {
                //nham' vao` diem? trung tam cua? mui~ sung' (cham' mau` xanh)
               gunController.Aim(point);
            }
           
        }

        //Thao tac' vu~ khi'
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        //neu chung' ta tha? chuot. 
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
    }
}
