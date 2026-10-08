using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModoCombate : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public Transform manoDerecha;
    public Transform espadaMango;
    public Transform espada;
    public Transform vaina;

    [Header("Estado")]
    public bool enCombate = false;
    public bool atacando = false;
    public bool cubriendose = false;
    public bool desenfundando = false;
    public bool enfundando = false;

    [Header("Referencias de Hitboxes")]
    public HitboxArma hitboxEspada;
    public HitboxArma hitboxEscudo;

    
    private InterfazJugador interfazJugador;

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        
        interfazJugador = GetComponent<InterfazJugador>();
    }

    void Update()
    {
        
        if (interfazJugador != null && interfazJugador.EsSaberSiEstaMuerto()) return;

        if (!enCombate && !desenfundando && !enfundando && Input.GetMouseButtonDown(0))
        {
            Desenfundar();
        }

        
        if (enCombate && !atacando && !cubriendose && !enfundando && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Enfundar();
        }

        
        if (enCombate && !atacando && !enfundando)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                cubriendose = true;
                animator.SetBool("Guard", true);
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                cubriendose = false;
                animator.SetBool("Guard", false);
            }
        }

        
        if (enCombate && !desenfundando && !enfundando && !cubriendose && !atacando)
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                AtacarEspada();
            }

            
            if (Input.GetMouseButtonDown(1))
            {
                AtacarEscudo();
            }
        }
    }

    
    void Desenfundar()
    {
        desenfundando = true;
        animator.ResetTrigger("Sheathe");
        animator.SetTrigger("Draw");
    }

    public void FinDesenfunde()
    {
        desenfundando = false;
        enCombate = true;
        animator.SetBool("CombatMode", true);
    }

    void Enfundar()
    {
        enfundando = true;
        animator.SetTrigger("Sheathe");
    }

    public void FinEnfunde()
    {
        enfundando = false;
        enCombate = false;
        animator.ResetTrigger("Sheathe");
        animator.SetBool("CombatMode", false);

        ColocarEspadaEnVaina();
    }

    
    void AtacarEspada()
    {
        atacando = true;
        animator.SetTrigger("Attack1");
    }

    void AtacarEscudo()
    {
        atacando = true;
        animator.SetTrigger("Attack3");
    }

    public void AtaqueTerminado()
    {
        atacando = false;
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack3");
    }

    
    public void ActivarHitbox() => Debug.Log("Hitbox ACTIVADA");

    public void ActivarHitboxEspada()
    {
        if (hitboxEspada != null) hitboxEspada.ActivarHitbox();
        Debug.Log("HITBOX ESPADA ACTIVADA");
    }

    public void ActivarHitboxEscudo()
    {
        if (hitboxEscudo != null) hitboxEscudo.ActivarHitbox();
        Debug.Log("HITBOX ESCUDO ACTIVADA");
    }
    public void DesactivarHitbox() => Debug.Log("Hitbox DESACTIVADA");

    public void DesactivarHitboxEspada()
    {
        if (hitboxEspada != null) hitboxEspada.DesactivarHitbox();
        Debug.Log("HITBOX ESPADA DESACTIVADA");
    }

    public void DesactivarHitboxEscudo()
    {
        if (hitboxEscudo != null) hitboxEscudo.DesactivarHitbox();
        Debug.Log("HITBOX ESCUDO DESACTIVADA");
    }

    public void EspadaAMano()
    {
        if (espada == null || espadaMango == null || manoDerecha == null) return;
        espada.SetParent(manoDerecha);
        Quaternion diferenciaRotacion = manoDerecha.rotation * Quaternion.Inverse(espadaMango.rotation);
        espada.rotation = diferenciaRotacion * espada.rotation;
        Vector3 diferenciaPosicion = manoDerecha.position - espadaMango.position;
        espada.position += diferenciaPosicion;
    }

    public void ColocarEspadaEnVaina()
    {
        if (espada == null || vaina == null) return;
        espada.SetParent(vaina);
        espada.localPosition = Vector3.zero;
        espada.localRotation = Quaternion.identity;
    }

    
    public bool PuedeMoverse()
{
    if (interfazJugador != null && (interfazJugador.EsSaberSiEstaMuerto() || interfazJugador.RecibiendoHit()))
        return false;

    if (desenfundando || cubriendose || atacando)
        return false;

    return true;
}

public bool PuedeRodar()
{
    if (interfazJugador != null && (interfazJugador.EsSaberSiEstaMuerto() || interfazJugador.RecibiendoHit()))
        return false;

    if (desenfundando || cubriendose || atacando)
        return false;

    return true;
}


}