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

    private InterfazJugador interfaz;

    private bool rodando = false;
    private float tiempoRodar = 0.0f;
    private Vector3 direccionRodado;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        // Buscamos el componente unificado en el mismo objeto
        interfaz = GetComponent<InterfazJugador>();
    }

    void Update()
    {
        if (rodando)
        {
            tiempoRodar += Time.deltaTime;

            Vector3 desplazamientoRodar = direccionRodado * (distanciaRodar / duracionRodar) * Time.deltaTime;
            rb.MovePosition(rb.position + desplazamientoRodar);

            if (tiempoRodar >= duracionRodar)
            {
                rodando = false;
            }

            return;
        }

        // Calcular los vectores de movimiento normales
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direccionCamara = camara.ObtenerDireccion();
        Vector3 derechaCamara = Vector3.Cross(Vector3.up, direccionCamara);

        Vector3 movimiento = direccionCamara * vertical + derechaCamara * horizontal;
        movimiento = Vector3.ClampMagnitude(movimiento, 1.0f);

        bool seEstaMoviendo = movimiento.magnitude > 0.1f;
        bool quiereCorrer = Input.GetKey(KeyCode.LeftShift);

        if (interfaz != null)
        {
            // Le pasamos los datos a la interfaz para que reduzca o regenere la barra
            interfaz.ManejarEstamina(quiereCorrer, seEstaMoviendo);
        }

        // Detectar comando para Rodar (Roll) con validación de Estamina
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Solo rueda si la interfaz confirma que hay suficiente energía
            if (interfaz != null && interfaz.ConsumirStaminaRodar())
            {
                rodando = true;
                tiempoRodar = 0.0f;

                if (seEstaMoviendo)
                {
                    direccionRodado = movimiento.normalized;
                    transform.rotation = Quaternion.LookRotation(direccionRodado);
                }
                else
                {
                    direccionRodado = transform.forward;
                }

                animator.SetTrigger("Roll");
                return;
            }
        }

        // Lógica normal de movimiento (Caminar / Correr)
        float velocidad = velocidadCaminar;

        // Solo corre si presionas Shift Y ADEMÁS la interfaz dice que no estás fatigado
        if (quiereCorrer && interfaz != null && interfaz.TieneEstaminaParaCorrer())
        {
            velocidad = velocidadCorrer;
        }

        if (seEstaMoviendo)
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

    // Invincibility Frames
    public void IniciarIFrames()
    {
        if (interfaz != null) interfaz.esInvulnerable = true;
    }

    public void TerminarIFrames()
    {
        if (interfaz != null) interfaz.esInvulnerable = false;
    }
}