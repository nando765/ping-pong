using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Jugar()
    {
        GestorSonido.Instancia.SonarClic();

        // Carga la escena del juego
        SceneManager.LoadScene("SampleScene");
    }

    public void Salir()
    {
        GestorSonido.Instancia.SonarClic();

        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}