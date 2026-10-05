using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHunterCamera : MonoBehaviour
{
    [Header("Anclaje")]
    [SerializeField] private Transform targetBone; // Arrastra aquí el hueso "Camera Angle" de tu Hunter

    [Header("Sensibilidad")]
    public float sensitivityX = 200f;
    public float sensitivityY = 200f;

    [Header("Límites de Ángulo (Vertical)")]
    public float minYLimit = -20f; // Límite para no enterrarse en el suelo
    public float maxYLimit = 60f;  // Límite para no mirar completamente vertical desde arriba

    [Header("Distancia del Cazador")]
    public float distanceToTarget = 5.0f; // Qué tan alejada está la cámara del cazador
    public float cameraHeightOffset = 0.5f; // Ajuste fino de altura respecto al hueso

    // Variables internas de rotación
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Bloquea y oculta el cursor del mouse para poder girar la cámara libremente como en MHW
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializar con la rotación actual de la cámara
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }

    void LateUpdate()
    {
        // Validar que el hueso esté asignado para evitar errores en la consola
        if (targetBone == null) return;

        // 1. Capturar el movimiento del mouse (o stick derecho de un mando)
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        // 2. Acumular y calcular los ángulos de rotación en sus propios ejes
        rotationX += mouseX;
        rotationY -= mouseY; // Invertimos el eje Y para que al mover el mouse arriba, la cámara suba

        // 3. Limitar la rotación vertical para que no haga un giro de 360 grados sobre el personaje
        rotationY = Mathf.Clamp(rotationY, minYLimit, maxYLimit);

        // 4. Crear la rotación final basada en los ejes acumulados
        Quaternion targetRotation = Quaternion.Euler(rotationY, rotationX, 0);

        // 5. Calcular la posición objetivo (Hueso + Altura extra - Distancia en base a la rotación)
        Vector3 targetPosition = targetBone.position + (Vector3.up * cameraHeightOffset);
        Vector3 finalPosition = targetPosition - (targetRotation * Vector3.forward * distanceToTarget);

        // 6. Aplicar posición y rotación a la Main Camera
        transform.rotation = targetRotation;
        transform.position = finalPosition;
    }
}