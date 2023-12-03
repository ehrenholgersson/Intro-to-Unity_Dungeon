using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    CharacterController _player;
    NavMeshAgent _agent;
    bool _stop = false;
    [SerializeField] GameObject _explosion;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
        AiLoop();
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enemy collision");
        if (other.tag == "Player")
        {
            UIText.Hurt();
        }
    }

    private void OnDestroy()
    {
        _stop = true;    
    }

    public void Die()
    {
        GameObject gO = Instantiate(_explosion);
        gO.transform.position = transform.position;
        Destroy(gameObject);
    }

    // Update is called once per frame
    async void AiLoop()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (_player.transform.position - transform.position).normalized, out hit, Mathf.Infinity, ~0) && hit.transform.tag == "Player") 
                _agent.destination = _player.transform.position;
            else
                _agent.destination = transform.position;

            await Task.Delay(500);
            if (_stop)
                return;
        }
    }
}
