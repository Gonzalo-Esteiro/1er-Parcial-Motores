using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterfazJugador : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public float vidaMaxima = 100f;
    public int vidasRestantes = 3;
    private float vidaActual;

    [Header("Estadísticas de Stamina")]
    public float staminaMaxima = 100f;
    public float costoRodar = 25f;
    public float costoCorrerPorSegundo = 10f;
    public float velocidadRegeneracion = 20f;
    public float recuperacionFatiga = 10f;
    private float staminaActual;
    private bool fatigado = false;

    [Header("Interfaz Gráfica (UI)")]
    public Image barraVidaUI;
    public Image barraStaminaFill;
    public Transform canvasPrincipal;

    [Header("Ajustes de Números de Daño")]
    public GameObject prefabTextoDamage;
    public Vector3 offsetEnemigo = new Vector3(0, 2f, 0);

    [Header("Estados y Físicas")]
    public bool esInvulnerable = false;
    private bool estaMuerto = false;

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        
        vidaActual = vidaMaxima;
        ActualizarVisualVidaUI();

        
        staminaActual = staminaMaxima;
        ActualizarVisualStaminaUI();
    }
    private void Morir()
    {
        estaMuerto = true;
        vidasRestantes--;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        
        if (barraVidaUI != null)
        {
            barraVidaUI.fillAmount = 0f;
        }

        
        if (MusicManager.Instancia != null)
        {
            if (vidasRestantes > 0)
            {
                
                MusicManager.Instancia.CambiarTema(MusicManager.Instancia.musicaDesmayo);
            }
            else
            {
                
                MusicManager.Instancia.CambiarTema(MusicManager.Instancia.musicaDerrota);
            }
        }

        
        StartCoroutine(SecuenciaReiniciar());
    }
    private IEnumerator SecuenciaReiniciar()
    {
        
        yield return new WaitForSeconds(6.0f);

        if (vidasRestantes > 0)
        {
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            Debug.Log("GAME OVER DEFINITIVO - El cazador se ha quedado sin vidas en la expedición.");
            
        }
    }
    public void RecibirDamage(float cantidad, Vector3 direccionAtaque)
    {
        if (estaMuerto || esInvulnerable) return;

        bool cancelarAnimacionHit = false;
        DefensaEscudo escudo = GetComponent<DefensaEscudo>();
        
        if (escudo != null)
        {
            cantidad = escudo.EvaluarGuardia(cantidad, out cancelarAnimacionHit);
        }

        vidaActual -= cantidad;
        ActualizarVisualVidaUI();

        Debug.Log("Cazador, Vida: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
        else
        {
            
            if (!cancelarAnimacionHit)
            {
                if (animator != null) animator.SetTrigger("Hit");
                if (rb != null) rb.AddForce(-direccionAtaque * 5f, ForceMode.Impulse);
            }
            else
            {
                
            }
        }
    }

    void ActualizarVisualVidaUI()
    {
        if (barraVidaUI != null)
        {
            barraVidaUI.fillAmount = vidaActual / vidaMaxima;
        }
    }
    public void ManejarEstamina(bool estaCorriendo, bool moviendose)
    {
        if (estaCorriendo && moviendose && !fatigado)
        {
            staminaActual -= costoCorrerPorSegundo * Time.deltaTime;

            if (staminaActual <= 0f)
            {
                fatigado = true;
            }
        }
        else if (staminaActual < staminaMaxima)
        {
            staminaActual += velocidadRegeneracion * Time.deltaTime;

            if (fatigado && staminaActual >= recuperacionFatiga)
            {
                fatigado = false;
            }
        }
        else if (staminaActual > staminaMaxima)
        {
            staminaActual = staminaMaxima;
        }

        staminaActual = Mathf.Clamp(staminaActual, 0f, staminaMaxima);
        ActualizarVisualStaminaUI();
    }

    public bool ConsumirStaminaRodar()
    {
        if (staminaActual >= costoRodar)
        {
            staminaActual -= costoRodar;

            if (staminaActual <= 0f)
            {
                fatigado = true;
            }

            staminaActual = Mathf.Clamp(staminaActual, 0f, staminaMaxima);
            ActualizarVisualStaminaUI();
            return true;
        }
        return false;
    }

    public bool TieneEstaminaParaCorrer()
    {
        return !fatigado && staminaActual > 0f;
    }

    void ActualizarVisualStaminaUI()
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

        Vector3 posicionMundoFinal = posicionMundoEnemigo + offsetEnemigo;
        Vector3 posicionPantalla = Camera.main.WorldToScreenPoint(posicionMundoFinal);

        if (posicionPantalla.z < 0) return;

        GameObject clonTexto = Instantiate(prefabTextoDamage, canvasPrincipal);
        clonTexto.transform.position = posicionPantalla;

        TextoDamageFlotante scriptTexto = clonTexto.GetComponent<TextoDamageFlotante>();
        if (scriptTexto != null)
        {
            scriptTexto.Inicializar(valorDamage);
        }
    }
    public bool EsSaberSiEstaMuerto()
    {
        return estaMuerto;
    }

    private bool bloqueadoPorHit = false;

    public bool RecibiendoHit()
    {
        return bloqueadoPorHit;
    }

    public void IniciarBloqueoHit()
    {
        bloqueadoPorHit = true;
    }

    public void TerminarBloqueoHit()
    {
        bloqueadoPorHit = false;
    }
}
