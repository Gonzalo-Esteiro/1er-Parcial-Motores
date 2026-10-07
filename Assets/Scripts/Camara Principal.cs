using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraPrincipal : MonoBehaviour
{
    public Transform cameraAngle;
    public float distanciaMax = 5.0f; // Renombrada para claridad
    public float sensibilidad = 3.0f;
    public LayerMask capasColision;    // ¡IMPORTANTE! Asigna aquí las capas del suelo/paredes
    public float radioCamara = 0.2f;   // Pequeño margen para que la cámara no se pegue al 100%

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
        Quaternion rotacion = Quaternion.Euler(rotacionX, rotacionY, 0);

        // Dirección hacia donde quiere ir la cámara desde el cameraAngle
        Vector3 direccionDeseada = rotacion * Vector3.forward;

        // Posición ideal máxima sin colisiones
        Vector3 posicionIdeal = cameraAngle.position - direccionDeseada * distanciaMax;

        // Por defecto, la distancia actual es la máxima
        float distanciaActual = distanciaMax;

        // Lanzamos un rayo desde el origen hasta la posición ideal de la cámara
        RaycastHit hit;
        if (Physics.Raycast(cameraAngle.position, -direccionDeseada, out hit, distanciaMax, capasColision))
        {
            // Si choca con algo, restamos el radio de la cámara para que no traspase sutilmente
            distanciaActual = hit.distance - radioCamara;
        }

        // Calculamos la posición final con la distancia corregida por la colisión
        Vector3 posicionFinal = cameraAngle.position - direccionDeseada * distanciaActual;

        // Aplicamos posición y rotación
        transform.position = posicionFinal;
        transform.LookAt(cameraAngle);
    }

    public Vector3 ObtenerDireccion()
    {
        Vector3 direccion = transform.forward;
        direccion.y = 0;
        return direccion.normalized;
    }
}