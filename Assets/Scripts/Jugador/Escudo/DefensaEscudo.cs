using UnityEngine;

public class DefensaEscudo : MonoBehaviour
{
    [Header("Ajustes de Guardia")]
    [Range(0f, 1f)]
    public float reduccionDanio = 0.20f;

    private ModoCombate modoCombate;

    void Start()
    {
        
        modoCombate = GetComponent<ModoCombate>();
    }

    public float EvaluarGuardia(float damageOriginal, out bool debecancelarHit)
    {
        
        debecancelarHit = false;

       
        if (modoCombate != null && modoCombate.cubriendose)
        {
            
            float damageMitigado = damageOriginal * reduccionDanio;
            float damageFinal = damageOriginal - damageMitigado;
            

            
            debecancelarHit = true;

            Debug.Log($"Ataque de {damageOriginal} fue bloqueado a {damageFinal}.");
            return damageFinal;
        }

        return damageOriginal;
    }
}