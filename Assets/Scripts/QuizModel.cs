using System;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public class QuizModel
{
    public int img;
    public int opt;
    public string answer;

    public void Quizer(int i, int o, string s)
    {
        img=i;
        opt=o;
        answer=s;
    }
}

[Serializable]
public class QuizList
{
    public List<QuizModel> qList = new List<QuizModel>();
    public void QLister(List<QuizModel> q)
    {
        qList = q;
    }
}
