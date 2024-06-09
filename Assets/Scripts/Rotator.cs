using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rotator : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _angleStep = 1f;

    private Transform _transform;
    private Rigidbody _rigidBody;

    private Quaternion _targetRotation;
    private float _targetAngle;
    private float _angleDifference;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    public void RotateTowardsTarget(Transform target)
    {
        _targetRotation = Quaternion.LookRotation(new Vector3(target.position.x - _transform.position.x, 0, target.position.z - _transform.position.z).normalized);
        _targetAngle = _targetRotation.eulerAngles.y;
        _angleDifference = Mathf.Abs(_transform.rotation.eulerAngles.y - _targetAngle);

        if (_angleDifference > _angleStep)
        {
            _rigidBody.MoveRotation(Quaternion.Slerp(_transform.rotation, _targetRotation, _speed * Time.deltaTime));
        }
    }
}