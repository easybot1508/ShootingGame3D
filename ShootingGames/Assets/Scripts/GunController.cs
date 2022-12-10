using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;
    public Gun startingGun;
    Gun equippedGun;

    private void Start()
    {
        if (startingGun != null)
        {
            EquipGun(startingGun);
        }
    }
    public void EquipGun(Gun gunToEquip)
    {
        //kiem? tra xem player da~ co san~ vu~ khi' hay chua neu' dang co' thi` hay~ pha huy? vu~ khi cu~ roi` moi trang bi. vu~ khi' moi'
        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        //trang bi vu~ khi moi'
        equippedGun = Instantiate(gunToEquip,weaponHold.transform.position,weaponHold.rotation) as Gun;
        //chung ta can` dat no la con ,de doi' tuong. sung' di theo player
        equippedGun.transform.parent = weaponHold;
    }

    public void OnTriggerHold()
    {
        //Kiem? tra xem minh` co' vu~ khi' hay khong
        if(equippedGun != null)
        {
            equippedGun.OnTriggerHold();
        }
    }
    public void OnTriggerRelease()
    {
        //Kiem? tra xem minh` co' vu~ khi' hay khong
        if (equippedGun != null)
        {
            equippedGun.OnTriggerRelease();
        }
    }
    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }   

    public void Aim(Vector3 aimPoint)
    {
        //Kiem? tra xem minh` co' vu~ khi' hay khong
        if (equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
   }
}
