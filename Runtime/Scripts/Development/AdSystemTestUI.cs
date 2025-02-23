using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FABStudios.AdSystem
{
    public class AdSystemTestUI : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private bool showInRelease = false;
        [SerializeField] private Color panelColor = new Color(0, 0, 0, 0.8f);
        [SerializeField] private float panelHeight = 100f;
        [SerializeField] private float buttonWidth = 200f;
        [SerializeField] private float buttonHeight = 60f;

        private Canvas testCanvas;
        private GameObject panelObject;
        private GameObject bannerButton;
        private bool isInitialized = false;

        private void Start()
        {
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
            if (!showInRelease)
            {
                Debug.Log("AdSystemTestUI: Test UI disabled in release build");
                Destroy(gameObject);
                return;
            }
#endif

            InitializeTestUI();
        }

        private void InitializeTestUI()
        {
            try
            {
                Debug.Log("AdSystemTestUI: Starting setup");
                CreateTestCanvas();
                CreateTestPanel();
                CreateButtons();
                isInitialized = true;
                Debug.Log("AdSystemTestUI: Setup complete");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"AdSystemTestUI: Failed to initialize test UI - {e.Message}");
                if (testCanvas != null) Destroy(testCanvas.gameObject);
                if (panelObject != null) Destroy(panelObject);
                Destroy(gameObject);
            }
        }

        private void CreateTestCanvas()
        {
            Debug.Log("AdSystemTestUI: Setting up canvas");

            GameObject canvasObj = new GameObject("Ad Test Canvas");
            canvasObj.transform.SetParent(transform, false);

            testCanvas = canvasObj.AddComponent<Canvas>();
            testCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            testCanvas.sortingOrder = 32767;

            // Add EventSystem if it doesn't exist
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("Event System");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("AdSystemTestUI: Created EventSystem");
            }

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;

            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("AdSystemTestUI: Canvas setup complete");
        }

        private void CreateTestPanel()
        {
            if (testCanvas == null)
            {
                Debug.LogError("AdSystemTestUI: Cannot create panel - Canvas is null!");
                return;
            }

            Debug.Log("AdSystemTestUI: Creating panel");

            panelObject = new GameObject("Test Panel");
            panelObject.transform.SetParent(testCanvas.transform, false);

            RectTransform rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(0, panelHeight);
            rect.anchoredPosition = Vector2.zero;

            Image image = panelObject.AddComponent<Image>();
            image.color = panelColor;
            image.raycastTarget = true;

            Debug.Log("AdSystemTestUI: Panel setup complete");
        }

        private void CreateButtons()
        {
            Debug.Log("AdSystemTestUI: Creating test buttons");
            if (panelObject == null)
            {
                Debug.LogError("AdSystemTestUI: Cannot create buttons -- Panel is null!");
                return;
            }

            // Create interstitial button
            CreateTestButton("Test Interstitial", new Vector2(0.25f, 0.5f), () =>
            {
                Debug.Log("AdSystemTestUI: Interstitial button clicked");
                if (AdManagerNew.Instance != null)
                {
                    AdManagerNew.Instance.ShowInterstitialAd();
                }
                else
                {
                    Debug.LogError("AdSystemTestUI: Cannot show interstitial - AdManagerNew instance is null");
                }
            });

            // Create banner toggle button and store reference
            bannerButton = CreateTestButton("Show Banner", new Vector2(0.5f, 0.5f), () =>
            {
                Debug.Log("AdSystemTestUI: Banner button clicked");
                if (AdManagerNew.Instance != null)
                {
                    // Use local variable to safely access text component
                    var textComp = bannerButton?.GetComponentInChildren<TextMeshProUGUI>();

                    if (AdManagerNew.Instance.IsBannerShowing)
                    {
                        AdManagerNew.Instance.HideBannerAd();
                        if (textComp != null)
                        {
                            textComp.text = "Show Banner";
                        }
                    }
                    else
                    {
                        AdManagerNew.Instance.ShowBannerAd();
                        if (textComp != null)
                        {
                            textComp.text = "Hide Banner";
                        }
                    }
                }
                else
                {
                    Debug.LogError("AdSystemTestUI: Cannot toggle banner - AdManagerNew instance is null");
                }
            });

            // Create app open button
            CreateTestButton("Test App Open", new Vector2(0.75f, 0.5f), () =>
            {
                Debug.Log("AdSystemTestUI: App Open button clicked");
                if (AdManagerNew.Instance != null)
                {
                    AdManagerNew.Instance.ShowAppOpenAd();
                }
                else
                {
                    Debug.LogError("AdSystemTestUI: Cannot show app open ad - AdManagerNew instance is null");
                }
            });
        }

        private GameObject CreateTestButton(string text, Vector2 position, UnityEngine.Events.UnityAction onClick)
        {
            Debug.Log($"AdSystemTestUI: Creating button '{text}'");

            GameObject buttonObj = new GameObject(text);
            buttonObj.transform.SetParent(panelObject.transform, false);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = position;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1);
            buttonImage.raycastTarget = true;

            Button button = buttonObj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1);
            colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1);
            colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1);
            button.colors = colors;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.color = Color.white;
            tmp.fontSize = 24;
            tmp.alignment = TextAlignmentOptions.Center;

            button.onClick.AddListener(() =>
            {
                Debug.Log($"AdSystemTestUI: '{text}' button clicked!");
                onClick?.Invoke();
            });

            Debug.Log($"AdSystemTestUI: Button '{text}' setup complete");
            return buttonObj;
        }

        private void OnValidate()
        {
            Debug.Log("AdSystemTestUI: Components validated");
            if (testCanvas != null && testCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                Debug.LogWarning("AdSystemTestUI: Canvas missing GraphicRaycaster component!");
            }
        }

        private void OnDestroy()
        {
            if (isInitialized)
            {
                Debug.Log("AdSystemTestUI: Cleaning up test UI");
            }
        }
    }
}