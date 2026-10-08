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

    void Start()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // --------------------------------
        // DESENFUNDE
        // --------------------------------
        if (!enCombate && !desenfundando && !enfundando && Input.GetMouseButtonDown(0))
        {
            Desenfundar();
        }

        // Enfundar el arma cuando se presiona la tecla Shift, importante en futuros combates para recuperarse y tener movilidad
        if (enCombate && !atacando && !cubriendose && !enfundando && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Enfundar();
        }

        // Cubrirse con el escudo mientras se mantiene presionada la tecla R
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

        // Esto busca que el jugador no pueda atacar mientras desenfunda, enfunda, se cubre o ya está atacando
        if (enCombate && !desenfundando && !enfundando && !cubriendose && !atacando)
        {
            // Espada - Attack1
            if (Input.GetMouseButtonDown(0))
            {
                AtacarEspada();
            }

            // Escudo - Attack3
            if (Input.GetMouseButtonDown(1))
            {
                AtacarEscudo();
            }
        }
    }

    // Enfundar y Desenfundar Espada
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

    // Ataques con Espada y Escudo
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

    // Hitbox de ambas piezas de ataque (Espada y Escudo)
    public void ActivarHitbox() => Debug.Log("Hitbox ACTIVADA");
    public void DesactivarHitbox() => Debug.Log("Hitbox DESACTIVADA");

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

    // Confirmar sí el jugador puede moverse o no
    public bool PuedeMoverse()
    {
        if (desenfundando    || cubriendose || atacando)
            return false;

        return true;
    }

    public bool PuedeRodar()
    {
        if (desenfundando || enfundando || cubriendose || atacando)
            return false;

        return true;
    }
   
}