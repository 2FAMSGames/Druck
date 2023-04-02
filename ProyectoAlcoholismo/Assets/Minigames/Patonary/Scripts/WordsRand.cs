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
    //                                      { "huevo", "arco", "reloj de pared", "serpiente mordi�ndose la cola" },
    //                                      { "casa para patos", "cucha", "pajarera", "casa" },
    //                                      { "escopeta", "escoba", "pistola", "pala" },
    //                                      { "trono", "zoo", "zumo de naranja", "sistema solar" },
    //                                      { "ordenador", "Internet", "agua salada", "teclado" }
    //};

    string [] all_words = new string[] {"pato", "pollo", "ganso", "cocodrilo", "copa de vino", "laberinto",
                                         "huevo", "videoc�mara", "reloj de pared", "serpiente mordi�ndose la cola",
                                         "casa para patos", "casa", "bote", "motocicleta", "enchufe", "langosta",
                                         "escopeta", "escoba", "pala", "copa de vino", "cine", "autob�s", "escuela",
                                         "trono", "zoo", "zumo de naranja", "sistema solar", "camisa", "hamburguesa",
                                         "ordenador", "Internet", "teclado", "�rbol de Navidad", "escuela", "bar",
                                         "c�scara de pl�tano", "queso", "granja", "tarta", "flor", "bufanda", "viol�n", 
                                         "gafas de sol", "ca�a de pescar", "bote de basura", "sandalia", "dedo", "ping pong", 
                                         "florero", "borrador", "jam�n", "hurac�n", "cepillo", "espantap�jaros", "pilas",
                                         "astronauta", "ascensor", "costa", "tigre", "escalera", "tienda de campa�a",
                                         "Donald Duck"
    };

    void Awake()
    {
        //wordText.text = GetRandomWord();
        textWord = GetRandomWord();
    }

    public string GetRandomWord()
    {
        rand_id = UnityEngine.Random.Range(0, all_words.Length);
        rand_word = all_words[rand_id];
        return rand_word;
    }
}