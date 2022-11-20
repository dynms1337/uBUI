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
    public static class SWHelper
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

        static SWHelper()
        {
            TEXT_FONT = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private static GameObject CreateUIElement(string goName, UIInfo uiInfo, Vector2 sizeDelta = default(Vector2), GameObject parent = null)
        {
            if (sizeDelta == default(Vector2)) sizeDelta = UIELEMENT_SIZE;
            GameObject ret = new GameObject(goName);
            RectTransform rectTransform = ret.AddComponent<RectTransform>();
            rectTransform.sizeDelta = sizeDelta;
            {
                switch (uiInfo.m_fitWidth)
                {
                    case UIInfo.Fit.Fixed:
                    case UIInfo.Fit.Flexible:
                        {
                            LayoutElement le = ret.AddComponent<LayoutElement>();
                            if (uiInfo.m_uiSize.x != 0) le.minWidth = uiInfo.m_uiSize.x;
                            if (uiInfo.m_uiSize.y != 0) le.minHeight = uiInfo.m_uiSize.y;
                            if (uiInfo.m_fit == UIInfo.Fit.Fixed)
                            {
                                if (uiInfo.m_uiSize.x != 0) le.preferredWidth = uiInfo.m_uiSize.x;
                                if (uiInfo.m_uiSize.y != 0) le.preferredHeight = uiInfo.m_uiSize.y;
                                le.flexibleWidth = 0;
                                le.flexibleHeight = 0;
                            }
                            else
                            {
                                if (uiInfo.m_flexWidth != 0) le.flexibleWidth = uiInfo.m_flexWidth;
                                if (uiInfo.m_flexHeight != 0) le.flexibleHeight = uiInfo.m_flexHeight;
                            }
                        }
                        break;
                    case UIInfo.Fit.UnSpecified:
                    default: break; //pass
                }

                //switch (uiInfo.m_fit)
                //{
                //    case UIInfo.Fit.Parent:
                //        rectTransform.anchorMin = Vector2.zero;
                //        rectTransform.anchorMax = Vector2.one;
                //        rectTransform.anchoredPosition = Vector2.zero;
                //        rectTransform.sizeDelta = uiInfo.m_margin;
                //        break;
                //    case UIInfo.Fit.WParentHSelf:
                //        //width:Parentへアンカーする。 height:子要素のサイズに合わせる
                //        rectTransform.anchorMin = Vector2.zero;
                //        rectTransform.anchorMax = Vector2.one;
                //        rectTransform.anchoredPosition = Vector2.zero;
                //        rectTransform.sizeDelta = uiInfo.m_margin;
                //        LayoutElement le_pow = ret.AddComponent<LayoutElement>();
                //        le_pow.flexibleWidth = 1;
                //        le_pow.flexibleHeight = 0;
                //        break;
                //    case UIInfo.Fit.WSelfHParent:
                //        rectTransform.anchorMin = Vector2.zero;
                //        rectTransform.anchorMax = Vector2.one;
                //        rectTransform.anchoredPosition = Vector2.zero;
                //        rectTransform.sizeDelta = uiInfo.m_margin;
                //        LayoutElement le_wshp = ret.AddComponent<LayoutElement>();
                //        le_wshp.flexibleHeight = 1;
                //        break;
                //    case UIInfo.Fit.Self:
                //        //ContentSizeFitter csf = ret.AddComponent<ContentSizeFitter>();
                //        //csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;    //width/heightを中身（文字など）サイズに合わせる
                //        //csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                //        break;
                //    case UIInfo.Fit.Fixed:
                //    case UIInfo.Fit.Flexible:
                //        RectTransform rt = ret.transform as RectTransform;
                //        rt.localPosition = uiInfo.m_position;
                //        rt.sizeDelta = new Vector2(
                //            uiInfo.m_uiSize.x != 0 ? uiInfo.m_uiSize.x : rt.sizeDelta.x,
                //            uiInfo.m_uiSize.y != 0 ? uiInfo.m_uiSize.y : rt.sizeDelta.y);
                //        LayoutElement le_fixed = ret.AddComponent<LayoutElement>();
                //        if (uiInfo.m_uiSize.x != 0) le_fixed.minWidth = uiInfo.m_uiSize.x;
                //        if (uiInfo.m_uiSize.y != 0) le_fixed.minHeight = uiInfo.m_uiSize.y;
                //        if (uiInfo.m_fit == UIInfo.Fit.Fixed)
                //        {
                //            if (uiInfo.m_uiSize.x != 0) le_fixed.preferredWidth = uiInfo.m_uiSize.x;
                //            if (uiInfo.m_uiSize.y != 0) le_fixed.preferredHeight = uiInfo.m_uiSize.y;
                //            le_fixed.flexibleWidth = 0;
                //            le_fixed.flexibleHeight = 0;
                //        }
                //        else
                //        {
                //            if (uiInfo.m_flexWidth != 0) le_fixed.flexibleWidth = uiInfo.m_flexWidth;
                //            if (uiInfo.m_flexHeight != 0) le_fixed.flexibleHeight = uiInfo.m_flexHeight;
                //        }
                //        break;
                //    case UIInfo.Fit.WParentHFrexible:
                //        rectTransform.anchorMin = Vector2.zero;
                //        rectTransform.anchorMax = Vector2.one;
                //        rectTransform.anchoredPosition = Vector2.zero;
                //        rectTransform.sizeDelta = uiInfo.m_margin;
                //        LayoutElement le_h = ret.AddComponent<LayoutElement>();
                //        le_h.flexibleHeight = 1;
                //        break;
                //    case UIInfo.Fit.UnSpecified:
                //    default: break; //pass
                //}
            }
            if (default_layer != -1)
                ret.layer = default_layer;
            else
                ret.layer = LayerMask.NameToLayer(UI_LAYER_NAME);
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
        private static LayoutType getLayoutGroupTyep(GameObject go)
        {
            if (go == null) return LayoutType.None;
            LayoutGroup lg = go.GetComponent<LayoutGroup>();
            if (lg is VerticalLayoutGroup) return LayoutType.Vertical;
            else if (lg is HorizontalLayoutGroup) return LayoutType.Horizontal;
            else if (lg is GridLayoutGroup) return LayoutType.Grid;
            else return LayoutType.None;
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
            if (uiInfo.m_textColor == default(Color)) uiInfo = uiInfo.Clone().textColor(COLOR_TEXT);
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
            if (uiInfo == null) uiInfo = new UIInfo().fit_Fixed();
            if (uiInfo.m_bgColor == default(Color)) { uiInfo.m_bgColor = COLOR_IMAGE_DEFAULT; }
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

        /// <summary>
        /// - goCanvas  : Canvas, CanvasScaler, GraphicRaycaster, BoxCollider
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="renderMode"></param>
        /// <param name="uiInfo"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="goName"></param>
        /// <returns></returns>
        public static Canvas CreateCanvas(GameObject parent = null, RenderMode renderMode = RenderMode.ScreenSpaceOverlay, UIInfo uiInfo = null,
            Vector2 position = default(Vector2), Vector2 size = default(Vector2), string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.CANVAS_DEFAULT;
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_Fixed();
            if (position == default(Vector2)) position = WINDOW_POSITION;
            if (size == default(Vector2)) size = WINDOW_SIZE;
            var goCanvas = CreateUIElement(goName == "" ? goname_Canvas.get() : goName, uiInfo, size, parent: parent); // new GameObject(goName == "" ? goname_Canvas.get() : goName);
            Canvas canvas = goCanvas.AddComponent<Canvas>();
            canvas.renderMode = renderMode;
            goCanvas.AddComponent<CanvasScaler>();
            var gr = goCanvas.AddComponent<GraphicRaycaster>();

            CreateEventSystem();

            if (goCanvas.transform.parent as RectTransform)
            {
                RectTransform rect = goCanvas.transform as RectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = Vector2.zero;
            }
            var collider = addBoxCollider(goCanvas);
            return canvas;
        }

        /// <summary>スクリーンにUIを描画する場合</summary>
        public static LayoutGroup CreateWindowWithCanvas_onScreen(Vector2 leftbottom = default(Vector2), Vector2 size = default(Vector2),
            UIInfo uiInfo = null, LayoutType layoutGroup = LayoutType.Vertical, bool draggable = true, float canvasScale = 1f,
            GameObject parent = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = new UIInfo();
            if (leftbottom == default(Vector2)) leftbottom = WINDOW_POSITION;
            if (size == default(Vector2)) size = WINDOW_SIZE;
            Canvas canvas = CreateCanvas(parent, RenderMode.ScreenSpaceOverlay, new UIInfo().fit_Fixed(), leftbottom, size, goName);

            CanvasScaler canvasScaler = canvas.gameObject.GetOrAddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = canvasScale;

            LayoutGroup container = CreatePanel(uiInfo.fit_Fixed(leftbottom, size), layoutGroup, canvas.gameObject, "container");

            RectTransform rt = container.gameObject.GetOrAddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;            //anchorMin/Max 画面左下が原点。
            rt.anchorMax = Vector2.zero;
            rt.pivot = Vector2.zero;
            rt.sizeDelta = size;
            rt.anchoredPosition = leftbottom;

            if (draggable) container.gameObject.GetOrAddComponent<DragBehaviour>();
            return container;
        }
        /// <summary>ゲーム空間にUIを描画する場合</summary>
        public static LayoutGroup CreateWindowWithCanvas_onWorld(Vector2 positionFromLeftBottom = default(Vector2), Vector2 size = default(Vector2),
            UIInfo uiInfo = null, LayoutType layoutGroup = LayoutType.Vertical, Camera camera = null, float meterPerPx = 0.001f, float canvasScale = 1f,
            GameObject parent = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = new UIInfo();
            if (positionFromLeftBottom == default(Vector2)) positionFromLeftBottom = WINDOW_POSITION;
            if (size == default(Vector2)) size = WINDOW_SIZE;
            Canvas canvas = CreateCanvas(parent, RenderMode.WorldSpace, new UIInfo().fit_Fixed(), positionFromLeftBottom, size, goName);


            CanvasScaler canvasScaler = canvas.gameObject.GetOrAddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = canvasScale;

            if (camera == null) camera = Camera.main;
            canvas.worldCamera = camera;
            canvas.gameObject.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 2;  //テキストに対する解像度。初期値1だとボケるため。
                                                                                      //スケールが0.001なら、0.001m/pxとなり、Windowの幅が800pxならゲーム空間での幅が80cmになる。
            canvas.gameObject.GetComponent<RectTransform>().localScale = new Vector3(meterPerPx, meterPerPx, meterPerPx);

            LayoutGroup container = CreatePanel(uiInfo.fit_Fixed(positionFromLeftBottom, size), layoutGroup, canvas.gameObject, "container");
            return container;
        }

        public static LayoutGroup CreatePanel(UIInfo uiInfo = null, LayoutType layoutGroup = LayoutType.Vertical, GameObject parent = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.PANEL_DEFAULT;
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_Parent();
            GameObject panelGO = CreateUIElement(goName == "" ? goname_Panel.get() + "-" + layoutGroup.ToString() : goName, uiInfo, parent: parent);
            if (uiInfo.m_bgColor == default(Color)) uiInfo.m_bgColor = SWHelper.COLOR_AREA_BG;
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


        /// <summary>HorizontalLayoutで左端・右端に配置したいとき、中間を埋めるためのUI要素。heightを指定することで縦方向の余白にも使える。</summary>
        public static Image CreateSpacer(GameObject parent, int height = 0, string goName = "Spacer")
        {

            Image ret = CreateImage(parent, uiInfo:
                new UIInfo().fit_Flexible(flexWidth: 1f, flexHeight: 0f).uiSize(new Vector2(0, height))
                , goName: goName);
            ret.color = Color.clear;
            return ret;
        }

        /// <summary>水平な区切り線</summary>
        public static Image CreateHorizontalBar(GameObject parent, int height = HR_HEIGHT, Color color = default(Color), string goName = "HorizontalBar")
        {
            if (color == default(Color)) color = Color.black;
            Image ret = CreateImage(parent, uiInfo:
                new UIInfo().fit_Flexible(flexWidth: 1f, flexHeight: 0f).uiSize(new Vector2(0, height))
                , goName: goName);
            ret.color = color;
            return ret;
        }

        /// <summary>
        /// - TetContainer  Component:Image, VerticalLayoutGroup
        ///   - Text        Component:Text
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="label"></param>
        /// <param name="uiInfo"></param>
        /// <param name="goName"></param>
        /// <returns></returns>
        public static Text CreateText(GameObject parent, string label, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.TEXT_DEFAULT;
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_WParentHSelf();
            if (uiInfo.m_bgColor == default(Color)) uiInfo.m_bgColor = COLOR_AREA_BG;
            Image image = CreateImage(parent, uiInfo: uiInfo, goName: goName == "" ? "TextContainer" : goName);
            GameObject container = image.gameObject;
            addLyaoutGroup(container, LayoutType.Vertical, uiInfo.padding(1));
            GameObject go = CreateUIElement(goname_Text.get(), uiInfo, parent: container);
            (go.transform as RectTransform).pivot = new Vector2(0f, 0.5f);  //拡縮・回転の基点、文字数が増えたときに右へサイズが膨らんでいくようにする。

            Text ret = addTextComponent(go, label, uiInfo);

            return ret;
        }



        /// <summary>
        /// - Button　　    Component:Button, Image（背景色用）, VerticalLayoutGroup
        ///   - buttonImage Component:Image　（ボタンの画像用）
        ///   - Text        Component:Text
        /// </summary>
        public static Button CreateButton(GameObject parent, UnityAction TaskOnClick, string labelStr = "",
            Sprite sprite = null, int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.BUTTON_DEFAULT;
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_WParentHSelf();
            Text text = CreateText(parent, labelStr, uiInfo, goName == "" ? goname_Button.get() : goName);
            text.alignment = TextAnchor.MiddleCenter;

            GameObject goButton = text.gameObject.getParent();  //CreateUIElement(goName == "" ? goname_Button.get() : goName, uiInfo, parent: parent);
            LayoutGroup lg = goButton.GetOrAddComponent<VerticalLayoutGroup>(); //CreateTextで作成したLayoutGroupを取得
            lg.childAlignment = TextAnchor.MiddleCenter;

            Image img = goButton.GetOrAddComponent<Image>();
            img.setUIInfo(uiInfo);

            if (sprite != null)  //ボタン画像がないときはGameObjectを追加しない。レイアウトが崩れないようにするため。
            {
                UIInfo uiImage = uiInfo.fit_Self();
                if (imgSize > 0)
                    uiImage = uiImage.uiSize(new Vector2(imgSize, imgSize)).fit_Fixed();
                Image buttonImage = CreateImage(goButton, uiInfo: uiImage, goName: "buttonImage");
                buttonImage.sprite = sprite;
                buttonImage.preserveAspect = true;  //引き伸ばしたときに画像のアスペクト比を保つ
                buttonImage.gameObject.transform.SetSiblingIndex(0);  //画像がテキストの上にくるように、GameObjectの順番を入れ替え
                if (imgColor == null) buttonImage.color = Color.white;        // new Color(1f, 1f, 1f, 0.5f);  //画像の色は変えずに半透明にする。
                else buttonImage.color = imgColor.Value;
                //ボタン画像あり・テキストが空のときは、Textに対応するGameObjectをActive:falseにする
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
            if (uiInfo != null && uiInfo.m_uiSize.x != 0f & uiInfo.m_uiSize.y != 0f)
            {
                width = (int)uiInfo.m_uiSize.x;
                height = (int)uiInfo.m_uiSize.y;
                tex = ResizeTexture(tex, width, height);
            }
            return CreateButton(parent, TaskOnClick, labelStr, Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.zero), imgSize: imgSize, imgColor: imgColor, uiInfo, goName);
        }
        public static Button CreateButton(GameObject parent, string texPath, UnityAction TaskOnClick = null,
            string labelStr = "", int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null, string goName = "")
        { return CreateButton(parent, TaskOnClick, labelStr, loadTexture(texPath), imgSize, imgColor, uiInfo, goName); }

        public static Image CreateImage(GameObject parent, Sprite sprite = null, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.IMAGE_DEFAULT;
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_Self();
            GameObject go = CreateUIElement(goName == "" ? goname_Image.get() : goName, uiInfo, parent: parent);
            return addImageComponent(go, sprite, uiInfo);
        }
        public static Image CreateImage(GameObject parent, Texture2D tex, UIInfo uiInfo = null, string goName = "")
        { return CreateImage(parent, Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero), uiInfo, goName); }
        public static Image CreateImage(GameObject parent, string texturePath, int width = 0, int height = 0, UIInfo uiInfo = null, string goName = "")
        {
            Texture2D tex = loadTexture(texturePath);
            if (width != 0 & height != 0) tex = ResizeTexture(tex, width, height);
            return CreateImage(parent, tex, uiInfo, goName);
        }

        public static Texture2D ResizeTexture(Texture2D srcTexture, int newWidth, int newHeight)
        {
            var resizedTexture = new Texture2D(newWidth, newHeight);
            Graphics.ConvertTexture(srcTexture, resizedTexture);
            return resizedTexture;
        }

        // <param name="wholeNumbers">整数のみに制限</param>

        /// <summary>
        /// - root      Component:Slider
        ///     - background
        ///     - fillArea
        ///     - handleArea
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="onValueChanged"></param>
        /// <param name="initialValue"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <param name="wholeNumbers"></param>
        /// <param name="goName"></param>
        /// <returns></returns>
        public static Slider CreateSlider(GameObject parent, UnityAction<float> onValueChanged, float initialValue, float max = 1f, float min = 0f, bool wholeNumbers = false, string goName = "")
        {
            // Create GOs Hierarchy
            GameObject root = CreateUIElement(goName == "" ? goname_Slider.get() : goName, new UIInfo(), parent: parent);
            GameObject background = CreateUIElement("Background", new UIInfo(), parent: root);
            GameObject fillArea = CreateUIElement("Fill Area", new UIInfo(), parent: root);
            GameObject fill = CreateUIElement("Fill", new UIInfo(), parent: fillArea);
            GameObject handleArea = CreateUIElement("Handle Slide Area", new UIInfo(), parent: root);
            GameObject handle = CreateUIElement("Handle", new UIInfo(), parent: handleArea);

            // Background
            Image backgroundImage = addImageComponent(background, uiInfo: new UIInfo().fit_Parent().bgColor(COLOR_SLIDER_BACKGROUND));
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            // Fill
            addImageComponent(fill, uiInfo: new UIInfo().fit_Parent().bgColor(COLOR_AREA_BG));

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(10, 0);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            Image handleImage = addImageComponent(handle, uiInfo: new UIInfo().fit_Parent().bgColor(COLOR_SLIDER_HANDLE));

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);

            // Setup slider component
            LayoutElement sliderLe = root.GetOrAddComponent<LayoutElement>();
            sliderLe.flexibleWidth = 1f;
            sliderLe.minWidth = SLIDER_MIN_WIDTH; sliderLe.minHeight = SLIDER_MIN_HEIGHT;
            Slider slider = root.AddComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            configSelectableColors(slider);
            slider.wholeNumbers = wholeNumbers; //整数のみ
            slider.maxValue = max;
            slider.minValue = min;
            slider.value = initialValue;
            slider.onValueChanged.AddListener(onValueChanged);

            return slider;
        }



        public static Scrollbar CreateScrollbar(GameObject parent, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.SCROLLBAR_DEFAULT;
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_Fixed().position(Vector2.zero).uiSize(UIELEMENT_SIZE);
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElement(goName == "" ? goname_ScrollBar.get() : goName, uiInfo, parent: parent);
            GameObject sliderArea = CreateUIElement("Sliding Area", new UIInfo(), parent: scrollbarRoot);
            GameObject handle = CreateUIElement("Handle", new UIInfo(), parent: sliderArea);


            Image bgImage = addImageComponent(scrollbarRoot, uiInfo: new UIInfo().fit_Parent().bgColor(COLOR_SLIDER_BACKGROUND));

            Image handleImage = addImageComponent(handle);

            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            Scrollbar scrollbar = scrollbarRoot.AddComponent<Scrollbar>();
            scrollbar.handleRect = handleRect;
            scrollbar.targetGraphic = handleImage;
            configSelectableColors(scrollbar);
            return scrollbar;
        }

        /// <summary>
        /// - goToggleRoot      Component:Toggle
        ///     - bgImage
        ///     - label
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="onValueChanged"></param>
        /// <param name="labelStr"></param>
        /// <param name="isOn"></param>
        /// <param name="uiInfo"></param>
        /// <param name="goName"></param>
        /// <returns></returns>
        public static Toggle CreateToggle(GameObject parent, UnityAction<bool> onValueChanged, string labelStr = "Toggle", bool isOn = true,
            UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = new UIInfo().fit_Parent();
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_Parent();
            // Set up hierarchy
            GameObject goToggleRoot = CreateUIElement(goName == "" ? goname_Toggle.get() : goName, uiInfo, parent: parent);
            LayoutGroup lg = addLyaoutGroup(goToggleRoot, LayoutType.Horizontal, uiInfo.padding(0));
            lg.childAlignment = TextAnchor.MiddleLeft;
            Image bgImage = CreateImage(parent: goToggleRoot,
                uiInfo: new UIInfo().fit_Flexible(0f, 0f, new Vector2(TOGGLE_BACKGROUND_SIZE, TOGGLE_BACKGROUND_SIZE)).bgColor(COLOR_SELECTABLE_ON), goName: "Background");
            GameObject goBackground = bgImage.gameObject;

            Image checkmarkImage = CreateImage(parent: goBackground,
              uiInfo: new UIInfo().fit_Fixed().bgColor(Color.white), goName: "Checkmark");
            GameObject goCheckmark = checkmarkImage.gameObject;
            RectTransform checkmarkRect = goCheckmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(TOGGLE_BACKGROUND_SIZE / 2, TOGGLE_BACKGROUND_SIZE / 2);

            Text label = CreateText(parent: goToggleRoot, uiInfo: new UIInfo().fit_Parent(), label: labelStr);
            label.gameObject.getParent().GetOrAddComponent<LayoutElement>().flexibleWidth = 1;

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
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_WParentHSelf();
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

        /// <summary>
        /// - goInputField      Component : InputField, Image, LayoutElement
        ///     - goPlaceholder         : Text
        ///     - goText                : Text
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="onEndEdit"></param>
        /// <param name="onValueChanged"></param>
        /// <param name="initialText"></param>
        /// <param name="lineCount"></param>
        /// <param name="uiInfo"></param>
        /// <param name="goName"></param>
        /// <returns></returns>
        public static InputField CreateInputField(GameObject parent, UnityAction<string> onEndEdit, UnityAction<string> onValueChanged,
           string initialText = "", int lineCount = 1, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.INPUTFIELD_DEFAULT;
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_WParentHSelf();
            GameObject goInputField = CreateUIElement(goName == "" ? goname_InputField.get() : goName, uiInfo, parent: parent);
            GameObject goPlaceholder = CreateUIElement("Placeholder", new UIInfo(), parent: goInputField);
            GameObject goText = CreateUIElement("Text", new UIInfo(), parent: goInputField);

            Image image = addImageComponent(goInputField, uiInfo: new UIInfo().fit_Parent().bgColor(COLOR_SELECTABLE));

            InputField inputField = goInputField.AddComponent<InputField>();
            LayoutElement leInputField = goInputField.GetOrAddComponent<LayoutElement>();
            leInputField.minWidth = INPUTFIELD_MIN_SIZE.x;
            leInputField.minHeight = INPUTFIELD_MIN_SIZE.y;

            Text text = addTextComponent(goText, "", uiInfo);
            text.supportRichText = false;

            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            Text placeholder = addTextComponent(goPlaceholder, "Enter text...", uiInfo: new UIInfo().textColor(placeholderColor));

            if (lineCount >= 2)  //複数行の入力の場合の設定
            {
                inputField.lineType = InputField.LineType.MultiLineNewline;     //垂直にスクロールし、オーバーフローし、リターンキーで改行文字を挿入する複数行の InputField です。
                leInputField.minHeight = (INPUTFIELD_MIN_SIZE.y - INPUTFIELD_LINE_HEIGHT) + INPUTFIELD_LINE_HEIGHT * lineCount;
                text.alignment = TextAnchor.UpperLeft;
                placeholder.alignment = TextAnchor.UpperLeft;
            }

            RectTransform textRectTransform = goText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = new Vector2(10, 6);
            textRectTransform.offsetMax = new Vector2(-10, -7);

            RectTransform placeholderRectTransform = goPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;
            placeholderRectTransform.offsetMin = new Vector2(10, 6);
            placeholderRectTransform.offsetMax = new Vector2(-10, -7);

            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.text = initialText;
            inputField.onValueChanged.AddListener(s => { leInputField.preferredHeight = text.preferredHeight; });
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


        /// <summary>
        /// - goScrollView      Component : ScrollRect, Image
        ///     - goViewport                Mask, Image
        ///         - goContent             LayoutGroup
        ///     - hScrollbar                Scrollbar
        ///     - vScrollbar                Scrollbar
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="contentPanelLayoutGroupType"></param>
        /// <param name="scrollSensitivity"></param>
        /// <param name="uiInfo"></param>
        /// <param name="goName"></param>
        /// <returns>contentのGameObject,ScrollViewに要素を追加するときはこのGameObjectに追加すること。</returns>
        public static LayoutGroup CreateScrollView(GameObject parent, LayoutType contentPanelLayoutGroupType = LayoutType.Vertical,
            float scrollSensitivity = 20, UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = UIInfo.SCROLLVIEW_DEFAULT;
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_Parent();
            if (uiInfo.m_bgColor == default(Color)) uiInfo = uiInfo.bgColor(COLOR_AREA_BG);
            GameObject goScrollView = CreateUIElement(goName == "" ? goname_ScrollView.get() : goName, uiInfo, parent: parent);
            GameObject goViewport = CreateUIElement("Viewport", new UIInfo().fit_Parent(), parent: goScrollView);
            GameObject goContent = CreateUIElement("Content", new UIInfo().fit_Self(), parent: goViewport);

            GameObject hScrollbar = CreateScrollbar(parent: goScrollView).gameObject;
            hScrollbar.name = "ScrollbarHorizontal";
            RectTransform hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.anchorMin = Vector2.zero;
            hScrollbarRT.anchorMax = Vector2.right;
            hScrollbarRT.pivot = Vector2.zero;
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            GameObject vScrollbar = CreateScrollbar(parent: goScrollView).gameObject;
            vScrollbar.name = "Scrollbar Vertical";
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            RectTransform viewportRT = goViewport.GetComponent<RectTransform>();
            viewportRT.pivot = Vector2.up;

            RectTransform contentRT = goContent.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;

            ScrollRect scrollRect = goScrollView.AddComponent<ScrollRect>();
            scrollRect.content = contentRT;
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
