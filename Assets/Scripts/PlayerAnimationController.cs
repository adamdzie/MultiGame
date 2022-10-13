using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimationController : MonoBehaviour
{

    [SerializeField]
    private GameObject StringStretch;

    [SerializeField]
    private Transform StringConstantPos;

    [SerializeField]
    private Transform CurrentStringPos;

    private float _currentWeight = 0f;


    private void Start()
    {
        _currentWeight = 0f;
        StringStretch.GetComponent<MultiParentConstraint>().weight = 0f;
    }
    void Update()
    {
        StringStretch.GetComponent<MultiParentConstraint>().weight = _currentWeight;
    }
    public void OnAnimationStart()
    {

    }
    public void OnStringStretch()
    {
        _currentWeight = 1.0f;
    }
    public void OnStringEndStretch()
    {
        _currentWeight = 0f;
        CurrentStringPos.position = StringConstantPos.position;

    }
}
