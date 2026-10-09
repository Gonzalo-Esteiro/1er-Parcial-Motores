using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instancia;

    [Header("Temas")]
    public AudioClip musicaNormal;
    public AudioClip musicaCombate;
    public AudioClip musicaEnemigoCritico;
    public AudioClip musicaVictoria;
    public AudioClip musicaDesmayo;
    public AudioClip musicaDerrota;

    private AudioSource audioSource;
    private Coroutine corrutinaFade;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    void Start()
    {
        CambiarTema(musicaNormal);
    }

    public void CambiarTema(AudioClip nuevoTema)
    {
        if (audioSource.clip == nuevoTema) return;

        if (corrutinaFade != null) StopCoroutine(corrutinaFade);
        corrutinaFade = StartCoroutine(TransicionMusical(nuevoTema));
    }

    private IEnumerator TransicionMusical(AudioClip nuevoClip)
    {
        float duracionFade = 0.8f;
        float tiempo = 0f;
        float volumenInicial = audioSource.volume;

        
        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(volumenInicial, 0f, tiempo / duracionFade);
            yield return null;
        }

        audioSource.clip = nuevoClip;

        if (nuevoClip != null)
        {
            audioSource.Play();
            tiempo = 0f;

           
            while (tiempo < duracionFade)
            {
                tiempo += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, 0.24f, tiempo / duracionFade);
                yield return null;
            }
        }
    }
}
