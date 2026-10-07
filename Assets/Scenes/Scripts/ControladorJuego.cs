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

    private void Awake()
    {
        Instancia = this;
    }

    public void AnotarGol(int jugador)
    {
        if (jugador == 1) puntosJ1++;
        else puntosJ2++;

        ActualizarUI();

        if (puntosJ1 >= puntosParaGanar || puntosJ2 >= puntosParaGanar)
        {
            if (textoGanador != null)
                textoGanador.text = puntosJ1 >= puntosParaGanar ? "¡Gana Jugador 1!" : "¡Gana Jugador 2!";

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
        SceneManager.LoadScene("MenuPrincipal");
    }
}