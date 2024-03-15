using System;
using UnityEngine;

namespace LibOnward.SpectatorCamera;

public class SmoothFollowBehavior : MonoBehaviour
{
    public float smooth = 0.2f;
    
    private void Update()
    {
        var _camera = Camera.main;
        transform.position = _camera.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, _camera.transform.rotation, smooth);
    }
}