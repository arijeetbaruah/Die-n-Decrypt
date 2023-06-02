using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float sprintMovementSpeed;

    [SerializeField]
    private Animator animator;

    private Vector2 movement;
    private bool isSprinting = false;
    private Rigidbody rb;
    private bool grounded = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue action)
    {
        if (grounded)
        {
            movement = action.Get<Vector2>();
            movement.y = 0;
            movement.Normalize();
        }
    }

    public void OnRun(InputValue action)
    {
        if (grounded)
        {
            isSprinting = action.Get<float>() != 0;
        }
    }

    public void OnJump(InputValue action)
    {
        grounded = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddRelativeForce(new Vector3(0, 5, 0), ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null && collision.collider.tag == "Ground")
        {
            if (!grounded)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                grounded = true;
            }
        }
    }

    private void LateUpdate()
    {
        float speed = (movement == Vector2.zero ? 0 : (isSprinting ? 1 : 0.5f));
        animator.SetFloat("movementSpeed", speed);

        if (movement.x < 0)
        {
            transform.localScale = new Vector3(1, 1, -1);
        }
        else if (movement.x > 0)
        {
            transform.localScale = Vector3.one;
        }

        rb.MovePosition(
            transform.position
                + new Vector3(movement.x, 0, 0)
                    * (isSprinting ? sprintMovementSpeed : movementSpeed)
                    * Time.deltaTime
        );
    }
}
