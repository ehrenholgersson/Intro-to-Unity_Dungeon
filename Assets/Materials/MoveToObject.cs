using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToObject : MonoBehaviour
{
    NavMeshAgent m_Agent;
    public GameObject followObject;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin,ray.direction,out RaycastHit hitinfo))
            {
                if (hitinfo.collider.gameObject.tag.Contains("WayPoint"))
                {
                    followObject = hitinfo.collider.gameObject;
                }
                else
                    MoveToPoint(hitinfo.point);
            }
        }
        if (!(followObject==null))
            m_Agent.destination = followObject.transform.position;
    }
    void MoveToPoint(Vector3 point)
    {
        followObject = null;
        m_Agent.destination = point;
    }
    void FollowObject(GameObject objToFollow)
    {
        followObject = objToFollow;
    }
}
