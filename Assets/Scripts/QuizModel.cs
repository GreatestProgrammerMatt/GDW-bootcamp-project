using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class QuizModel
{
    public int[] img;
    public List<MultiLangText> hint = new List<MultiLangText>();
    public List<MultiLangText> answer = new List<MultiLangText>();

    public void Quizer(int[] i, List<MultiLangText> h, List<MultiLangText> a)
    {
        img=i;
        hint=h;
        answer=a;
    }
}

[Serializable]
public class QuizList
{
    public string[] nombre;
    public bool isLlocked;
    public bool[] categoryStars;
    public int costToUnlock;
    public List<QuizModel> quizList = new List<QuizModel>();
    public void QLister(string[] n, List<QuizModel> q, bool l, bool[] s, int c)
    {
        nombre = n;
        isLlocked = l;
        categoryStars = s;
        costToUnlock = c;
        quizList = q;
    }
}

[Serializable]
public class MultiLangText
{
    public string[] texto;
    public void MLTexter(string[] t)
    {
        texto = t;
    }
}

[Serializable]
public class GameDataSave
{
    public int SavedLang;
    public int SavedScore;
    public bool SavedSound;
    public bool SavedFirstTime;
    public void GDSinit(int l, int c, bool s, bool f)
    {
        SavedLang =l;
        SavedScore = c;
        SavedSound = s;
        SavedFirstTime = f;
    }
}