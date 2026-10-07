using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego Instancia;

    public int puntosJ1 = 0;
    public int puntosJ2 = 0;
    public int puntosParaGanar = 5;

    public Text textoPuntosJ1;
    public Text textoPuntosJ2;
    public Text textoGanador;

    // Cuando alguien gana ya no se cuentan mas goles
    private bool partidaTerminada = false;

    private void Awake()
    {
        Instancia = this;
    }

    void Start()
    {
        ActualizarUI();
    }

    public void AnotarGol(int jugador)
    {
        if (partidaTerminada) return;

        if (jugador == 1) puntosJ1++;
        else puntosJ2++;

        ActualizarUI();
        GestorSonido.Instancia.SonarGol();

        if (puntosJ1 >= puntosParaGanar || puntosJ2 >= puntosParaGanar)
        {
            partidaTerminada = true;

            if (textoGanador != null)
                textoGanador.text = puntosJ1 >= puntosParaGanar ? "¡Gana Jugador 1!" : "¡Gana Jugador 2!";

            GestorSonido.Instancia.SonarGanar();

            Invoke("VolverAlMenu", 2.5f);
        }
    }

    void ActualizarUI()
    {
        if (textoPuntosJ1 != null) textoPuntosJ1.text = puntosJ1.ToString();
        if (textoPuntosJ2 != null) textoPuntosJ2.text = puntosJ2.ToString();
    }

    void VolverAlMenu()
    {
        SceneManager.LoadScene("Menu");
        // La escena del menu se llama "Menu"
    }
}