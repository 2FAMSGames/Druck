using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    public float tiempoDeJuego = 60;

    public float tono;
    float posicion;
    float tiempoEnTono;
    float frecuanciaAnterior;
    int puntuacion;
    VisualElement root;
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
        puntuacion = 0;

        root = FindObjectOfType<UIDocument>().rootVisualElement;
    }

    private void FixedUpdate()
    {
        canvas.SetActive(copaActiva);

        if (copaActiva)
        {
            float[] spectrum = new float[1024];
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

            float frecuenciaTotal = 0;
            for (int i = 1; i < spectrum.Length; i++)
            {
                frecuenciaTotal += spectrum[i] * i;
            }

            float diferenciaFrecuencia = frecuenciaTotal - frecuanciaAnterior;
            float nuevaFrecuencia = frecuanciaAnterior + (diferenciaFrecuencia / 100);
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
                puntuacion += 1;
                GameState.GetMyPlayer().SetData(0, puntuacion);
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
        tiempoDeJuego -= Time.deltaTime;
        if (tiempoDeJuego < 0 && !Empate())
        {
            AcabarJuego();
        }

        if (tiempoEnTono > 0)
        {
            camara.transform.position = posInicialCamara + new Vector3(Mathf.Sin(Time.time * 50) * 0.02f * tiempoEnTono, Mathf.Sin(Time.time * 40) * 0.02f * tiempoEnTono, 0.0f);
        }
        else
        {
            camara.transform.position = posInicialCamara;
        }

        root.Q<Label>("Puntuacion").text = puntuacion.ToString();
        root.Q<Label>("Tiempo").text = Mathf.Ceil(tiempoDeJuego).ToString();
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
        float tamanioCopa = (0.5f + tonoObjetivo);

        modeloCopa.localScale = new Vector3(tamanioCopa, 1, tamanioCopa);
    }

    private bool Empate()
    {
        bool empate = false;
        float puntuacionMaxima = 0;

        var result = PlayerRegistry.Instance.SortedScores();

        foreach (var player in result)
        {
            if (GameState.GetPlayer(player.Item1).data[0] > puntuacionMaxima)
            {
                puntuacionMaxima = GameState.GetPlayer(player.Item1).data[0];
            }
        }

        int empatados = 0;

        foreach (var player in result)
        {
            if (GameState.GetPlayer(player.Item1).data[0] == puntuacionMaxima)
            {
                empatados++;
            }
        }

        if (empatados > 1)
        {
            empate = true;
        }

        return empate;
    }

    private bool Ganador()
    {
        bool ganador = false;
        float puntuacionMaxima = 0;

        var result = PlayerRegistry.Instance.SortedScores();

        foreach (var player in result)
        {
            if (GameState.GetPlayer(player.Item1).data[0] > puntuacionMaxima)
            {
                puntuacionMaxima = GameState.GetPlayer(player.Item1).data[0];
            }
        }

        if (GameState.GetMyPlayer().data[0] == puntuacionMaxima)
        {
            ganador = true;
        }

        return ganador;
    }

    private void AcabarJuego()
    {
        if (Ganador())
        {

        }
        SceneManager.LoadScene("Ranking");
    }
}
