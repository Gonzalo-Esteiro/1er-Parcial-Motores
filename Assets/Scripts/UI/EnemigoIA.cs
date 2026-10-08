using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoIA : MonoBehaviour
{
    public enum EstadoEnemigo { Oculto, Persiguiendo, PreparandoCarga, Cargando, Recuperandose, Muerto }
    public EstadoEnemigo estadoActual = EstadoEnemigo.Oculto;

    [Header("Referencias")]
    public Transform jugador;
    public float vidaEnemigo = 100f;

    [Header("Rangos y Velocidades")]
    public float distanciaDeteccion = 15f;
    public float distanciaAtaque = 4f;
    public float velocidadPersecucion = 3.5f;

    [Header("Ajustes de Carga")]
    public float velocidadPasoAtras = 1.5f;
    public float fuerzaEmbestida = 15f;
    public float danioCarga = 25f;

    private Rigidbody rb;
    private Animator animator;
    private bool musicaCombateIniciada = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        if (jugador == null) jugador = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (estadoActual == EstadoEnemigo.Muerto || jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        switch (estadoActual)
        {
            case EstadoEnemigo.Oculto:
                if (distancia <= distanciaDeteccion)
                {
                    estadoActual = EstadoEnemigo.Persiguiendo;
                    if (!musicaCombateIniciada)
                    {
                        MusicManager.Instancia.CambiarTema(MusicManager.Instancia.musicaCombate);
                        musicaCombateIniciada = true;
                    }
                }
                break;

            case EstadoEnemigo.Persiguiendo:
                MirarAlJugador();
                Vector3 direccion = (jugador.position - transform.position).normalized;
                direccion.y = 0;
                rb.MovePosition(rb.position + direccion * velocidadPersecucion * Time.deltaTime);

                if (distancia <= distanciaAtaque)
                {
                    StartCoroutine(SecuenciaDeCarga());
                }
                break;
        }
    }

    private void MirarAlJugador()
    {
        Vector3 direccionLook = (jugador.position - transform.position).normalized;
        direccionLook.y = 0;
        if (direccionLook != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direccionLook), 5f * Time.deltaTime);
        }
    }

    private IEnumerator SecuenciaDeCarga()
    {
        estadoActual = EstadoEnemigo.PreparandoCarga;
        if (animator) animator.SetTrigger("PrepareCharge");

        float tiempoPreparacion = 0.8f;
        float reloj = 0f;
        while (reloj < tiempoPreparacion)
        {
            reloj += Time.deltaTime;

            // =========================================================================
            // CORRECCIÓN: El monstruo sigue pivotando hacia ti mientras retrocede
            // =========================================================================
            MirarAlJugador();
            // =========================================================================

            rb.MovePosition(rb.position - transform.forward * velocidadPasoAtras * Time.deltaTime);
            yield return null;
        }

        // Lanzarse (Fase Activa de Daño)
        estadoActual = EstadoEnemigo.Cargando;
        if (animator) animator.SetTrigger("LaunchCharge");

        Vector3 direccionCarga = transform.forward;
        rb.AddForce(direccionCarga * fuerzaEmbestida, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.6f);

        rb.velocity = Vector3.zero;
        estadoActual = EstadoEnemigo.Recuperandose;
        yield return new WaitForSeconds(1.5f);

        estadoActual = EstadoEnemigo.Persiguiendo;
    }

    // HITBOX DEL ENEMIGO: Detecta de forma física el impacto contra la Hurtbox
    private void OnCollisionEnter(Collision collision)
    {
        if (estadoActual == EstadoEnemigo.Cargando && collision.gameObject.CompareTag("Player"))
        {
            VidaJugador vida = collision.gameObject.GetComponent<VidaJugador>();
            if (vida != null)
            {
                Vector3 direccionImpacto = (collision.transform.position - transform.position).normalized;
                vida.RecibirDanio(danioCarga, direccionImpacto);
            }
        }
    }

    public void RecibirDanioMonstruo(float cantidad)
    {
        vidaEnemigo -= cantidad;

        // Si le queda poca vida, cambia la música a fase crítica
        if (vidaEnemigo <= 50f && vidaEnemigo > 0)
        {
            MusicManager.Instancia.CambiarTema(MusicManager.Instancia.musicaEnemigoCritico);
        }

        if (vidaEnemigo <= 0 && estadoActual != EstadoEnemigo.Muerto)
        {
            estadoActual = EstadoEnemigo.Muerto;
            if (animator) animator.SetTrigger("Die");

            // Regresa a la música de exploración cuando el enemigo cae
            MusicManager.Instancia.CambiarTema(MusicManager.Instancia.musicaVictoria);
            Debug.Log("¡Monstruo derrotado!");
        }
    }
}