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
    [SerializeField] private float cooldownPasos = 0.25f; 
    [SerializeField] private float cooldownAtaques = 0.15f;

    private float tiempoUltimoPaso;
    private float tiempoUltimoAtaque;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void EjecutarFootstep()
    {
        if (Time.time - tiempoUltimoPaso < cooldownPasos) return;
        if (animator != null && animator.GetFloat("Speed") < 0.2f) return;

        if (sourceFootsteps != null && audioPaso != null)
        {
            sourceFootsteps.PlayOneShot(audioPaso);
            tiempoUltimoPaso = Time.time;
        }
    }

    public void EjecutarRoll()
    {
        if (sourceRoll != null && audioRoll != null)
        {
            sourceRoll.PlayOneShot(audioRoll);
        }
    }

    public void EjecutarPosRoll()
    {
        if (sourcePosRoll != null && audioPosRoll != null)
        {
            sourcePosRoll.PlayOneShot(audioPosRoll);
        }
    }

    // Se ejecuta al inicio de la animación de desenfundar
    public void EjecutarDesenfunde()
    {
        if (sourceDesenfunde != null && audioDesenfunde != null)
        {
            sourceDesenfunde.PlayOneShot(audioDesenfunde);
        }
    }

    // Se ejecuta al iniciar la animación de guardar el arma
    public void EjecutarEnfunde()
    {
        if (sourceEnfunde != null && audioEnfunde != null)
        {
            sourceEnfunde.PlayOneShot(audioEnfunde);
        }
    }

    // Se ejecuta en el swing del Ataque 1
    public void EjecutarAttack1()
    {
        if (Time.time - tiempoUltimoAtaque < cooldownAtaques) return;

        if (sourceAttack1 != null && audioAttack1 != null)
        {
            sourceAttack1.PlayOneShot(audioAttack1);
            tiempoUltimoAtaque = Time.time;
        }
    }

    // Se ejecuta en el frame de impacto del Ataque 3
    public void EjecutarAttack3()
    {
        if (Time.time - tiempoUltimoAtaque < cooldownAtaques) return;

        if (sourceAttack3 != null && audioAttack3 != null)
        {
            sourceAttack3.PlayOneShot(audioAttack3);
            tiempoUltimoAtaque = Time.time;
        }
    }
}