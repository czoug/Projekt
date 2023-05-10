using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{   
    public TurretControl turretControl;
    [HideInInspector]
    public Vector3 targetPos;
    [SerializeField, Range(1,50)]
    private float sensitivity;
    [SerializeField]
    private Transform turretParent;
    [SerializeField, Range(0,10)]
    private float distance;
    [SerializeField, Range(0,10)]
    private float heightPosition;
    [SerializeField, Range(0.1f, 5f)] 
    private float smoothTime = 3f; //A smaller value will reach the target faster.
    
    
    [Header("Vertical rotation settings")]
    [SerializeField, Range(1,180)]
    private float upwardRotationLimit;
    [SerializeField, Range(1,180)]
    private float downwardRotationLimit;    
   
   
    private float rotationX;
    private float rotationY;
    private Vector3 currentRotation;
    private Vector3 velocity;


    private void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;     
    }

    private void Update() {
        turretControl.SetAim(targetPos);
    }   
    private void LateUpdate()
    {   
        CameraMovement();
        turretControl.SetAim(targetPos);
    }

    private void CameraMovement()
    {  
            targetPos = transform.TransformPoint(Vector3.forward * 200.0f);
            // Get player input and multiply the input with sensitivity value;
            float axisX = Input.GetAxis("Mouse X") * sensitivity;
            float axisY = -Input.GetAxis("Mouse Y") * sensitivity;

            // Accumulate the input value 
            rotationX += axisX;
            rotationY += axisY;

            // Limit the rotation
            rotationY = Mathf.Clamp(rotationY, -upwardRotationLimit, downwardRotationLimit);

            //Smoothing the rotation
            Vector3 newRotation = new Vector3(rotationY, rotationX,0);
            currentRotation = Vector3.SmoothDamp(currentRotation, newRotation, ref velocity, smoothTime);

            // apply the rotation value to this gameobject's rotation 
            transform.localEulerAngles = currentRotation;

            // set gameobject's forward position away from the barrel position with distance value
            transform.position = (turretParent.position + transform.up * heightPosition) - transform.forward * distance;

    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(targetPos, 5.0f);
        Gizmos.DrawLine(transform.position, transform.forward * 200.0f);
    }

}