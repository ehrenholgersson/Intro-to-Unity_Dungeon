using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    Rigidbody rb;
    bool grounded;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            rb.angularVelocity = new Vector3(0, -5, 0);
        else if (Input.GetKey(KeyCode.RightArrow))
            rb.angularVelocity = new Vector3(0, 5, 0);
        else
            rb.angularVelocity = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.UpArrow) && grounded && Vector3.Project(rb.velocity,transform.forward).magnitude< 5)
            rb.AddForce(transform.forward*5000*Time.deltaTime);
        if (Input.GetKey(KeyCode.DownArrow) && grounded && Vector3.Project(rb.velocity, -transform.forward).magnitude < 5)
            rb.AddForce(-transform.forward * 3000 * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space)&&grounded)
        {
            rb.AddForce(transform.up * 500);
            grounded = false;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        grounded = true;
        rb.drag = 3f;
    }
    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
        rb.drag = 1.5f;
    }
}
