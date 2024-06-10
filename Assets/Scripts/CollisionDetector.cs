using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _upperStepPoint;
    [SerializeField] private Transform _lowerStepPoint;
    [SerializeField] private float _botRadius;
    [SerializeField] private float _botHeight;
    [SerializeField] private float _skinWidth = 0.08f;
    [SerializeField] private float _maxSlopeAngle = 45;
    [SerializeField] private float _stepHeight = 0.3f;

    private Collider _collider;

    private bool _isOnGround = false;
    private bool _isOnSlope = false;

    private float _botHeightMultiplier;
    private float _skinWidthMultiplier;
    private float _slopeAngle;
    private float _groundDistance;
    private RaycastHit _groundHitInfo;

    public bool IsOnGround => _isOnGround;
    public bool IsOnSlope => _isOnSlope;
    public RaycastHit GroundHitInfo => _groundHitInfo;
    public float StepHeight => _stepHeight;

    private void Awake()
    {
        _collider = GetComponent<Collider>();

        _upperStepPoint.position = new Vector3(_upperStepPoint.position.x, _lowerStepPoint.position.y + _stepHeight, _upperStepPoint.position.z);
    }

    public bool IsBeforeStairs()
    {
        float lowerRayLength = 0.1f;
        float upperRayLength = 0.2f;
        float lowerRayCorrectedLength = lowerRayLength + _botRadius - _lowerStepPoint.localPosition.z;

        if (Physics.Raycast(_lowerStepPoint.position, _lowerStepPoint.forward, out _, lowerRayCorrectedLength, _groundLayer))
        {
            if (!Physics.Raycast(_upperStepPoint.position, _upperStepPoint.forward, out _, upperRayLength, _groundLayer))
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

    public void CheckGround()
    {
        _botHeightMultiplier = 0.5f;
        _skinWidthMultiplier = 2f;
        _groundDistance = _botHeight * _botHeightMultiplier - _botRadius + _skinWidth * _skinWidthMultiplier;

        if (Physics.SphereCast(_collider.bounds.center, _botRadius - _skinWidth, Vector3.down, out _groundHitInfo, _groundDistance, _groundLayer))
        {
            _isOnGround = true;
            _slopeAngle = Vector3.Angle(Vector3.up, _groundHitInfo.normal);

            if (_slopeAngle < _maxSlopeAngle && _slopeAngle != 0)
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
}