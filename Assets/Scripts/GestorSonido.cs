using UnityEngine;

// Reproduce los sonidos del juego.
// Se crea solo, asi no hay que montar nada en la escena.
public class GestorSonido : MonoBehaviour
{
    private static GestorSonido instancia;
    private AudioSource fuente;

    public static GestorSonido Instancia
    {
        get
        {
            if (instancia == null)
            {
                GameObject objeto = new GameObject("GestorSonido");
                instancia = objeto.AddComponent<GestorSonido>();
            }

            return instancia;
        }
    }

    void Awake()
    {
        instancia = this;
        fuente = gameObject.AddComponent<AudioSource>();
        fuente.playOnAwake = false;
    }

    void OnDestroy()
    {
        if (instancia == this) instancia = null;
    }

    public void SonarRebote() { Sonar("rebote"); }
    public void SonarGol() { Sonar("gol"); }
    public void SonarClic() { Sonar("clic"); }
    public void SonarGanar() { Sonar("ganar"); }

    void Sonar(string nombre)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sonidos/" + nombre);

        if (clip != null && fuente != null)
        {
            fuente.PlayOneShot(clip);
        }
    }
}
