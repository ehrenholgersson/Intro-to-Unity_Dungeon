using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MouseLookScript : MonoBehaviour
{

    enum RotationalAxis { mouseX, mouseY }

    #region Variables
    [Header("Rotation")]

    [SerializeField] private RotationalAxis _axis;
    [Header("Sensitivity"), Range(0, 100)]
    public static Vector2 sensitivity = new Vector2(10, 10);
    [SerializeField] Vector2 _clamp = new Vector2(-60, 60);
    public bool InvertMouse = true;
    private float _rotationMouseY;
    bool _crouched;
    GameObject _knife;

    [SerializeField] Vector3 _moveDirection;
    [SerializeField] CharacterController _characterController;
    [SerializeField] float _movementSpeed = 5;
    [SerializeField] float _jumpspeed = 8, _crouchspeed = 2.5f, _walkspeed = 5, _sprintspeed = 10, _gravity = 9.8f;
    [SerializeField] GameObject _camera;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = Camera.main.gameObject;//transform.Find("Main Camera").gameObject;
        GlobalState.UpdateState(GameStates.MovementEnabled);
        _knife = transform.Find("Knife").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalState.GameState == GameStates.MovementEnabled)
        {
            MoveCharacter();
            MoveView();
        }
    }

    void MoveView()
    {
        #region Mouse X
        //if (_axis == RotationalAxis.mouseX)
        
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity.x, 0);
           
        
        #endregion
        #region Mouse Y
        //else
        
            //_rotationMouseY = transform.localRotation.eulerAngles.x;
            if (!InvertMouse)
            {
                _rotationMouseY -= Input.GetAxis("Mouse Y") * sensitivity.y;
            }
            else
            {
                _rotationMouseY += Input.GetAxis("Mouse Y") * sensitivity.y;
            }
        _rotationMouseY = Mathf.Clamp(_rotationMouseY, _clamp.x, _clamp.y);

        //Debug.Log("RotationY: " + _rotationMouseY);
        // _camera.transform.localEulerAngles = new Vector3(_rotationMouseY, 0, 0);
        _camera.transform.localRotation = Quaternion.Euler(_rotationMouseY, 0, 0);//transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        
        #endregion
    }

    private void MoveCharacter()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _crouched = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftShift))// || Input.GetKeyUp(KeyCode.LeftControl)) - Don't think I need this?
        {
            _crouched = !_crouched;
        }
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            _crouched = false;
        }
        if (_crouched)
        {
            _movementSpeed = _crouchspeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            _movementSpeed = _sprintspeed;
        }
        else
        {
            _movementSpeed = _walkspeed;
        }
        if (Input.GetMouseButtonDown(0) && !_knife.activeSelf)
        {
            _knife.SetActive(true);
        }

        if (_characterController.isGrounded) // ground check
        {
            _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            _moveDirection = transform.TransformDirection(_moveDirection);
            _moveDirection *= _movementSpeed;
            if (Input.GetButton("Jump"))
            {
                _moveDirection.y = _jumpspeed;
            }

            if (true) //jump
            {

            }
        }
        else
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
        }
        _characterController.Move(_moveDirection * Time.deltaTime);


    }
}
