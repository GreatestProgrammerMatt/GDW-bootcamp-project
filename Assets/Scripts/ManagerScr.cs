using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManagerScr : MonoBehaviour
{
    [Header("Archivos y arrays")]
    [SerializeField] Sprite[] images;
    [SerializeField] TextAsset jsonGameTexts;
    private MultiLangText _gameTexts;
    [SerializeField] string jsonGameDataFile;
    private GameDataSave _gameData;
    
    [SerializeField] TextAsset jsonLocal;
    
    
    private int opcionCorrecta;
    private List<QuizList> categoryList = new List<QuizList>();
    private List<QuizModel> quizes = new List<QuizModel>();
    private int currentCategory;
    private int currentQuiz;
    
    [Header("Game data")]
    [SerializeField] int maxLang;
    [SerializeField] private int lang;
    [SerializeField] private int score;
    [SerializeField] private bool sound;
    [SerializeField] private bool firstTime;
    
    [Header("Game object references")]
    [SerializeField] GameObject pantallaJuego;

    [SerializeField] GameObject pantallaCategos;
    [SerializeField] GameObject botonSelector;
    [SerializeField] GameObject grid;

    [SerializeField] TextMeshProUGUI scoreBoard;
    [SerializeField] GameObject progressionBar;
    [SerializeField] GameObject progressionDot;

    [SerializeField] Image[] figure;
    [SerializeField] TextMeshProUGUI hintPrompt;
    [SerializeField] TextMeshProUGUI answerPrompt;

    [SerializeField] GameObject mainPrompt;

    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject winEffect;
    [SerializeField] Image winIcon;


    private void Start()
    {

        //Loads Data from JSON files or TextAssets
        LoadProtocol();
        
    }

    private void LoadProtocol()
    {
        //Load game data (cambiar el JSON reader con file)
        _gameData = JSONHandler.ReaderJSONSingle<GameDataSave>(jsonGameDataFile);
        if (_gameData!=null)
        {
            lang = _gameData.SavedLang;
            score = _gameData.SavedScore;
            sound = _gameData.SavedSound;
            firstTime = _gameData.SavedFirstTime;
        }
        scoreBoard.text = score.ToString();

        _gameTexts = JSONHandler.ReaderJSONSingle<MultiLangText>(jsonGameTexts);
        mainPrompt.GetComponent<TextMeshProUGUI>().text = _gameTexts.texto[lang];

        //categoryList = JSONHandler.ReadJSONLocal<QuizList>(jsonLocal,"all-quitzes.json");
        categoryList = JSONHandler.ReadJSONTextAsset<QuizList>(jsonLocal);

        //Carga los botones para seleccionar la categoría
        int i = 0;
        foreach (QuizList qL in categoryList)
        {
            GameObject selector = Instantiate(botonSelector,grid.transform);
            selector.GetComponent<BotonSelector>().index = i; 
            selector.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = qL.nombre[lang];
            selector.GetComponent<BotonSelector>().manager = this;
            i+=1;
        }

        //Initialize the game proper
        Initialize();
    }

    
    
    
    private void Initialize()
    {
        if (firstTime)
        {
            currentCategory=0;
            QuitzStart(currentCategory);
        }
        else
        {
            //Mostrar la selección de categorías
            ShowCategories(true);
        }
    }



    private void QuitzStart(int selectedCategory)
    {
        quizes = categoryList[selectedCategory].quizList;
        opcionCorrecta = Random.Range(0,3);
        currentQuiz=0;
        Painter();
    }

    private void ProgressBar()
    {
        foreach (Transform child in progressionBar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //puebla la barra de progreso con las estrellas que haya
        for (int i = 0; i<10 ; i++)
        {
            Vector2 pos = progressionBar.transform.position;
            pos.x -= 270 - (60*i);
            GameObject dot = Instantiate(progressionDot,pos,progressionBar.transform.rotation,progressionBar.transform);
            if (categoryList[currentCategory].categoryStars[i])
            {
                dot.transform.GetChild(2).gameObject.SetActive(true);
            }
            if (i==currentQuiz)
            {
                dot.transform.localScale = new Vector3 (2,2,2);
            }
        }
    }


    private void Painter()
    {
        for (int i=0;i<3;i++)
        {
            figure[i].sprite = images[quizes[currentQuiz].img[i]];
        }
        
        hintPrompt.text = quizes[currentQuiz].hint[opcionCorrecta].texto[lang];
        answerPrompt.text = quizes[currentQuiz].answer[opcionCorrecta].texto[lang];
        ProgressBar();
    }


    //Botones
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
            //check if star unlocked
            if (!categoryList[currentCategory].categoryStars[currentQuiz])
            {
                score+=1;
                scoreBoard.text = score.ToString();
                categoryList[currentCategory].categoryStars[currentQuiz] = true;
                ProgressBar();
            }
            
        }
        else
        {
            winEffect.SetActive(false);
        }
    }

    public void NextQuiz()
    {
        //if it is the last quiz or the tenth, move on to categories.
        if ( currentQuiz == 10 || currentQuiz == quizes.Count-1)
        {
            //Salir de los quizes e ir a las categorías
            ShowCategories(true);
        }
        else
        {
            //Move to next quiz
            opcionCorrecta = Random.Range(0,3);
            currentQuiz+=1;
            Painter();
        }
        mainPrompt.SetActive(true);
        hintPrompt.gameObject.SetActive(true);
        winScreen.SetActive(false);
    }

    public void SelectCategory(int index)
    {
        ShowCategories(false);
        currentCategory=index;
        QuitzStart(currentCategory); 
    }



    private void ShowCategories(bool activate)
    {
        pantallaJuego.SetActive(!activate);
        pantallaCategos.SetActive(activate);
    }
}
