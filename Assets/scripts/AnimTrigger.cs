using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTrigger : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] string _name;
    [SerializeField] AudioSource _audio;
    [SerializeField] AudioClip _clip;

    private void OnTriggerEnter(Collider other)
    {
        if (_animator != null && other.tag == "Player" && _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _audio?.PlayOneShot(_clip);
            _animator?.Play(_name);
        }
    }
}
