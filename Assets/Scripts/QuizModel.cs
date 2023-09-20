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
    public bool isLlocked;
    public bool[] categoryStars;
    public int costToUnlock;
    public List<QuizModel> category = new List<QuizModel>();
    public void QLister(List<QuizModel> q, bool l, bool[] s, int c)
    {
        isLlocked = l;
        categoryStars = s;
        costToUnlock = c;
        category = q;
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
