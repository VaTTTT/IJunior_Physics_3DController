using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyMover : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;

    private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 playerSpeed = new Vector3(Input.GetAxis("Horizontal") * _speed, _rigidBody.velocity.y, Input.GetAxis("Vertical") * _speed);
        //layerSpeed *= Time.deltaTime * _speed;

        _rigidBody.velocity = playerSpeed;
        _rigidBody.velocity += Physics.gravity;
    }
}