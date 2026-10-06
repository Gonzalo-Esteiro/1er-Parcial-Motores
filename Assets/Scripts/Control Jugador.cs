using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    public float velocidadCaminar = 2.0f;
    public float velocidadCorrer = 5.0f;

    public float distanciaRodar = 3.0f;
    public float duracionRodar = 0.5f;

    public CamaraPrincipal camara;

    private Animator animator;

    private bool rodando = false;
    private float tiempoRodar = 0.0f;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        
        if (rodando)
        {
            tiempoRodar += Time.deltaTime;

            transform.position += transform.forward *
                                  (distanciaRodar / duracionRodar) *
                                  Time.deltaTime;

            if (tiempoRodar >= duracionRodar)
            {
                rodando = false;
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rodando = true;
            tiempoRodar = 0.0f;

            animator.SetTrigger("Roll");

            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direccionCamara = camara.ObtenerDireccion();

        Vector3 derechaCamara = Vector3.Cross(
            Vector3.up,
            direccionCamara
        );

        Vector3 movimiento =
            direccionCamara * vertical +
            derechaCamara * horizontal;

        movimiento = Vector3.ClampMagnitude(
            movimiento,
            1.0f
        );
        float velocidad = velocidadCaminar;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidad = velocidadCorrer;
        }

        if (movimiento != Vector3.zero)
        {
            Quaternion rotacionObjetivo =
                Quaternion.LookRotation(movimiento);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                10.0f * Time.deltaTime
            );
        }

        transform.position +=
            movimiento * velocidad * Time.deltaTime;

        animator.SetFloat(
            "Speed",
            movimiento.magnitude * velocidad
        );
    }
}