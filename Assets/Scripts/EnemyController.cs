using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject _rotator;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _triggeredVisualizer;

    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField]private float _rotationSpeed = 100f;
    [SerializeField]private float _playerDetectionRange = 10f;

    private Vector3 _moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        if(!_player)
            _player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(Vector3.Distance(_player.transform.position, _rb.position) < _playerDetectionRange)
        {
            _moveDirection = (_player.transform.position - _rb.position).normalized;
            _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.fixedDeltaTime);
            _triggeredVisualizer.SetActive(true);
        }
        else
        {
            _triggeredVisualizer.SetActive(false);
        }


        _rotator.transform.LookAt(transform.position + _moveDirection);
    }
}
