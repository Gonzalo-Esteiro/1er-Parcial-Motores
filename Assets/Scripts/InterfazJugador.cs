using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfazCazador : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image barraStaminaFill;

    [Header("Ajustes de Stamina")]
    public float staminaMaxima = 100f;
    public float costoRodar = 25f;
    public float costoCorrerPorSegundo = 10f;
    public float velocidadRegeneracion = 20f;

    private float staminaActual;

    void Start()
    {
        staminaActual = staminaMaxima;
        ActualizarVisualUI();
    }

    
    public void ManejarEstamina(bool estaCorriendo, bool Moviendose)
    {
        if (estaCorriendo && Moviendose)
        {
            // Restamos estamina en base al tiempo transcurrido
            staminaActual -= costoCorrerPorSegundo * Time.deltaTime;
        }
        else if (staminaActual < staminaMaxima)
        {
            // Si no corre, se regenera normalmente
            staminaActual += velocidadRegeneracion * Time.deltaTime;
        }

        // me aseguro que la stamina nunca llegue a cero ni supere el máximo
        staminaActual = Mathf.Clamp(staminaActual, 0f, staminaMaxima);

        ActualizarVisualUI();
    }

    // Comprobación para el comando Roll
    public bool ConsumirStaminaRodar()
    {
        if (staminaActual >= costoRodar)
        {
            staminaActual -= costoRodar;
            staminaActual = Mathf.Clamp(staminaActual, 0f, staminaMaxima);
            ActualizarVisualUI();
            return true;
        }
        return false;
    }

    // Para confirmar sí el jugador puede correr
    public bool TieneEstaminaParaCorrer()
    {

        return staminaActual > 0.5f; // Deja de correr justo antes de llegar a 0 absoluto
    }

    void ActualizarVisualUI()
    {
        if (barraStaminaFill != null)
        {
            barraStaminaFill.fillAmount = staminaActual / staminaMaxima;
        }
    }
}
