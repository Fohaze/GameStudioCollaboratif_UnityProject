using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MoveDirectionMode _moveDirectionMode;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _rotator;

    [SerializeField] private ParticleSystem _dashParticleSystem;

    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField]private float _rotationSpeed = 100f;

    [SerializeField] private float _dashForce = 10f;

    private Vector3 _moveDirection;
    private Vector3 _cameraDirectionGroundProjection_forward;
    private Vector3 _cameraDirectionGroundProjection_right;

    public enum MoveDirectionMode{CAMERA_PROJECTION, WORLD_AXIS};

    private string _runAnimationBoolName = "Run";
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(_rotator.transform.forward * _dashForce, ForceMode.Impulse);
            _dashParticleSystem.Play();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_moveDirectionMode == MoveDirectionMode.CAMERA_PROJECTION)
        {
            _cameraDirectionGroundProjection_forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            _cameraDirectionGroundProjection_right = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
            _moveDirection = _cameraDirectionGroundProjection_forward * Input.GetAxis("Vertical") + _cameraDirectionGroundProjection_right * Input.GetAxis("Horizontal");
            _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.fixedDeltaTime);
        }
        else if(_moveDirectionMode == MoveDirectionMode.WORLD_AXIS)
        {
            _moveDirection = new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.fixedDeltaTime);
        }

        _animator.SetBool(_runAnimationBoolName, _moveDirection != Vector3.zero);
        _rotator.transform.LookAt(transform.position + _moveDirection);
    }
}
