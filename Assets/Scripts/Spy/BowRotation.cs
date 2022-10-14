using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowRotation : MonoBehaviour
{
    [SerializeField]
    private Transform Bow;

    private void Start()
    {
        transform.position = Bow.position;
    }
    private void Update()
    {
        transform.position = Bow.position;
    }
}
