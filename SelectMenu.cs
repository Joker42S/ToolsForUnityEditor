using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if true
[InitializeOnLoad]
public class SelectMenu
{
    static GenericMenu menu = new GenericMenu();
    static SelectMenu()
    {
        SceneView.onSceneGUIDelegate -= Update;
        SceneView.onSceneGUIDelegate += Update;
    }

    public static void Update(SceneView scene)
    {
        if (Event.current.control == true && Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            AddMenu();
            menu.ShowAsContext();
        }
    }

    static void AddMenu()
    {
        menu = new GenericMenu();
        if (Selection.gameObjects.Length > 0)
        {

            var root = Selection.activeTransform;

            // Show all children
            AddSelectMenu(root);

            // Merge Collider
            AddMergeCollider(root);
            AddMergeBoundsCollider(root);
        }
        else
        {
            AddTransform("     ", false, null);
        }
    }

    public static void AddMergeCollider(Transform root)
    {
        menu.AddItem(new GUIContent("Add Collider/Merge Collider"), false, MergeCollider, root);
    }

    public static void AddMergeBoundsCollider(Transform root)
    {
        menu.AddItem(new GUIContent("Add Collider/Merge Bounds"), false, MergeBound, root);
    }
    #region Add Collider
    public static void MergeCollider(object obj)
    {

        var rootTrans = obj as Transform;

        Collider[] allCollider = rootTrans.GetComponentsInChildren<Collider>();
        if (allCollider.Length == 0)
        {
            return;
        }

        Transform parent = rootTrans.parent;
        Vector3 position = rootTrans.localPosition;
        Vector3 scale = rootTrans.localScale;
        Quaternion rotaion = rootTrans.localRotation;

        rootTrans.parent = null;
        rootTrans.localRotation = Quaternion.Euler(Vector3.zero);
        rootTrans.localScale = Vector3.one;
        rootTrans.localPosition = Vector3.zero;

        Bounds mergeBound = allCollider[0].bounds;
        foreach (Collider collider in allCollider)
        {
            mergeBound.Encapsulate(collider.bounds);
        }
        
        rootTrans.parent = parent;
        rootTrans.localPosition = position;
        rootTrans.localRotation = rotaion;
        rootTrans.localScale = scale;

        foreach (Collider collider in allCollider)
        {
            Undo.DestroyObjectImmediate(collider);
        }
        BoxCollider mergeCollider = Undo.AddComponent<BoxCollider>(rootTrans.gameObject);
        mergeCollider.center = mergeBound.center;
        mergeCollider.size = mergeBound.size;
    }

    public static void MergeBound(object obj)
    {
        Transform rootTrans = obj as Transform;

        Renderer[] allRenderer = rootTrans.GetComponentsInChildren<Renderer>();
        if (allRenderer.Length == 0)
        {
            return;
        }

        Transform parent = rootTrans.parent;
        Vector3 position = rootTrans.localPosition;
        Vector3 scale = rootTrans.localScale;
        Quaternion rotaion = rootTrans.localRotation;

        rootTrans.parent = null;
        rootTrans.localRotation = Quaternion.Euler(Vector3.zero);
        rootTrans.localScale = Vector3.one;
        rootTrans.localPosition = Vector3.zero;

        Bounds mergeBound = allRenderer[0].bounds;
        foreach (var render in allRenderer)
        {
            mergeBound.Encapsulate(render.bounds);
        }

        rootTrans.parent = parent;
        rootTrans.localPosition = position;
        rootTrans.localRotation = rotaion;
        rootTrans.localScale = scale;

        BoxCollider mergeCollider = Undo.AddComponent<BoxCollider>(rootTrans.gameObject);
        mergeCollider.center = mergeBound.center;
        mergeCollider.size = mergeBound.size;
    }
    #endregion

    #region AddSelectMenu
    static void AddSelectMenu(Transform obj)
    {
        AddWithChild(obj, "Select/");
        menu.AddSeparator("Select/");
        while (obj.parent != null)
        {
            obj = obj.parent;
            AddWithChild(obj, "Select/");
        }
        AddWithChild(obj, "Select/");
    }
    static void AddWithChild(Transform obj, string pathPrefix)
    {
        AddTransform(pathPrefix + obj.name, false, obj);
        int childNum = obj.transform.childCount;
        if (childNum > 0)
        {
            for (int i = 0; i < childNum; ++i)
            {
                AddWithChild(obj.GetChild(i), pathPrefix + obj.name + "/");
            }
            menu.AddSeparator(pathPrefix);
        }
    }
    static void AddTransform(string path, bool active, Transform obj)
    {
        menu.AddItem(new GUIContent(path), active, SelectObj, obj);
    }
    private static void SelectObj(object obj)
    {
        Selection.activeTransform = obj as Transform;
    }
    #endregion
}
#endif
