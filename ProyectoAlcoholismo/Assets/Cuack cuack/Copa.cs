using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Copa : MonoBehaviour
{
    public GameObject copaRota;
    public Transform modeloCopa;
    public Transform camara;

    public float tonoObjetivoMaximo = 3;
    public float tonoObjetivoMinimo = 1;
    public float tiempoTonoParaRomper = 0.5f;
    public float canvasSize = 4;
    public GameObject canvas;
    public RectTransform fondo;
    public RectTransform objetivo;
    public RectTransform actual;
    public bool copaActiva;
    public float distanciaEntreCopas = 5;

    public float tono;
    float posicion;
    float tiempoEnTono;
    float frecuanciaAnterior;
    AudioSource audioSource;

    Vector3 posInicial;
    Vector3 posInicialCamara;

    void Start()
    {
        RandomTone();
        copaActiva = false;
        posInicial = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (Microphone.devices.Length > 0)
        {
            audioSource.clip = Microphone.Start(Microphone.devices[0], true, 1000, 44100); ;
            while (!(Microphone.GetPosition(null) > 0)) { };
            audioSource.Play();
        }

        posInicialCamara = camara.transform.position;
    }

    private void FixedUpdate()
    {
        canvas.SetActive(copaActiva);

        if (copaActiva)
        {
            float[] spectrum = new float[1024];
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

            float frecuanciaTotal = 0;
            for (int i = 1; i < spectrum.Length; i++)
            {
                frecuanciaTotal += spectrum[i] * i;
            }

            float diferenciaFrecuancia = frecuanciaTotal - frecuanciaAnterior;
            float nuevaFrecuencia = frecuanciaAnterior + (diferenciaFrecuancia / 100);
            frecuanciaAnterior = nuevaFrecuencia;

            tono = nuevaFrecuencia - 5;

            posicion = tono / 60 * canvasSize;
            posicion = Mathf.Clamp(posicion, 0, canvasSize);

            actual.offsetMin = new Vector2(actual.offsetMin.x, posicion - 0.1f);
            actual.offsetMax = new Vector2(actual.offsetMax.x, -4 + posicion + 0.1f);

            objetivo.offsetMin = new Vector2(actual.offsetMin.x, tonoObjetivoMinimo);
            objetivo.offsetMax = new Vector2(actual.offsetMax.x, tonoObjetivoMaximo - 4);

            if (posicion > tonoObjetivoMinimo && posicion < tonoObjetivoMaximo)
            {
                tiempoEnTono += Time.fixedDeltaTime;
            }
            else
            {
                tiempoEnTono = 0;
            }

            if (tiempoEnTono > tiempoTonoParaRomper)
            {
                RomperCopa();
            }
        }
        else
        {
            float nuevaFrecuencia = 0;
            frecuanciaAnterior = nuevaFrecuencia;
        }

        if (transform.position.x < posInicial.x)
        {
            copaActiva = true;
            FindObjectOfType<CintaTransportadora>().mov = false;
        }
    }

    private void Update()
    {
        if (tiempoEnTono > 0)
        {
            camara.transform.position = posInicialCamara + new Vector3(Mathf.Sin(Time.time * 50) * 0.02f * tiempoEnTono, Mathf.Sin(Time.time * 40) * 0.02f * tiempoEnTono, 0.0f);
        }
        else
        {
            camara.transform.position = posInicialCamara;
        }
    }

    public void RomperCopa()
    {
        GameObject nuevaCopaRota = Instantiate(copaRota, transform.position, Quaternion.identity);
        nuevaCopaRota.transform.localScale = modeloCopa.localScale;
        NuevaCopa();
    }

    public void NuevaCopa()
    {
        copaActiva = false;
        tiempoEnTono = 0;
        transform.position = posInicial + new Vector3(distanciaEntreCopas, 0, 0);
        transform.rotation = Quaternion.identity;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        FindObjectOfType<CintaTransportadora>().mov = true;

        RandomTone();
    }

    private void RandomTone()
    {
        float tonoObjetivo = Random.Range(0.5f, 3.5f);
        tonoObjetivoMaximo = tonoObjetivo + 0.5f;
        tonoObjetivoMinimo = tonoObjetivo - 0.5f;

        tonoObjetivo /= 4;
        tonoObjetivo = 1 - tonoObjetivo;
        float tamañoCopa = (0.5f + tonoObjetivo);

        modeloCopa.localScale = new Vector3(tamañoCopa, 1, tamañoCopa);
    }
}
