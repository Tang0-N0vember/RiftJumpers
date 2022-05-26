using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public GameObject playerModel;

    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    private float ySpeed;
    private float originalStepOffset;
    float verticalLookRotation;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;

    }
    private void Start()
    {
        playerModel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (playerModel.activeSelf == false)
            {
                SetPosition();
                playerModel.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }


            if (hasAuthority)
            {
                Movement();
            }
        }
    }
    public void SetPosition()
    {
        transform.position = Vector3.zero;
    }
    public void Movement()
    {
        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * mouseSensitivity));

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;


        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 offset = new Vector3(horizontal, Physics.gravity.y, vertical) * walkSpeed;
        offset = transform.TransformDirection(offset);

        ySpeed += Physics.gravity.y * Time.deltaTime;


        if (characterController.isGrounded)
        {
            characterController.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ySpeed = jumpForce;
                Debug.Log("Jumping");
            }
        }
        else
        {
            characterController.stepOffset = 0;
        }

        offset.y = ySpeed;

        characterController.Move(offset * Time.deltaTime);
    }
}
