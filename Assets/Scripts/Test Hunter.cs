using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterMovement : MonoBehaviour
{
    [Header("Componentes")]
    private Animator hunterAnimator;
    private CharacterController characterController; // O Rigidbody si tu proyecto lo usa

    [Header("Ajustes de Movimiento")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;
    public float rotationSpeed = 10.0f;

    [Header("Controles Teclado/Mando")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode rollKey = KeyCode.Space;

    // Variables internas para el cálculo
    private Vector3 moveDirection;
    private float currentSpeed;

    void Start()
    {
        // Buscamos automáticamente el Animator asignado a tu "Hunter Controller"
        hunterAnimator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        ManejarMovimiento();
        ManejarAcciones();
    }

    void ManejarMovimiento()
    {
        // 1. Obtener entradas del jugador (Ejes horizontales y verticales)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 2. Calcular vector de dirección en base al espacio global
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 3. Determinar si está caminando o corriendo
        if (moveDirection.magnitude > 0.1f)
        {
            bool isRunning = Input.GetKey(runKey);
            float targetSpeed = isRunning ? runSpeed : walkSpeed;

            // Suavizamos el cambio de velocidad actual
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, Time.deltaTime * 10f);

            // Rotar al Cazador hacia la dirección del movimiento de forma fluida
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Mover físicamente el transform si usas CharacterController
            if (characterController != null && characterController.isGrounded)
            {
                characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Si no hay input, la velocidad disminuye a 0 (Idle)
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, Time.deltaTime * 10f);
        }

        // 4. PASAR EL VALOR AL ANIMATOR (Hunter Controller)
        // Usamos currentSpeed para alimentar el parámetro float "Speed" del Blend Tree
        hunterAnimator.SetFloat("Speed", currentSpeed);
    }

    void ManejarAcciones()
    {
        // 5. Detectar el comando para Rodar (Roll)
        if (Input.GetKeyDown(rollKey))
        {
            // Disparamos el parámetro Trigger "Roll" del Animator
            hunterAnimator.SetTrigger("Roll");
        }
    }
}