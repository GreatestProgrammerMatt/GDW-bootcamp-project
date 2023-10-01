using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ManagerScr : MonoBehaviour
{
    [Header("Archivos y arrays")]
    [SerializeField] private Sprite[] images;
    [SerializeField] private TextAsset jsonGameTexts;
    private List<MultiLangText> _gameTexts =new List<MultiLangText>();
    [SerializeField] private TextAsset jsonGameDataAsset;
    private GameDataSave _gameData;
    [SerializeField] private string gameDataSaveFaileName;

    [SerializeField] private TextAsset jsonLocal;
    [SerializeField] private string quizListFileName;
    private List<QuizList> categoryList = new List<QuizList>();
    private List<QuizModel> quizes = new List<QuizModel>();
    private int currentCategory;
    private int currentQuiz;
    private List<GameObject> categoryButtons = new List<GameObject>();
    
    [Header("Game data")]
    [SerializeField] private int maxLang;
    [SerializeField] private int lang;
    [SerializeField] private int score;
    [SerializeField] private bool sound;
    [SerializeField] private bool ads;
    [SerializeField] private bool firstTime;
    [SerializeField] private int quizesPerCategory;
    private int opcionCorrecta;
    private bool canSelectCategory=true;
    private bool streak;
    
    [Header("Game object references")]
    [SerializeField] private Image background;
    [SerializeField] private GameObject pantallaJuego;

    [SerializeField] private GameObject pantallaCategos;
    [SerializeField] private GameObject botonSelector;
    [SerializeField] private GameObject grid;

    [SerializeField] private GameObject pantallaPop;
    [SerializeField] private GameObject messageBox;
    [SerializeField] private TextMeshProUGUI popMessage;

    [SerializeField] private GameObject optPop;
    [SerializeField] private Sprite[] optIcons;
    [SerializeField] private Image soundButton;
    [SerializeField] private Image adsButton;
    [SerializeField] private Image flagButton;
    [SerializeField] private TextMeshProUGUI langButton;

    [SerializeField] private TextMeshProUGUI scoreBoard;
    [SerializeField] private GameObject progressionBar;
    [SerializeField] private GameObject progressionDot;

    [SerializeField] private Image[] figure;
    [SerializeField] private TextMeshProUGUI hintPrompt;
    [SerializeField] private TextMeshProUGUI answerPrompt;
    [SerializeField] private GameObject mainPrompt;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject winEffect;
    [SerializeField] private Image winIcon;
    [SerializeField] private Image winFrame;
    [Space(10)]
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private AudioClip[] audioClips;
    
    [Header("ColorReferences")]
    [SerializeField] private Color C_VERDE;
    [SerializeField] private Color C_ROJO;
    [SerializeField] private Color[] C_BKG;


    private void Start()
    {

        //Loads Data from JSON files or TextAssets
        LoadProtocol();
        
    }

    private void LoadProtocol()
    {
        //Load game data
        _gameData = JSONHandler.ReaderJSONSingle<GameDataSave>(jsonGameDataAsset,gameDataSaveFaileName);
        if (_gameData!=null)
        {
            lang = _gameData.SavedLang;
            score = _gameData.SavedScore;
            sound = _gameData.SavedSound;
            firstTime = _gameData.SavedFirstTime;
        }


        _gameTexts = JSONHandler.ReadJSONTextAsset<MultiLangText>(jsonGameTexts);
        mainPrompt.GetComponent<TextMeshProUGUI>().text = _gameTexts[0].texto[lang];

        scoreBoard.text = score.ToString();
        if (sound) {soundButton.sprite = optIcons[0];}else{soundButton.sprite = optIcons[1];}
        AudioListener.pause = !sound;
        if (ads) {adsButton.sprite = optIcons[2];}else{adsButton.sprite = optIcons[3];}
        langButton.text = _gameTexts[3].texto[lang];
        flagButton.sprite = optIcons[4+lang];

        categoryList = JSONHandler.ReadJSONLocal<QuizList>(jsonLocal,quizListFileName);
        //categoryList = JSONHandler.ReadJSONTextAsset<QuizList>(jsonLocal);

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
                selector.GetComponent<Image>().color = C_ROJO;
                selector.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = qL.costToUnlock.ToString();
            }
            else
            {
                selector.GetComponent<Image>().color = C_VERDE;
                selector.transform.GetChild(1).gameObject.SetActive(false);
            }

            selector.GetComponent<Tweener>().myTween = Tweener.ETypeOfTween.PopUpGrow;
            selector.GetComponent<Tweener>().speed = 0.4f;
            selector.GetComponent<Tweener>().delay = 0.1f*i;
            selector.GetComponent<Tweener>().ease = LeanTweenType.easeInOutBack;

            categoryButtons.Add(selector);
            i+=1;
        }

        background.color = C_BKG[0];

        //Initialize the game proper
        Initialize();
    }

    
    
    
    private void Initialize()
    {
        if (firstTime)
        {
            firstTime=false;
            StartCoroutine(SaveGameData(gameDataSaveFaileName));
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
        streak=false;
        ProgressBar();
        Painter();
    }

    private void ProgressBar()
    {
        foreach (Transform child in progressionBar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //puebla la barra de progreso con las estrellas que haya
        AudioPlay(1,1f,3);
        
        Vector2 pos = progressionBar.transform.position;
        progressionBar.transform.localScale = new Vector3 (0,0,0);
        LeanTween.scale(progressionBar,new Vector3(1,1,1),0.2f).setDelay(0.2f).setEase(LeanTweenType.linear).setOnComplete(
            ()=>{
                    for (int i = 0; i< quizesPerCategory ; i++)
                    {
                        pos.y += 2f;
                        GameObject dot = Instantiate(progressionDot,pos,progressionBar.transform.rotation,progressionBar.transform);
                        
                        dot.transform.localScale  = new Vector3 (0,0,0);
                        LeanTween.scale(dot,new Vector3(2,2,2),0.2f).setDelay(0.1f*i).setEase(LeanTweenType.easeInOutBack).setOnComplete(
                            ()=> {LeanTween.scale(dot,new Vector3(1,1,1),0.2f);}
                        );
                        
                        if (categoryList[currentCategory].categoryStars[i])
                        {
                            dot.transform.GetChild(1).gameObject.SetActive(true);
                        }
                        if (i==0)
                        {
                            dot.transform.localScale = new Vector3 (2,2,2);
                        }
                    }
                    LeanTween.scale(progressionBar.transform.GetChild(0).gameObject,new Vector3(2,2,2),0.2f).setDelay(0.5f);
                    
            }
        );
    }

    private void ProgressionDotCheck()
    {
        foreach(Transform child in progressionBar.transform)
        {
            if (child.GetChild(1).GetChild(0).gameObject.activeSelf) {child.GetChild(1).GetChild(0).gameObject.SetActive(false);}
            child.localScale = new Vector3(1,1,1);
        }
        LeanTween.scale(progressionBar.transform.GetChild(currentQuiz).gameObject,new Vector3(2,2,2),0.2f);
    }


    private void Painter()
    {
        int colorAleatorio = Random.Range(0,C_BKG.Length);
        background.color = C_BKG[colorAleatorio];
        if (colorAleatorio < C_BKG.Length-1) {winFrame.color = C_BKG[colorAleatorio+1];}else{winFrame.color = C_BKG[0];}
        
        for (int i=0;i<3;i++)
        {
            figure[i].sprite = images[quizes[currentQuiz].img[i]];
        }
        
        hintPrompt.text = quizes[currentQuiz].hint[opcionCorrecta].texto[lang];
        answerPrompt.text = quizes[currentQuiz].answer[opcionCorrecta].texto[lang];
    }


/*
 
 ::::::::::::::::::'##:::::::'##:'########:::'#######::'########::'#######::'##::: ##:'########::'######::
 :::::::::::::::::'##:::::::'##:: ##.... ##:'##.... ##:... ##..::'##.... ##: ###:: ##: ##.....::'##... ##:
 ::::::::::::::::'##:::::::'##::: ##:::: ##: ##:::: ##:::: ##:::: ##:::: ##: ####: ##: ##::::::: ##:::..::
 :::::::::::::::'##:::::::'##:::: ########:: ##:::: ##:::: ##:::: ##:::: ##: ## ## ##: ######:::. ######::
 ::::::::::::::'##:::::::'##::::: ##.... ##: ##:::: ##:::: ##:::: ##:::: ##: ##. ####: ##...:::::..... ##:
 :::::::::::::'##:::::::'##:::::: ##:::: ##: ##:::: ##:::: ##:::: ##:::: ##: ##:. ###: ##:::::::'##::: ##:
 ::::::::::::'##:::::::'##::::::: ########::. #######::::: ##::::. #######:: ##::. ##: ########:. ######::
 ::::::::::::..::::::::..::::::::........::::.......::::::..::::::.......:::..::::..::........:::......:::
 
*/
    
    public void ResetTheGameAndGameFiles()
    {
        //Erease the local memory files trhough the JSONHandler
        JSONHandler.EraseFile(gameDataSaveFaileName);
        JSONHandler.EraseFile(quizListFileName);

        //Then restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        float pitch = 1f;

        if (opt == opcionCorrecta)
        {
            winEffect.SetActive(true);
            //check if star unlocked
            if (!categoryList[currentCategory].categoryStars[currentQuiz])
            {
                score+=1;
                scoreBoard.text = score.ToString();
                StartCoroutine(SaveGameData(gameDataSaveFaileName));

                categoryList[currentCategory].categoryStars[currentQuiz] = true;
                StartCoroutine(SaveData(categoryList,quizListFileName));
                
                progressionBar.transform.GetChild(currentQuiz).GetChild(1).gameObject.SetActive(true);
                progressionBar.transform.GetChild(currentQuiz).GetChild(1).GetChild(0).gameObject.SetActive(true);

                if (!streak) { streak=true; } else { pitch+=0.2f; }
                AudioPlay(0,pitch,0);
            }
            else
            {
                AudioPlay(0,1f,1);
            }
            
        }
        else
        {
            if (streak) {streak=false; pitch=1f;}
            AudioPlay(0,pitch,2);
            winEffect.SetActive(false);
        }
    }

    public void NextQuiz()
    {
        GameObject tweeningObject = winScreen.transform.GetChild(0).GetChild(1).gameObject;
        if (!LeanTween.isTweening(tweeningObject))
        {
            //if it is the last quiz or the tenth, move on to categories.
            if ( currentQuiz == quizesPerCategory-1 || currentQuiz == quizes.Count-1)
            {
                //Salir de los quizes e ir a las categorías

                ShowCategories(true);
            }
            else
            {
                //Move to next quiz
                opcionCorrecta = Random.Range(0,3);
                currentQuiz+=1;
                ProgressionDotCheck();
                Painter();
            }
            mainPrompt.SetActive(true);
            hintPrompt.gameObject.SetActive(true);
            winScreen.SetActive(false);
        }
    }

    public void SelectCategory(int index)
    {
        if (canSelectCategory)
        {
            canSelectCategory=false;
            currentCategory=index;
            AudioPlay(1,2f);

            //Pregunto si está bloquedao, si lo está pop message 
            if (categoryList[currentCategory].isLocked)
            {
                LeanTween.scale(categoryButtons[index],new Vector3(0.8f,0.8f,0.8f),0.1f).setLoopPingPong(1).setOnComplete(()=>{canSelectCategory=true;});

                //Pop message based on score
                PopScreenToggle(true);
                messageBox.SetActive(true);
                optPop.SetActive(false);

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
                LeanTween.scale(categoryButtons[index],new Vector3(0.8f,0.8f,0.8f),0.1f).setLoopPingPong(1).setOnComplete(
                    ()=>{
                        ShowCategories(false);
                        canSelectCategory=true;
                        QuitzStart(currentCategory);
                    }
                );
            }
        }
         
    }

    public void OkButton()
    {
        if (!LeanTween.isTweening(messageBox.transform.GetChild(1).gameObject))
        {
            AudioPlay(1);
            LeanTween.scale(messageBox.transform.GetChild(1).gameObject,new Vector3(0.8f,0.8f,0.8f),0.1f).setLoopPingPong(1).setOnComplete(
                ()=> {
                        if (score >= categoryList[currentCategory].costToUnlock)
                        {
                            score -= categoryList[currentCategory].costToUnlock;
                            scoreBoard.text = score.ToString();
                            StartCoroutine(SaveGameData(gameDataSaveFaileName));

                            categoryList[currentCategory].isLocked=false;
                            StartCoroutine(SaveData(categoryList,quizListFileName));

                            categoryButtons[currentCategory].GetComponent<Image>().color = C_VERDE;
                            categoryButtons[currentCategory].transform.GetChild(1).gameObject.SetActive(false);

                            ShowCategories(false);
                            QuitzStart(currentCategory);
                        }
                        PopScreenToggle(false);
                }
            );
        }
    }

    public void OptionsMenuToggle()
    {
        PopScreenToggle(true);
        messageBox.SetActive(false);
        optPop.SetActive(true);
        AudioPlay(1,2f);
    }

    public void SoundOptionToggle()
    {
        if (!LeanTween.isTweening(soundButton.gameObject))
        {
            AudioPlay(1);
            LeanTween.scale(soundButton.gameObject,new Vector3(0.8f,0.8f,0.8f),0.1f).setLoopPingPong(1).setOnComplete(
                ()=> {
                    sound = !sound;
                    AudioListener.pause = !sound;
                    if (sound) {soundButton.sprite = optIcons[0];}else{soundButton.sprite = optIcons[1];}
                    StartCoroutine(SaveGameData(gameDataSaveFaileName));
                }
            );
        }
    }
    public void AdsOptionToggle()
    {
        if (!LeanTween.isTweening(adsButton.gameObject))
        {
            AudioPlay(1);
            LeanTween.scale(adsButton.gameObject,new Vector3(0.8f,0.8f,0.8f),0.1f).setLoopPingPong(1).setOnComplete(
                ()=> {
                    ads = !ads;
                    if (ads) {adsButton.sprite = optIcons[2];}else{adsButton.sprite = optIcons[3];}
                }
            );
        }
    }
    
    
    public void LangOptionToggle()
    {
        if (!LeanTween.isTweening(optPop.transform.GetChild(2).gameObject))
        {
            AudioPlay(1);
            LeanTween.scale(optPop.transform.GetChild(2).gameObject,new Vector3(0.8f,0.8f,0.8f),0.1f).setLoopPingPong(1).setOnComplete(
                ()=> {
                    lang = (lang<maxLang-1)? lang+=1 : 0;
                    langButton.text = _gameTexts[3].texto[lang];
                    flagButton.sprite = optIcons[4+lang];

                    mainPrompt.GetComponent<TextMeshProUGUI>().text = _gameTexts[0].texto[lang];
                    if (pantallaJuego.activeSelf)
                    {
                        hintPrompt.text = quizes[currentQuiz].hint[opcionCorrecta].texto[lang];
                        answerPrompt.text = quizes[currentQuiz].answer[opcionCorrecta].texto[lang];
                    }
                    var i=0;
                    foreach (GameObject button in categoryButtons)
                    {
                        button.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = categoryList[i].nombre[lang];
                        i+=1;
                    }

                    StartCoroutine(SaveGameData(gameDataSaveFaileName));
                }
            );
        }
    }



    private void ShowCategories(bool activate)
    {
        if (activate)
        {
            AudioPlay(1,1f,3);
        }
        
        pantallaJuego.SetActive(!activate);
        pantallaCategos.SetActive(activate);
    }


    public void PopScreenToggle(bool activate)
    {
        if (!LeanTween.isTweening(messageBox) && !LeanTween.isTweening(optPop))
        {
            pantallaPop.SetActive(activate);
        }
    }


    private void AudioPlay(int source, float pitch=1f, int clip=2)
    {
        audioSources[source].clip = audioClips[clip];
        audioSources[source].pitch = pitch;
        audioSources[source].Play();
    }


/*
 
 ::::::::::::::::::'##:::::::'##::'######:::'#######::'########:::'#######::'##::::'##:'########:'####:'##::: ##:'########::'######::
 :::::::::::::::::'##:::::::'##::'##... ##:'##.... ##: ##.... ##:'##.... ##: ##:::: ##:... ##..::. ##:: ###:: ##: ##.....::'##... ##:
 ::::::::::::::::'##:::::::'##::: ##:::..:: ##:::: ##: ##:::: ##: ##:::: ##: ##:::: ##:::: ##::::: ##:: ####: ##: ##::::::: ##:::..::
 :::::::::::::::'##:::::::'##:::: ##::::::: ##:::: ##: ########:: ##:::: ##: ##:::: ##:::: ##::::: ##:: ## ## ##: ######:::. ######::
 ::::::::::::::'##:::::::'##::::: ##::::::: ##:::: ##: ##.. ##::: ##:::: ##: ##:::: ##:::: ##::::: ##:: ##. ####: ##...:::::..... ##:
 :::::::::::::'##:::::::'##:::::: ##::: ##: ##:::: ##: ##::. ##:: ##:::: ##: ##:::: ##:::: ##::::: ##:: ##:. ###: ##:::::::'##::: ##:
 ::::::::::::'##:::::::'##:::::::. ######::. #######:: ##:::. ##:. #######::. #######::::: ##::::'####: ##::. ##: ########:. ######::
 ::::::::::::..::::::::..:::::::::......::::.......:::..:::::..:::.......::::.......::::::..:::::....::..::::..::........:::......:::
 
*/

    private IEnumerator SaveData (List<QuizList> listaDeDatos, string fileName)
    {
        JSONHandler.SaveJSON<QuizList>(listaDeDatos,fileName);
        yield return null;
    }

    private IEnumerator SaveGameData (string fileName)
    {
        _gameData.SavedLang = lang;
        _gameData.SavedScore = score;
        _gameData.SavedSound = sound;
        _gameData.SavedFirstTime = firstTime;
        
        
        JSONHandler.WriteJSONSingle<GameDataSave>(_gameData,fileName);
        yield return null;
    }
}
