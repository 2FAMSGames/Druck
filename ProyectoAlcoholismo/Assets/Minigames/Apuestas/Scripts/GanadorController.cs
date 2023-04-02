using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Photon.Realtime;

public class GanadorController : MonoBehaviour
{
    [SerializeField]
    private GameObject ApuestasObject;
    private ApuestasController apuestasController;
    private UIDocument doc;
    private Label winner;
    private Label nTragos;

    void OnEnable()
    {
        GameState.Instance.PlayerChangedData += OnPlayerChangedData;
        
        apuestasController = ApuestasObject.GetComponent<ApuestasController>();
        doc = GetComponent<UIDocument>();
        winner = doc.rootVisualElement.Q<Label>("winner");
        winner.text = "Esperando votaciones...";
        //winner.text = apuestasController.winner;
        nTragos = doc.rootVisualElement.Q<Label>("number");
        nTragos.text = apuestasController.prize;

        // TODO: debería haber un botón que se haga "enabled" cuando hayan votado todos
        // Y conectar el resto del código (en botonPulsado) en el método de pulsar el botón
    }

    private void OnPlayerChangedData(int arg1, NetworkDictionary<int, float> arg2)
    {
        bool todosHanVotado = true;
        int yes = 0;
        int no = 0;
        foreach (var player in PlayerRegistry.Instance.ObjectByRef)
        {
            var votacion = player.Value.data[0];
            todosHanVotado &= votacion is -1 or 1;
            if (votacion != 0)
            {
                yes += votacion == 1 ? 1 : 0;
                no += votacion == -1 ? 1 : 0;
            }
        }

        if (todosHanVotado)
        {
            apuestasController.Votando = false;
            winner.text = (yes >= no) ? apuestasController.challengedPlayer : apuestasController.CurrentPlayer;
            
            // TODO: Enable button para continuar y conectar a código "botonPulsado".
        }
    }

    private void botonPulsado()
    {
        // El host asignará los tragos al perdedor.
        if (GameState.isServer)
        {
            int id = -1;
            foreach (var player in PlayerRegistry.Instance.ObjectByRef)
            {
                if (player.Value.playerName == apuestasController.winner)
                {
                    id = player.Value.playerId;
                    break;
                }
            }
            Debug.Assert(id != -1);

            int tragos = -1;
            try
            {
                tragos = Int32.Parse(apuestasController.prize);
            }
            catch (FormatException)
            {
                tragos = 1;
            }
            Debug.Assert(tragos != -1);
        
            GameState.GetPlayer(id).SetData(5, tragos);
        }
        
        // Limpiamos para la siguiente votación
        GameState.GetMyPlayer().SetData(0,0);

        StartCoroutine(WaitAndNext());
    }

    private IEnumerator WaitAndNext()
    {
        apuestasController.Votando = false;
        yield return new WaitForSeconds(5);
        apuestasController.ChooseRandomPlayer();
    }

}
