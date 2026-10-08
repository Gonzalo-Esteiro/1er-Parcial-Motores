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

    public float recuperacionFatiga = 10f;
    private float staminaActual;
    private bool fatigado = false;
    void Start()
    {
        staminaActual = staminaMaxima;
        ActualizarVisualUI();
    }


    public void ManejarEstamina(bool estaCorriendo, bool moviendose)
    {
        if (estaCorriendo && moviendose && !fatigado)
        {
            staminaActual -= costoCorrerPorSegundo * Time.deltaTime;

            // Si tocamos el fondo absoluto, entramos en estado de fatiga inmediatamente
            if (staminaActual <= 0f)
            {
                fatigado = true;
            }
        }
        else if (staminaActual < staminaMaxima)
        {
            staminaActual += velocidadRegeneracion * Time.deltaTime;

            // Desactivamos la fatiga SOLO cuando la energía supera el umbral de seguridad (ej: 10f)
            if (fatigado && staminaActual >= recuperacionFatiga)
            {
                fatigado = false;
            }
        }

        staminaActual = Mathf.Clamp(staminaActual, 0f, staminaMaxima);
        ActualizarVisualUI();
    }

    // Comprobación para el comando Roll
    public bool ConsumirStaminaRodar()
    {
        if (staminaActual >= costoRodar)
        {
            staminaActual -= costoRodar;

            // Rodar también puede gatillar la fatiga si te deja en 0
            if (staminaActual <= 0f)
            {
                fatigado = true;
            }

            staminaActual = Mathf.Clamp(staminaActual, 0f, staminaMaxima);
            ActualizarVisualUI();
            return true;
        }
        return false;
    }

    // El script de movimiento ahora consulta directamente si está fatigado
    public bool TieneEstaminaParaCorrer()
    {
        return !fatigado && staminaActual > 0f;
    }

    void ActualizarVisualUI()
    {
        if (barraStaminaFill != null)
        {
            barraStaminaFill.fillAmount = staminaActual / staminaMaxima;
        }
    }
}
