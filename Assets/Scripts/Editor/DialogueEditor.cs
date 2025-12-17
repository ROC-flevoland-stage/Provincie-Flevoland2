using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DialogueEditor : EditorWindow
{
    public class DialogueNode
    {
        public Vector2 position;
        public int choiceID;
        public string text;
        public DialogueNode[] next;
    }

    public static DialogueEditor window;
    public static TextAsset dialogueFile;
    public static string dialogueFilePath;
    public static List<DialogueNode> nodes = new();
    public static DialogueNode selectedNode;

    public static string RelativeFilePath => dialogueFilePath.Substring(dialogueFilePath.IndexOf("Assets"));

    public static void ShowWindow()
    {
        // Show existing window instance. If one doesn't exist, make one.
        GetWindow<DialogueEditor>("Dialogue Editor");

        // Focus the window
        FocusWindowIfItsOpen<DialogueEditor>();

        // Set minimum size
        window = GetWindow<DialogueEditor>();
        window.minSize = new Vector2(400, 300);

        ParseDialogueFile();
    }

    public static void ParseDialogueFile()
    {
        if (dialogueFile == null) return;

        // Read all lines from the dialogue file
        string[] lines = dialogueFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            DialogueNode node = new DialogueNode();
            string line = lines[i].Trim();
            int separatorIndex = line.IndexOf(':');
            if (separatorIndex > 0)
            {
                // Extract choice index and dialogue line
                if (int.TryParse(line.Substring(0, separatorIndex).Trim(), out int lineNumber))
                {
                    // Populate the dialogue node
                    node.choiceID = lineNumber;
                    node.text = line.Substring(separatorIndex + 1).Trim();
                    node.position = new Vector2(i * 200, lineNumber * 200);
                    nodes.Add(node);
                }
                else
                    Debug.LogWarning($"Invalid choice index number in dialogue file line {i}: {line}");
            }
        }
    }

    [MenuItem("Tools/Dialogue/Load")]
    public static void LoadDialogueFile()
    {
        string path = EditorUtility.OpenFilePanel("Load Dialogue File", "Assets/Resources", "txt");
        if (!string.IsNullOrEmpty(path))
        {
            // Load the dialogue file logic here
            dialogueFilePath = path;
            dialogueFile = AssetDatabase.LoadAssetAtPath<TextAsset>(RelativeFilePath);
            ShowWindow();
        }
        else Debug.LogWarning("No dialogue file selected.");
    }

    [MenuItem("Tools/Dialogue/New")]
    public static void CreateNewDialogueFile()
    {
        dialogueFilePath = EditorUtility.SaveFilePanelInProject("Create New Dialogue File", "NewDialogue", "txt", "Specify where to save the new dialogue file.");
        if (!string.IsNullOrEmpty(dialogueFilePath))
        {
            // Create the new dialogue file
            System.IO.File.WriteAllText(dialogueFilePath, "");
            AssetDatabase.Refresh();
            dialogueFile = AssetDatabase.LoadAssetAtPath<TextAsset>(RelativeFilePath);
        }
        else return;

        ShowWindow();
    }

    public static void SaveDialogueFile()
    {
        if (dialogueFile == null) return;
        List<string> lines = new List<string>();
        foreach (var node in nodes)
        {
            lines.Add($"{node.choiceID}: {node.text}");
        }
        System.IO.File.WriteAllLines(dialogueFilePath, lines);
        AssetDatabase.Refresh();
    }

    public void OnGUI()
    {
        if (window == null) return;

        // Create the UI header with the current dialogue file name, buttons to save, load or create a new file and buttons to add/remove nodes
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (dialogueFile != null)
            GUILayout.Label($"Dialogue File: {dialogueFile.name}", EditorStyles.boldLabel);
        else
            GUILayout.Label("No Dialogue File Loaded", EditorStyles.boldLabel);

        // New, load, and save buttons
        if (GUILayout.Button("New Dialogue", EditorStyles.toolbarButton))
        {
            CreateNewDialogueFile();
        }
        if (GUILayout.Button("Load Dialogue", EditorStyles.toolbarButton))
        {
            LoadDialogueFile();
        }
        if (GUILayout.Button("Save Dialogue", EditorStyles.toolbarButton))
        {
            SaveDialogueFile();
        }

        // Display node count and buttons to add/remove nodes
        GUILayout.Label($"Nodes: {nodes.Count}", EditorStyles.boldLabel);

        if (GUILayout.Button("Add", EditorStyles.toolbarButton))
        {
            DialogueNode newNode = new DialogueNode
            {
                choiceID = nodes.Count,
                text = "New Dialogue Line",
                position = new Vector2(50, 50)
            };
            nodes.Add(newNode);
        }
        // Only set remove button interactable if selectedNode is not null
        GUI.enabled = selectedNode != null;
        if (GUILayout.Button("Remove", EditorStyles.toolbarButton))
        {
            nodes.Remove(selectedNode);
            selectedNode = null;
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();

        // Create a canvas area for the dialogue nodes
        Rect canvasRect = new Rect(0, 20, position.width, position.height - 20);
        EditorGUI.DrawRect(canvasRect, new Color(0.1f, 0.1f, 0.1f)); // light black

        // Draw each dialogue node as a box within the canvas area
        foreach (var node in nodes)
        {
            DrawNode(node);
        }
    }

    public void DrawNode(DialogueNode node)
    {
        Rect nodeRect = new Rect(node.position.x, node.position.y, 180, 100);

        // Node dragging
        nodeRect = GUI.Window(node.choiceID, nodeRect, _ =>
        {
            GUILayout.Label($"ID: {node.choiceID}", EditorStyles.boldLabel);
            node.text = EditorGUILayout.TextField(node.text);
            GUI.DragWindow();
        }, "");
    }
}
