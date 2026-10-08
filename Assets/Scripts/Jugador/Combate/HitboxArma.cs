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
        // Previene la autolesión
        if (other.CompareTag("Player") || other.name.Contains("Hunter"))
        {
            return;
        }

        // Evitamos detectar el escenario
        if (other.name.ToLower().Contains("piso") || other.name.ToLower().Contains("pared") || other.name.ToLower().Contains("ground"))
        {
            return;
        }

        // Alerta de diagnóstico
        Debug.Log($"[COLISIÓN VALIDADA] El arma {gameObject.name} golpeó a: {other.name}");

        ReceptorImpacto receptor = other.GetComponent<ReceptorImpacto>();

        EnemigoIA scriptEnemigo = other.GetComponent<EnemigoIA>();
        if (scriptEnemigo != null)
        {
            scriptEnemigo.RecibirDamageMonstruo(damageGolpe);
            Debug.Log($"[IA COMBATE] Damage de {damageGolpe} enviado exitosamente al sistema de música del enemigo.");
        }

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