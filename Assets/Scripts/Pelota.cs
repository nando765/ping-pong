using UnityEngine;

public class Pelota : MonoBehaviour
{
    public float velocidadInicial = 8f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LanzarPelota();
    }

    public void LanzarPelota()
    {
        transform.position = Vector3.zero;
        rb.linearVelocity = Vector2.zero;

        float x = Random.Range(0, 2) == 0 ? -1f : 1f;
        float y = Random.Range(-0.5f, 0.5f);

        Vector2 direccion = new Vector2(x, y).normalized;
        rb.AddForce(direccion * velocidadInicial, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "GolIzquierda")
        {
            ControladorJuego.Instancia.AnotarGol(2);
            LanzarPelota();
        }
        else if (collision.gameObject.name == "GolDerecha")
        {
            ControladorJuego.Instancia.AnotarGol(1);
            LanzarPelota();
        }
    }
}