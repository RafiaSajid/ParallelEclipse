using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Riddle
    {
        [TextArea] public string question;
        public string answer;
        public string hint;
        public string key;
        public bool solved = false;
    }

    public Riddle[] riddles;
    public TMP_Text riddleText, feedbackText;
    public TMP_InputField answerInput;
    public GameObject hintButton;
    public TMP_Text keysDisplayText;
    private string collectedKeys = "";
    private int currentIndex = 0;
    private int keysCollected = 0;
    public int GetKeysCount() => keysCollected;



  void Start()
{
    riddles = new Riddle[3];
    
    riddles[0] = new Riddle
    {
        question = "I’m tall when I’m young, and short when I’m old. What am I?",
        answer = "candle",
        hint = "Think of something that melts.",
        key = "F"
    };

    riddles[1] = new Riddle
    {
        question = "The more of this you take, the more you leave behind. What is it?",
        answer = "footsteps",
        hint = "It’s related to walking.",
        key = "x"
    };

    riddles[2] = new Riddle
    {
        question = "I speak without a mouth and hear without ears. I have no body, but I come alive with wind. What am I?",
        answer = "echo",
        hint = "It repeats what you say.",
        key = "9"
    };

    DisplayRiddle();
}


    public void DisplayRiddle()
    {
        riddleText.text = riddles[currentIndex].question;
        feedbackText.text = "";
        answerInput.text = "";
    }
    void UpdateKeysDisplay(){
          keysDisplayText.text = "Keys Collected: " + collectedKeys;
          }

    public void CheckAnswer()
    {
        string playerAnswer = answerInput.text.Trim().ToLower();
        if (playerAnswer == riddles[currentIndex].answer.ToLower())
{
    feedbackText.text = $"✅ Correct! You earned a key: {riddles[currentIndex].key}";
    riddles[currentIndex].solved = true;
    keysCollected++;
    collectedKeys += riddles[currentIndex].key; // Save the key character
    UpdateKeysDisplay();

    NextRiddle();
}


        else
        {
            feedbackText.text = "❌ Wrong answer. Try again!";
        }
        
    }
    

    public void ShowHint()
    {
        feedbackText.text = "💡 Hint: " + riddles[currentIndex].hint;
    }

    void NextRiddle()
    {
        currentIndex++;
        if (currentIndex < riddles.Length)
        {
            DisplayRiddle();
        }
        else
        {
            feedbackText.text = $"🎉 You solved all riddles and got {keysCollected} keys!";
            // You can trigger the door unlock here
        }
    }
}
