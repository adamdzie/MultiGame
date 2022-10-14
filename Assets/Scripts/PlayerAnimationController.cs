using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;

public class PlayerAnimationController : MonoBehaviour
{

    public delegate void OnShootCallback();
    public event OnShootCallback OnShoot;

    [SerializeField]
    private GameObject StringStretch;

    [SerializeField]
    private GameObject Arrow;

    [SerializeField]
    private GameObject HandConstraint;

    [SerializeField]
    private Transform StringConstantPos;

    [SerializeField]
    private Transform CurrentStringPos;


    private ParentConstraint _string;

    private float _currentWeight = 0f;
    private bool isArrowRotating;


    private void Start()
    {
        _currentWeight = 0f;
        _string  = StringStretch.GetComponent<ParentConstraint>();

        _string.weight = 0f;
    }
    void Update()
    {
        _string.weight = _currentWeight;
    }

    public void OnStringStretch()
    {
        _currentWeight = 1.0f;
        
    }
    public void OnStringEndStretch()
    {
        _currentWeight = 0f;
        OnShoot?.Invoke();

    }
}
