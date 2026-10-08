using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    [Header("Estadísticas")]
    public float vidaMaxima = 100f;
    private float vidaActual;
    public int vidasRestantes = 3;

    [Header("Estados")]
    public bool esInvulnerable = false;
    private bool estaMuerto = false;

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    public void RecibirDanio(float cantidad, Vector3 direccionAtaque)
    {
        if (estaMuerto || esInvulnerable) return; // Ignora el golpe si rueda o está muerto

        vidaActual -= cantidad;
        Debug.Log("¡Cazador golpeado! Vida: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            // Reacción de daño: Activa tu animación de caer/tropezar
            animator.SetTrigger("Hit");
            // Pequeño empujón físico hacia atrás basado en el impacto
            rb.AddForce(-direccionAtaque * 1f, ForceMode.Impulse);
        }
    }

    private void Morir()
    {
        estaMuerto = true;
        vidasRestantes--;
        animator.SetTrigger("Die"); // Animación de caer derrotado

        MusicManager.Instancia.CambiarTema(MusicManager.Instancia.musicaDesmayo);
        StartCoroutine(SecuenciaReiniciar());
    }

    private IEnumerator SecuenciaReiniciar()
    {
        yield return new WaitForSeconds(3.0f); // Espera a que termine la animación

        if (vidasRestantes > 0)
        {
            // Repite el loop: Reinicia la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("GAME OVER DEFINITIVO - No quedan vidas");
        }
    }

    // =========================================================================
    // EVENTOS PARA TU ANIMACIÓN DE ROLL (Añádelos directamente en el clip)
    // =========================================================================
    public void IniciarIFrames() { esInvulnerable = true; }
    public void TerminarIFrames() { esInvulnerable = false; }
}