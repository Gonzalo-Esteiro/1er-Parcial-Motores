using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceptorImpacto : MonoBehaviour
{
    [Header("Ajustes de Vida")]
    public float maximoImpactoSoportado = 1000f;
    private float impactoAcumulado = 0f;

    [Header("Referencias del Sistema")]
    private InterfazJugador interfazUI;

    void Start()
    {
        interfazUI = Object.FindFirstObjectByType<InterfazJugador>();
    }

    // Esta función la llamará la Hitbox de tu espada/escudo al colisionar
    public void RecibirGolpe(float cantidadDamage)
    {
        if (impactoAcumulado >= maximoImpactoSoportado) return;

        impactoAcumulado += cantidadDamage;

        // Enviamos el valor a la UI para generar el número flotante
        if (interfazUI != null)
        {
            interfazUI.CrearNumeroDamage(cantidadDamage, transform.position);
        }

        Debug.Log($"{gameObject.name} recibió {cantidadDamage} de daño. Total acumulado: {impactoAcumulado}/100");

        if (impactoAcumulado >= maximoImpactoSoportado)
        {
            DesaparecerEnemigo();
        }
    }

    void DesaparecerEnemigo()
    {
        Debug.Log($"{gameObject.name} ha sido derrotado.");
        Destroy(gameObject);
    }
}