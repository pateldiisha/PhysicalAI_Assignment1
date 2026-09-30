using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class ActorController : MonoBehaviour
{
    [SerializeField] private float acceleration = 24f;
    [SerializeField] private float maxSpeed = 5.5f;
    [SerializeField] private float braking = 9f;

    private Rigidbody body;
    private Vector2 input;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        body.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = Vector2.ClampMagnitude(input, 1f);
    }

    private void FixedUpdate()
    {
        Vector3 desired = new Vector3(input.x, 0f, input.y) * acceleration;
        body.AddForce(desired, ForceMode.Acceleration);

        Vector3 planar = new Vector3(body.velocity.x, 0f, body.velocity.z);
        if (input.sqrMagnitude < 0.01f)
            body.AddForce(-planar * braking, ForceMode.Acceleration);
        else if (planar.magnitude > maxSpeed)
            body.velocity = new Vector3(planar.normalized.x * maxSpeed, body.velocity.y, planar.normalized.z * maxSpeed);
    }
}
