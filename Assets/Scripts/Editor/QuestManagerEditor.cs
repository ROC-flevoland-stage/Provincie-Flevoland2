using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestManager))]
public class QuestManagerEditor : Editor
{
    private bool showActiveQuests = true;
    private bool showCompletedQuests = false;
    private bool showQuestDetails = false;

    public override void OnInspectorGUI()
    {
        QuestManager questManager = (QuestManager)target;

        showActiveQuests = EditorGUILayout.Foldout(true, "Active Quests");
        if (questManager.ActiveQuests != null && showActiveQuests)
        {
            foreach (var quest in questManager.ActiveQuests)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Name: " + quest.Name, EditorStyles.label);
                EditorGUILayout.LabelField("Description: " + quest.Description, EditorStyles.label);
                showQuestDetails = EditorGUILayout.Foldout(true, "Associated Objects");
                if (showQuestDetails)
                {
                    foreach (var obj in quest.AssociatedObjects)
                    {
                        EditorGUILayout.LabelField("- " + obj.name, EditorStyles.miniLabel);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
        showCompletedQuests = EditorGUILayout.Foldout(true, "Completed Quests");
        if (questManager.CompletedQuests != null && showCompletedQuests)
        {
            foreach (var quest in questManager.CompletedQuests)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Name: " + quest.Name, EditorStyles.label);
                EditorGUILayout.LabelField("Description: " + quest.Description, EditorStyles.label);
                showQuestDetails = EditorGUILayout.Foldout(true, "Associated Objects");
                if (showQuestDetails)
                {
                    foreach (var obj in quest.AssociatedObjects)
                    {
                        EditorGUILayout.LabelField("- " + obj.name, EditorStyles.miniLabel);
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }

        DrawDefaultInspector();
    }
}
