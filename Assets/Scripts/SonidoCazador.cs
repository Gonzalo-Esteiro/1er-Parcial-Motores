using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAudioEvents : MonoBehaviour
{
    [Header("Fuentes de Audio (Movimiento)")]
    [SerializeField] private AudioSource sourceFootsteps;
    [SerializeField] private AudioSource sourceRoll;
    [SerializeField] private AudioSource sourcePosRoll;

    [Header("Fuentes de Audio (Arma)")]
    [SerializeField] private AudioSource sourceDesenfunde;
    [SerializeField] private AudioSource sourceEnfunde;
    [SerializeField] private AudioSource sourceAttack1;
    [SerializeField] private AudioSource sourceAttack3;

    [Header("Clips de Sonido (Movimiento)")]
    public AudioClip audioPaso;
    public AudioClip audioRoll;
    public AudioClip audioPosRoll;

    [Header("Clips de Sonido (Arma)")]
    public AudioClip audioDesenfunde;
    public AudioClip audioEnfunde;
    public AudioClip audioAttack1;
    public AudioClip audioAttack3;

    [Header("Ajustes Antispam")]
    [SerializeField] private float cooldownPasos = 0.25f; // Tiempo mínimo entre pisadas
    [SerializeField] private float cooldownAtaques = 0.15f; // Evita ráfagas si la animación se corta rápido

    private float tiempoUltimoPaso;
    private float tiempoUltimoAtaque;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // =========================================================================
    // EVENTOS DE AUDIO: MOVIMIENTO
    // =========================================================================

    public void EjecutarFootstep()
    {
        if (Time.time - tiempoUltimoPaso < cooldownPasos) return;
        if (animator != null && animator.GetFloat("Speed") < 0.2f) return;

        if (sourceFootsteps != null && audioPaso != null)
        {
            sourceFootsteps.pitch = Random.Range(0.85f, 1.15f);
            sourceFootsteps.PlayOneShot(audioPaso);
            tiempoUltimoPaso = Time.time;
        }
    }

    public void EjecutarRoll()
    {
        if (sourceRoll != null && audioRoll != null)
        {
            sourceRoll.pitch = Random.Range(0.95f, 1.05f);
            sourceRoll.PlayOneShot(audioRoll);
        }
    }

    public void EjecutarPosRoll()
    {
        if (sourcePosRoll != null && audioPosRoll != null)
        {
            sourcePosRoll.pitch = Random.Range(0.95f, 1.05f);
            sourcePosRoll.PlayOneShot(audioPosRoll);
        }
    }

    // =========================================================================
    // EVENTOS DE AUDIO NUEVOS: HOLDER ARMA
    // =========================================================================

    // Se ejecuta al inicio de la animación de desenfundar
    public void EjecutarDesenfunde()
    {
        if (sourceDesenfunde != null && audioDesenfunde != null)
        {
            sourceDesenfunde.pitch = Random.Range(0.95f, 1.05f); // Variación sutil mecánica
            sourceDesenfunde.PlayOneShot(audioDesenfunde);
        }
    }

    // Se ejecuta al iniciar la animación de guardar el arma
    public void EjecutarEnfunde()
    {
        if (sourceEnfunde != null && audioEnfunde != null)
        {
            sourceEnfunde.pitch = Random.Range(0.95f, 1.05f);
            sourceEnfunde.PlayOneShot(audioEnfunde);
        }
    }

    // Se ejecuta en el frame de impacto o swing del Ataque 1
    public void EjecutarAttack1()
    {
        if (Time.time - tiempoUltimoAtaque < cooldownAtaques) return;

        if (sourceAttack1 != null && audioAttack1 != null)
        {
            sourceAttack1.pitch = Random.Range(0.9f, 1.1f); // Un poco más de variación para los golpes
            sourceAttack1.PlayOneShot(audioAttack1);
            tiempoUltimoAtaque = Time.time;
        }
    }

    // Se ejecuta en el frame de impacto o swing del Ataque 3 (Golpe fuerte)
    public void EjecutarAttack3()
    {
        if (Time.time - tiempoUltimoAtaque < cooldownAtaques) return;

        if (sourceAttack3 != null && audioAttack3 != null)
        {
            sourceAttack3.pitch = Random.Range(0.9f, 1.1f);
            sourceAttack3.PlayOneShot(audioAttack3);
            tiempoUltimoAtaque = Time.time;
        }
    }
}