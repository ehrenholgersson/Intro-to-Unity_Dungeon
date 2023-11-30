using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] string _name;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Inventory.Add(_name);
            Destroy(gameObject);
        }
    }
}
