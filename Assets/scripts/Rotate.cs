using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float _speed;
    private void Update()
    {
        transform.Rotate(0, _speed * Time.deltaTime, 0);
    }
}
