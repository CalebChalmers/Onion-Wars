using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 15f;
    public float jumpForce = 10f;
    public float gravity = 20f;
    public float maxSlideSpeed;
    public float minSlideAngle;
    public float pushPower = 2.0f;

    private CharacterController cc;
    private Vector3 velocity = Vector3.zero;
    private Vector3 oldMovement = Vector3.zero;
    private bool isSliding = false;

    public override void OnStartLocalPlayer()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        Vector3 movement = Vector3.zero;

        if (CursorHelper.CursorLocked)
        {
            // Movement
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            movement = transform.TransformDirection(new Vector3(h, 0f, v).normalized);
            movement *= moveSpeed;
        }

        movement = Vector3.Lerp(oldMovement, movement, 9f * Time.deltaTime); // Smooth movement (accel/decel)
        oldMovement = movement;

        velocity.x = movement.x;
        velocity.z = movement.z;

        if (cc.isGrounded)
        {
            /*isSliding = false;

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, cc.radius, Vector3.down, out hit))
            {
                float angle = Mathf.Clamp(Vector3.Angle(hit.normal, Vector3.up), 0f, 90f);

                if (angle >= minSlideAngle)
                {
                    isSliding = true;

                    Vector3 a = Vector3.Cross(Vector3.up, hit.normal);
                    Vector3 b = Vector3.Cross(a, hit.normal);
                    float slideSpeed = (angle / 90f) * maxSlideSpeed;
                    velocity += b * slideSpeed;
                }
            }*/

            if (!isSliding && Input.GetKeyDown(KeyCode.Space) && CursorHelper.CursorLocked)
            {
                velocity.y = jumpForce;
            }
        }


        velocity.y -= gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!isLocalPlayer) return;

        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }
}
