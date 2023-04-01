using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using NoteList = System.Collections.Generic.List<float>;
using Cancion = System.Collections.Generic.List<System.Collections.Generic.List<float>>;

public class SimonGame : MonoBehaviour
{
    [SerializeField] private GameObject menusObject;
    [SerializeField] private SimonMessages messagesController;
    [SerializeField] private SpriteRenderer fondo;
    private SimonSceneController menusController;
    private AudioSource audioSource;
    [SerializeField] private AudioClip cuackClip;

    [Header("Private gameObject things")]
    private UIDocument doc;
    private List<Button> Buttons = new List<Button>(); 
    private Label Texto;

    [Header("Helpers and constants")]
    private static readonly List<string> ButtonNames = new List<string> { "Button1", "Button2", "Button3", "Button4", "Button5" };
    
    // Sé lo que estás pensando... pero esto es necesario para el comportamiento que quiero.
    private List<bool> ButtonActivated = new List<bool>();

    // 0 == C
    // 2 == D
    // 4 == E
    // 5 == F
    // 7 == G
    // 9 == A
    // 11 == B
    // 12 == C
    // 14 == D
    private static List<List<float>> NeverGonnaGiveYouUp = new Cancion
    {
        new List<float>{ 4.05f, 5.05f, 7.05f, 5.05f, 11.03f, 11.03f, 9.1f},
        new List<float>{ 4.05f, 5.05f, 7.05f, 5.05f, 4.05f, 2.1f},
        new List<float>{ 4.05f, 5.05f, 7.05f, 5.05f, 7.05f, 4.1f},
        new List<float>{ 7.05f, 11.05f, 9.1f}
    };
    
    private static readonly Color whitebg = new Color(1, 1, 1);
    private static readonly Color redbg = new Color(0.6f, 0.4f, 0.4f);
    
    [Header("Game variables")]
    private float nextEventTime = 0;
    private bool buttonsDisabled = false;
    private Dictionary<int, string> CurrentButtonNotes = new Dictionary<int, string>();
    private Cancion CurrentSong = new Cancion();
    private NoteList CurrentRound = new NoteList();
    public int failedRounds = 0;
    public float totalTime = 0;
    public bool finished = false;
    private int RoundNumber = 0;

    private void Awake()
    {
        fondo.color = new Color(1, 1, 1);
        menusController = menusObject.GetComponent<SimonSceneController>();
        audioSource = menusObject.GetComponent<AudioSource>();
        doc = GetComponent<UIDocument>();
        
        CurrentSong = NeverGonnaGiveYouUp;
    }

    private void AssignButtons()
    {
        Buttons.Clear();
        for (int i = 0; i < 5; ++i)
        {
            var item = doc.rootVisualElement.Q<Button>(ButtonNames[i]);
            
            // Atenuar color fijado
            Color.RGBToHSV(item.resolvedStyle.backgroundColor, out var h, out var s, out var v);
            item.style.backgroundColor =  new StyleColor(Color.HSVToRGB(h, s, v - 0.3f));
            item.text = String.Empty;

            // Añadir callbacks para las acciones.
            Buttons.Add(item);
            ButtonActivated.Add(false);
            item.clickable.activators.Clear();
            item.RegisterCallback<MouseDownEvent>(delegate { ButtonClicked(ref item); });
            item.RegisterCallback<MouseUpEvent>(delegate { ButtonReleased(ref item); });
            item.RegisterCallback<MouseLeaveEvent>(delegate { ButtonLeaved(ref item); });
        }
    }

    private void ActivateButton(ref Button item)
    {
        Color.RGBToHSV(item.resolvedStyle.backgroundColor, out var h, out var s, out var v);
        item.style.backgroundColor =  new StyleColor(Color.HSVToRGB(h, s, v + 0.3f));
        var idx = Buttons.IndexOf(item);
        ButtonActivated[idx] = true;
    }

    private void DeactivateButton(ref Button item)
    {
        Color.RGBToHSV(item.resolvedStyle.backgroundColor, out var h, out var s, out var v);
        item.style.backgroundColor =  new StyleColor(Color.HSVToRGB(h, s, v - 0.3f));
        ButtonActivated[Buttons.IndexOf(item)] = false;
    }

    private void ButtonClicked(ref Button item)
    {
        if (buttonsDisabled || ButtonActivated[Buttons.IndexOf(item)]) return;
        ActivateButton(ref item);
        ResetPlayerTime();
        
        var buttonName = item.name;
        var note = CurrentButtonNotes.FirstOrDefault(x => x.Value == buttonName).Key;
        var duration = CurrentRound[0] - (int)Math.Truncate(CurrentRound[0]);
        StartCoroutine(PlayNote(note + duration));
        Invoke("nullFunction", duration);
    }

    private void ResetPlayerTime()
    {
        nextEventTime = 5.9f;
    }

    private void nullFunction()
    {
        // No, no es un error, deja esto aquí.
    }

    private void ButtonReleased(ref Button item)
    {
        if (buttonsDisabled || !ButtonActivated[Buttons.IndexOf(item)]) return;
        DeactivateButton(ref item);

        var buttonName = item.name;
        var note = CurrentButtonNotes.FirstOrDefault(x => x.Value == buttonName).Key;
        
        if(!buttonsDisabled)
        if (note != (int)Math.Truncate(CurrentRound[0]))
        {
            ++failedRounds;
            CurrentRound.Clear();
            messagesController.texto = "Te has\nequivocado\nde nota...\n\nHas perdido\nesta ronda!";
            fondo.color = whitebg;
            menusController.ShowMessage();
        }
        else
        {
            CurrentRound.RemoveAt(0);
            totalTime += 5.9f - nextEventTime;
            ResetPlayerTime();
            
            if (CurrentRound.Count == 0)
            {
                fondo.color = whitebg;
                if (CurrentSong.Count == 0)
                {
                    // Fin del juego
                    finished = true;
                    messagesController.texto = "Enhorabuena!\n\nCanción completada!";
                    menusController.ShowMessage();
                }
                else
                {
                    messagesController.texto = "Enhorabuena!\n\nRonda superada!";
                    menusController.ShowMessage();
                }
            }
        }
    }

    private void ButtonLeaved(ref Button item)
    {
        if(buttonsDisabled) return;
        if(ButtonActivated[Buttons.IndexOf(item)]) ButtonReleased(ref item);
    }

    void OnEnable()
    {
        AssignButtons();
        buttonsDisabled = true;
        Texto = doc.rootVisualElement.Q<Label>("Texto");
        
        StartCoroutine(SingleRound());
    }

    private void FixedUpdate()
    {
        if (this.enabled)
        {
            if (!buttonsDisabled)
            {
                if (nextEventTime > 0)
                {
                    nextEventTime -= Time.fixedDeltaTime;
                    Texto.text = "Ronda " + RoundNumber.ToString() + "\nRepite en " + ((int)(nextEventTime)).ToString();
                }
                else
                {
                    fondo.color = whitebg;
                    messagesController.texto = "Se ha\nconsumido\nel tiempo...\n\nHas perdido\nesta ronda!";
                    menusController.ShowMessage();
                    ++failedRounds;
                }
            }
        }
    }

    private void DisableButtons()
    {
        for (int i = 0; i < 5; ++i)
        {
            var item = Buttons[i];
            if (ButtonActivated[Buttons.IndexOf(item)])
                ButtonReleased(ref item);
        }

        buttonsDisabled = true;
    }

    private void EnableButtons()
    {
        buttonsDisabled = false;
    }
   
    private void PlaySound(int note)
    {
        var transpose = -4;  // transpose in semitones
        if (note >= 0)
        {
            audioSource.Stop();
            audioSource.clip = cuackClip;
            audioSource.pitch =  Mathf.Pow(2, (float)((note+transpose)/12.0));
            audioSource.Play();
        }
    }
    
    public IEnumerator PlayNote(float noteAndDuration)
    {
        var note = (int)Math.Truncate(noteAndDuration);
        PlaySound(note);

        if (buttonsDisabled)
        {
            var duration = (noteAndDuration - note) * 10; // seconds
            yield return new WaitForSeconds(duration);
        }
    }   
    
    public IEnumerator SingleRound()
    {
        // TODO:
        if(CurrentSong.Count == 0)
            yield break;

        ++RoundNumber;
        CurrentRound = CurrentSong.First();
        CurrentSong.RemoveAt(0);
        
        DisableButtons();
        
        // Randomizar botones.
        var buttons = ButtonNames.CloneViaSerialization();
        
        CurrentButtonNotes.Clear();
        foreach (var noteAndDuration in CurrentRound)
        {
            var note = (int)Math.Truncate(noteAndDuration);
            if (CurrentButtonNotes.ContainsKey(note) || buttons.Count == 0) continue;
            
            var pos = Random.Range(0, buttons.Count);
            CurrentButtonNotes.Add(note, buttons[pos]);
            buttons.RemoveAt(pos);
        }
        
        Texto.text = "Ronda " + RoundNumber.ToString() + "\nPreparado...";
        yield return new WaitForSeconds(1);
        Texto.text = "Ronda " + RoundNumber.ToString() + "\nListo...";
        yield return new WaitForSeconds(1);
        Texto.text = "Ronda " + RoundNumber.ToString() + "\nAtento!";
        yield return new WaitForSeconds(1);

        foreach (float noteAndDuration in CurrentRound)
        {
            var note = (int)Math.Truncate(noteAndDuration);
            float duration = (noteAndDuration - note) * 10; // seconds
            var button = doc.rootVisualElement.Q<Button>(CurrentButtonNotes[note]);
            ActivateButton(ref button);
            StartCoroutine(PlayNote(note));
            yield return new WaitForSeconds(duration);
            DeactivateButton(ref button);
            yield return new WaitForSeconds(0.5f);
        }

        Texto.text = "Ronda " + RoundNumber.ToString() + "\nEs tu turno...";
        yield return new WaitForSeconds(1);
        Texto.text = "Ronda " + RoundNumber.ToString() + "\nPreparate!";
        yield return new WaitForSeconds(1);
        ResetPlayerTime();
        Texto.text = "Ronda " + RoundNumber.ToString() + "\nRepite en " + ((int)(nextEventTime)).ToString();
        fondo.color = redbg;
        EnableButtons();
    }
}
