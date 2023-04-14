using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace Utils
{
    public static class GameConstants
    {
        public static List<string> GameList = new List<String>
        {
           "AHuevo",
           //"Apuestas",
           "CuakCuak",
           "Lanzapato",
           "Patonary",
           "SimonSays"
        };

        public static Dictionary<string, string> GameNames = new Dictionary<string, string>
        {
            {"AHuevo","A huevo"},
            {"Apuestas", "No hay huevos"},
            {"CuakCuak", "Cuak Cuak"},
            {"Lanzapato", "Lanza Pato"},
            {"Patonary", "Patonary"},
            {"SimonSays", "Pato Dice..."}
        };

        public static Dictionary<string, string> GameSuffix = new Dictionary<string, string>
        {
            {"AHuevo"," puntos"},
            {"Apuestas", ""},
            {"CuakCuak", " puntos"},
            {"Lanzapato", " aciertos"},
            {"SimonSays", " fallos"},
            {"Patonary", " aciertos"},
            {"General", "% sobrio"}
        };
    }

    public static class ListUtils
    {
        private static Random rand = new Random();        

        public static void Shuffle<T>(this IList<T> values)
        {
            for (int i = values.Count - 1; i > 0; i--)
            {
                int k = rand.Next(i + 1);
                (values[k], values[i]) = (values[i], values[k]);
            }
        }
    }

    public static class GameUtils
    {
        public static IEnumerator GoToRankings()
        {
            yield return new WaitForSeconds(1);
            AsyncOperation async = SceneManager.LoadSceneAsync("Ranking");
            yield return async; //Wait for your level to load
        }
        
        public static void Log(string message,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.Log(filePath + ":"+ lineNumber + " -> " + message);
        }
    }
}