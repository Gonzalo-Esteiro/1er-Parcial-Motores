using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraPrincipal : MonoBehaviour
{
    public Transform cameraAngle;

    public float distancia = 5.0f;
    public float sensibilidad = 3.0f;

    private float rotacionX = 0.0f;
    private float rotacionY = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        // Movimiento del mouse
        rotacionY += Input.GetAxis("Mouse X") * sensibilidad;
        rotacionX -= Input.GetAxis("Mouse Y") * sensibilidad;

        // Limitar cuánto podemos mirar arriba y abajo
        rotacionX = Mathf.Clamp(rotacionX, -60.0f, 60.0f);

        // Rotación de la cámara
        Quaternion rotacion = Quaternion.Euler(
            rotacionX,
            rotacionY,
            0
        );

        // Posición alrededor de Camera Angle
        Vector3 posicion =
            cameraAngle.position -
            rotacion * Vector3.forward * distancia;

        transform.position = posicion;

        // La cámara siempre mira hacia Camera Angle
        transform.LookAt(cameraAngle);
    }

    public Vector3 ObtenerDireccion()
    {
        Vector3 direccion = transform.forward;

        direccion.y = 0;

        return direccion.normalized;
    }
}
