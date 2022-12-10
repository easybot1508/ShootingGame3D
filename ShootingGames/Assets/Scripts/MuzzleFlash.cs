using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;

    public float flashTime;
    private void Start()
    {
        Deactivate();
    }
    public void Activate()
    {
        flashHolder.SetActive(true);
        int flashSpriteIndex = Random.Range(0, flashSprites.Length);
        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }
        //sau khi method nay` dc goi. thi` huy? kich hoat phuong thuc Deactivate() sau khoang? flashTime
        Invoke("Deactivate", flashTime);
    }

    void Deactivate()
    {
        flashHolder.SetActive(false);
    }
}
