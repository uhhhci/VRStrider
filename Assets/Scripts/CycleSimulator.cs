using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleSimulator : MonoBehaviour {

    public GameObject _axis;
    public GameObject _cycle;
    
    [Range(-5,5)]
    public float _speed = 1;
	// Use this for initialization
	void Start () {
        transform.position = _cycle.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        _axis.transform.Rotate(new Vector3(-_speed,0,0));
	}
}
