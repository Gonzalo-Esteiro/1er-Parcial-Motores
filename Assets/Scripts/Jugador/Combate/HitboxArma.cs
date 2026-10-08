using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxArma : MonoBehaviour
{
    [Header("Ajustes de Impacto")]
    public float damageGolpe = 20f; // Mantenemos tu variable original

    private Collider colliderArma;

    void Start()
    {
        colliderArma = GetComponent<Collider>();
        DesactivarHitbox();
    }

    private void OnTriggerEnter(Collider other)
    {
        // FILTRO 1: Ignoramos al propio jugador (Hunter) para evitar el suicidio accidental
        if (other.CompareTag("Player") || other.name.Contains("Hunter"))
        {
            return;
        }

        // FILTRO 2: Ignoramos el escenario (Suelo, paredes, etc.)
        if (other.name.ToLower().Contains("piso") || other.name.ToLower().Contains("pared") || other.name.ToLower().Contains("ground"))
        {
            return;
        }

        // Alerta de diagnóstico
        Debug.Log($"[COLISIÓN VALIDADA] El arma {gameObject.name} golpeó a: {other.name}");

        // 1. Buscamos el receptor tradicional para mantener tu lógica de destrucción activa
        ReceptorImpacto receptor = other.GetComponent<ReceptorImpacto>();

        // =========================================================================
        // CONEXIÓN CLAVE CON LA IA: Buscamos si el objetivo tiene el script del Monstruo
        // =========================================================================
        EnemigoIA scriptEnemigo = other.GetComponent<EnemigoIA>();
        if (scriptEnemigo != null)
        {
            // Le enviamos el damage directo a la IA para que controle la UI y los cambios de música
            scriptEnemigo.RecibirDamageMonstruo(damageGolpe);
            Debug.Log($"[IA COMBATE] Damage de {damageGolpe} enviado exitosamente al sistema de música del enemigo.");
        }
        // =========================================================================

        if (receptor != null)
        {
            Debug.Log($"[HIT] ¡Objetivo válido confirmado! Aplicando {damageGolpe} de damage a {other.name}.");
            receptor.RecibirGolpe(damageGolpe);
        }
    }

    public void ActivarHitbox()
    {
        if (colliderArma != null) colliderArma.enabled = true;
    }

    public void DesactivarHitbox()
    {
        if (colliderArma != null) colliderArma.enabled = false;
    }
}