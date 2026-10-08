using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxArma : MonoBehaviour
{
    [Header("Ajustes de Impacto")]
    public float damageGolpe = 20f;

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

        // Alerta de diagnóstico: Solo nos avisará si golpeamos un objeto interactivo real
        Debug.Log($"[COLISIÓN VALIDADA] El arma {gameObject.name} golpeó a: {other.name}");

        ReceptorImpacto receptor = other.GetComponent<ReceptorImpacto>();

        if (receptor != null)
        {
            Debug.Log($"[HIT] ¡Objetivo válido confirmado! Aplicando {damageGolpe} de daño a {other.name}.");
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