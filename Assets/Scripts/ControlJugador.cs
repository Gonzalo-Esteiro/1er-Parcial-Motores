using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ControlJugador : MonoBehaviour
{
    private Rigidbody rb;
    public int rapidez;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");
        Vector3 vectorMovimiento = new Vector3(movimientoHorizontal, 0.0f, movimientoVertical);
        rb.AddForce(vectorMovimiento * rapidez);
    }
}