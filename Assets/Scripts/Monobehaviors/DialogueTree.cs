using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTree : MonoBehaviour
{
    [System.Serializable]
    public struct InitialdialogueState
    {
        public int currentLineIndex;
        public int choiceIndex;
    }

    [SerializeField] private TextAsset dialogueFile;                             // The txt file containing the dialogue lines
    [SerializeField] private List<string> currentLines = new();                 // The current set of dialogue lines being used
    [SerializeField] private string currentLine;                                // The current dialogue line being displayed
    [SerializeField] private int currentLineIndex = 0;                          // The index of the current dialogue line
    [SerializeField] private int choiceIndex = 0;                               // The index of the current choice made by the player
    [SerializeField] private Dictionary<int, List<string>> allLines = new();    // All dialogue lines categorized by choice index
    [SerializeField] private Dictionary<int, string> choices = new();           // The current choices available to the player
    [SerializeField] private bool shouldClose;                                  // Whether the dialogue should close after the current line
    [SerializeField] private InitialdialogueState initialState;                  // The initial state of the dialogue for resetting

    public string CurrentLine => currentLine;
    public Dictionary<int, string> Choices => choices;

    private void Start()
    {
        // Set the initial state
        initialState.currentLineIndex = currentLineIndex;
        initialState.choiceIndex = choiceIndex;

        // Parse the dialogue file
        ParsedialogueFile();

        // Check if there are any lines for the initial choice index
        if (!allLines.TryGetValue(choiceIndex, out _))
        {
            Debug.LogWarning("No dialogue lines found for the initial choice index. No dialogue will be displayed.");
            return;
        }

        // Set the current lines to the initial choice index
        currentLines = allLines[choiceIndex];
        // Load the first line
        LoadLine(0);
    }

    /// <summary>
    /// Resets the dialogue to its initial state.
    /// </summary>
    public void Resetdialogue()
    {
        // Set the dialogue state to the initial state
        currentLineIndex = initialState.currentLineIndex;
        choiceIndex = initialState.choiceIndex;
        currentLines = allLines[choiceIndex];
        currentLine = currentLines[currentLineIndex];
        shouldClose = false;
    }

    /// <summary>
    /// Parses the dialogue file and populates the allLines dictionary.
    /// Lines in the dialogue file should be in the format:
    /// choiceIndex:dialogue line
    /// </summary>
    public void ParsedialogueFile()
    {
        // Check if the dialogue file exists
        if (dialogueFile == null)
        {
            Debug.LogError("dialogue file is not assigned.");
            return;
        }
        // Read all lines from the dialogue file
        string[] lines = dialogueFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            int separatorIndex = line.IndexOf(':');
            if (separatorIndex > 0)
            {
                // Extract choice index and dialogue line
                if (int.TryParse(line.Substring(0, separatorIndex).Trim(), out int lineNumber))
                {
                    // Check if the lineNumber key exists in the dictionary
                    if (!allLines.ContainsKey(lineNumber))
                        allLines[lineNumber] = new List<string>();

                    // Add the dialogue line to the appropriate list
                    string dialogueLine = line.Substring(separatorIndex + 1).Trim();
                    allLines[lineNumber].Add(dialogueLine);
                }
                else
                    Debug.LogWarning($"Invalid choice index number in dialogue file line {i}: {line}");
            }
        }
    }

    /// <summary>
    /// Parses a single line of dialogue for special tokens or formatting and trigger dialogue branches.
    /// </summary>
    /// <param name="line">The line to parse</param>
    /// <returns>The parsed line</returns>
    public string ParseLine(string line)
    {
        string parsedLine = line;
        string[] textFlags = { "<o>", "<j>", "<s>", "<c>", "<i>" };

        // Check the line for text flags and parse them
        foreach (string flag in textFlags)
        {
            while (parsedLine.Contains(flag))
                parsedLine = ParseLineTextFlag(parsedLine, flag);
        }

        string[] styleFlags = { "[italic", "[bold", "[strikethrough", "[underline", "[color" };
        // Check the line for style flags and parse them
        foreach (string flag in styleFlags)
        {
            while (parsedLine.Contains(flag))
                parsedLine = ParseLineStyleFlag(parsedLine, flag);
        }

        string[] animFlags = { "{shake", "{wave", "{spin", "{color", "{pulse"};
        // Check the line for animation flags and parse them
        foreach (string flag in animFlags)
        {
            while (parsedLine.Contains(flag))
                parsedLine = ParseLineAnimFlags(parsedLine, flag);
        }

        string[] varFlags = { "(display", "(change", "(add" };
        // Check the line for variable flags and parse them
        foreach (string flag in varFlags)
        {
            int tryCount = 0;
            while (parsedLine.Contains(flag) && tryCount < 1)
            {
                parsedLine = ParseLineVariableFlag(parsedLine, flag);
                tryCount++;
            }
        }

        return parsedLine;
    }

    /// <summary>
    /// Loads the dialogue line at the currentLineIndex plus the change value.
    /// </summary>
    /// <param name="change">The amount to change the currentLineIndex by</param>
    public void LoadLine(int change)
    {
        // Check if we should close the dialogue
        if (shouldClose)
        {
            Resetdialogue();
            DialogueManager.Instance.Stopdialogue();
            return;
        }

        // Update the current line index
        currentLineIndex += change;
        // Check if the current line index is out of bounds, if it is, reset the dialogue and close the textbox.
        if (currentLineIndex < 0 || currentLineIndex >= currentLines.Count)
        {
            Debug.LogWarning("Current line index is out of bounds.");
            Resetdialogue();
            DialogueManager.Instance.Stopdialogue();
            return;
        }
        // Parse and set the current line
        currentLine = ParseLine(currentLines[currentLineIndex]);
    }

    /// <summary>
    /// Advances to the next dialogue line.
    /// </summary>
    public void Next()
    {
        LoadLine(1);
    }

    /// <summary>
    /// Goes back to the previous dialogue line.
    /// </summary>
    public void Previous()
    {
        LoadLine(-1);
    }

    public void ChangeChoiceIndex(int newIndex)
    {
        // Check if the new index is the same as the current choice index. If it is, just load the next line
        if (newIndex == choiceIndex)
        {
            Next();
            DialogueManager.Instance.ReloadLine();
            return;
        }

        // Change the choice index and reset the current line index
        choiceIndex = newIndex;
        currentLineIndex = 0;
        currentLines = allLines[choiceIndex];
        LoadLine(0);
        DialogueManager.Instance.ReloadLine();
    }

    /// <summary>
    /// Gets the parameters for a text flag in a dialogue line.
    /// The format for text flags is: <flag>parameters</flag>
    /// </summary>
    /// <param name="line">The full line to parse.</param>
    /// <param name="flag">The flag to use.</param>
    /// <returns>
    /// A tuple containing:
    /// - startIndex: The index in the line where the opening tag of the flag ends.
    /// - endIndex: The index in the line where the closing tag of the flag begins.
    /// - parameters: An array of parameters extracted from the flag.
    /// </returns>
    private (int startIndex, int endIndex, string[] parameters) GetTextFlagParameters(string line, string flag)
    {
        int startIndex = line.IndexOf(flag) + flag.Length;
        string endFlag = flag[0] + "/" + flag.Substring(1);
        int endIndex = line.IndexOf(endFlag);
        string flagContent = line.Substring(startIndex, endIndex - startIndex).Trim();
        string[] parameters = flagContent.Split(',');
        return (startIndex, endIndex, parameters);
    }

    /// <summary>
    /// Parses a dialogue line for a specific text flag and performs the associated action.
    /// </summary>
    /// <param name="line">The line to parse</param>
    /// <param name="flag">The flag action to preform.</param>
    /// <returns>The new line without the flag.</returns>
    private string ParseLineTextFlag(string line, string flag)
    {
        var (startIndex, endIndex, parameters) = GetTextFlagParameters(line, flag);

        // Handle special flag actions here
        switch (flag)
        {
            // Option flag, opens a choice dialogue menu.
            // option format: <o>lineIndex:choiceIndex,lineIndex:choiceIndex,...</o>
            // lineIndex is the index of the dialogue line to display for that choice
            // choiceIndex is the index to set the choiceIndex to when that choice is selected
            case "<o>":
                choices = new();
                foreach (string choice in parameters)
                {
                    string[] parts = choice.Split(':');
                    int choiceLineIndex = int.Parse(parts[0].Trim());
                    int choiceIndex = int.Parse(parts[1].Trim());
                    choices[choiceIndex] = allLines[choiceLineIndex][0];
                    DialogueManager.Instance.ShowChoices();
                }
                break;

            // Jump flag, jumps to a specific dialogue line on the next LoadLine call.
            // jump format: <j>lineIndex</j> or <j>lineIndex:choiceIndex</j>
            // choiceIndex is optional, if provided it sets the choiceIndex to that value
            case "<j>":
                string[] jumpParams = parameters[0].Trim().Split(':');
                currentLineIndex = int.Parse(jumpParams[0].Trim()) - 1;
                if (jumpParams.Length == 2)
                {
                    choiceIndex = int.Parse(jumpParams[1].Trim());
                    currentLines = allLines[choiceIndex];
                }
                break;

            // Skip flag, skips over the specified amount of lines.
            // skip format: <s>linesToSkip</s>
            case "<s>":
                currentLineIndex += int.Parse(parameters[0].Trim());
                break;

            // Close flag, closes the textbox on the next LoadLine call.
            // close format: <c></c>
            case "<c>":
                shouldClose = true;
                break;

            // Initial state flag, changes the initial dialogue state.
            // initial state format: <i>c:choiceindex</i> or <i>l:lineindex</i>
            // c sets the initial choice index, l sets the initial line index
            case "<i>":
                switch (parameters[0].Trim()[0])
                {
                    case 'c':
                        initialState.choiceIndex = int.Parse(parameters[0].Trim().Substring(2));
                        break;
                    case 'l':
                        initialState.currentLineIndex = int.Parse(parameters[0].Trim().Substring(2));
                        break;
                    default:
                        Debug.LogWarning("Invalid initial state flag parameter: " + parameters[0]);
                        break;
                }
                break;

            default: break;
        }
        // Remove everything from startIndex to endIndex from the line
        line = line.Remove(startIndex - flag.Length, endIndex + flag.Length + 1 - (startIndex - flag.Length));
        return line;
    }

    /// <summary>
    /// Gets the parameters for a style flag in a dialogue line.
    /// The format for style flags is: [flag:parameters]stylised text[/flag]
    /// </summary>
    /// <param name="line">The full line to parse.</param>
    /// <param name="flag">The flag to use.</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">  
    /// <item><description>startIndex: The index in the line where the opening tag of the flag begins.</description></item>  
    /// <item><description>length: The length of the opening tag for the flag.</description></item>  
    /// <item><description>endIndex: The index in the line where the closing tag of the flag begins.</description></item>  
    /// <item><description>parameters: An array of parameters extracted from the flag.</description></item>  
    /// </list> 
    /// </returns>
    private (int startIndex, int length, int endIndex, string[] parameters) GetFlagParametersFromInsideTag(string line, string flag)
    {
        char flagStartChar = flag[0];
        Dictionary<char, char> startToEndChars = new()
        {
            { '[', ']' },
            { '{', '}' },
            { '(', ')' }
        };
        char flagEndChar = startToEndChars[flagStartChar];

        int startIndex = line.IndexOf(flag);
        string endFlag = flagStartChar + "/" + flag.Substring(1);
        int endIndex = line.IndexOf(endFlag);
        string openingTag = line.Substring(startIndex, line.IndexOf(flagEndChar, startIndex) - startIndex + 1);
        int length = openingTag.Length;
        string[] parameters = new string[0];
        // Check if there are parameters in the opening tag
        if (openingTag.Contains(":"))
        {
            string flagContent = openingTag.Split(":")[1].TrimEnd(flagEndChar).Trim();
            parameters = flagContent.Split(',');
        }
        return (startIndex, length, endIndex, parameters);
    }

    /// <summary>
    /// Calculates the new end index after replacing the opening tag with the TextMeshPro tag.
    /// </summary>
    /// <param name="endIndex">The originaly calculated endIndex.</param>
    /// <param name="length">The length of the opening tag.</param>
    /// <param name="tag">The style tag.</param>
    /// <returns>The correct endIndex after replacing the style flags for TextMeshPro tags.</returns>
    private int CalculateEndIndex(int endIndex, int length, string tag)
    {
        Dictionary<string, string> tagReplacements = new()
        {
            { "[italic]", "<i>" },
            { "[bold]", "<b>" },
            { "[strikethrough]", "<s>" },
            { "[underline]", "<u>" },
            { "[color]", $"<color=#000000>" }
        };
        // Calculate the new end index after replacing the opening tag with the TextMeshPro tag
        string textMeshProTag = tagReplacements[tag];
        int newEndIndex = endIndex - length + textMeshProTag.Length;
        return newEndIndex;
    }

    /// <summary>
    /// Parses a dialogue line for a specific style flag and replaces it with the appropriate TextMeshPro tags.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <param name="flag">The style flag to parse.</param>
    /// <returns>The new line with the style flag replaced.</returns>
    private string ParseLineStyleFlag(string line, string flag)
    {
        var (startIndex, length, endIndex, parameters) = GetFlagParametersFromInsideTag(line, flag);

        flag += ']'; // Add the closing "]" to the flag for easier comparison
        endIndex = CalculateEndIndex(endIndex, length, flag); // Initial calculation without replacement
        switch (flag)
        {
            // Italic tag
            case "[italic]":
                // Replace the opening tag with the TextMeshPro italic tag
                line = line.Remove(startIndex, length).Insert(startIndex, "<i>");
                // Replace the closing tag with the TextMeshPro italic closing tag
                line = line.Remove(endIndex, flag.Length + 1).Insert(endIndex, "</i>");
                break;

            // Bold tag
            case "[bold]":
                // Replace the opening tag with the TextMeshPro bold tag
                line = line.Remove(startIndex, length).Insert(startIndex, "<b>");
                // Replace the closing tag with the TextMeshPro bold closing tag
                line = line.Remove(endIndex, flag.Length + 1).Insert(endIndex, "</b>");
                break;

            // Strikethrough tag
            case "[strikethrough]":
                // Replace the opening tag with the TextMeshPro strikethrough tag
                line = line.Remove(startIndex, length).Insert(startIndex, "<s>");
                // Replace the closing tag with the TextMeshPro strikethrough closing tag
                line = line.Remove(endIndex, flag.Length + 1).Insert(endIndex, "</s>");
                break;

            // Underline tag
            case "[underline]":
                // Replace the opening tag with the TextMeshPro underline tag
                line = line.Remove(startIndex, length).Insert(startIndex, "<u>");
                // Replace the closing tag with the TextMeshPro underline closing tag
                line = line.Remove(endIndex, flag.Length + 1).Insert(endIndex, "</u>");
                break;

            // Color tag
            // color tag format: [color:hexColor]
            case "[color]":
                // Replace the opening tag with the TextMeshPro color tag
                line = line.Remove(startIndex, length).Insert(startIndex, $"<color={parameters[0].Trim()}>");
                // Replace the closing tag with the TextMeshPro color closing tag
                line = line.Remove(endIndex, flag.Length + 1).Insert(endIndex, "</color>");
                break;
        }
        return line;
    }

    /// <summary>
    /// Parses a dialogue line for a specific animation flag and performs the associated action.
    /// </summary>
    /// <param name="line"> The line to parse.</param>
    /// <param name="flag">The animation flag to parse.</param>
    /// <returns>The new line with the animation flag removed.</returns>
    private string ParseLineAnimFlags(string line, string flag)
    {
        var (startIndex, length, endIndex, p) = GetFlagParametersFromInsideTag(line, flag);
        endIndex -= length;
        var textMesh = DialogueManager.Instance.TextMesh;

        flag += '}'; // Add the closing "}" to the flag for easier comparison
        Dictionary<string, object> parameters = new();
        string param;
        string[] keyValue;
        bool isKeyValueFormat = false;
        switch (flag)
        {
            // Shake animation, makes the text shake back and forth
            // shake format: {shake:magnitude}
            // magnitude is a float value representing the shake intensity. 1 by default
            case "{shake}":
                if (p.Length != 0)
                {
                    param = p[0].Trim();
                    keyValue = param.Split('=');
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        parameters[key] = float.Parse(value);
                    }
                    else parameters["magnitude"] = float.Parse(param);
                }
                DialogueAnimator.StartTextShakeAnimation(textMesh, startIndex, endIndex, parameters);
                break;

            // Wave animation, makes the text move in a wavelike pattern
            // wave format: {wave:amplitude,angle,speed,delay}
            // amplitude is a float value representing how far each character moves from it's original position. 1 by default
            // angle is a float value representing the angle at which the characters move. 0 by default
            // speed is a float value representing how quickly a character completes a full wave cycle. 1 by default
            // delay is a float value representing the delay between each character's wave start time. 0.1 by default
            case "{wave}":
                for (int i = 0; i < p.Length; i++)
                {
                    param = p[i];
                    string[] keyValueWave = param.Split('=');
                    if (keyValueWave.Length == 2)
                    {
                        isKeyValueFormat = true;
                        string key = keyValueWave[0].Trim();
                        string value = keyValueWave[1].Trim();
                        parameters[key] = float.Parse(value);
                    }
                    else
                    {
                        if (isKeyValueFormat)
                        {
                            Debug.LogWarning("Inconsistent parameter format in wave animation flag. Mixing key=value and positional parameters is not allowed.");
                            continue;
                        }
                        string[] keys = { "amplitude", "angle", "speed", "delay" };
                        parameters[keys[i]] = float.Parse(param);
                    }
                }
                DialogueAnimator.StartTextWaveAnimation(textMesh, startIndex, endIndex, parameters);
                break;

            // Spin animation, makes the text spin around its center
            // spin format: {spin:speed}
            // speed is a float value representing how quickly the character spins clockswise around its center. 1 by default
            case "{spin}":
                if (p.Length != 0)
                {
                    param = p[0].Trim();
                    keyValue = param.Split('=');
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        parameters[key] = float.Parse(value);
                    }
                    else parameters["speed"] = float.Parse(param);
                }
                DialogueAnimator.StartTextSpinAnimation(textMesh, startIndex, endIndex, parameters);
                break;

            // Color animation, makes the text color transition between two colors repeatedly
            // color format: {color:color1,color2,speed}
            // color1 is the starting color
            // color2 is the ending color
            // speed is a float value representing how quickly the color transitions between color1 and color2. 1 by default
            case "{color}":
                if (p.Length != 0)
                {
                    parameters["color1"] = p[0].Trim();
                    parameters["color2"] = p[1].Trim();
                    if (p.Length >= 3)
                    {
                        param = p[2].Trim();
                        keyValue = param.Split('=');
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1].Trim();
                            parameters[key] = float.Parse(value);
                        }
                        else parameters["speed"] = float.Parse(param);
                    }
                }
                DialogueAnimator.StartTextColorAnimation(textMesh, startIndex, endIndex, parameters);
                break;

            // Pulse animation, makes the text scale up and down repeatedly
            // pulse format: {pulse:size,speed}
            // size is a float value representing a multiplier for the text's original size. 2 by default
            // speed is a float value representing how quickly the text scales up and down. 1 by default
            case "{pulse}":
                for (int i = 0; i < p.Length; i++)
                {
                    param = p[i];
                    string[] keyValueWave = param.Split('=');
                    if (keyValueWave.Length == 2)
                    {
                        isKeyValueFormat = true;
                        string key = keyValueWave[0].Trim();
                        string value = keyValueWave[1].Trim();
                        parameters[key] = float.Parse(value);
                    }
                    else
                    {
                        if (isKeyValueFormat)
                        {
                            Debug.LogWarning("Inconsistent parameter format in wave animation flag. Mixing key=value and positional parameters is not allowed.");
                            continue;
                        }
                        string[] keys = { "size", "speed", "delay" };
                        parameters[keys[i]] = float.Parse(param);
                    }
                }
                DialogueAnimator.StartTextPulseAnimation(textMesh, startIndex, endIndex, parameters);
                break;

            default: break;
        }

        // Remove the opening tag from the line
        line = line.Remove(startIndex, length);
        // Remove the closing tag from the line
        line = line.Remove(endIndex, flag.Length + 1);
        return line;
    }

    /// <summary>
    /// Parses a dialogue line for a specific variable flag and performs the associated action.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <param name="flag">The style flag to parse.</param>
    /// <returns>The new line with the animation flag removed.</returns>
    private string ParseLineVariableFlag(string line, string flag)
    {
        var (startIndex, length, endIndex, p) = GetFlagParametersFromInsideTag(line, flag);
        flag += ')'; // Add the closing ")" to the flag for easier comparison

        string varName = p[0].Trim();
        switch (flag)
        {
            // Display variable flag, displays a variable's value in the dialogue line.
            // display format: (display:variableName)
            // variableName is the name of the variable to display
            case "(display)":
                // Extract the variable name
                // Find the variable value
                var value = DialogueVariables.Instance.GetVariable<object>(varName);
                // Replace the flag with the variable value
                line = line.Insert(startIndex, value.ToString());
                startIndex += value.ToString().Length;
                break;

            // Change variable flag, changes a variable's value.
            // change format: (change:variableName,newValue)
            case "(change)":
                // Extract the variable name and new value
                string newValue = p[1].Trim();
                // Change the variable value
                Type varType = DialogueVariables.Instance.GetVariableType(varName);
                DialogueVariables.Instance.SetVariable(varName, Convert.ChangeType(newValue, varType));
                break;

            // Add variable flag, adds a value to a variable.
            // add format: (add:variableName,toAdd)
            // variableName is the name of the variable to add to
            // toAdd is the amount to add to the variable
            case "(add)":
                // Extract the variable name and amount to add
                string toAdd = p[1].Trim();
                // Add to the variable value
                varType = DialogueVariables.Instance.GetVariableType(varName);
                var curValue = DialogueVariables.Instance.GetVariable<object>(varName);
                object newVarValue = curValue;
                switch (varType)
                {
                    case Type t when t == typeof(int):
                        newVarValue = (int)curValue + int.Parse(toAdd);
                        break;
                    case Type t when t == typeof(float):
                        newVarValue = (float)curValue + float.Parse(toAdd);
                        break;
                    case Type t when t == typeof(string):
                        newVarValue = (string)curValue + toAdd;
                        break;
                    default:
                        Debug.LogError($"Add operation not supported for variable type: {varType}");
                        break;
                }
                DialogueVariables.Instance.SetVariable(varName, newVarValue);
                break;

        }
        // Remove the opening tag from the line
        line = line.Remove(startIndex, length);
        return line;
    }
}
