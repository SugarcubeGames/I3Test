using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Hides the ground plane if the camera drops below it's top plane
public class GroundPlaneHider : MonoBehaviour
{
    private float _topHeight; //y-Axis height of top of the ground plane

    private MeshRenderer _mr; //Mesh renderer toturn on and off
    private BoxCollider _bc; 

    // Start is called before the first frame update
    void Start()
    {
        _mr = this.gameObject.GetComponent<MeshRenderer>();
        _bc = this.gameObject.GetComponent<BoxCollider>();
        _topHeight = _mr.bounds.max.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (_mr == null) return;
        if(Camera.main.transform.position.y < _topHeight)
        {
            if (_mr.enabled)
            {
                _mr.enabled = false;
                if (_bc != null)
                {
                    _bc.enabled = false;
                }
            }
        } else
        {
            if (!_mr.enabled)
            {
                _mr.enabled = true;
                if (_bc == null)
                {
                    _bc.enabled = true;
                }
            }
        }
    }
}
