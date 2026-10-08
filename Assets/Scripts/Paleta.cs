using UnityEngine;

public class Paleta : MonoBehaviour
{
    public bool esJugador1 = true;
    public float velocidad = 10f;
    public float limiteY = 1.4f;

    void Update()
    {
        float movimiento = 0f;

        if (esJugador1)
        {
            if (Input.GetKey(KeyCode.W)) movimiento = 1f;
            if (Input.GetKey(KeyCode.S)) movimiento = -1f;
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow)) movimiento = 1f;
            if (Input.GetKey(KeyCode.DownArrow)) movimiento = -1f;
        }

        transform.Translate(Vector3.up * movimiento * velocidad * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, -limiteY, limiteY);
        transform.position = pos;
    }
}