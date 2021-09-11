using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if true
public class RulerWindow : EditorWindow
{
    bool rulerEnabled = false;
    bool sceneRulerEnabled = false;
    Vector2 mousePos = Vector2.zero;
    Vector2 mousePosStart = Vector2.zero;
    Vector2 mousePosLast = Vector2.zero;
    Vector2 start2Last = Vector2.zero;
    float distance = 0;
    GUIStyle style = new GUIStyle();

    [MenuItem("SceneRuler/RulerWindow")]
    //[InitializeOnLoadMethod]
    public static void Init()
    {
        RulerWindow window = EditorWindow.GetWindow<RulerWindow>();
        window.Show();
    }
    void OnEnable()
    {
        //autoRepaintOnSceneChange = true;
    }
    void OnGUI()
    {
        GUILayout.Label("Scene Unit Ruler (Alt + Click to record position)");
        bool isEnabled = EditorGUILayout.Toggle("Enable Ruler", rulerEnabled);
        if (isEnabled != rulerEnabled)
        {
            TurnOnRuler(isEnabled);
            rulerEnabled = isEnabled;
        }
        sceneRulerEnabled = EditorGUILayout.Toggle("Ruler on scene", sceneRulerEnabled);
        GUILayout.Label("Current mouse position:" + mousePos.ToString("F2"));
        EditorGUILayout.LabelField("Starting mouse position:" + mousePosStart.ToString("F2"));
        EditorGUILayout.LabelField("Last mouse position:" + mousePosLast.ToString("F2"));
        EditorGUILayout.LabelField("From start to last:" + distance.ToString("F2") + start2Last.ToString("F2"));
    }

    void OnDisable()
    {
        rulerEnabled = false;
        sceneRulerEnabled = false;
    }

    public void TurnOnRuler(bool action)
    {
        SceneView.onSceneGUIDelegate -= UpdatePosition;
        if (action)
        {
            SceneView.lastActiveSceneView.in2DMode = true;
            SceneView.onSceneGUIDelegate += UpdatePosition;
        }
    }
    public void UpdatePosition(SceneView scene)
    {
        mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
        if (Event.current.alt && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
        {
            mousePosStart = mousePosLast;
            mousePosLast = mousePos;
            distance = Vector2.Distance(mousePosStart, mousePosLast);
            CountDistance();
            Event.current.Use();
        }
        Repaint();
        if (sceneRulerEnabled)
        {
            DrawSceneRuler();
        }
    }

    public void DrawSceneRuler()
    {
        style.normal.textColor = Color.green;
        Handles.BeginGUI();
        GUILayout.Label("Current mouse position:" + mousePos.ToString("F2"), style);
        GUILayout.Label("Starting mouse position:" + mousePosStart.ToString("F2"), style);
        GUILayout.Label("Last mouse position:" + mousePosLast.ToString("F2"), style);
        GUILayout.Label("From start to last:" + distance.ToString("F2") + start2Last.ToString("F2"), style);
        Handles.EndGUI();
        Handles.color = Color.red;
        Handles.DrawLine(mousePosStart, mousePosLast);
        Handles.Label(mousePosStart, "Start: " + mousePosStart.ToString("F2"), style);
        Handles.Label(mousePosLast, "End: " + mousePosLast.ToString("F2"), style);
        Handles.Label(mousePosStart + start2Last / 2, "DIstance: " + distance.ToString("F2") + start2Last.ToString("F2"), style);
        if (Event.current.alt)
        {
            Handles.color = Color.cyan;
            Handles.DrawLine(mousePosLast, mousePos);
            Handles.Label(mousePos, mousePos.ToString("F2"), style);
            Vector2 vectBetween = mousePos - mousePosLast;
            float distance2 = Vector2.Distance(mousePos, mousePosLast);
            Handles.Label(mousePosLast + vectBetween / 2, "DIstance: " + distance2.ToString("F2") + vectBetween.ToString("F2"), style);
        }
    }

    public void CountDistance()
    {
        start2Last = mousePosLast - mousePosStart;
    }
} 
#endif