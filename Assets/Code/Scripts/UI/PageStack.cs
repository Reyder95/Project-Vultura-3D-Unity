using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PageStack
{
    private List<VisualElement> stack = new List<VisualElement>();

    public VisualElement Pop()
    {
        VisualElement poppedPage = stack[stack.Count - 1];
        poppedPage.style.display = DisplayStyle.None;
        stack.RemoveAt(stack.Count - 1);

        stack[stack.Count - 1].style.display = DisplayStyle.Flex;
        
        return poppedPage;
    }

    public void Push(VisualElement newPage)
    {
        if (stack.Count > 0)
            stack[stack.Count - 1].style.display = DisplayStyle.None;

        newPage.style.display = DisplayStyle.Flex;

        stack.Add(newPage);
    }

    public VisualElement Top()
    {
        return stack[stack.Count - 1];
    }

    public VisualElement PreviousPage()
    {
        if (CanGoBack())
            return stack[stack.Count - 2];

        return null;
    }

    public bool CanGoBack()
    {
        if (stack.Count > 1)
            return true;

        return false;
    }
}