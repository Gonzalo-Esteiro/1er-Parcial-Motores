using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfazJugador : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image barraStaminaFill;

    [Header("Ajustes de Stamina")]
    public float staminaMaxima = 100f;
    public float costoRodar = 25f;
    public float costoCorrerPorSegundo = 10f;
    public float velocidadRegeneracion = 20f;

    [Header("Ajustes de Números de Daño")]
    public GameObject prefabTextoDamage;
    public Vector3 offsetEnemigo = new Vector3(0, 2f, 0);
    public Transform canvasPrincipal;

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

    public void CrearNumeroDamage(float valorDamage, Vector3 posicionMundoEnemigo)
    {
        if (prefabTextoDamage == null) return;

        if (canvasPrincipal == null)
        {
            Canvas objCanvas = Object.FindFirstObjectByType<Canvas>();
            if (objCanvas != null) canvasPrincipal = objCanvas.transform;
        }

        // 1. Calculamos la posición real en el espacio 3D sumando la altura (offset) sobre el enemigo
        Vector3 posicionMundoFinal = posicionMundoEnemigo + offsetEnemigo;

        // 2. CONVERSIÓN CRÍTICA: Transformamos la posición 3D del mundo a la posición 2D de la pantalla del jugador
        Vector3 posicionPantalla = Camera.main.WorldToScreenPoint(posicionMundoFinal);

        // Si el enemigo está detrás de la cámara, ignoramos el renderizado para evitar glitches visuales
        if (posicionPantalla.z < 0) return;

        // 3. Instanciamos el texto flotante directamente dentro del Canvas
        GameObject clonTexto = Instantiate(prefabTextoDamage, canvasPrincipal);

        // 4. Asignamos de forma exacta la posición en coordenadas de pantalla (2D)
        clonTexto.transform.position = posicionPantalla;

        TextoDamageFlotante scriptTexto = clonTexto.GetComponent<TextoDamageFlotante>();
        if (scriptTexto != null)
        {
            scriptTexto.Inicializar(valorDamage);
        }
    }


}
