using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAudioEvents : MonoBehaviour
{
    [Header("Fuentes de Audio (Holders)")]
    [SerializeField] private AudioSource sourceFootsteps;
    [SerializeField] private AudioSource sourceRoll;
    [SerializeField] private AudioSource sourcePosRoll;

    [Header("Clips de Sonido")]
    public AudioClip audioPaso;
    public AudioClip audioRoll;
    public AudioClip audioPosRoll;

    private float tiempoUltimoPaso;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void EjecutarFootstep()
    {
        if (sourceFootsteps != null && audioPaso != null)
        {
            // Modificamos sutilmente el Pitch (tono) en cada paso para que no suene robótico ni monótono
            sourceFootsteps.pitch = Random.Range(0.85f, 1.15f);

            // PlayOneShot permite reproducir el sonido sin cortar el paso anterior si se llegan a encabalgar
            sourceFootsteps.PlayOneShot(audioPaso);
            tiempoUltimoPaso = Time.time;
        }
    }

    // Se ejecuta mediante un Evento de Animación al inicio del clip de Roll
    public void EjecutarRoll()
    {
        if (sourceRoll != null && audioRoll != null)
        {
            // El Roll es una acción única, no requiere cooldown estricto porque el estado "rodando" 
            // en tu script de movimiento ya bloquea que hagas spam de la barra espaciadora.
            sourceRoll.pitch = Random.Range(0.95f, 1.05f);
            sourceRoll.PlayOneShot(audioRoll);
        }
    }

    // Se ejecuta mediante un Evento de Animación al final del clip de Roll (recuperación)
    public void EjecutarPosRoll()
    {
        if (sourcePosRoll != null && audioPosRoll != null)
        {
            sourcePosRoll.pitch = Random.Range(0.95f, 1.05f);
            sourcePosRoll.PlayOneShot(audioPosRoll);
        }
    }
}
