using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManagerScr : MonoBehaviour
{
    [SerializeField] Sprite[] images;
    [SerializeField] TextAsset jsonGameTexts;
    private MultiLangText _gameTexts;
    [SerializeField] TextAsset jsonLocal;
    private int opcionCorrecta;

    private List<QuizList> categoryList = new List<QuizList>();
    private List<QuizModel> quizes = new List<QuizModel>();
    private int currentQuiz;
    private int lang;
    [SerializeField] int maxLang;

    [SerializeField] Image[] figure;
    [SerializeField] TextMeshProUGUI hintPrompt;
    [SerializeField] TextMeshProUGUI answerPrompt;

    [SerializeField] GameObject mainPrompt;

    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject winEffect;
    [SerializeField] Image winIcon;


    private void Start()
    {
        lang=0;
        
        _gameTexts = JSONHandler.ReaderJSONSingle<MultiLangText>(jsonGameTexts);
        mainPrompt.GetComponent<TextMeshProUGUI>().text = _gameTexts.texto[lang];
        
        
        opcionCorrecta = Random.Range(0,3);
        //categoryList = JSONHandler.ReadJSONLocal<QuizList>(jsonLocal,"all-quitzes.json");
        categoryList = JSONHandler.ReadJSONTextAsset<QuizList>(jsonLocal);
        quizes = categoryList[0].category;
        currentQuiz=0;
        Painter();
    }

    private void Painter()
    {
        for (int i=0;i<3;i++)
        {
            figure[i].sprite = images[quizes[currentQuiz].img[i]];
        }
        
        hintPrompt.text = quizes[currentQuiz].hint[opcionCorrecta].texto[lang];
        answerPrompt.text = quizes[currentQuiz].answer[opcionCorrecta].texto[lang];
    }

    public void buttonPress(int option)
    {
        WinnScreen(option);
    }

    private void WinnScreen(int opt)
    {
        mainPrompt.SetActive(false);
        hintPrompt.gameObject.SetActive(false);

        winIcon.sprite = images[quizes[currentQuiz].img[opcionCorrecta]];
        
        winScreen.SetActive(true);
        if (opt == opcionCorrecta+1)
        {
            winEffect.SetActive(true);
        }
        else
        {
            winEffect.SetActive(false);
        }
    }

    public void NextQuitz()
    {
        opcionCorrecta = Random.Range(0,3);
        currentQuiz = (currentQuiz<(quizes.Count-1)) ? currentQuiz+=1 : 0;
        Painter();
        mainPrompt.SetActive(true);
        hintPrompt.gameObject.SetActive(true);
        winScreen.SetActive(false);
    }
}
