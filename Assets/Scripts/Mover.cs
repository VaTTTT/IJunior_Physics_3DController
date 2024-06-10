using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CollisionDetector))]
public class Mover : MonoBehaviour
{
    [SerializeField] private float _speed = 4;
    [SerializeField] private float _groundDrag = 5;
    [SerializeField] private float _gravityFactor = 2;

    private Rigidbody _rigidBody;
    private Transform _transform;
    private CollisionDetector _collisionDetector;

    private Vector3 _verticalVelocity;
    private float _planeVelocityMultiplier = 10f;
    private float _slopeVelocityMultiplier = 5f;
    private float _stairsVelocityMultiplier = 20f;
    private Vector3 _targetDirection;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _collisionDetector = GetComponent<CollisionDetector>();

        _verticalVelocity = Physics.gravity * _gravityFactor;
    }

    private void FixedUpdate()
    {
        _collisionDetector.CheckGround();

        if (_collisionDetector.IsOnGround)
        {
            _rigidBody.drag = _groundDrag;
        }
        else
        {
            _rigidBody.drag = 0;
            _rigidBody.AddForce(_verticalVelocity, ForceMode.Force);
        }
    }

    public void MoveTowardsTarget(Transform target)
    {
        _targetDirection = new Vector3(target.position.x - _transform.position.x, 0, target.position.z - _transform.position.z).normalized;

        if (_collisionDetector.IsOnSlope)
        {
            _rigidBody.useGravity = false;
            _rigidBody.AddForce(GetSlopeMovementDirection() * _speed * _slopeVelocityMultiplier, ForceMode.Force);
        }
        else if (_collisionDetector.IsBeforeStairs())
        {
            _rigidBody.useGravity = false;
            _rigidBody.AddForce(_transform.up * _stairsVelocityMultiplier * _collisionDetector.StepHeight * _speed, ForceMode.Force);
        }
        else
        {
            _rigidBody.useGravity = true;
            _rigidBody.AddForce(_targetDirection * _planeVelocityMultiplier * _speed, ForceMode.Force);
        }

        LimitVelocity();
    }

    private Vector3 GetSlopeMovementDirection()
    {
        return Vector3.ProjectOnPlane(_targetDirection, _collisionDetector.GroundHitInfo.normal).normalized;
    }

    private void LimitVelocity()
    {
        Vector3 currentVelocity = new Vector3(_rigidBody.velocity.x, 0f, _rigidBody.velocity.z);

        if (currentVelocity.magnitude > _speed)
        {
            Vector3 maximalVelocity = currentVelocity.normalized * _speed;
            _rigidBody.velocity = new Vector3(maximalVelocity.x, _rigidBody.velocity.y, maximalVelocity.z);
        }
    }
}