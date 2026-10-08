using UnityEngine;

public class ControlJugador : MonoBehaviour
{
    public float velocidadCaminar = 2.0f;
    public float velocidadCorrer = 5.0f;

    [Header("Ajustes del Roll")]
    public float distanciaRodar = 3.0f;
    public float duracionRodar = 0.5f;
    public AnimationCurve curvaVelocidadRoll = AnimationCurve.Linear(0, 2, 1, 0);

    public CamaraPrincipal camara;

    private Animator animator;
    private Rigidbody rb;

    private InterfazJugador interfaz;

    private bool rodando = false;
    private float tiempoRodar = 0.0f;
    private Vector3 direccionRodado;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        
        interfaz = GetComponent<InterfazJugador>();
    }

    void Update()
    {
        if (rodando)
        {
            tiempoRodar += Time.deltaTime;
            float progresoNormalizado = Mathf.Clamp01(tiempoRodar / duracionRodar);
            float multiplicadorCurva = curvaVelocidadRoll.Evaluate(progresoNormalizado);
            float velocidadBase = distanciaRodar / duracionRodar;

            Vector3 desplazamientoRodar = direccionRodado * (velocidadBase * multiplicadorCurva) * Time.deltaTime;
            rb.MovePosition(rb.position + desplazamientoRodar);

            if (tiempoRodar >= duracionRodar)
            {
                rodando = false;
            }
            return;
        }


        
        ModoCombate combate = GetComponent<ModoCombate>();

        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direccionCamara = camara.ObtenerDireccion();
        Vector3 derechaCamara = Vector3.Cross(Vector3.up, direccionCamara);

        Vector3 movimiento = direccionCamara * vertical + derechaCamara * horizontal;
        movimiento = Vector3.ClampMagnitude(movimiento, 1.0f);

        if (combate != null && !combate.PuedeMoverse())
        {
            movimiento = Vector3.zero;
        }
        

        bool seEstaMoviendo = movimiento.magnitude > 0.1f;
        bool quiereCorrer = Input.GetKey(KeyCode.LeftShift);

        if (interfaz != null)
        {
            
            interfaz.ManejarEstamina(quiereCorrer, seEstaMoviendo);
        }

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            if (combate != null && combate.PuedeRodar())
            {
                
                if (interfaz != null && interfaz.ConsumirStaminaRodar())
                {
                    rodando = true;
                    tiempoRodar = 0.0f;

                    if (seEstaMoviendo)
                    {
                        direccionRodado = movimiento.normalized;
                        transform.rotation = Quaternion.LookRotation(direccionRodado);
                    }
                    else
                    {
                        direccionRodado = transform.forward;
                    }

                    animator.SetTrigger("Roll");
                    return;
                }
            }
        }

        
        float velocidad = velocidadCaminar;

        
        if (quiereCorrer && interfaz != null && interfaz.TieneEstaminaParaCorrer())
        {
            velocidad = velocidadCorrer;
        }

        if (seEstaMoviendo)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(movimiento);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                10.0f * Time.deltaTime
            );
        }

        Vector3 desplazamientoNormal = movimiento * velocidad * Time.deltaTime;
        rb.MovePosition(rb.position + desplazamientoNormal);

        animator.SetFloat(
            "Speed",
            movimiento.magnitude * velocidad
        );
    }
    public void IniciarIFrames()
    {
        if (interfaz != null) interfaz.esInvulnerable = true;
    }

    public void TerminarIFrames()
    {
        if (interfaz != null) interfaz.esInvulnerable = false;
    }

    public void IniciarBloqueoHitDesdeAnimacion()
    {
        if (interfaz != null) interfaz.IniciarBloqueoHit();
    }

    public void TerminarBloqueoHitDesdeAnimacion()
    {
        if (interfaz != null) interfaz.TerminarBloqueoHit();
    }
}