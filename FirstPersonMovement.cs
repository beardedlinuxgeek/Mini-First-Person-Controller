using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace berlin.colin.UltralightCapsuleMovement
{
    public class FirstPersonMovement : MonoBehaviour
    {
        public float MoveSpeed = 5f;
        public float RunSpeed = 9f;
        public float JumpStrength = 200f;
        public float MouseSensitivity = 20f;

        public InputActionAsset inputActionAsset;
        public InputActionReference MoveAction;
        public InputActionReference RunAction;
        public InputActionReference JumpAction;
        public InputActionReference LookAction;
        public InputActionReference ZoomAction;

        private bool isRunning = false;
        private float defaultFOV = 60f;
        private float maxZoomFOV = 15f;
        private float currentZoom = 0f;

        private Rigidbody rb;
        private Camera firstPersonCamera;

        void Awake()
        {
            inputActionAsset.Enable();

            RunAction.action.performed += StartRun;
            RunAction.action.canceled += StopRun;
            JumpAction.action.performed += OnJump;
            ZoomAction.action.performed += OnZoom;

            rb = GetComponent<Rigidbody>();
            firstPersonCamera = GetComponentInChildren<Camera>();
            defaultFOV = firstPersonCamera.fieldOfView;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            firstPersonCamera.transform.localEulerAngles = Vector3.zero;
        }

        private void Update()
        {
            Vector2 mouseDelta = LookAction.action.ReadValue<Vector2>() * Time.smoothDeltaTime;
            transform.Rotate(Vector3.up * mouseDelta.x * MouseSensitivity);
            firstPersonCamera.transform.localEulerAngles = new Vector3(firstPersonCamera.transform.localEulerAngles.x - mouseDelta.y * MouseSensitivity, 0, 0);
        }

        void FixedUpdate()
        {
            float targetMovingSpeed = isRunning ? RunSpeed : MoveSpeed;
            Vector2 moveValue = MoveAction.action.ReadValue<Vector2>();
            rb.velocity = transform.rotation * new Vector3(moveValue.x * targetMovingSpeed, rb.velocity.y, moveValue.y * targetMovingSpeed);
        }

        private void StartRun(InputAction.CallbackContext context)
        {
            isRunning = true;
        }

        private void StopRun(InputAction.CallbackContext context)
        {
            isRunning = false;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (isGrounded())
            {
                rb.AddForce(Vector3.up * 100 * JumpStrength);
            }
        }

        private bool isGrounded()
        {
            return Physics.Raycast((transform.position + Vector3.up * .001f), Vector3.down, 0.15f * 2);
        }

        private void OnZoom(InputAction.CallbackContext context)
        {
            currentZoom += context.ReadValue<float>() * .002f;
            currentZoom = Mathf.Clamp01(currentZoom);
            firstPersonCamera.fieldOfView = Mathf.Lerp(defaultFOV, maxZoomFOV, currentZoom);
        }
    }
}