using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class DialogueEditor : EditorWindow
{
    public class DialogueNode
    {
        public Vector2 position;
        public int choiceID;
        public string text;
        public List<int> connections = new List<int>(); // Stores connected node IDs
        public int lineIndexInChoice = 0; // Position within the choice branch
    }

    public static DialogueEditor window;
    public static TextAsset dialogueFile;
    public static string dialogueFilePath;
    public static List<DialogueNode> nodes = new();
    public static DialogueNode selectedNode;
    private static Dictionary<int, float> branchStartX = new();

    // Camera/viewport variables
    private Vector2 cameraOffset = Vector2.zero;
    private Vector2 dragStartPos;
    private bool isDraggingCamera = false;
    private bool isDraggingNode = false;
    private float zoomLevel = 1f;
    private const float minZoom = 0.3f;
    private const float maxZoom = 2f;

    // Grid settings
    private const float gridSpacing = 50f;
    private Color gridColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

    public static string RelativeFilePath => dialogueFilePath.Substring(dialogueFilePath.IndexOf("Assets"));

    private Vector2 WorldToScreen(Vector2 world)
    {
        return (world + cameraOffset) * zoomLevel;
    }

    private Vector2 ScreenToWorld(Vector2 screen)
    {
        return (screen / zoomLevel) - cameraOffset;
    }


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

        nodes.Clear();
        Dictionary<int, List<string>> linesByChoiceID = new();

        // 1. Parse file
        string[] lines = dialogueFile.text.Split(
            new[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            int separatorIndex = line.IndexOf(':');
            if (separatorIndex <= 0) continue;

            if (!int.TryParse(line.Substring(0, separatorIndex).Trim(), out int choiceID))
                continue;

            if (!linesByChoiceID.ContainsKey(choiceID))
                linesByChoiceID[choiceID] = new List<string>();

            linesByChoiceID[choiceID].Add(line.Substring(separatorIndex + 1).Trim());
        }

        // 2. Create nodes (temporary positions)
        foreach (var kvp in linesByChoiceID)
        {
            int choiceID = kvp.Key;
            var choiceLines = kvp.Value;

            for (int i = 0; i < choiceLines.Count; i++)
            {
                var node = new DialogueNode
                {
                    choiceID = choiceID,
                    lineIndexInChoice = i,
                    text = choiceLines[i],
                    position = Vector2.zero // temp
                };

                ParseConnectionsFromText(node);
                nodes.Add(node);
            }
        }

        // 3. Sequential connections
        foreach (var node in nodes)
        {
            var nextNode = nodes.FirstOrDefault(n =>
                n.choiceID == node.choiceID &&
                n.lineIndexInChoice == node.lineIndexInChoice + 1
            );

            if (nextNode != null)
            {
                int index = nodes.IndexOf(nextNode);
                node.connections.Add(-index - 1);
            }
        }

        // 4. Build branchStartX
        branchStartX.Clear();
        const float startX = 50f;
        const float xSpacing = 240f;
        const float ySpacing = 160f;

        foreach (var node in nodes)
        {
            if (node.lineIndexInChoice == 0 && !branchStartX.ContainsKey(node.choiceID))
                branchStartX[node.choiceID] = startX;
        }

        foreach (var node in nodes)
        {
            foreach (int c in node.connections)
            {
                if (c >= 0) // real branch jump
                    branchStartX[c] = node.position.x;
            }
        }

        // 5. Build branchY AFTER branchStartX
        Dictionary<int, float> branchY = new();
        float nextY = 50f;

        foreach (int choice in branchStartX.Keys.OrderBy(k => k))
        {
            branchY[choice] = nextY;
            nextY += ySpacing;
        }

        // 6. Apply final positions
        foreach (var node in nodes)
        {
            float baseX = branchStartX[node.choiceID];
            float y = branchY[node.choiceID];

            node.position = new Vector2(
                baseX + node.lineIndexInChoice * xSpacing,
                y
            );
        }
    }

    private static void ParseConnectionsFromText(DialogueNode node)
    {
        // Look for <o>...</o> pattern for choices
        Regex optionRegex = new Regex(@"<o>(.*?)</o>");
        Match match = optionRegex.Match(node.text);

        if (match.Success)
        {
            string optionsContent = match.Groups[1].Value;
            string[] options = optionsContent.Split(',');

            foreach (string option in options)
            {
                string[] parts = option.Split(':');
                if (parts.Length == 2)
                {
                    // Second part is the choiceIndex this connects to
                    if (int.TryParse(parts[1].Trim(), out int targetChoiceID))
                    {
                        if (!node.connections.Contains(targetChoiceID))
                            node.connections.Add(targetChoiceID);
                    }
                }
            }
        }

        // Look for <j>...</j> pattern for jumps
        Regex jumpRegex = new Regex(@"<j>(.*?)</j>");
        Match jumpMatch = jumpRegex.Match(node.text);

        if (jumpMatch.Success)
        {
            string jumpContent = jumpMatch.Groups[1].Value;
            string[] parts = jumpContent.Split(':');

            if (parts.Length >= 2)
            {
                // Format: lineIndex:choiceIndex
                if (int.TryParse(parts[1].Trim(), out int targetChoiceID))
                {
                    if (!node.connections.Contains(targetChoiceID))
                        node.connections.Add(targetChoiceID);
                }
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

        // Group nodes by choiceID and lineIndex to maintain order
        var groupedNodes = nodes.OrderBy(n => n.choiceID).ThenBy(n => n.lineIndexInChoice);

        List<string> lines = new List<string>();
        foreach (var node in groupedNodes)
        {
            lines.Add($"{node.choiceID}: {node.text}");
        }
        System.IO.File.WriteAllLines(dialogueFilePath, lines);
        AssetDatabase.Refresh();
    }

    public void OnGUI()
    {
        if (window == null) return;

        // Create the UI header with buttons
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        // Display opened file name
        if (dialogueFile != null)
            GUILayout.Label($"Opened file: {dialogueFile.name}", EditorStyles.label);
        else
            GUILayout.Label("Opened file: None", EditorStyles.label);

        GUILayout.FlexibleSpace();

        // New, load, and save buttons
        if (GUILayout.Button("new file", EditorStyles.toolbarButton))
        {
            CreateNewDialogueFile();
        }
        if (GUILayout.Button("save file", EditorStyles.toolbarButton))
        {
            SaveDialogueFile();
        }
        if (GUILayout.Button("load file", EditorStyles.toolbarButton))
        {
            LoadDialogueFile();
        }

        GUILayout.FlexibleSpace();

        // Display node count
        GUILayout.Label($"# nodes: {nodes.Count}", EditorStyles.label);

        // Add/Remove buttons
        if (GUILayout.Button("add node", EditorStyles.toolbarButton))
        {
            DialogueNode newNode = new DialogueNode
            {
                choiceID = 0,
                text = "New Dialogue Line",
                lineIndexInChoice = 0,
                position = new Vector2(50, 50)
            };
            nodes.Add(newNode);
        }

        // Only enable remove button if selectedNode is not null
        GUI.enabled = selectedNode != null;
        if (GUILayout.Button("remove node", EditorStyles.toolbarButton))
        {
            nodes.Remove(selectedNode);
            selectedNode = null;
        }
        GUI.enabled = true;

        GUILayout.EndHorizontal();

        // Create a canvas area for the dialogue nodes
        Rect canvasRect = new Rect(0, 20, position.width, position.height - 20);
        EditorGUI.DrawRect(canvasRect, new Color(0.2f, 0.2f, 0.2f));

        // Draw grid
        DrawGrid(canvasRect);

        // Handle camera dragging and zooming
        HandleCameraInput(canvasRect);

        // Draw connections first
        DrawConnections();

        // Draw nodes
        BeginWindows();
        for (int i = 0; i < nodes.Count; i++)
        {
            DrawNode(nodes[i], i);
        }
        EndWindows();

        // Request repaint for smooth dragging
        if (isDraggingCamera || isDraggingNode)
            Repaint();
    }

    private void DrawGrid(Rect canvasRect)
    {
        Handles.BeginGUI();
        Handles.color = gridColor;

        float spacing = gridSpacing * zoomLevel;

        // Calculate grid offset based on camera position
        float xOffset = (cameraOffset.x * zoomLevel) % spacing;
        float yOffset = (cameraOffset.y * zoomLevel) % spacing;

        // Draw vertical lines
        for (float x = xOffset; x < canvasRect.width; x += spacing)
        {
            Handles.DrawLine(
                new Vector3(x, canvasRect.y, 0),
                new Vector3(x, canvasRect.y + canvasRect.height, 0)
            );
        }

        // Draw horizontal lines
        for (float y = canvasRect.y + yOffset; y < canvasRect.y + canvasRect.height; y += spacing)
        {
            Handles.DrawLine(
                new Vector3(0, y, 0),
                new Vector3(canvasRect.width, y, 0)
            );
        }

        Handles.EndGUI();
    }

    private void HandleCameraInput(Rect canvasRect)
    {
        Event e = Event.current;

        // Middle mouse button drag for panning
        if (e.type == EventType.MouseDown && e.button == 2 && canvasRect.Contains(e.mousePosition))
        {
            isDraggingCamera = true;
            dragStartPos = e.mousePosition;
            e.Use();
        }
        else if (e.type == EventType.MouseDrag && isDraggingCamera)
        {
            Vector2 delta = e.mousePosition - dragStartPos;
            cameraOffset += delta / zoomLevel;
            dragStartPos = e.mousePosition;
            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 2)
        {
            isDraggingCamera = false;
            e.Use();
        }

        // Scroll wheel for zooming
        if (e.type == EventType.ScrollWheel && canvasRect.Contains(e.mousePosition))
        {
            float oldZoom = zoomLevel;
            zoomLevel = Mathf.Clamp(zoomLevel - e.delta.y * 0.05f, minZoom, maxZoom);

            Vector2 mouseWorldBefore = ScreenToWorld(e.mousePosition);
            Vector2 mouseWorldAfter = ScreenToWorld(e.mousePosition);

            cameraOffset += mouseWorldBefore - mouseWorldAfter;

            e.Use();
        }
    }

    private void DrawConnections()
    {
        foreach (var node in nodes)
        {
            foreach (int connection in node.connections)
            {
                DialogueNode targetNode = null;

                if (connection < 0)
                {
                    // Sequential connection (negative index marker)
                    int targetIndex = -connection - 1;
                    if (targetIndex >= 0 && targetIndex < nodes.Count)
                        targetNode = nodes[targetIndex];
                }
                else
                {
                    // Choice connection - find first node with this choiceID
                    targetNode = nodes.FirstOrDefault(n => n.choiceID == connection && n.lineIndexInChoice == 0);
                }

                if (targetNode != null)
                {
                    Vector2 startPos = WorldToScreen(node.position + new Vector2(180, 50)); // right side
                    Vector2 endPos = WorldToScreen(targetNode.position + new Vector2(0, 50)); // left side

                    // Draw curved connection line
                    Handles.DrawBezier(
                        startPos,
                        endPos,
                        startPos + Vector2.right * 50,
                        endPos + Vector2.left * 50,
                        Color.white,
                        null,
                        2f
                    );

                    // Draw arrow at the end
                    Vector2 arrowDir = (startPos - endPos).normalized;
                    Vector2 arrowPerp = new Vector2(-arrowDir.y, arrowDir.x);
                    Handles.color = Color.white;
                    Handles.DrawAAPolyLine(3f,
                        endPos,
                        endPos + arrowDir * 10 + arrowPerp * 5,
                        endPos + arrowDir * 10 - arrowPerp * 5,
                        endPos
                    );
                }
            }
        }
    }

    public void DrawNode(DialogueNode node, int windowID)
    {
        Vector2 screenPos = WorldToScreen(node.position);
        Rect nodeRect = new Rect(screenPos.x, screenPos.y, 180 * zoomLevel, 100 * zoomLevel);

        // Highlight selected node
        Color backgroundColor = (node == selectedNode) ? new Color(0.3f, 0.5f, 0.8f) : new Color(0.25f, 0.25f, 0.25f);

        GUI.backgroundColor = backgroundColor;

        // Node window
        nodeRect = GUI.Window(windowID, nodeRect, (id) =>
        {
            // Node header with index
            GUILayout.Label($"Node index: {node.choiceID}", EditorStyles.boldLabel);

            GUILayout.Space(5);

            // Dialogue text (editable)
            GUILayout.Label("Dialogue text:", EditorStyles.label);

            // Use TextArea for multi-line editing
            string newText = EditorGUILayout.TextArea(node.text, GUILayout.Height(50));
            if (newText != node.text)
            {
                node.text = newText;
                // Re-parse connections when text changes
                node.connections.Clear();
                ParseConnectionsFromText(node);

                // Re-add sequential connection if needed
                DialogueNode nextNode = nodes.Find(n =>
                    n.choiceID == node.choiceID &&
                    n.lineIndexInChoice == node.lineIndexInChoice + 1
                );
                if (nextNode != null)
                {
                    int nextNodeIndex = nodes.IndexOf(nextNode);
                    if (!node.connections.Contains(-nextNodeIndex - 1))
                        node.connections.Add(-nextNodeIndex - 1);
                }
            }

            // Handle node selection
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                selectedNode = node;
                isDraggingNode = true;
                GUI.FocusControl(null); // Unfocus text fields when clicking outside
                Repaint();
            }

            // Allow dragging
            GUI.DragWindow();
        }, "");

        GUI.backgroundColor = Color.white;

        // Update node position when dragged
        if (!isDraggingCamera)
        {
            node.position = ScreenToWorld(nodeRect.position);
        }

        if (Event.current.type == EventType.MouseUp)
        {
            isDraggingNode = false;
        }
    }
}