using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class Jewel : MonoBehaviour
{
    private void OnEnable()
    {
        rise();
    }

    async void rise()
    {
        Vector3 startPos = transform.position;
        float timer = Time.time;
        while (Time.time < timer + 1) 
        { 
            transform.position = Vector3.Lerp(startPos, startPos + new Vector3(0,2,0), Time.time - timer);
            await Task.Delay(16);
        }
    }
}
