using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Knife : MonoBehaviour
{
    [SerializeField] float _range;
    [SerializeField] float _speed;

    float _timer;
    Vector3 _startPosition;

    // Start is called before the first frame update
    private void OnEnable()
    {
        _timer = 0;
        _startPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_timer < 1) 
        {
            transform.localPosition = Vector3.Lerp(_startPosition, _startPosition + Vector3.forward * _range, _timer);
        }
        else
        {
            if (_timer >=2)
            {
                gameObject.SetActive(false);
                return;
            }
            transform.localPosition = Vector3.Lerp( _startPosition + Vector3.forward * _range, _startPosition, _timer -1);
        }
        _timer += Time.deltaTime / _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Stab");
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
            enemy.Die();
    }
}
