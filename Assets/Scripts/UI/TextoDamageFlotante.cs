using UnityEngine;
using UnityEngine.UI;

public class TextoDamageFlotante : MonoBehaviour
{
    public Text componenteTexto;
    public float velocidadSubida = 1.5f;
    public float tiempoVida = 1.0f;

    private float cronometro = 0f;
    private Color colorOriginal;

    public void Inicializar(float valorDamage)
    {
        if (componenteTexto == null) componenteTexto = GetComponent<Text>();

        componenteTexto.text = valorDamage.ToString();
        colorOriginal = componenteTexto.color;
    }

    void Update()
    {
        cronometro += Time.deltaTime;

        // Movimiento flotante hacia arriba
        transform.position += Vector3.up * velocidadSubida * Time.deltaTime;

        // Desvanecimiento suave calculando el Alpha
        float progresoAlpha = 1f - (cronometro / tiempoVida);
        componenteTexto.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, Mathf.Clamp01(progresoAlpha));

        if (cronometro >= tiempoVida)
        {
            Destroy(gameObject);
        }
    }
}
