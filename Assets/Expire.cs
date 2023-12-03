using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Expire : MonoBehaviour
{
    [SerializeField] float _expiry = 5;
    float _timer;
    // Start is called before the first frame update
    void Start()
    {
        _timer = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > _timer+_expiry)
            Destroy(gameObject);
    }
}
