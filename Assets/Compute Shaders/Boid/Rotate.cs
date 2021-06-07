using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    Vector3 rotacion;
    // Start is called before the first frame update
    void Start()
    {
        rotacion = new Vector3(0, 15, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotacion * Time.deltaTime);
    }
}
