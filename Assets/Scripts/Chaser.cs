using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CollisionDetector))]
[RequireComponent(typeof(Rotator))]
[RequireComponent(typeof(Mover))]
public class Chaser : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _chaseDistance = 4f;

    private Transform _transform;
    private Rotator _rotator;
    private Mover _mover;
    private CollisionDetector _collisionDetector;
    private float _targetDistance;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _rotator = GetComponent<Rotator>();
        _mover = GetComponent<Mover>();
        _collisionDetector = GetComponent<CollisionDetector>();
    }

    private void FixedUpdate()
    {
        _targetDistance = Vector3.Distance(_transform.position, _target.position);

        if (_target != null)
        {
            _rotator.RotateTowardsTarget(_target);

            if (_chaseDistance < _targetDistance)
            {
                _mover.MoveTowardsTarget(_target);
            }
        }
    }
}