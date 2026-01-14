using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestChoiceUI : MonoBehaviour
{
    private static QuestChoiceUI _instance;

    private Canvas _canvas;
    private GameObject _panel;
    private RectTransform _content;
    private Button _closeBtn;

    public static QuestChoiceUI Instance
    {
        get
        {
            if (_instance != null) return _instance;

            var go = new GameObject("QuestChoiceUI");
            _instance = go.AddComponent<QuestChoiceUI>();
            DontDestroyOnLoad(go);
            _instance.BuildUI();   // 100% code
            _instance.Hide();
            return _instance;
        }
    }

    private void BuildUI()
    {
        // Canvas
        var canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(transform, false);
        _canvas = canvasGO.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel
        _panel = new GameObject("Panel");
        _panel.transform.SetParent(canvasGO.transform, false);
        var panelImg = _panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.75f);

        var panelRT = _panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.3f, 0.2f);
        panelRT.anchorMax = new Vector2(0.7f, 0.8f);
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // Title
        var title = CreateText(_panel.transform, "Quest keuze", 28);
        var titleRT = title.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.05f, 0.85f);
        titleRT.anchorMax = new Vector2(0.95f, 0.98f);
        titleRT.offsetMin = Vector2.zero;
        titleRT.offsetMax = Vector2.zero;

        // Close button
        _closeBtn = CreateButton(_panel.transform, "Sluiten", () => Hide());
        var closeRT = _closeBtn.GetComponent<RectTransform>();
        closeRT.anchorMin = new Vector2(0.75f, 0.02f);
        closeRT.anchorMax = new Vector2(0.95f, 0.12f);
        closeRT.offsetMin = Vector2.zero;
        closeRT.offsetMax = Vector2.zero;

        // Content area (scroll-like simple column)
        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(_panel.transform, false);
        _content = contentGO.AddComponent<RectTransform>();
        _content.anchorMin = new Vector2(0.05f, 0.15f);
        _content.anchorMax = new Vector2(0.95f, 0.83f);
        _content.offsetMin = Vector2.zero;
        _content.offsetMax = Vector2.zero;

        var layout = contentGO.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.spacing = 10;

        contentGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public void ShowQuestChoices(List<(string questId, string title, string description)> quests)
    {
        ClearContent();

        if (quests == null || quests.Count == 0)
        {
            CreateText(_content, "Geen quests beschikbaar.", 18);
            Show();
            return;
        }

        foreach (var q in quests)
        {
            var container = new GameObject($"Quest_{q.questId}");
            container.transform.SetParent(_content, false);
            var rt = container.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 80);

            var bg = container.AddComponent<Image>();
            bg.color = new Color(1, 1, 1, 0.08f);

            var title = CreateText(container.transform, q.title, 20);
            var tRT = title.GetComponent<RectTransform>();
            tRT.anchorMin = new Vector2(0.03f, 0.55f);
            tRT.anchorMax = new Vector2(0.97f, 0.95f);
            tRT.offsetMin = Vector2.zero;
            tRT.offsetMax = Vector2.zero;

            var desc = CreateText(container.transform, q.description, 14);
            var dRT = desc.GetComponent<RectTransform>();
            dRT.anchorMin = new Vector2(0.03f, 0.2f);
            dRT.anchorMax = new Vector2(0.97f, 0.55f);
            dRT.offsetMin = Vector2.zero;
            dRT.offsetMax = Vector2.zero;

            var btn = CreateButton(container.transform, "Start", () =>
            {
                QuestManager.Instance.StartQuest(q.questId);
                Hide();
            });

            var bRT = btn.GetComponent<RectTransform>();
            bRT.anchorMin = new Vector2(0.75f, 0.05f);
            bRT.anchorMax = new Vector2(0.97f, 0.45f);
            bRT.offsetMin = Vector2.zero;
            bRT.offsetMax = Vector2.zero;
        }

        Show();
    }

    private void ClearContent()
    {
        for (int i = _content.childCount - 1; i >= 0; i--)
            Destroy(_content.GetChild(i).gameObject);
    }

    private void Show()
    {
        _canvas.enabled = true;
        _panel.SetActive(true);
    }

    private void Hide()
    {
        _panel.SetActive(false);
        _canvas.enabled = false;
    }

    private Text CreateText(Transform parent, string txt, int size)
    {
        var go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<Text>();
        t.text = txt;
        t.fontSize = size;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleLeft;
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return t;
    }

    private Button CreateButton(Transform parent, string label, Action onClick)
    {
        var go = new GameObject("Button");
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0.2f);

        var btn = go.AddComponent<Button>();
        btn.onClick.AddListener(() => onClick());

        var text = CreateText(go.transform, label, 16);
        text.alignment = TextAnchor.MiddleCenter;

        return btn;
    }
}
