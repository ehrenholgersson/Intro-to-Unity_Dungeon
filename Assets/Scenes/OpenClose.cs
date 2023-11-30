using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OpenClose : MonoBehaviour
{
    public Vector3 PositionOffset = Vector3.zero;
    Vector3 _openPosition;
    Vector3 _closedPosition;
    bool _isOpen;
    // Start is called before the first frame update
    void Start()
    {
        _closedPosition = transform.position;
        _openPosition = _closedPosition + PositionOffset;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 goal = _openPosition;
        Vector3.MoveTowards(transform.position, goal, 20f * Time.deltaTime);
    }
    IEnumerator MoveObject()
    {
        Vector3 goal;
        if (_isOpen)
            goal = _closedPosition;
        else
            goal = _openPosition;

        yield return null;
    }
}

public class OpenClosePosition
{
    Vector3 _openPosition;
    Vector3 _closedPosition;
    float _speed;
    GameObject gameObject;
    bool _isOpen;
    public bool IsOpen { get => _isOpen; }
    bool _isBusy;
    public bool IsBusy { get => _isBusy; }

    public OpenClosePosition(Vector3 closed,Vector3 open, float speed,GameObject target)
    {
        _closedPosition = closed;
        _openPosition = open;
        _speed = speed;
        gameObject = target;
    }
    public async void Open()
    {
        while (_isBusy)
        {
            await Task.Delay(200);
        }
        _isBusy = true;
        _isOpen = true;
        Debug.Log("Opening door");
        while ((gameObject.transform.position - _openPosition).magnitude <0.1f) 
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, _openPosition, _speed * Time.deltaTime);
            await Task.Delay(25);
        }

        _isBusy = false;
        return;
    }

    public async void Close()
    {
        while (_isBusy)
        {
            await Task.Delay(200);
        }
        _isBusy = true;
        _isOpen = false;
        Debug.Log("Closing door");
        while ((gameObject.transform.position - _closedPosition).magnitude < 0.1f)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, _closedPosition, _speed * Time.deltaTime);
            await Task.Delay(25);
        }

        _isBusy = false;
        return;
    }
}

public class OpenCloseRotation
{
    Quaternion _openPosition;
    Quaternion _closedPosition;
    float _speed;
    GameObject gameObject;
    bool _isOpen;
    public bool IsOpen { get => _isOpen; }
    bool _isBusy;
    public bool IsBusy { get => _isBusy; }

    public OpenCloseRotation(Quaternion closed, Quaternion open, float speed, GameObject target)
    {
        _closedPosition = closed;
        _openPosition = open;
        _speed = speed;
        gameObject = target;
        
    }
    public async void Open()
    {
        while (_isBusy)
        {
            await Task.Delay(200);
        }
        _isBusy = true;
        _isOpen = true;
        while (Mathf.Abs(Quaternion.Dot(gameObject.transform.rotation, _openPosition)) < 0.999999f)
            {

            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, _openPosition, _speed * Time.deltaTime);
            await Task.Delay(25);
        }

        _isBusy = false;
        return;
    }

    public async void Close()
    {
        while (_isBusy)
        {
            await Task.Delay(200);
        }
        _isBusy = true;
        _isOpen = false;
        while (Mathf.Abs(Quaternion.Dot(gameObject.transform.rotation, _closedPosition)) < 0.999999f)
        {

            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, _closedPosition, _speed * Time.deltaTime);
            await Task.Delay(25);
        }

        _isBusy = false;
        return;
    }
}
