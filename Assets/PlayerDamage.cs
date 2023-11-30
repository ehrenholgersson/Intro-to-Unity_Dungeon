using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Enemy collision");
        if (collision.collider.tag == "Player")
        {
            UIText.Hurt();
        }

    }
}
