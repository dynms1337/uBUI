using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using uBUI;
using System.Reflection;
using System.IO;
using Assets.core;


namespace uBUI
{
    /// <summary> uGUI Builder Helper</summary>
    public static class BHelper
    {
        internal const string UI_LAYER_NAME = "UI";
        public static int default_layer = 0;  // GameObjectのレイヤ。-1なら、GameObjectのレイヤをUIレイヤ（5）に設定
        internal static int LAYOUTGROUP_SPACING = 1; //パネル内にxピクセル間隔で配置
        internal static int LAYOUTGROUP_PADDING = 5;
        internal const int HR_HEIGHT = 1;
        internal const int FONT_SIZE = 14;
        internal const float SLIDER_MIN_WIDTH = 30f;
        internal const float SLIDER_MIN_HEIGHT = 10f;
        internal const float TOGGLE_BACKGROUND_SIZE = 20f;
        internal static int INPUTFIELD_LINE_HEIGHT = FONT_SIZE + 4;
        internal static Vector2 UIELEMENT_SIZE = new Vector2(160f, 20f);
        internal static Vector2 WINDOW_POSITION = new Vector2(10, 10);
        internal static Vector2 WINDOW_SIZE = new Vector2(400, 600);
        internal static Vector2 INPUTFIELD_MIN_SIZE = new Vector2(100, 30);
        internal static Color COLOR_TEXT = new Color(1f, 1f, 1f, 1f);
        internal static Color COLOR_AREA_BG = new Color(0.1f, 0.1f, 0.1f, 0.1f);
        internal static Color COLOR_SLIDER_HANDLE = new Color(0.8F, 0.8f, 0.8f, 1f);
        internal static Color COLOR_SLIDER_BACKGROUND = new Color(0F, 0f, 0f, 0.5f);
        internal static Color COLOR_SELECTABLE = new Color(0.6F, 0.6f, 0.6f, 1f);    //編集領域　テキストボックスetc
        internal static Color COLOR_CHECKMARK = new Color(0.8F, 0.8f, 0.8f, 1f);
        internal static Color COLOR_SCROLLVIEW_MASK = new Color(0.8F, 0.8f, 0.8f, 0.5f);
        internal static Color COLOR_SELECTABLE_NORMAL = new Color(0.6F, 0.6f, 0.6f, 1f);
        internal static Color COLOR_SELECTABLE_HOVER = new Color(0.7F, 0.7f, 0.7f, 1f);
        internal static Color COLOR_SELECTABLE_SELECTED = new Color(0.6F, 0.6f, 0.6f, 1f);
        internal static Color COLOR_SELECTABLE_PRESSED = new Color(1F, 1f, 1f, 1f);
        internal static Color COLOR_SELECTABLE_ON = new Color(1f, 1f, 1f, 1f);
        internal static Color COLOR_SELECTABLE_DISABLED = new Color(0.0F, 0.0f, 0.0f, 0.5f);
        internal static Color COLOR_IMAGE_DEFAULT = new Color(1F, 1f, 1f, 0.5f);

        internal static Font TEXT_FONT;
        private static UniqueName goname_Text = new UniqueName("Text");
        private static UniqueName goname_Button = new UniqueName("Button");
        private static UniqueName goname_Image = new UniqueName("Image");
        private static UniqueName goname_RawImage = new UniqueName("RawImage");
        private static UniqueName goname_Panel = new UniqueName("Panel");
        private static UniqueName goname_Window = new UniqueName("Window");
        private static UniqueName goname_Canvas = new UniqueName("Canvas");
        private static UniqueName goname_InputField = new UniqueName("InputField");
        private static UniqueName goname_Slider = new UniqueName("Slider");
        private static UniqueName goname_ScrollBar = new UniqueName("ScrollBar");
        private static UniqueName goname_ScrollView = new UniqueName("ScrollView");
        private static UniqueName goname_Toggle = new UniqueName("Toggle");
        private static UniqueName goname_RadioButton = new UniqueName("RadioButton");

        static BHelper()
        {
            TEXT_FONT = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static GameObject CreateUIElement(string goName, UIInfo uiInfo, GameObject parent = null)
        {
            GameObject ret = new GameObject(goName);
            {
                RectTransform rt = ret.AddComponent<RectTransform>();

                if (uiInfo.m_rtSizeDelta.notNull) rt.sizeDelta = uiInfo.m_rtSizeDelta.Value;
                if (uiInfo.m_rtPosition.notNull) rt.position = uiInfo.m_rtPosition.Value;
                if (uiInfo.m_rtAnchoredPosition.notNull) rt.anchoredPosition = uiInfo.m_rtAnchoredPosition.Value;
                if (uiInfo.m_rtAnchorMin.notNull) rt.anchorMin = uiInfo.m_rtAnchorMin.Value;
                if (uiInfo.m_rtAnchorMax.notNull) rt.anchorMax = uiInfo.m_rtAnchorMax.Value;
                if (uiInfo.m_rtPivot.notNull) rt.pivot = uiInfo.m_rtPivot.Value;
                if (uiInfo.m_rtOffsetMin.notNull) rt.offsetMin = uiInfo.m_rtOffsetMin.Value;
                if (uiInfo.m_rtOffsetMax.notNull) rt.offsetMax = uiInfo.m_rtOffsetMax.Value;
            }

            if (uiInfo.m_leMinSize.notNull |
                uiInfo.m_lePreferredSize.notNull |
                uiInfo.m_leFlexWeight.notNull)
            {
                LayoutElement le = ret.AddComponent<LayoutElement>();
                if (uiInfo.m_leMinSize.notNull)
                {
                    if (uiInfo.m_leMinSize.Value.x != 0) le.minWidth = uiInfo.m_leMinSize.Value.x;
                    if (uiInfo.m_leMinSize.Value.y != 0) le.minHeight = uiInfo.m_leMinSize.Value.y;
                }
                if (uiInfo.m_lePreferredSize.notNull)
                {
                    if (uiInfo.m_lePreferredSize.Value.x != 0) le.preferredWidth = uiInfo.m_lePreferredSize.Value.x;
                    if (uiInfo.m_lePreferredSize.Value.y != 0) le.preferredHeight = uiInfo.m_lePreferredSize.Value.y;
                }
                if (uiInfo.m_leFlexWeight.notNull)
                {
                    le.flexibleWidth = uiInfo.m_leFlexWeight.Value.x;
                    le.flexibleHeight = uiInfo.m_leFlexWeight.Value.y;
                }
            }

            if (default_layer != -1) ret.layer = default_layer;
            else ret.layer = LayerMask.NameToLayer(UI_LAYER_NAME);

            if (parent != null)
            {
                ret.transform.SetParent(parent.transform, false);
                ret.layer = parent.layer;
            }
            return ret;
        }

        public static EventSystem CreateEventSystem()
        {
            var esys = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
            return esys;
        }

        private static void configSelectableColors(Selectable selectable)
        {
            ColorBlock colors = selectable.colors;
            colors.normalColor = COLOR_SELECTABLE_NORMAL;
            colors.highlightedColor = COLOR_SELECTABLE_HOVER;
            colors.pressedColor = COLOR_SELECTABLE_PRESSED;
            colors.disabledColor = COLOR_SELECTABLE_DISABLED;
            selectable.colors = colors;
        }

        private static LayoutGroup addLyaoutGroup(GameObject go, LayoutType layoutGroupType, UIInfo uiInfo = null)
        {
            if (uiInfo == null) uiInfo = UIInfo.PANEL_DEFAULT;
            switch (layoutGroupType)
            {
                case LayoutType.Grid:
                    GridLayoutGroup glg = go.AddComponent<GridLayoutGroup>();
                    setPadding(5, 5, 5, 5);
                    glg.padding = new RectOffset(uiInfo.m_padding_left, uiInfo.m_padding_right, uiInfo.m_padding_top, uiInfo.m_padding_bottom);
                    glg.spacing = new Vector2(uiInfo.m_spacing, uiInfo.m_spacing);
                    return glg;
                case LayoutType.Horizontal:
                    HorizontalLayoutGroup hlg = go.AddComponent<HorizontalLayoutGroup>();
                    hlg.childControlWidth = true;
                    hlg.childControlHeight = true;
                    hlg.childForceExpandWidth = false;
                    hlg.childForceExpandHeight = false;
                    hlg.spacing = uiInfo.m_spacing;
                    setPadding(5, 5, 2, 2);
                    hlg.padding = new RectOffset(uiInfo.m_padding_left, uiInfo.m_padding_right, uiInfo.m_padding_top, uiInfo.m_padding_bottom);
                    return hlg;
                case LayoutType.Vertical:
                    VerticalLayoutGroup vlg = go.AddComponent<VerticalLayoutGroup>();
                    vlg.childControlWidth = true;   // false;
                    vlg.childControlHeight = true;
                    vlg.childForceExpandWidth = false;
                    vlg.childForceExpandHeight = false;
                    vlg.spacing = uiInfo.m_spacing;
                    setPadding(2, 2, 5, 5);
                    vlg.padding = new RectOffset(uiInfo.m_padding_left, uiInfo.m_padding_right, uiInfo.m_padding_top, uiInfo.m_padding_bottom);
                    return vlg;
                case LayoutType.None: break;
            }
            return null;

            void setPadding(int left, int right, int top, int bottom)
            {
                if (uiInfo.m_padding_left == -1) uiInfo.m_padding_left = left;
                if (uiInfo.m_padding_right == -1) uiInfo.m_padding_right = right;
                if (uiInfo.m_padding_top == -1) uiInfo.m_padding_top = top;
                if (uiInfo.m_padding_bottom == -1) uiInfo.m_padding_bottom = bottom;
            }

        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T ret = go.GetComponent<T>();
            return (ret != null) ? ret : go.AddComponent<T>();
        }

        public static Text addTextComponent(GameObject go, string label, UIInfo uiInfo)
        {
            Text lbl = go.AddComponent<Text>();
            lbl.text = label;
            lbl.font = TEXT_FONT;
            lbl.setUIInfo(uiInfo);
            return lbl;
        }
        public static void setUIInfo(this Text lbl, UIInfo uiInfo)
        {
            lbl.color = uiInfo.m_textColor;
            lbl.alignment = uiInfo.m_textAlignment;
            lbl.fontSize = uiInfo.m_textSize;
            Image img = lbl.gameObject.getParent().GetComponent<Image>();
            if (img != null) img.setUIInfo(uiInfo);
        }

        public static void updateColliderSize(this BoxCollider collider)
        {
            //var rt = collider?.gameObject?.GetComponent<RectTransform>();
            var rt = collider.GetComponent<RectTransform>();
            if (rt == null)
            {
                return;
            }
            Vector3 size = new Vector3(rt.rect.width, rt.rect.height, 0); // rt.rect.size;
            collider.size = size;
        }
        public static BoxCollider addBoxCollider(GameObject go)
        {
            var collider = go.GetOrAddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.updateColliderSize();
            return collider;
        }

        public static Image addImageComponent(GameObject go, Sprite sprite = null, UIInfo uiInfo = null)
        {
            if (uiInfo == null) uiInfo = UIInfo.IMAGE_DEFAULT;
            Image image = go.AddComponent<Image>();
            if (sprite != null)
            {
                image.sprite = sprite;
            }
            image.type = Image.Type.Simple;
            image.setUIInfo(uiInfo);
            return image;
        }
        public static void setUIInfo(this Image img, UIInfo uiInfo)
        {
            img.color = uiInfo.m_bgColor;
        }
        public static Texture2D loadTexture(string path)
        {
            if (!File.Exists(path)) { Debug.Log($"loadTexture : {path} not found."); return null; }
            Texture2D texture = new Texture2D(1, 1);
            texture = LoadTexture_workaround(File.ReadAllBytes(path));
            return texture;
        }
        public static Texture2D LoadTexture_workaround(byte[] texData)
        {
            var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);

            // Around Unity 2018 the LoadImage and other export/import methods got moved from Texture2D to extension methods
            var loadMethod = typeof(Texture2D).GetMethod("LoadImage", new[] { typeof(byte[]) });
            if (loadMethod != null)
            {
                loadMethod.Invoke(tex, new object[] { texData });
            }
            else
            {
                var converter = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
                if (converter == null) throw new ArgumentNullException(nameof(converter));
                var converterMethod = converter.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]) });
                if (converterMethod == null) throw new ArgumentNullException(nameof(converterMethod));
                converterMethod.Invoke(null, new object[] { tex, texData });
            }

            return tex;
        }

        /// <param name="renderMode">RenderMode.WorldSpace or RenderMode.ScreenSpaceOverlay</param>
        /// <param name="uiInfo">rtAnchoredPosition:container position, rtSizeDelta: container size</param>
        public static LayoutGroup CreateCanvas(RenderMode renderMode,
            UIInfo uiInfo = null, LayoutType layoutGroup = LayoutType.Vertical, float canvasScale = 1f,
            Camera camera4world = null, float meterPerPx4world = 0.001f,  // WorldSpace parameters
            bool draggable4screen = true,   // ScreenSpace parameters
            GameObject parent = null, string goName = "")
        {
            Vector2? leftbottom = null; Vector2? size = null;
            if (uiInfo.m_rtAnchoredPosition.notNull) leftbottom = uiInfo.m_rtAnchoredPosition.Value;
            if (leftbottom == null) leftbottom = WINDOW_POSITION;
            if (uiInfo.m_rtSizeDelta.notNull) size = uiInfo.m_rtSizeDelta.Value;
            if (size == null) size = WINDOW_SIZE;
            if (uiInfo == null) uiInfo = UIInfo.CANVAS_DEFAULT;

            Canvas canvas = null;
            {
                var goCanvas = CreateUIElement(goName == "" ? goname_Canvas.get() : goName, uiInfo.rtSizeDelta(size.Value), parent: parent);
                canvas = goCanvas.AddComponent<Canvas>();
                canvas.renderMode = renderMode;
                goCanvas.AddComponent<CanvasScaler>();
                goCanvas.AddComponent<GraphicRaycaster>();

                CreateEventSystem();
                var collider = addBoxCollider(goCanvas);
            }

            CanvasScaler canvasScaler = canvas.gameObject.GetOrAddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = canvasScale;

            UIInfo uiInfo2 = null;
            if (renderMode == RenderMode.WorldSpace)
            {
                if (camera4world == null) camera4world = Camera.main;
                canvas.worldCamera = camera4world;
                // Resolution for text. Because the default value of 1 is blurred.
                // If the scale is 0.001, it will be 0.001m/px, and if the width of the window is 800px, the width in the game space will be 80cm.
                canvas.gameObject.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 2;

                canvas.gameObject.GetComponent<RectTransform>().localScale = new Vector3(meterPerPx4world, meterPerPx4world, meterPerPx4world);
                uiInfo2 = UIInfo.PANEL_DEFAULT.lePreferredSize(size.Value);
            }
            else if (renderMode == RenderMode.ScreenSpaceOverlay)
            {
                uiInfo2 = UIInfo.PANEL_DEFAULT.rtSizeDelta(size.Value).rtAnchoredPosition(leftbottom.Value)
                    .rtAnchorMin(Vector2.zero).rtAnchorMax(Vector2.zero).rtPivot(Vector2.zero);
            }
            else { Debug.LogError($"CreateCanvas : process not implemented for {renderMode}"); }

            uiInfo2 = uiInfo2.bgColor(uiInfo.m_bgColor);
            LayoutGroup container = CreatePanel(uiInfo2, layoutGroup, canvas.gameObject, "container");
            if (renderMode == RenderMode.ScreenSpaceOverlay & draggable4screen) container.gameObject.GetOrAddComponent<DragBehaviour>();

            return container;
        }


        public static LayoutGroup CreatePanel(UIInfo uiInfo = null, LayoutType layoutGroup = LayoutType.Vertical, GameObject parent = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.PANEL_DEFAULT;
            GameObject panelGO = CreateUIElement(goName == "" ? goname_Panel.get() + "-" + layoutGroup.ToString() : goName, uiInfo, parent: parent);
            Image image = addImageComponent(panelGO, uiInfo: uiInfo);
            LayoutGroup lg = null;
            switch (layoutGroup)
            {
                case LayoutType.Horizontal:
                    lg = addLyaoutGroup(panelGO, layoutGroup, uiInfo);
                    lg.childAlignment = uiInfo.m_layoutAlignment;
                    break;
                case LayoutType.Vertical:
                    lg = addLyaoutGroup(panelGO, layoutGroup, uiInfo);
                    lg.childAlignment = uiInfo.m_layoutAlignment;
                    break;
                default: lg = addLyaoutGroup(panelGO, layoutGroup); break;
            }
            return lg;
        }
        public static void setUIInfo(this LayoutGroup lg, UIInfo uiInfo)
        {
            Image img = lg.gameObject.GetComponent<Image>();
            if (img != null) img.setUIInfo(uiInfo);
        }


        public static Image CreateSpacer(GameObject parent, int height = 0, string goName = "Spacer")
        {

            Image ret = CreateImage(parent, uiInfo:
                UIInfo.IMAGE_DEFAULT.leFlexWeight(1, 0).lePreferredSize(0, height)
                , goName: goName);
            ret.color = Color.clear;
            return ret;
        }

        public static Image CreateHorizontalBar(GameObject parent, int height = HR_HEIGHT, Color color = default(Color), string goName = "HorizontalBar")
        {
            Image ret = CreateImage(parent, uiInfo:
                UIInfo.IMAGE_DEFAULT.leFlexWeight(1, 0).lePreferredSize(0, height)
                , goName: goName);
            ret.color = color;
            return ret;
        }

        public static Text CreateText(GameObject parent, string label, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.TEXT_DEFAULT;
            Image image = CreateImage(parent, uiInfo: uiInfo, goName: goName == "" ? "TextContainer" : goName);
            GameObject container = image.gameObject;
            addLyaoutGroup(container, LayoutType.Vertical, uiInfo.padding(1));
            GameObject go = CreateUIElement(goname_Text.get(), uiInfo, parent: container);
            (go.transform as RectTransform).pivot = new Vector2(0f, 0.5f);  // Base point for scaling and rotation. Expands to the right when the number of characters increases.

            Text ret = addTextComponent(go, label, uiInfo);
            return ret;
        }

        public static Button CreateButton(GameObject parent, UnityAction TaskOnClick, string labelStr = "",
            Sprite sprite = null, int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.BUTTON_DEFAULT;
            //if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_WParentHSelf();
            Text text = CreateText(parent, labelStr, uiInfo, goName == "" ? goname_Button.get() : goName);
            text.alignment = TextAnchor.MiddleCenter;

            GameObject goButton = text.gameObject.getParent();
            LayoutGroup lg = goButton.GetOrAddComponent<VerticalLayoutGroup>();
            lg.childAlignment = TextAnchor.MiddleCenter;

            Image img = goButton.GetOrAddComponent<Image>();
            img.setUIInfo(uiInfo);

            if (sprite != null)  //Don't add a GameObject when there is no button image. To keep the layout from collapsing.
            {
                UIInfo uiImage = uiInfo;   //.fit_Self();
                if (imgSize > 0)
                    uiImage = uiImage.lePreferredSize(imgSize, imgSize);
                Image buttonImage = CreateImage(goButton, uiInfo: uiImage, goName: "buttonImage");
                buttonImage.sprite = sprite;
                buttonImage.preserveAspect = true;  //Keep image aspect ratio when stretched
                buttonImage.gameObject.transform.SetSiblingIndex(0);  //Swap the order of GameObjects so that the image is above the text
                if (imgColor == null) buttonImage.color = Color.white;   //Makes the image semi-transparent without changing its color.
                else buttonImage.color = imgColor.Value;
                // When there is a button image and the text is empty, set the GameObject corresponding to the Text to Active: false
                text.gameObject.SetActive(false);
            }

            Button btn = goButton.AddComponent<Button>();
            configSelectableColors(btn);
            if (TaskOnClick != null) btn.onClick.AddListener(TaskOnClick);
            btn.setUIInfo(uiInfo);
            return btn;
        }

        public static Button bgColor(this Button btn, Color color)
        {
            var cb = btn.colors;
            cb.normalColor = color;
            btn.colors = cb;
            return btn;
        }

        public static Button imgColor(this Button btn, Color color)
        {
            btn.gameObject.Foreach(g =>
            {
                Image img = g.GetComponent<Image>();
                if (img != null) img.color = color;
            });
            return btn;
        }

        public static void setUIInfo(this Button btn, UIInfo uiInfo)
        {
            btn.gameObject.Foreach(g =>
           {
               Text lbl = g.GetComponent<Text>();
               if (lbl != null) lbl.setUIInfo(uiInfo);
           });
        }
        public static Text getText(this Button self)
        {
            Text ret = null;
            self.gameObject.Foreach(g =>
            {
                Text lbl = g.GetComponent<Text>();
                if (lbl != null)
                {
                    ret = lbl;
                    ret.gameObject.SetActive(true);
                }
            });
            return ret;
        }
        public static Image getImage(this Button self)
        {
            Image ret = null;
            self.gameObject.Foreach(g =>
            {
                var img = g.GetComponent<Image>();
                if (img != null) ret = img;
            });
            return ret;
        }

        public static Button CreateButton(GameObject parent, UnityAction TaskOnClick, string labelStr = "", Texture2D tex = null, int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null, string goName = "")
        {
            if (tex == null) return CreateButton(parent, TaskOnClick: TaskOnClick, labelStr: labelStr, sprite: null, imgSize: imgSize, uiInfo: uiInfo, goName: goName);
            int width = tex.width;
            int height = tex.height;
            if (uiInfo != null && uiInfo.m_lePreferredSize.Value.x != 0f & uiInfo.m_lePreferredSize.Value.y != 0f)
            {
                width = (int)uiInfo.m_lePreferredSize.Value.x;
                height = (int)uiInfo.m_lePreferredSize.Value.y;
                tex = ResizeTexture(tex, width, height);
            }
            return CreateButton(parent, TaskOnClick, labelStr, Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.zero), imgSize: imgSize, imgColor: imgColor, uiInfo, goName);
        }
        public static Button CreateButton(GameObject parent, string texPath, UnityAction TaskOnClick = null,
            string labelStr = "", int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null, string goName = "")
        { return CreateButton(parent, TaskOnClick, labelStr, loadTexture(texPath), imgSize, imgColor, uiInfo, goName); }

        /// <param name="tex_sprite_path">Image Texture2D or Sprite or Path to image file. If null, Create without Sprite.</param>
        /// <param name="uiInfo">When tex, resized to uiInfo.lePreferredSize or uiInfo.rtSizeDelta</param>
        public static Image CreateImage(GameObject parent, object tex_sprite_path = null, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.IMAGE_DEFAULT;
            if (tex_sprite_path == null | tex_sprite_path is Sprite)
            {
                GameObject go = CreateUIElement(goName == "" ? goname_Image.get() : goName, uiInfo, parent: parent);
                return addImageComponent(go, tex_sprite_path as Sprite, uiInfo);
            }
            else if (tex_sprite_path is Texture2D)
            {
                Texture2D tex = tex_sprite_path as Texture2D;
                return CreateImage(parent, Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero), uiInfo, goName);
            }
            else if (tex_sprite_path is string)
            {
                Texture2D tex = loadTexture(tex_sprite_path as string);
                (int width, int height) = (0, 0);
                if (uiInfo.m_lePreferredSize.notNull) (width, height) = ((int)uiInfo.m_lePreferredSize.Value.x, (int)uiInfo.m_lePreferredSize.Value.y);
                else if (uiInfo.m_rtSizeDelta.notNull) (width, height) = ((int)uiInfo.m_rtSizeDelta.Value.x, (int)uiInfo.m_rtSizeDelta.Value.y);
                if (width != 0 & height != 0) tex = ResizeTexture(tex, width, height);
                return CreateImage(parent, tex, uiInfo, goName);
            }
            else throw new ArgumentException($"CreateImage > tex_sprite_path `{tex_sprite_path}` is not PathString or Texture2D or Sprite.");
        }

        public static Texture2D ResizeTexture(Texture2D srcTexture, int newWidth, int newHeight)
        {
            var resizedTexture = new Texture2D(newWidth, newHeight);
            Graphics.ConvertTexture(srcTexture, resizedTexture);
            return resizedTexture;
        }

        // <param name="wholeNumbers">Limit to Integer if True</param>
        public static Slider CreateSlider(GameObject parent, UnityAction<float> onValueChanged, float initialValue, float max = 1f, float min = 0f, bool wholeNumbers = false, string goName = "")
        {
            // Create GOs Hierarchy
            GameObject root = CreateUIElement(goName == "" ? goname_Slider.get() : goName, new UIInfo().leFlexWeight(1, 0).leMinSize(SLIDER_MIN_WIDTH, SLIDER_MIN_HEIGHT), parent: parent);

            GameObject background = CreateUIElement("Background", new UIInfo().rtAnchorMin(0, 0.25f).rtAnchorMax(1, 0.75f).rtSizeDelta(0, 0), parent: root);
            Image backgroundImage = addImageComponent(background, uiInfo: new UIInfo().bgColor(COLOR_SLIDER_BACKGROUND));

            GameObject fillArea = CreateUIElement("Fill Area", new UIInfo().rtAnchorMin(0, 0.25f).rtAnchorMax(1, 0.75f).rtAnchoredPosition(-5, 0).rtSizeDelta(-20, 0), parent: root);

            GameObject fill = CreateUIElement("Fill", new UIInfo().rtSizeDelta(10, 0), parent: fillArea);
            addImageComponent(fill, uiInfo: new UIInfo().rtAnchorMin(0, 0).rtAnchorMax(1, 1).rtAnchoredPosition(0, 0).bgColor(COLOR_AREA_BG));

            GameObject handleArea = CreateUIElement("Handle Slide Area", new UIInfo().rtSizeDelta(-20, 0).rtAnchorMin(0, 0).rtAnchorMax(1, 1), parent: root);

            GameObject handle = CreateUIElement("Handle", new UIInfo().rtSizeDelta(20, 0), parent: handleArea);
            Image handleImage = addImageComponent(handle, uiInfo: new UIInfo().rtAnchorMin(0, 0).rtAnchorMax(1, 1).rtAnchoredPosition(0, 0).bgColor(COLOR_SLIDER_HANDLE));


            // Setup slider component
            Slider slider = root.AddComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            configSelectableColors(slider);
            slider.wholeNumbers = wholeNumbers;
            slider.maxValue = max;
            slider.minValue = min;
            slider.value = initialValue;
            slider.onValueChanged.AddListener(onValueChanged);

            return slider;
        }



        public static Scrollbar CreateScrollbar(GameObject parent, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.SCROLLBAR_DEFAULT;
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElement(goName == "" ? goname_ScrollBar.get() : goName, uiInfo, parent: parent);

            GameObject sliderArea = CreateUIElement("Sliding Area", new UIInfo().rtSizeDelta(-20, -20).rtAnchorMin(0, 0).rtAnchorMax(1, 1), parent: scrollbarRoot);

            GameObject handle = CreateUIElement("Handle", new UIInfo().rtSizeDelta(20, 20), parent: sliderArea);

            Image bgImage = addImageComponent(scrollbarRoot, uiInfo: new UIInfo().leFlexWeight(1, 0).bgColor(COLOR_SLIDER_BACKGROUND));
            Image handleImage = addImageComponent(handle, uiInfo: new UIInfo().bgColor(COLOR_SLIDER_HANDLE));

            Scrollbar scrollbar = scrollbarRoot.AddComponent<Scrollbar>();
            scrollbar.handleRect = handle.GetComponent<RectTransform>();
            scrollbar.targetGraphic = handleImage;
            configSelectableColors(scrollbar);
            return scrollbar;
        }

        public static Toggle CreateToggle(GameObject parent, UnityAction<bool> onValueChanged, string labelStr = "Toggle", bool isOn = true,
            UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.TOGGLE_DEFAULT;

            GameObject goToggleRoot = CreateUIElement(goName == "" ? goname_Toggle.get() : goName, uiInfo, parent: parent);
            LayoutGroup lg = addLyaoutGroup(goToggleRoot, LayoutType.Horizontal, uiInfo.padding(0));
            lg.childAlignment = TextAnchor.MiddleLeft;

            Image bgImage = CreateImage(parent: goToggleRoot,
                uiInfo: new UIInfo().leMinSize(TOGGLE_BACKGROUND_SIZE).bgColor(COLOR_SELECTABLE_ON), goName: "Background");
            GameObject goBackground = bgImage.gameObject;

            Image checkmarkImage = CreateImage(parent: goBackground,
              uiInfo: new UIInfo().bgColor(Color.white).rtAnchorMin(0.5f).rtAnchorMax(0.5f).rtAnchoredPosition(0, 0).rtSizeDelta(TOGGLE_BACKGROUND_SIZE / 2),
              goName: "Checkmark");

            Text label = CreateText(parent: goToggleRoot, uiInfo: new UIInfo().rtAnchorParent().leFlexWeight(1, 0), label: labelStr);

            Toggle toggle = goToggleRoot.AddComponent<Toggle>();
            toggle.isOn = isOn;
            toggle.graphic = checkmarkImage;
            toggle.targetGraphic = bgImage;
            configSelectableColors(toggle);
            toggle.onValueChanged.AddListener(onValueChanged);

            return toggle;
        }

        public static ToggleGroup CreateRadioButton<T>(GameObject parent, UnityAction<T> onValueChanged, Dictionary<string, T> showValueDict, int selected = 0,
            LayoutType layoutGroup = LayoutType.Horizontal, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.RADIO_BUTTON_DEFAULT;

            GameObject goRadioPanel = CreatePanel(parent: parent, layoutGroup: layoutGroup, uiInfo: uiInfo, goName: goname_RadioButton.get()).gameObject;
            ToggleGroup toggleGroup = goRadioPanel.AddComponent<ToggleGroup>();
            int i = 0;
            foreach (var kvp in showValueDict)
            {
                string showStr = kvp.Key;
                T selectedValue = kvp.Value;
                Toggle t = CreateToggle(parent: goRadioPanel, (b) => { if (b) onValueChanged(selectedValue); }, labelStr: showStr, isOn: i == selected);
                t.group = toggleGroup;
                i++;
            }
            return toggleGroup;
        }

        public static InputField CreateInputField(GameObject parent, UnityAction<string> onEndEdit, UnityAction<string> onValueChanged,
           string initialText = "", int lineCount = 1, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.INPUTFIELD_DEFAULT;
            //if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_WParentHSelf();
            GameObject goInputField = CreateUIElement(goName == "" ? goname_InputField.get() : goName, uiInfo.leMinSize(INPUTFIELD_MIN_SIZE), parent: parent);
            InputField inputField = goInputField.AddComponent<InputField>();
            LayoutElement leInputField = goInputField.GetOrAddComponent<LayoutElement>();

            GameObject goPlaceholder = CreateUIElement("Placeholder", new UIInfo().rtAnchorParent().rtSizeDelta(0).rtOffsetMin(10, 6).rtOffsetMax(-10, -7),
                parent: goInputField);

            GameObject goText = CreateUIElement("Text", new UIInfo().rtAnchorParent().rtSizeDelta(0).rtOffsetMin(10, 6).rtOffsetMax(-10, -7),
                parent: goInputField);

            Image image = addImageComponent(goInputField, uiInfo: new UIInfo().rtAnchorParent().bgColor(COLOR_SELECTABLE));


            Text text = addTextComponent(goText, "", uiInfo);
            text.supportRichText = false;

            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            Text placeholder = addTextComponent(goPlaceholder, "Enter text...", uiInfo: new UIInfo().textColor(placeholderColor));

            if (lineCount >= 2)  // Settings for multi-line input
            {
                inputField.lineType = InputField.LineType.MultiLineNewline;     // A multiline InputField that scrolls vertically, overflows, and inserts a newline character on the return key.
                leInputField.minHeight = (INPUTFIELD_MIN_SIZE.y - INPUTFIELD_LINE_HEIGHT) + INPUTFIELD_LINE_HEIGHT * lineCount;
                text.alignment = TextAnchor.UpperLeft;
                placeholder.alignment = TextAnchor.UpperLeft;
            }

            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.text = initialText;
            if (onValueChanged != null) inputField.onValueChanged.AddListener(onValueChanged);
            if (onEndEdit != null) inputField.onEndEdit.AddListener(onEndEdit);
            inputField.setUIInfo(uiInfo);
            configSelectableColors(inputField);

            return inputField;
        }

        public static void setUIInfo(this InputField inputfield, UIInfo uiInfo)
        {
            Foreach(inputfield.gameObject, g =>
            {
                Text text = g.GetComponent<Text>();
                if (text != null) text.setUIInfo(uiInfo);
            });
        }


        public static LayoutGroup CreateScrollView(GameObject parent, LayoutType contentPanelLayoutGroupType = LayoutType.Vertical,
            float scrollSensitivity = 20, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.SCROLLVIEW_DEFAULT;
            GameObject goScrollView = CreateUIElement(goName == "" ? goname_ScrollView.get() : goName, uiInfo, parent: parent);
            GameObject goViewport = CreateUIElement("Viewport", new UIInfo().rtAnchorParent(), parent: goScrollView);

            GameObject goContent = CreateUIElement("Content", parent: goViewport,
                uiInfo: new UIInfo().rtAnchorMin(Vector2.up).rtAnchorMax(1).rtSizeDelta(0, 300).rtPivot(Vector2.up)/*.fit_Self()*/);

            GameObject hScrollbar = CreateScrollbar(uiInfo: UIInfo.SCROLLBAR_DEFAULT.rtAnchorMin(0).rtAnchorMax(Vector2.right).rtPivot(0, 0),
                parent: goScrollView).gameObject;
            hScrollbar.name = "ScrollbarHorizontal";
            RectTransform hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            GameObject vScrollbar = CreateScrollbar(uiInfo: UIInfo.SCROLLBAR_DEFAULT.rtAnchorMin(Vector2.right).rtAnchorMax(1).rtPivot(1, 1),
                parent: goScrollView).gameObject;
            vScrollbar.name = "Scrollbar Vertical";
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            RectTransform viewportRT = goViewport.GetComponent<RectTransform>();
            viewportRT.pivot = Vector2.up;


            ScrollRect scrollRect = goScrollView.AddComponent<ScrollRect>();
            scrollRect.content = goContent.GetComponent<RectTransform>();
            scrollRect.viewport = viewportRT;
            scrollRect.horizontalScrollbar = hScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;
            scrollRect.verticalScrollbarSpacing = -3;
            scrollRect.scrollSensitivity = scrollSensitivity;
            Image rootImage = addImageComponent(goScrollView, uiInfo: uiInfo);

            Mask viewportMask = goViewport.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            Image viewportImage = addImageComponent(goViewport, uiInfo: new UIInfo().bgColor(COLOR_SCROLLVIEW_MASK));

            LayoutGroup lgContent = addLyaoutGroup(goContent, contentPanelLayoutGroupType);
            var csf = goContent.GetOrAddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            return lgContent;
        }

        internal static float calcScrollViewWidth(GameObject referenceWidthGameObject)
        {
            return referenceWidthGameObject.GetOrAddComponent<RectTransform>().rect.width - (SLIDER_MIN_WIDTH + 5);
        }

        /// <summary>親GameObjectを取得</summary>
        internal static GameObject getParent(this GameObject childObject) { return childObject.transform.parent.gameObject; }
        public static List<GameObject> Foreach(this GameObject go, UnityAction<GameObject> unityAction = null)
        {
            List<GameObject> ret = new List<GameObject>();
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                GameObject tgt = go.transform.GetChild(i).gameObject;
                if (unityAction != null) unityAction(tgt);
                ret.Add(tgt);
            }
            return ret;
        }

        public static Color parseColor(string html_color_code, Color? defaultColor = null)
        {
            if (defaultColor == null) defaultColor = Color.white;
            if (ColorUtility.TryParseHtmlString(html_color_code, out var color))
                return color;
            else
            {
                Debug.LogWarning($"parseColor : Color code `{html_color_code}` cannot parse.");
                return defaultColor.Value;
            }
        }

        class UniqueName
        {
            private long _count = 0;
            private readonly string Text;
            internal UniqueName(string str) { this.Text = str; }
            internal string get() { _count++; return Text + _count; }
        }
    }

}
