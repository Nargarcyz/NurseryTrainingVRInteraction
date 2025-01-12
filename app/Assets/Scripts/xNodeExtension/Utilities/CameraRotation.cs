﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float scrollSpeed = 20.0f;
 
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
 
    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public bool hasFocus = false;
 

    public LayerMask floor;
 
    float x = 0.0f;
    float y = 0.0f;
 
    // Use this for initialization
    void Start () 
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        ApplyRotation();
    }
 
    void LateUpdate () 
    {
        if(!hasFocus) return;
                
        if (target) 
        {


            if(Input.GetKey(KeyCode.LeftArrow) )    x +=  xSpeed * distance * 0.02f;
            if(Input.GetKey(KeyCode.RightArrow) )   x -=  xSpeed * distance * 0.02f;

            if(Input.GetKey(KeyCode.UpArrow) )      y +=  ySpeed * 0.02f; 
            if(Input.GetKey(KeyCode.DownArrow) )    y -=  ySpeed * 0.02f;
 
            ApplyRotation();
        }
    }

    void ApplyRotation(){
        y = ClampAngle(y, yMinLimit, yMaxLimit);
 
        Quaternion rotation = Quaternion.Euler(y, x, 0);

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*scrollSpeed, distanceMin, distanceMax);

        RaycastHit hit;
        if (Physics.Linecast (target.position, transform.position, out hit, floor)) 
        {
            distance -=  hit.distance;
        }
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
 
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
