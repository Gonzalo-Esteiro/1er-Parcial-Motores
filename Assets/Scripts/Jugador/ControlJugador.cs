using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    public ModoCombate modoCombate;
    public InterfazJugador interfazUI;

    public float velocidadCaminar = 2.0f;
    public float velocidadCorrer = 5.0f;

    [Header("Ajustes del Roll")]
    public float distanciaRodar = 5.0f;
    public float duracionRodar = 0.5f;
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
            float progresoNormalizado = Mathf.Clamp01(tiempoRodar / duracionRodar);
            float multiplicadorCurva = curvaVelocidadRoll.Evaluate(progresoNormalizado);
            float velocidadBase = distanciaRodar / duracionRodar;

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

        // Control de comando para el Roll (Gasta estamina fija de golpe)
        if (Input.GetKeyDown(KeyCode.Space) && modoCombate.PuedeRodar() && interfazUI != null && interfazUI.ConsumirStaminaRodar())
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

        if (!modoCombate.PuedeMoverse())
        {
            animator.SetFloat("Speed", 0.5f);
            
            if (interfazUI != null) interfazUI.ManejarEstamina(false, false);
            return;
        }

        // Esto busca si el jugador está intentando correr y si tiene estamina suficiente para hacerlo
        bool intentandoCorrer = Input.GetKey(KeyCode.LeftShift) && movimiento != Vector3.zero;
        bool puedeCorrer = intentandoCorrer && interfazUI != null && interfazUI.TieneEstaminaParaCorrer();

        float velocidad = puedeCorrer ? velocidadCorrer : velocidadCaminar;

        // Avisamos a la UI del estado actual para que reste o sume energía en este frame
        if (interfazUI != null)
        {
            interfazUI.ManejarEstamina(puedeCorrer, movimiento != Vector3.zero);
        }

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