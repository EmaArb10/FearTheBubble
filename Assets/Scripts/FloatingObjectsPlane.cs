using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LowPolyWater; // Aseg�rate de que este namespace coincide con el de tu sistema de olas

public class FloatingObjectsPlane : MonoBehaviour
{
    public Rigidbody rb;
    public float depthBeforeSubmerged = 0.1f;
    public float displacementAmount = 6f;
    public int floaterCount = 4;
    public float waterAngularDrag = 25f;
    public float waterDrag = 6f;
     
    // Referencia al script LowPolyWaterClass del objeto de agua
    public LowPolyWaterClass water;

    private void FixedUpdate()
    {
        // Aplicar la gravedad
        rb.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);

        // Obtener la altura de la ola en la posici�n del objeto usando LowPolyWaterClass
        float waveHeight = water.GetWaveHeightAtPosition(transform.position);

        // Si el objeto se encuentra por debajo de la altura de la ola, aplica fuerza de flotaci�n
        if (transform.position.y < waveHeight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rb.AddForce(displacementMultiplier * -rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rb.AddTorque(displacementMultiplier * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}