using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Random = System.Random;

namespace Utils
{
    public static class GameConstants
    {
        public static List<string> GameList = new List<String>
        {
//           "AHuevo",
//           "Apuestas",
//           "CuakCuak",
//           "Lanzapato",
           "Patonary"
//           "SimonSays"
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
            {"CuakCuak", ""},
            {"Lanzapato", " aciertos"},
            {"SimonSays", " fallos"},
            {"Patonary", " aciertos"}
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

    public static class StringUtils
    {
        public static string generateRandomString(int length = 16)
        {
            Random rand = new Random();
            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ123456789".ToCharArray();

            var str = "";
            while(str.Length < 16)
            {
                str += chars[rand.Next(0, chars.Length)];
            }
            
            return str;
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
    }
}