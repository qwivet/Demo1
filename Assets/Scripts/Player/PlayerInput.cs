using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof (Rigidbody))]
public class PlayerInput : MonoBehaviour
{
    private Transform transform;
    private Rigidbody rigidbody;
    private Vector3 rotationVector;

    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float startSpeed;
    [SerializeField]
    private float speedMultiplier;

    public void Start ()
    {
        transform = this.GetComponent<Transform>();
        rigidbody = this.GetComponent<Rigidbody>();
    }

    public void LookRotate (InputAction.CallbackContext callback)
    {
        rotationVector = new Vector3(-callback.ReadValue<Vector2>().y,0,callback.ReadValue<Vector2>().x);
    }

    private void Update() 
    {
        transform.Rotate (rotationVector * Time.deltaTime * rotationSpeed);
        rigidbody.velocity = -transform.up * (startSpeed + speedMultiplier * Time.realtimeSinceStartup);
    }
}
//