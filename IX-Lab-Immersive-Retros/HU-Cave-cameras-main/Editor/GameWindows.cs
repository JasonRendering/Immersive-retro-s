using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameWindows
{
    [MenuItem("Window/Cave/Cave Game Displays")]
    private static void ShowGameDisplays()
    {
        CreateOrGetWindow("Cave Forward", 1);
        CreateOrGetWindow("Cave Right", 2);
        CreateOrGetWindow("Cave Backward", 3);
        CreateOrGetWindow("Cave Left", 4);
    }

    private static void CreateOrGetWindow(string direction, int screenId)
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        var screens = (EditorWindow[])Resources.FindObjectsOfTypeAll(T);
        EditorWindow screen = null;
        foreach (var window in screens)
        {
            if (window.name == direction)
                screen = window;
        }

        if (screen == null)
        {
            screen = (EditorWindow)ScriptableObject.CreateInstance(T);
            screen.name = direction;
            screen.titleContent = new GUIContent(direction);
            screen.ShowUtility();
        }

        object[] parameters = { screenId };
        T.GetMethod("set_targetDisplay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(screen, parameters);

        Rect screenPos = new Rect();
        var list = new List<DisplayInfo>(8);
        Screen.GetDisplayLayout(list);

        if (screenId >= list.Count)
            throw new System.Exception("Less displays connected then expected with the cave. Are all of the displays connected? (including the pc display.)");

        for (int i = 1; i <= screenId; i++)
        {
            screenPos.x -= list[i].workArea.width; //TODO: make dynamic based on real position. Now it is assuming the main display is on the total right as unity {0,0} is the top left of the main display.
        }
        screenPos.width = list[screenId].workArea.width;
        screenPos.height = list[screenId].workArea.height;

        screen.position = screenPos;
    }
}
