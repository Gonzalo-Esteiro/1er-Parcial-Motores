using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    public float velocidadCaminar = 2.0f;
    public float velocidadCorrer = 5.0f;

    [Header("Ajustes del Roll")]
    public float distanciaRodar = 3.0f;
    public float duracionRodar = 0.5f;

    public CamaraPrincipal camara;

    private Animator animator;
    private Rigidbody rb;

    private bool rodando = false;
    private float tiempoRodar = 0.0f;

    // VARIABLE NUEVA: Guarda la dirección exacta al iniciar el Roll para evitar que gire o se desvíe
    private Vector3 direccionRodado;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rodando)
        {
            tiempoRodar += Time.deltaTime;

            // CAMBIO: Ahora usamos 'direccionRodado' fija en lugar de 'transform.forward' dinámico.
            // Esto garantiza que el cazador avance en una línea recta perfecta sin importar si intentas girar.
            Vector3 desplazamientoRodar = direccionRodado * (distanciaRodar / duracionRodar) * Time.deltaTime;
            rb.MovePosition(rb.position + desplazamientoRodar);

            if (tiempoRodar >= duracionRodar)
            {
                rodando = false;
            }

            return; // Bloquea por completo el resto del código de movimiento y rotación ordinaria
        }

        // 1. Calcular los vectores de movimiento normales primero
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direccionCamara = camara.ObtenerDireccion();
        Vector3 derechaCamara = Vector3.Cross(Vector3.up, direccionCamara);

        Vector3 movimiento = direccionCamara * vertical + derechaCamara * horizontal;
        movimiento = Vector3.ClampMagnitude(movimiento, 1.0f);

        // 2. Detectar comando para Rodar (Roll)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rodando = true;
            tiempoRodar = 0.0f;

            // ASIGNACIÓN CLAVE: Si el jugador se está moviendo, rueda hacia esa dirección del input.
            // Si el jugador está quieto (Idle), rueda hacia el frente actual del personaje.
            if (movimiento != Vector3.zero)
            {
                direccionRodado = movimiento.normalized;

                // Forzamos al personaje a mirar INSTANTÁNEAMENTE hacia donde va a rodar
                transform.rotation = Quaternion.LookRotation(direccionRodado);
            }
            else
            {
                direccionRodado = transform.forward;
            }

            animator.SetTrigger("Roll");
            return;
        }

        // 3. Lógica normal de movimiento (Caminar / Correr)
        float velocidad = velocidadCaminar;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidad = velocidadCorrer;
        }

        if (movimiento != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(movimiento);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                10.0f * Time.deltaTime
            );
        }

        Vector3 desplazamientoNormal = movimiento * velocidad * Time.deltaTime;
        rb.MovePosition(rb.position + desplazamientoNormal);

        animator.SetFloat(
            "Speed",
            movimiento.magnitude * velocidad
        );
    }
}
