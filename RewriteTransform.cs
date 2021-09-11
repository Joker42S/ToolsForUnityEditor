using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if true
[CustomEditor(typeof(Transform))]
public class RewriteTransform : Editor
{
    Editor editor;
    Transform targetTransform;
    Transform newParent;
    //SerializedProperty m_LocalPosition;
    bool enableInspector = false;
    bool keepLocalPos = false;
    //SerializedProperty m_LocalScale;


    // Use this for initialization
    void OnEnable()
    {
        editor = CreateEditor(target, Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.TransformInspector"));
        targetTransform = target as Transform;
        newParent = null;
        //var a = serializedObject.GetIterator();
        //Debug.Log(a.name);
        //Debug.Log(a.Next(true));
        //while (a.Next(true))
        //{
        //    Debug.Log(a.name);
        //}
    }
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;

        //base.OnInspectorGUI();
        //DrawDefaultInspector();
        editor.OnInspectorGUI();
        //start custom instpector
        if (enableInspector = GUILayout.Toggle(enableInspector, "Enable Custom Inspector"))
        {

            //world position
            Vector3 worldPosition = EditorGUILayout.Vector3Field("WorldPosition", targetTransform.position);
            if (worldPosition != targetTransform.position)
            {
                Undo.RecordObject(targetTransform, "Set world position");
                targetTransform.position = worldPosition;
            }
            //local position
            //m_LocalPosition = serializedObject.FindProperty("m_LocalPosition");
            //serializedObject.Update();
            //EditorGUILayout.PropertyField(m_LocalPosition, label);
            //serializedObject.ApplyModifiedProperties();
            //set parent
            Vector3 localPosTemp = targetTransform.localPosition;
            newParent = EditorGUILayout.ObjectField("Parent Transform", newParent, typeof(Transform), true) as Transform;
            if (newParent != null)
            {
                Vector3 localPosition = newParent.InverseTransformPoint(targetTransform.position);
                EditorGUILayout.Vector3Field("LocalPosition", localPosition);
            }
            EditorGUILayout.BeginHorizontal();
            keepLocalPos = EditorGUILayout.Toggle("Keep Local Position", keepLocalPos);
            if (GUILayout.Button("Change Parent"))
            {
                int groupID = Undo.GetCurrentGroup();
                Undo.SetTransformParent(targetTransform, newParent, "Set parent");
                if (keepLocalPos)
                {
                    Undo.RecordObject(targetTransform, "Set local position");
                    targetTransform.localPosition = localPosTemp;
                }
                Undo.CollapseUndoOperations(groupID);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
} 
#endif
