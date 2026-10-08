using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    public ModoCombate modoCombate;

    public float velocidadCaminar = 2.0f;
    public float velocidadCorrer = 5.0f;

    [Header("Ajustes del Roll")]
    public float distanciaRodar = 3.0f;
    public float duracionRodar = 0.5f;

    [Tooltip("Diseña la curva: Empieza alto (ej: 2.0) y termina en 0 para simular la pérdida de fuerza.")]
    public AnimationCurve curvaVelocidadRoll = AnimationCurve.Linear(0, 2, 1, 0);

    public CamaraPrincipal camara;

    private Animator animator;
    private Rigidbody rb;

    private bool rodando = false;
    private float tiempoRodar = 0.0f;
    private Vector3 direccionRodado;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        if (modoCombate == null)
            modoCombate = GetComponent<ModoCombate>();
    }

    void Update()
    {
        if (rodando)
        {
            tiempoRodar += Time.deltaTime;

            // 1. Calculamos el progreso actual del Roll entre 0.0 (inicio) y 1.0 (fin)
            float progresoNormalizado = Mathf.Clamp01(tiempoRodar / duracionRodar);

            // 2. Evaluamos la curva en base al progreso para obtener el multiplicador de fuerza actual
            float multiplicadorCurva = curvaVelocidadRoll.Evaluate(progresoNormalizado);

            // 3. Calculamos la velocidad base necesaria para cubrir la distancia objetivo
            float velocidadBase = distanciaRodar / duracionRodar;

            // 4. Aplicamos el desplazamiento afectado por la curva (mucha fuerza al inicio, poca al final)
            Vector3 desplazamientoRodar = direccionRodado * (velocidadBase * multiplicadorCurva) * Time.deltaTime;
            rb.MovePosition(rb.position + desplazamientoRodar);

            if (tiempoRodar >= duracionRodar)
            {
                rodando = false;
            }
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direccionCamara = camara.ObtenerDireccion();
        Vector3 derechaCamara = Vector3.Cross(Vector3.up, direccionCamara);

        Vector3 movimiento = direccionCamara * vertical + derechaCamara * horizontal;
        movimiento = Vector3.ClampMagnitude(movimiento, 1.0f);

        // Comprobación de Roll mediante permisos de ModoCombate
        if (Input.GetKeyDown(KeyCode.Space) && modoCombate.PuedeRodar())
        {
            rodando = true;
            tiempoRodar = 0.0f;

            if (movimiento != Vector3.zero)
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

        // Comprobación de Movimiento mediante permisos de ModoCombate
        if (!modoCombate.PuedeMoverse())
        {
            animator.SetFloat("Speed", 0.5f);
            return;
        }

        float velocidad = Input.GetKey(KeyCode.LeftShift) ? velocidadCorrer : velocidadCaminar;

        if (movimiento != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(movimiento);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 10.0f * Time.deltaTime);
        }

        Vector3 desplazamientoNormal = movimiento * velocidad * Time.deltaTime;
        rb.MovePosition(rb.position + desplazamientoNormal);

        animator.SetFloat("Speed", movimiento.magnitude * velocidad);
    }
}