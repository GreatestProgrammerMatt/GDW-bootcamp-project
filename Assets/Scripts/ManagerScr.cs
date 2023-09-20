using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManagerScr : MonoBehaviour
{
    [SerializeField] Sprite[] images;
    [SerializeField] TextAsset jsonLocal;
    private string[] inputJson;

    private List<QuizList> quizesList = new List<QuizList>();
    private List<QuizModel> quizes = new List<QuizModel>();
    private int currentQuiz;

    [SerializeField] Image[] figure;
    [SerializeField] TextMeshProUGUI answerPrompt;

    [SerializeField] GameObject mainPrompt;

    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject winEffect;
    [SerializeField] Image winIcon;


    private void Start()
    {
        quizesList = JSONHandler.ReadJSONTextAsset<QuizList>(jsonLocal);
        quizes = quizesList[0].qList;
        currentQuiz=0;
        Painter();
    }

    private void Painter()
    {
        for (int i=0;i<3;i++)
        {
            figure[i].sprite = images[quizes[currentQuiz].img+(1*i)];
        }
        answerPrompt.text = quizes[currentQuiz].answer;
    }

    public void buttonPress(int option)
    {
        WinnScreen(option);
    }

    private void WinnScreen(int opt)
    {
        mainPrompt.SetActive(false);

        winIcon.sprite = images[quizes[currentQuiz].img+(quizes[currentQuiz].opt-1)];
        
        winScreen.SetActive(true);
        if (opt == quizes[currentQuiz].opt)
        {
            winEffect.SetActive(true);
        }
        else
        {
            winEffect.SetActive(false);
        }
    }

    public void Resetter()
    {
        currentQuiz = (currentQuiz<(quizes.Count-1)) ? currentQuiz+=1 : 0;
        Painter();
        mainPrompt.SetActive(true);
        winScreen.SetActive(false);
    }
}
