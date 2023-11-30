using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    [SerializeField] GameObject _red;
    [SerializeField] GameObject _green;
    [SerializeField] GameObject _blue;
    [SerializeField] Animator _door;
    [SerializeField] GameObject _light;

    Animator _lid;
    // Start is called before the first frame update
    private void Start()
    {
        _lid = GetComponentInChildren<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            if (Inventory.Contents.Contains("RedKey"))
                _red.SetActive(true);
            if (Inventory.Contents.Contains("BlueKey"))
                _blue.SetActive(true);
            if (Inventory.Contents.Contains("GreenKey"))
                _green.SetActive(true);
        }
        if (_red.activeSelf && _blue.activeSelf && _green.activeSelf)
        {
            _lid?.Play("Open");
            _door?.Play("GateOpen");
            _light?.SetActive(true);
        }
    }
}
