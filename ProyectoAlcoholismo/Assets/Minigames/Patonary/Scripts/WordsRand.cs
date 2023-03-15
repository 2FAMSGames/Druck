using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordsRand : MonoBehaviour
    {
    public Text wordText;
    public int rand_row_id;
    public int rand_col_id;


    string[,] all_words = new string[6, 4] { { "pato", "pollo", "ganso", "cocodrilo" },
                                          { "huevo", "arco", "reloj de pared", "serpiente mordiéndose la cola" },
                                          { "casa para patos", "cucha", "pajarera", "casa" },
                                          { "escopeta", "escoba", "pistola", "pala" },
                                          { "trono", "zoo", "zumo de naranja", "sistema solar" }, //{ "trono", "silla", "taburete", "sillon" }
                                          { "ordenador", "Internet", "agua salada", "teclado" }
                                          
    
    };

    void Start()
    {
        wordText.text = GetRandomWord();
    }

    public string GetRandomWord()
    {
        rand_row_id = UnityEngine.Random.Range(0, 7);
        rand_col_id = UnityEngine.Random.Range(0, 4);
        string rand_word = all_words[rand_row_id, rand_col_id];
        return rand_word;
    }
}