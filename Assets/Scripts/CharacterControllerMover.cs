using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMover : MonoBehaviour
{
    [SerializeField] private float _speed = 7f;
    [SerializeField] private float _strafeSpeed = 7f;
    [SerializeField] private float _jumpSpeed = 7f;
    [SerializeField] private float _gravityFactor = 2f;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _horizontalTurnSensitivity = 7;
    [SerializeField] private float _verticalTurnSensitivity = 10;
    [SerializeField] private float _verticalMinAngle = -89f;
    [SerializeField] private float _verticalMaxAngle = 89f;

    private Vector3 _verticalVelocity;
    private Transform _transform;
    private CharacterController _characterController;
    private float _cameraAngle = 0;

    private void Awake()
    {
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        _cameraAngle = _cameraTransform.localEulerAngles.x;
    }

    private void Update()
    {
        Vector3 forward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;

        _cameraAngle -= Input.GetAxis("Mouse Y") * _verticalTurnSensitivity;
        _cameraAngle = Mathf.Clamp(_cameraAngle, _verticalMinAngle, _verticalMaxAngle);
        _cameraTransform.localEulerAngles = Vector3.right * _cameraAngle;

        _transform.Rotate(Vector3.up * _horizontalTurnSensitivity * Input.GetAxis("Mouse X"));

        if (_characterController != null)
        {
            Vector3 playerSpeed = forward * Input.GetAxis("Vertical") * _speed + right * Input.GetAxis("Horizontal") * _strafeSpeed;

            if (_characterController.isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _verticalVelocity = Vector3.up * _jumpSpeed;
                }
                else
                {
                    _verticalVelocity = Vector3.down;
                }

                _characterController.Move((playerSpeed + _verticalVelocity) * Time.deltaTime);
            }
            else
            {
                Vector3 horizontalVelocity = _characterController.velocity;
                horizontalVelocity.y = 0;
                _verticalVelocity += Physics.gravity * Time.deltaTime * _gravityFactor;
                _characterController.Move((horizontalVelocity + _verticalVelocity) * Time.deltaTime);
            }
        }
    }
}