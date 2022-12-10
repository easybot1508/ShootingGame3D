using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
    public void LookAt(Vector3 lookPoint)
    {
        //giu~ nguyen vi. tri' cua? truc. y
        Vector3 heightCorrectPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        //player se~ huong' ve` phia' con tro? chuot.
        transform.LookAt(heightCorrectPoint);
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }
}
