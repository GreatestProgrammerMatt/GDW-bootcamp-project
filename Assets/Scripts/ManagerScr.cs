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
    private List<MultiLangText> _gameTexts =new List<MultiLangText>();
    [SerializeField] string jsonGameDataFile;
    private GameDataSave _gameData;
    
    [SerializeField] TextAsset jsonLocal; 
    private List<QuizList> categoryList = new List<QuizList>();
    private List<QuizModel> quizes = new List<QuizModel>();
    private int currentCategory;
    private int currentQuiz;
    private List<GameObject> categoryButtons = new List<GameObject>();
    
    [Header("Game data")]
    [SerializeField] int maxLang;
    [SerializeField] private int lang;
    [SerializeField] private int score;
    [SerializeField] private bool sound;
    [SerializeField] private bool firstTime;
    private int opcionCorrecta;
    
    [Header("Game object references")]
    [SerializeField] GameObject pantallaJuego;

    [SerializeField] GameObject pantallaCategos;
    [SerializeField] GameObject botonSelector;
    [SerializeField] GameObject grid;

    [SerializeField] GameObject pantallaPop;
    [SerializeField] GameObject messageBox;
    [SerializeField] TextMeshProUGUI popMessage;

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
    
    [Header("ColorReferences")]
    [SerializeField] Color VERDE_BOTON;
    [SerializeField] Color ROJO_BOTON;


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

        _gameTexts = JSONHandler.ReadJSONTextAsset<MultiLangText>(jsonGameTexts);
        mainPrompt.GetComponent<TextMeshProUGUI>().text = _gameTexts[0].texto[lang];

        //categoryList = JSONHandler.ReadJSONLocal<QuizList>(jsonLocal,"all-quitzes.json");
        categoryList = JSONHandler.ReadJSONTextAsset<QuizList>(jsonLocal);

        //Carga los botones para seleccionar la categoría
        int i = 0;
        foreach (QuizList qL in categoryList)
        {
            GameObject selector = Instantiate(botonSelector,grid.transform);
            selector.GetComponent<BotonSelector>().index = i; 
            selector.GetComponent<BotonSelector>().manager = this;
            selector.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = qL.nombre[lang];
            if (qL.isLocked)
            {
                selector.GetComponent<Image>().color = ROJO_BOTON;
                selector.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = qL.costToUnlock.ToString();
            }
            else
            {
                selector.GetComponent<Image>().color = VERDE_BOTON;
                selector.transform.GetChild(1).gameObject.SetActive(false);
            }
            categoryButtons.Add(selector);
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
        currentCategory=index;

        //Pregunto si está bloquedao, si lo está pop message 
        if (categoryList[currentCategory].isLocked)
        {
            //Pop message based on score
            PopScreenToggle(true);
            messageBox.SetActive(true);

            if (score < categoryList[currentCategory].costToUnlock)
            {
                popMessage.text=_gameTexts[1].texto[lang];
            }
            else
            {
                popMessage.text=_gameTexts[2].texto[lang];
            }
            
        }
        else
        {
            ShowCategories(false);
            QuitzStart(currentCategory);
        } 
    }

    public void OkButton()
    {
        if (score >= categoryList[currentCategory].costToUnlock)
            {
                score -= categoryList[currentCategory].costToUnlock;
                categoryList[currentCategory].isLocked=false;

                categoryButtons[currentCategory].GetComponent<Image>().color = VERDE_BOTON;
                categoryButtons[currentCategory].transform.GetChild(1).gameObject.SetActive(false);

                ShowCategories(false);
                QuitzStart(currentCategory);
            }
        PopScreenToggle(false);
    }



    private void ShowCategories(bool activate)
    {
        pantallaJuego.SetActive(!activate);
        pantallaCategos.SetActive(activate);
    }


    public void PopScreenToggle(bool activate)
    {
        pantallaPop.SetActive(activate);
    }
}
