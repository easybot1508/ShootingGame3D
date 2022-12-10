using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var gameObj = gameObject.transform.position;
        
        gameObj = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);


    }
}
