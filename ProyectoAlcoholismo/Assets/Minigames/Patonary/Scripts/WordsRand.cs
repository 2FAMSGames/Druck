using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordsRand : MonoBehaviour
    {
    public string textWord;
    //public Text wordText;
    //public int rand_row_id;
    //public int rand_col_id;

    private string rand_word;
    private int rand_id;

    //string[,] all_words = new string[6, 4] { { "pato", "pollo", "ganso", "cocodrilo" },
    //                                      { "huevo", "arco", "reloj de pared", "serpiente mordiéndose la cola" },
    //                                      { "casa para patos", "cucha", "pajarera", "casa" },
    //                                      { "escopeta", "escoba", "pistola", "pala" },
    //                                      { "trono", "zoo", "zumo de naranja", "sistema solar" },
    //                                      { "ordenador", "Internet", "agua salada", "teclado" }
    //};

    string [] all_words = new string[] {"pato", "pollo", "ganso", "cocodrilo", "copa de vino", "laberinto",
                                         "huevo", "videocámara", "reloj de pared", "serpiente mordiéndose la cola",
                                         "casa para patos", "casa", "bote", "motocicleta", "enchufe", "langosta",
                                         "escopeta", "escoba", "pala", "copa de vino", "cine", "autobús", "escuela",
                                         "trono", "zoo", "zumo de naranja", "sistema solar", "camisa", "hamburguesa",
                                         "ordenador", "Internet", "teclado", "árbol de Navidad", "escuela", "bar",
                                         "cáscara de plátano", "queso", "granja", "tarta", "flor", "bufanda", "violín", 
                                         "gafas de sol", "caña de pescar", "bote de basura", "sandalia", "dedo", "ping pong", 
                                         "florero", "borrador", "jamón", "huracán", "cepillo", "espantapájaros", "pilas",
                                         "astronauta", "ascensor", "costa", "tigre", "escalera", "tienda de campaña",
                                         "Donald Duck"
    };

    void Awake()
    {
        //wordText.text = GetRandomWord();
        textWord = GetRandomWord();
    }

    public string GetRandomWord()
    {
        //rand_row_id = UnityEngine.Random.Range(0, 6);
        //rand_col_id = UnityEngine.Random.Range(0, 4);
        //string rand_word = all_words[rand_row_id, rand_col_id];
        rand_id = UnityEngine.Random.Range(0, all_words.Length);
        rand_word = all_words[rand_id];
        return rand_word;
    }
}