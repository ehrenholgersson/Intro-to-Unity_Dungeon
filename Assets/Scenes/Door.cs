using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Vector3 DoorMovement;
    [SerializeField] Vector3 DoorRotation;
    [SerializeField] float speed;
    OpenCloseRotation _doorRotation;
    OpenClosePosition _doorMovement;
    Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _doorRotation = new OpenCloseRotation(transform.rotation, Quaternion.Euler(DoorRotation.x, DoorRotation.y, DoorRotation.z)*transform.rotation, speed, this.gameObject);
        _doorMovement = new OpenClosePosition(transform.position, transform.position + DoorMovement, speed, this.gameObject);
        if (_animator != null )
            _animator.Play("DoorOpenOut");
    }

    private void OnMouseOver()
    {

        if (_doorMovement.IsBusy || _doorRotation.IsBusy)
        {
            Debug.Log("door in use");
            //return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Door Clicked");
            if (DoorRotation.magnitude > 0.1f)
            {
                if (_doorRotation.IsOpen)
                    _doorRotation.Close();
                else
                    _doorRotation.Open();
            }
            else
                Debug.Log("No rotation Value");
            if (DoorMovement.magnitude > 0.1f)
            {
                if (_doorMovement.IsOpen)
                    _doorMovement.Close();
                else
                    _doorMovement.Open();
            }
            else
                Debug.Log("No movement Value");
        }
    }
}
