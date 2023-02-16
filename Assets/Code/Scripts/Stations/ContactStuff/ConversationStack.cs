using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationStack
{
    private List<Conversation> convos;

    public ConversationStack()
    {
        convos = new List<Conversation>();
    }

    public void Push(Conversation convo)
    {
        Debug.Log("Pushing convo inside class");
        Debug.Log(convo.Prompt);
        convos.Add(convo);
    }

    public void Pop()
    {
        convos.RemoveAt(convos.Count - 1);
        Debug.Log(convos.Count);
    }

    public Conversation Top()
    {
        return convos[convos.Count - 1];
    }

    public bool IsEmpty()
    {
        if (convos.Count == 0)
            return true;

        return false;
    }

    public void Clear()
    {
        convos.Clear();
    }

    public void DisplayConvoStack()
    {
        Debug.Log("Displaying Stack");
        foreach (Conversation convo in convos)
        {
            Debug.Log(convo.Prompt);
        }
        Debug.Log("End display");
    }

    public List<Conversation> Convos 
    {
        get
        {
            return this.convos;
        }
    }
}
