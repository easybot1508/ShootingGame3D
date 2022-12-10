using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairs : MonoBehaviour
{
    public LayerMask layerMark;
    public SpriteRenderer dot;
    public Color dotHightLightColor;
    Color originalDotColor;

    private void Start()
    {
        //giau' con tro? chuot. khi chay. game
        Cursor.visible = false;
        originalDotColor = dot.color;

    }

    void Update()
    {
        
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
    }

    public void DetectTarget(Ray ray)
    {
        if (Physics.Raycast(ray, 100, layerMark))
        {
            dot.color = dotHightLightColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}
