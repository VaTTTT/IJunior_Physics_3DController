using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class BotMover : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 4;
    [SerializeField] private float _rotationSpeed = 5;
    [SerializeField] private float _maxSlopeAngle = 45;

    [SerializeField] private GameObject _target;
    [SerializeField] private float _chaseDistance = 0.5f;

    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private GameObject _upperStepPoint;
    [SerializeField] private GameObject _lowerStepPoint;
    [SerializeField] private float _stepHeight = 0.3f;
    [SerializeField] private float _skinWidth = 0.08f;

    [SerializeField] private float _groundDrag;
    [SerializeField] private float _gravityFactor = 2;

    private Transform _botTransform;
    private Rigidbody _botBody;
    private Collider _botCollider;

    private float _botRadius;
    private float _botHeight;
    private float _halfBotHeight;
    private float _doubleSkinWidth;

    private Vector3 _targetDirection;
    private Vector3 _verticalVelocity;
    private RaycastHit _groundHitInfo;

    private bool _isOnGround = false;
    private bool _isOnSlope = false;

    private float _planeVelocityFactor = 10f;
    private float _slopeVelocityFactor = 5f;
    private float _stairsVelocityFactor = 18f;

    private void Awake()
    {
        _botBody = GetComponent<Rigidbody>();
        _botCollider = GetComponent<Collider>();
        _botTransform = GetComponent<Transform>();
        _verticalVelocity = Physics.gravity * _gravityFactor;
        _botRadius = (_botCollider.bounds.max.x - _botCollider.bounds.min.x) * 0.5f;
        _botHeight = _botCollider.bounds.max.y - _botCollider.bounds.min.y;
        _halfBotHeight = _botHeight * 0.5f;
        _doubleSkinWidth = _skinWidth * 2;
        _upperStepPoint.transform.position = new Vector3(_upperStepPoint.transform.position.x, _lowerStepPoint.transform.position.y + _stepHeight, _upperStepPoint.transform.position.z);
    }

    private void FixedUpdate()
    {
        if (_isOnGround)
        {
            _botBody.drag = _groundDrag;
        }
        else
        {
            _botBody.drag = 0;
            _botBody.AddForce(_verticalVelocity, ForceMode.Force);
        }

        if (_target != null)
        {
            CheckGround();
            RotateTowardsTarget(_target, _rotationSpeed);
            MoveTowardsTarget(_target, _movementSpeed, _chaseDistance);
        }
    }

    private bool IsBeforeStairs()
    {
        float lowerRayLength = 0.1f + _botRadius - _lowerStepPoint.transform.localPosition.z;
        float upperRayLength = 0.2f;

        if (Physics.Raycast(_lowerStepPoint.transform.position, _botTransform.TransformDirection(Vector3.forward), out _, lowerRayLength, _groundLayer))
        {
            if (!Physics.Raycast(_upperStepPoint.transform.position, _botTransform.TransformDirection(Vector3.forward), out _, upperRayLength, _groundLayer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void CheckGround()
    {
        float slopeAngle;
        float groundDistance = _halfBotHeight - _botRadius + _doubleSkinWidth;

        if (Physics.SphereCast(_botCollider.bounds.center, _botRadius - _skinWidth, Vector3.down, out _groundHitInfo, groundDistance, _groundLayer))
        {
            _isOnGround = true;
            slopeAngle = Vector3.Angle(Vector3.up, _groundHitInfo.normal);

            if (slopeAngle < _maxSlopeAngle && slopeAngle != 0)
            {
                _isOnSlope = true;
            }
            else
            {
                _isOnSlope = false;
            }
        }
        else
        {
            _isOnGround = false;
            _isOnSlope = false;
        }
    }

    private Vector3 GetSlopeMovementDirection()
    {
        return Vector3.ProjectOnPlane(_targetDirection, _groundHitInfo.normal).normalized;
    }

    private void LimitVelocity()
    {
        Vector3 currentVelocity = new Vector3(_botBody.velocity.x, 0f, _botBody.velocity.z);

        if (currentVelocity.magnitude > _movementSpeed)
        {
            Vector3 maximalVelocity = currentVelocity.normalized * _movementSpeed;
            _botBody.velocity = new Vector3(maximalVelocity.x, _botBody.velocity.y, maximalVelocity.z);
        }
    }

    private void MoveTowardsTarget(GameObject target, float speed, float stopDistance)
    {
        float targetDistance = Vector3.Distance(_botTransform.position, target.transform.position);
        _targetDirection = new Vector3(target.transform.position.x - _botTransform.position.x, 0, target.transform.position.z - _botTransform.position.z).normalized;

        if (stopDistance < targetDistance)
        {
            if (_isOnSlope)
            {
                _botBody.useGravity = false;
                _botBody.AddForce(GetSlopeMovementDirection() * speed * _slopeVelocityFactor, ForceMode.Force);
            }
            else if (IsBeforeStairs())
            {
                _botBody.useGravity = false;
                _botBody.AddForce(_botTransform.up * _stairsVelocityFactor * _stepHeight * speed, ForceMode.Force);
            }
            else
            {
                _botBody.useGravity = true;
                _botBody.AddForce(_targetDirection * _planeVelocityFactor * speed, ForceMode.Force);
            }
        }

        LimitVelocity();
    }

    private void RotateTowardsTarget(GameObject target, float speed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(target.transform.position.x - _botTransform.position.x, 0, target.transform.position.z - _botTransform.position.z).normalized);
        float targetAngle = targetRotation.eulerAngles.y;
        float deltaAngle = Mathf.Abs(_botTransform.rotation.eulerAngles.y - targetAngle);
        float minimalDeltaAngle = 1f;

        if (deltaAngle > minimalDeltaAngle)
        {
            _botBody.MoveRotation(Quaternion.Slerp(_botTransform.rotation, targetRotation, speed * Time.deltaTime));
        }
    }
}