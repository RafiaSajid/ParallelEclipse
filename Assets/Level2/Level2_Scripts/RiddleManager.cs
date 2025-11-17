using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class RiddleManager : MonoBehaviour
{
    [Header("Additive Scene")]
    public string finalMessageSceneName; // Assign "FinalMessageScene" in Inspector

    [Header("Next Scene")]
    public string nextSceneName; // Type the name of the next scene here in Inspector

    [Header("Riddles")]
    public List<RiddleData> riddles; // drag & drop RiddleData assets here
    private List<RiddleData> remainingRiddles;
    private RiddleData currentRiddle;

    private int correctRiddles = 0;
    public int riddlesToSolveForKey = 3;

    [Header("UI References")]
    public TMP_Text riddleText;
    public TMP_InputField answerInput;
    public TMP_Text feedbackText;
    public Button submitButton;
    public Button hintButton;

    [Header("Health")]
    public HealthSystem health;

    private int wrongAttempts = 0;

    void Start()
    {
        remainingRiddles = new List<RiddleData>(riddles);
        submitButton.onClick.AddListener(OnSubmit);
        hintButton.onClick.AddListener(OnHint);
        ShowRandomRiddle();
    }

    public void ShowRandomRiddle()
    {
        if (remainingRiddles.Count == 0)
        {
            feedbackText.text = "All riddles solved!";
            return;
        }

        int index = Random.Range(0, remainingRiddles.Count);
        currentRiddle = remainingRiddles[index];
        StartCoroutine(TypeWrite(currentRiddle.scrambled));
        feedbackText.text = "";
        answerInput.text = "";
        wrongAttempts = 0;
    }

    IEnumerator TypeWrite(string text)
    {
        riddleText.text = "";
        foreach (char c in text)
        {
            riddleText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }


    void OnSubmit()
    {
        string playerAnswer = answerInput.text.Trim().ToLower();
        if (playerAnswer == currentRiddle.answer.ToLower())
        {
            correctRiddles++;

            if (correctRiddles >= riddlesToSolveForKey)
            {
                feedbackText.text = "All riddles solved! Key obtained!";

                // If we are in Scene3, load additive scene
                if (SceneManager.GetActiveScene().name == "Room3")
                {
                    // Load additive scene on top of Room3
                    SceneManager.LoadScene(finalMessageSceneName, LoadSceneMode.Additive);
                }
                else
                {
                    // Normal scene loading for Room1 -> Room2, Room2 -> Room3
                    SceneManager.LoadScene(nextSceneName);
                }
            }
            else
            {
                feedbackText.text = "Correct!";
                remainingRiddles.Remove(currentRiddle);
                StartCoroutine(WaitAndShowNextRiddle());
            }
        }
        else
        {
            wrongAttempts++;
            feedbackText.text = "Wrong! Try again!";
            health.ReduceHealth();

            if (wrongAttempts >= 2)
            {
                ShowRandomRiddle();
            }
        }
    }


    IEnumerator WaitAndShowNextRiddle()
    {
        yield return new WaitForSeconds(1.5f);
        ShowRandomRiddle();
    }

    void OnHint()
    {
        feedbackText.text = "Hint: " + currentRiddle.hint;
    }
    public void RestartSession()
    {
        remainingRiddles = new List<RiddleData>(riddles);
        ShowRandomRiddle();

        // If you have health or score, reset them here too
    }

}
























/*using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class RiddleManager : MonoBehaviour
{
    [Header("Riddles")]
    private int correctRiddles = 0; // counts how many riddles the player solved
    public int riddlesToSolveForKey = 3; // total riddles to solve to get the key

    public RiddleData[] riddles;
[{ "name":"scrambled","type":3,"val":"EHLOL"},{ "name":"answer","type":3,"val":"HELLO"},{ "name":"hint","type":3,"val":"A common Greeting"}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"ODG"},{ "name":"answer","type":3,"val":"DOG"},{ "name":"hint","type":3,"val":"A common pet."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"SHDWOA"},{ "name":"answer","type":3,"val":"SHADOW"},{ "name":"hint","type":3,"val":"It follows you."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"HIGTSL"},{ "name":"answer","type":3,"val":"LIGHTS"},{ "name":"hint","type":3,"val":"They flicker in scary places."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"IWODNW"},{ "name":"answer","type":3,"val":"WINDOW"},{ "name":"hint","type":3,"val":"No hint"}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"CEPSAE"},{ "name":"answer","type":3,"val":"ESCAPE"},{ "name":"hint","type":3,"val":"Your main goal."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"ZSOBEIM"},{ "name":"answer","type":3,"val":"ZOMBIES"},{ "name":"hint","type":3,"val":"From level 1."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"DFRIEN"},{ "name":"answer","type":3,"val":"FRIEND"},{ "name":"hint","type":3,"val":"A trusted ally."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"NUTAHDE"},{ "name":"answer","type":3,"val":"HAUNTED"},{ "name":"hint","type":3,"val":"Like the building."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"HIERWSP"},{ "name":"answer","type":3,"val":"WHISPERS"},{ "name":"hint","type":3,"val":"Creepy soft voices."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"FERA"},{ "name":"answer","type":3,"val":"FEAR"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"LEHP"},{ "name":"answer","type":3,"val":"HELP"},{ "name":"hint","type":3,"val":"Often written on walls."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"TSILNE"},{ "name":"answer","type":3,"val":"SILENT"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"CEESRT"},{ "name":"answer","type":3,"val":"SECRET"},{ "name":"hint","type":3,"val":"Hidden Truth."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"IGNHT"},{ "name":"answer","type":3,"val":"NIGHT"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"GHOTS"},{ "name":"answer","type":3,"val":"GHOST"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"LVIE"},{ "name":"answer","type":3,"val":"EVIL"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"RDAGEN"},{ "name":"answer","type":3,"val":"DANGER"},{ "name":"hint","type":3,"val":"Always near."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"UZELPZ"},{ "name":"answer","type":3,"val":"PUZZLE"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"DROORRIC"},{ "name":"answer","type":3,"val":"CORRIDOR"},{ "name":"hint","type":3,"val":"Long, dark hallway."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"LFCIRKE"},{ "name":"answer","type":3,"val":"FLICKER"},{ "name":"hint","type":3,"val":"Brief fleeting light."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":" KCOL"},{ "name":"answer","type":3,"val":"LOCK"},{ "name":"hint","type":3,"val":"No hint"}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"TNHGI"},{ "name":"answer","type":3,"val":"THING"},{ "name":"hint","type":3,"val":"No hint"}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"YRCSAS"},{ "name":"answer","type":3,"val":"SCARY"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"GINLAS"},{ "name":"answer","type":3,"val":"SIGNALS"},{ "name":"hint","type":3,"val":"Something flickers on radios."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"SELOC"},{ "name":"answer","type":3,"val":"CLOSE"},{ "name":"hint","type":3,"val":"What doors do."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"TTHAER"},{ "name":"answer","type":3,"val":"THREAT"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"ROOLF"},{ "name":"answer","type":3,"val":"FLOOR"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"DOOLB"},{ "name":"answer","type":3,"val":"BLOOD"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"ESRUCPIT"},{ "name":"answer","type":3,"val":"PICTURES"},{ "name":"hint","type":3,"val":"Found on walls."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"YSMEYRT"},{ "name":"answer","type":3,"val":"MYSTERY"},{ "name":"hint","type":3,"val":"Needs uncovering."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"SODUN"},{ "name":"answer","type":3,"val":"SOUND"},{ "name":"hint","type":3,"val":"No hint"}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"RIPD"},{ "name":"answer","type":3,"val":"DRIP"},{ "name":"hint","type":3,"val":"That sound in dark rooms."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"ESYK"},{ "name":"answer","type":3,"val":"KEYS"},{ "name":"hint","type":3,"val":"What you must collect."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"TPROLA"},{ "name":"answer","type":3,"val":"PORTAL"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"LAPRALEL"},{ "name":"answer","type":3,"val":"PARALLEL"},{ "name":"hint","type":3,"val":"An unseen alternate timeline."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"RGOLW"},{ "name":"answer","type":3,"val":"GROWL"},{ "name":"hint","type":3,"val":"What monsters do."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"TOLS"},{ "name":"answer","type":3,"val":"LOST"},{ "name":"hint","type":3,"val":"You."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"RAEHT"},{ "name":"answer","type":3,"val":"HEART"},{ "name":"hint","type":3,"val":"Beats faster in fear."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"LEPIESC"},{ "name":"answer","type":3,"val":"ECLIPSE"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"LACEND"},{ "name":"answer","type":3,"val":"CANDLE"},{ "name":"hint","type":3,"val":"Light in darkness."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"NESDRKAS"},{ "name":"answer","type":3,"val":"DARKNESS"},{ "name":"hint","type":3,"val":"Where shadows live."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"EIEER"},{ "name":"answer","type":3,"val":"EERIE"},{ "name":"hint","type":3,"val":"Unsettling or strange."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"ORERTR"},{ "name":"answer","type":3,"val":"TERROR"},{ "name":"hint","type":3,"val":"Intense fear."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"NMNINGAEC"},{ "name":"answer","type":3,"val":"MENANCING"},{ "name":"hint","type":3,"val":"Warning of danger."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"RSCEU"},{ "name":"answer","type":3,"val":"CURSE"},{ "name":"hint","type":3,"val":"Magical Misfortune."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"CUERSE"},{ "name":"answer","type":3,"val":"RESCUE"},{ "name":"hint","type":3,"val":"No hint."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"LSAURPUETRAN"},{ "name":"answer","type":3,"val":"SUPERNATURAL"},{ "name":"hint","type":3,"val":"Beyond natural laws."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"YRECPE"},{ "name":"answer","type":3,"val":"CREEPY"},{ "name":"hint","type":3,"val":"Unsettling, skin crawling."}]},{ "name":"data","type":-1,"children":[{ "name":"scrambled","type":3,"val":"WGTHLIIT"},{ "name":"answer","type":3,"val":"TWILIGHT"},{ "name":"hint","type":3,"val":"Dim light before night."}]}]}]}
    private RiddleData currentRiddle;

    [Header("UI References")]
    public TMP_Text riddleText;
    public TMP_InputField answerInput;
    public TMP_Text feedbackText;
    public Button submitButton;
    public Button hintButton;

    [Header("Health")]
    public HealthSystem health;

    private int wrongAttempts = 0;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
        hintButton.onClick.AddListener(OnHint);
        ShowRandomRiddle();
    }

    public void ShowRandomRiddle()
    {
        int index = Random.Range(0, riddles.Length);
        currentRiddle = riddles[index];
        StartCoroutine(TypeWrite(currentRiddle.scrambled));
        feedbackText.text = "";
        answerInput.text = "";
        wrongAttempts = 0;
    }

    IEnumerator TypeWrite(string text)
    {
        riddleText.text = "";
        foreach (char c in text)
        {
            riddleText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void OnSubmit()
    {
        string playerAnswer = answerInput.text.Trim().ToLower();
        if (playerAnswer == currentRiddle.answer.ToLower())
        {
            correctRiddles++;

            if (correctRiddles >= riddlesToSolveForKey)
            {
                feedbackText.text = "All riddles solved! Key obtained!";
                // TODO: Give key or move to next scene
            }
            else
            {
                feedbackText.text = "Correct!";
                StartCoroutine(WaitAndShowNextRiddle()); // wait before showing next
            }
        }
        else
        {
            wrongAttempts++;
            feedbackText.text = "Wrong! Try again!";
            health.ReduceHealth();

            if (wrongAttempts >= 2)
            {
                ShowRandomRiddle();
            }
        }
    }

    IEnumerator WaitAndShowNextRiddle()
    {
        yield return new WaitForSeconds(1.5f); // show correct message for 1.5 seconds
        ShowRandomRiddle();
    }

    void OnHint()
    {
        feedbackText.text = "Hint: " + currentRiddle.hint;
    }
}
*/
