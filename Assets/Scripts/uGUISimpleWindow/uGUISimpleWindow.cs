using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace uGUISimpleWindow
{
    public enum LayoutType
    {
        Grid,
        Horizontal,
        Vertical,
        None,
    }

    [Serializable]
    public class UIInfo
    {
        public static readonly UIInfo TEXT_DEFAULT = new UIInfo().fit_WParentHSelf().bgColor(Color.clear/*SWHelper.COLOR_AREA_BG*/)
            .textSize(SWHelper.FONT_SIZE).textAlignment(TextAnchor.MiddleLeft);
        public static readonly UIInfo BUTTON_DEFAULT = TEXT_DEFAULT.fit_WParentHSelf().bgColor(Color.white)  //ボタンのデフォルト背景色は白。ホバーしたときの色変化を見やすくするため。)
            .textAlignment(TextAnchor.MiddleCenter);
        public static readonly UIInfo INPUTFIELD_DEFAULT = TEXT_DEFAULT.fit_WParentHSelf().bgColor(Color.gray);
        public static readonly UIInfo IMAGE_DEFAULT = new UIInfo().fit_Self();
        public static readonly UIInfo TOGGLE_DEFAULT = new UIInfo().fit_Parent();
        public static readonly UIInfo RADIO_BUTTON_DEFAULT = new UIInfo().fit_WParentHSelf();
        public static readonly UIInfo SCROLLBAR_DEFAULT = new UIInfo().fit_Fixed().position(Vector2.zero).uiSize(SWHelper.UIELEMENT_SIZE);
        public static readonly UIInfo SCROLLVIEW_DEFAULT = new UIInfo().fit_Parent().bgColor(SWHelper.COLOR_AREA_BG);
        public static readonly UIInfo CANVAS_DEFAULT = new UIInfo().fit_Fixed().fit_Fixed();
        public static readonly UIInfo PANEL_DEFAULT = new UIInfo().fit_Parent().fit_Parent().bgColor(SWHelper.COLOR_AREA_BG);

        public enum Fit { Parent, WParentHSelf, WSelfHParent, Self, Fixed, Flexible, WParentHFrexible, UnSpecified }
        public Fit m_fit = Fit.UnSpecified;
        public Vector2 m_margin = Vector2.zero;   //Parentで使用
        public Vector2 m_position = Vector2.zero;       //SpecifiedSizeで使用
        public Vector2 m_uiSize = Vector2.zero;           //SpecifiedSizeで使用、０なら設定しない
        public float m_flexWidth = 0f;        //SpecifiedSizeで使用、０なら設定しない
        public float m_flexHeight = 0f;       //SpecifiedSizeで使用、０なら設定しない
        public int m_textSize = SWHelper.FONT_SIZE;
        public Color m_textColor = SWHelper.COLOR_TEXT;
        public Color m_bgColor = default(Color);      //デフォルト値なら、各UI要素作成時に適切な色を選択
        public TextAnchor m_textAlignment = TextAnchor.MiddleLeft;
        public int m_spacing = SWHelper.LAYOUTGROUP_SPACING;
        public int m_padding_left = -1;
        public int m_padding_right = -1;
        public int m_padding_top = -1;
        public int m_padding_bottom = -1;

        public UIInfo() { }

        /* フィールド編集用メソッド
         * メソッドチェインでの利用を想定
         * Usage:
         *  UIInfo uiInfo1 = new UIInfo().fit_Parent().bgColor(Color.red);
         *  UIInfo uiInfo2 = uiInfo1.textSize(14);
         *  
         *  ※　呼び出し元のUIInfoの変更を避けるため、クローンを編集して返却している。
        */
        public UIInfo margin(Vector2 margin) { UIInfo ret = this.Clone(); ret.m_margin = margin; return ret; }
        public UIInfo position(Vector2 position) { UIInfo ret = this.Clone(); ret.m_position = position; return ret; }
        public UIInfo uiSize(Vector2 uiSize)
        {
            UIInfo ret = this.Clone();
            if (ret.is_fit_UnSpecified()) ret = ret.fit_Fixed();
            ret.m_uiSize = uiSize; return ret;
        }
        public UIInfo flexWidth(float flexWidth) { UIInfo ret = this.Clone(); ret.m_flexWidth = flexWidth; return ret; }
        public UIInfo flexHeight(float flexHeight) { UIInfo ret = this.Clone(); ret.m_flexHeight = flexHeight; return ret; }
        public UIInfo textSize(int sizePx) { UIInfo ret = this.Clone(); ret.m_textSize = sizePx; return ret; }
        public UIInfo textColor(Color textColor) { UIInfo ret = this.Clone(); ret.m_textColor = textColor; return ret; }
        public UIInfo bgColor(Color bgColor) { UIInfo ret = this.Clone(); ret.m_bgColor = bgColor; return ret; }
        public UIInfo bgColor(string colorCode) { UIInfo ret = this.Clone(); ret.m_bgColor = SWHelper.parseColor(colorCode); return ret; }
        public UIInfo textAlignment(TextAnchor textAlignment) { UIInfo ret = this.Clone(); ret.m_textAlignment = textAlignment; return ret; }
        public UIInfo spacing(int space) { UIInfo ret = this.Clone(); ret.m_spacing = space; return ret; }
        public UIInfo padding(int pad) { return this.padding(pad, pad, pad, pad); }
        public UIInfo padding(int left, int right, int top, int bottom)
        {
            UIInfo ret = this.Clone();
            ret.m_padding_left = left;
            ret.m_padding_right = right;
            ret.m_padding_top = top;
            ret.m_padding_bottom = bottom;
            return ret;
        }

        /// <summary>縦横固定サイズ[px]</summary>
        public UIInfo fit_Fixed(Vector2 position = default(Vector2), Vector2 uiSize = default(Vector2))
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.Fixed;
            if (position != default(Vector2)) ret.m_position = position;
            if (uiSize != default(Vector2)) ret.m_uiSize = uiSize;
            return ret;
        }
        /// <summary>親GameObjectのサイズに合わせる。</summary>
        public UIInfo fit_Parent(Vector2 margin = default(Vector2))
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.Parent;
            if (margin != default(Vector2)) ret.m_margin = margin;
            return ret;
        }
        /// <summary>自身のコンポーネントのサイズに合わせる</summary>
        public UIInfo fit_Self()
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.Self;
            return ret;
        }
        /// <summary>親GameObjectに隙間ができないように合わせる</summary>
        public UIInfo fit_Flexible(float flexWidth = 1f, float flexHeight = 1f, Vector2 minSize = default(Vector2))
        {
            if (minSize == default(Vector2)) minSize = Vector2.zero;
            UIInfo ret = this.Clone(); ret.m_fit = Fit.Flexible;
            ret.m_flexWidth = flexWidth; ret.m_flexHeight = flexHeight; ret.m_uiSize = minSize;
            return ret;
        }
        /// <summary>横幅は親、縦幅は自身のサイズに合わせる</summary>
        public UIInfo fit_WParentHSelf(Vector2 margin = default(Vector2))
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.WParentHSelf;
            if (margin != default(Vector2)) ret.m_margin = margin;
            return ret;
        }
        public UIInfo fit_WSelfHParent()
        {
            UIInfo ret = this.Clone(); ret.m_fit = Fit.WSelfHParent;
            return ret;
        }

        public UIInfo Clone()     // ディープコピー
        {
            string jsonStr = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<UIInfo>(jsonStr);
        }

        public bool is_fit_UnSpecified() { return this.m_fit == Fit.UnSpecified; }
    }

    public class SPanel
    {
        const int DEFAULT_GRIDPANEL_CELLWIDTH = 100;
        public readonly GameObject goPanel;
        public readonly LayoutGroup layoutGroup;

        public Vector2 Size { get { return goPanel.GetComponent<RectTransform>().rect.size; } }

        protected internal SPanel(LayoutGroup layoutGroup)
        {
            this.goPanel = layoutGroup.gameObject;
            this.layoutGroup = layoutGroup;
        }


        /// <summary>
        /// GameObject `parent`の子GameObjectを作成して、VerticalLayoutGroupコンポーネントを追加する。
        /// 戻り値のSPanelオブジェクトを通して、UI要素を追加できる。
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public static SPanel CreateVertical(GameObject parent, UIInfo uiInfo = null, string goName = "")
        { return new SPanel(SWHelper.CreatePanel(uiInfo, layoutGroup: LayoutType.Vertical, parent: parent, goName: goName)); }
        /// <summary>
        /// GameObject `parent`の子GameObjectを作成して、HorizontalLayoutGroupコンポーネントを追加する。
        /// 戻り値のSPanelオブジェクトを通して、UI要素を追加できる。
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public static SPanel CreateHorizontal(GameObject parent, UIInfo uiInfo = null)
        { return new SPanel(SWHelper.CreatePanel(uiInfo, layoutGroup: LayoutType.Horizontal, parent: parent)); }
        /// <summary>
        /// GameObject `parent`の子GameObjectを作成して、GridLayoutGroupコンポーネントを追加する。
        /// 戻り値のSPanelオブジェクトを通して、UI要素を追加できる。
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="cellWidthPx"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public static SPanel CreateGrid(GameObject parent, int cellWidthPx = -1, float aspectRatio = 1, UIInfo uiInfo = null)
        {
            if (uiInfo == null) uiInfo = new UIInfo();
            GridLayoutGroup lg = (GridLayoutGroup)SWHelper.CreatePanel(uiInfo.fit_WParentHSelf(), layoutGroup: LayoutType.Grid, parent: parent);
            float width = DEFAULT_GRIDPANEL_CELLWIDTH;
            if (cellWidthPx >= 1) { width = cellWidthPx; }
            lg.cellSize = new Vector2(width, width * aspectRatio);
            return new SPanel(lg);
        }
        /// <summary>
        /// `layoutGroup`コンポーネントから、SPanelを作成する。
        /// </summary>
        /// <param name="layoutGroup"></param>
        /// <returns></returns>
        public static SPanel CreateFromPanel(LayoutGroup layoutGroup) { return new SPanel(layoutGroup); }

        /// <summary>
        /// VerticalLayoutGroupコンポーネントを持ったGameObjectを子要素に追加して、LayoutGroupに対応するSPanelを返す
        /// </summary>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public SPanel addPanel_Vertical(UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = new UIInfo();
            if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fit_WParentHSelf();
            return CreateVertical(goPanel, uiInfo, goName: goName);
        }
        /// <summary>
        /// HorizontalLayoutGroupコンポーネントを持ったGameObjectを子要素に追加して、LayoutGroupに対応するSPanelを返す
        /// </summary>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public SPanel addPanel_Horizontal(UIInfo uiInfo = null) { return CreateHorizontal(goPanel, uiInfo); }
        /// <summary>
        /// GridLayoutGroupコンポーネントを持ったGameObjectを子要素に追加して、LayoutGroupに対応するSPanelを返す
        /// </summary>
        /// <param name="cellWidthPx">GridのCellの横幅のピクセル数</param>
        /// <param name="aspectRatio">Cellのアスペクト比</param>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public SPanel addPanel_Grid(int cellWidthPx = -1, float aspectRatio = 1, UIInfo uiInfo = null)
        { return CreateGrid(goPanel, cellWidthPx, aspectRatio, uiInfo); }
        public InfiniteScroll addInfiniteScroll(Func<GameObject, int, InfiniteScroll.Item> itemBuildFunc, float itemHeight, int initialItemCount)
        {
            GameObject.Destroy(this.goPanel.GetComponent<VerticalLayoutGroup>());
            var ret = this.goPanel.AddComponent<InfiniteScroll>();
            ret.initFields(this, itemBuildFunc, itemHeight, initialItemCount);
            return ret;
        }

        /// <summary>
        /// Textコンポーネントを持ったGameObjectを子要素に追加して、Textコンポーネントを返す
        /// </summary>
        /// <param name="label">Textに表示する文字列</param>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public Text addText(string label, UIInfo uiInfo = null)
        { return SWHelper.CreateText(goPanel, label, uiInfo: uiInfo); }
        /// <summary>
        /// 空白を表現するために空のImageコンポーネントを持ったGameObjectを子要素に追加して、Imageコンポーネントを返す
        /// </summary>
        /// <param name="height">空白の高さのピクセル数</param>
        /// <returns></returns>
        public Image addSpacer(int height = 15) { return SWHelper.CreateSpacer(goPanel, height); }
        /// <summary>
        /// 水平方向の罫線を表現するためにImageコンポーネントを持ったGameObjectを子要素に追加して、Imageコンポーネントを返す
        /// </summary>
        /// <param name="height">罫線の高さ（太さ）のピクセル数</param>
        /// <param name="color">罫線の色</param>
        /// <returns></returns>
        public Image addHorizontalBar(int height = SWHelper.HR_HEIGHT, Color color = default(Color)) { return SWHelper.CreateHorizontalBar(goPanel, height, color); }
        /// <summary>
        /// Buttonコンポーネントを持ったGameObjectを子要素に追加して、Buttonコンポーネントを返す
        /// </summary>
        /// <param name="TaskOnClick">ボタンをクリックしたときの動作</param>
        /// <param name="labelStr">ボタンに表示する文字列</param>
        /// <param name="texPath">ボタンに表示する画像の、ファイルへのパス。（画像を表示するときは、texPath/tex/spriteのいずれかを指定する）</param>
        /// <param name="tex">ボタンに表示する画像</param>
        /// <param name="sprite">ボタンに表示する画像</param>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public Button addButton(UnityAction TaskOnClick, string labelStr = "", string texPath = "",
            Texture2D tex = null, Sprite sprite = null, int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null)
        {
            // Spriteからボタンを作る場合
            if (sprite != null) return SWHelper.CreateButton(goPanel, TaskOnClick: TaskOnClick, labelStr: labelStr, sprite: sprite, imgSize: imgSize, imgColor: imgColor, uiInfo: uiInfo);
            // labelStrとTexture2Dオブジェクトからボタンを作る場合
            if (texPath == "") return SWHelper.CreateButton(goPanel, TaskOnClick: TaskOnClick, labelStr: labelStr, tex: tex, imgSize: imgSize, imgColor: imgColor, uiInfo: uiInfo);
            // texPathからTexture2Dを読込んでボタンを作る場合
            else return SWHelper.CreateButton(goPanel, texPath: texPath, TaskOnClick: TaskOnClick, labelStr: labelStr, imgSize: imgSize, imgColor: imgColor, uiInfo: uiInfo);
        }
        /// <summary>
        /// ラジオボタンを表示するために、ToggleGroupコンポーネントを持ったGameObjectを子要素に追加して、ToggleGroupコンポーネントを返す。
        /// ラジオボタンを選択すると、`showValueDict`で指定した値を引数にして`onValueChanged`が実行される。
        /// </summary>
        /// <typeparam name="T">onValueChangedの引数の型</typeparam>
        /// <param name="onValueChanged">ラジオボタンを選択したときに実行する処理。</param>
        /// <param name="showValueDict">キーがラジオボタンの表示文字列、値が`onValueChanged`に渡される値　の辞書。</param>
        /// <param name="selected">初期状態で選択されているラジオボタンの番号</param>
        /// <param name="layoutGroup">ラジオボタンの配置方法</param>
        /// <returns></returns>
        public ToggleGroup addRadioButton<T>(UnityAction<T> onValueChanged, Dictionary<string, T> showValueDict, int selected = 0,
            LayoutType layoutGroup = LayoutType.Horizontal)
        { return SWHelper.CreateRadioButton(goPanel, onValueChanged, showValueDict, selected, layoutGroup); }
        /// <summary>
        /// Sliderコンポーネントを持ったGameObjectを子要素に追加して、Sliderコンポーネントを返す
        /// </summary>
        /// <param name="onValueChanged">Sliderを動かしたときに実行する処理</param>
        /// <returns></returns>
        public Slider addSlider(UnityAction<float> onValueChanged, float initialValue = 0.5f, float max = 1f, float min = 0f, bool wholeNumbers = false)
        { return SWHelper.CreateSlider(goPanel, onValueChanged, initialValue, max, min, wholeNumbers); }
        /// <summary>
        /// Toggleコンポーネントを持ったGameObjectを子要素に追加して、Toggleコンポーネントを返す
        /// </summary>
        /// <param name="onValueChanged">Toggleを選択したときに実行する処理</param>
        /// <param name="labelStr">Toggleに表示する文字列</param>
        /// <param name="isOn">ToggleのON/OFFの初期状態</param>
        /// <returns></returns>
        public Toggle addToggle(UnityAction<bool> onValueChanged, string labelStr, bool isOn = true)
        { return SWHelper.CreateToggle(goPanel, onValueChanged, labelStr, isOn: isOn); }
        /// <summary>
        /// Imageコンポーネントを持ったGameObjectを子要素に追加して、Imageコンポーネントを返す
        /// </summary>
        /// <param name="path">ボタンに表示する画像の、ファイルへのパス</param>
        /// <param name="size">画像をリスケールするときのピクセル数</param>
        /// <param name="color">指定した色を画像に対して乗算する。デフォルト値（Color.white）なら元の色で表示される。</param>
        /// <returns></returns>
        public Image addImage(string path, Vector2 size = default(Vector2), Color color = default(Color))
        {
            UIInfo uiInfo = new UIInfo().fit_Self().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.fit_Fixed(Vector2.zero, size);
            return SWHelper.CreateImage(goPanel, path, width: (int)size.x, height: (int)size.y, uiInfo: uiInfo);
        }
        /// <summary>
        /// Imageコンポーネントを持ったGameObjectを子要素に追加して、Imageコンポーネントを返す
        /// </summary>
        /// <param name="sprite">ボタンに表示する画像</param>
        /// <param name="size">画像をリスケールするときのピクセル数</param>
        /// <param name="color">指定した色を画像に対して乗算する。デフォルト値（Color.white）なら元の色で表示される。</param>
        /// <returns></returns>
        public Image addImage(Sprite sprite = null, Vector2 size = default(Vector2), Color color = default(Color))
        {
            if (color == default(Color)) color = Color.white;
            UIInfo uiInfo = new UIInfo().fit_Self().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.fit_Fixed(Vector2.zero, size);
            return SWHelper.CreateImage(goPanel, sprite, uiInfo: uiInfo);
        }
        public Image addImage_byTexture2D(Texture2D tex = null, Vector2 size = default(Vector2), Color color = default(Color))
        {
            if (color == default(Color)) color = Color.white;
            UIInfo uiInfo = new UIInfo().fit_Self().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.fit_Fixed(Vector2.zero, size);
            return SWHelper.CreateImage(goPanel, tex, uiInfo: uiInfo);
        }
        /// <summary>
        /// TextFieldコンポーネントを持ったGameObjectを子要素に追加して、TextFieldコンポーネントを返す
        /// </summary>
        /// <param name="onEndEdit">文字列の編集を確定したときに実行する処理</param>
        /// <param name="onValueChanged">文字列を編集したときに実行する処理</param>
        /// <param name="initialText">表示する文字列の初期値</param>
        /// <param name="uiInfo"></param>
        /// <param name="lineCount">文字列の編集領域の行数</param>
        /// <returns></returns>
        public InputField addTextField(UnityAction<string> onEndEdit = null, UnityAction<string> onValueChanged = null,
            string initialText = "", UIInfo uiInfo = null, int lineCount = 1)
        { return SWHelper.CreateInputField(goPanel, onEndEdit, onValueChanged, initialText, lineCount, uiInfo); }

        public void clear(bool detatch = false)
        {
            if (!detatch)
            {
                foreach (Transform child in this.goPanel.gameObject.transform)
                    GameObject.Destroy(child.gameObject);
            }
            else
            {
                this.goPanel.gameObject.transform.DetachChildren();
            }
        }

    }
    public class SWindow
    {
        public SPanel container { get; protected internal set; }
        public SPanel header { get; protected internal set; }
        public SPanel title { get; protected internal set; }
        public SPanel caption { get; protected internal set; }
        public SPanel content { get; protected internal set; }
        public SPanel hooter { get; protected internal set; }
        public Text uiTitle { get; protected internal set; }

        UIInfo uiContainerPanel = new UIInfo().bgColor(new Color(1f, 1f, 1f, 0.1f));
        UIInfo uiContentPanel = new UIInfo().bgColor(new Color(0f, 0f, 0f, 0.3f));
        ActionRunner actionRunner;

        public SWindow() { }

        protected internal void _SWindow(LayoutGroup lgContainer, string title = "", LayoutType hooterLayout = LayoutType.Horizontal,
            UIInfo uiInfo = null, GameObject parent = null)
        {
            if (uiInfo == null) uiInfo = uiContentPanel.Clone();
            this.container = SPanel.CreateFromPanel(lgContainer);

            this.header = SPanel.CreateFromPanel(SWHelper.CreatePanel(uiInfo: new UIInfo().fit_WParentHSelf().bgColor(uiInfo.m_bgColor),
                layoutGroup: LayoutType.Vertical, parent: container.goPanel, goName: title + "-header"));
            SPanel _title_caption_container = SPanel.CreateFromPanel(SWHelper.CreatePanel(uiInfo: new UIInfo().fit_WParentHSelf().bgColor(uiInfo.m_bgColor),
                layoutGroup: LayoutType.Horizontal, parent: header.goPanel, goName: title + "-title-caption-container"));
            this.title = SPanel.CreateFromPanel(SWHelper.CreatePanel(uiInfo: new UIInfo().fit_WParentHSelf().bgColor(uiInfo.m_bgColor),
                layoutGroup: LayoutType.Horizontal, parent: _title_caption_container.goPanel, "left"));  //子要素のサイズに合わせる
            this.caption = SPanel.CreateFromPanel(SWHelper.CreatePanel(uiInfo: new UIInfo().fit_Self().bgColor(uiInfo.m_bgColor),
                layoutGroup: LayoutType.Horizontal, parent: _title_caption_container.goPanel, "right")); //子要素のサイズに合わせる
            uiTitle = this.title.addText(title);

            this.content = SPanel.CreateFromPanel(
                SWHelper.CreateScrollView(container.goPanel, uiInfo: uiInfo.fit_Flexible(), goName: title + "-content"));
            this.hooter = SPanel.CreateFromPanel(
                SWHelper.CreatePanel(uiInfo: uiInfo, layoutGroup: hooterLayout, parent: container.goPanel, goName: title + "-hooter"));

            actionRunner = container.goPanel.AddComponent<ActionRunner>();
        }

        class ActionRunner : MonoBehaviour
        {
            List<UnityAction> actionList = new List<UnityAction>();
            public void addEvent(UnityAction action) { actionList.Add(action); }
            public void Update()
            {
                foreach (var action in actionList) action();
            }
        }

        internal void updateScrollContentWidth()
        {
            content.goPanel.GetComponent<LayoutElement>().preferredWidth = calcScrollViewWidth();
        }

        public float calcScrollViewWidth()
        { return SWHelper.calcScrollViewWidth(container.goPanel); }



        /// <summary>
        /// ウィンドウを作成して、スクリーン上にUIを配置する。（CanvasのRenderModeがScreenSpaceOverlayになる）
        /// </summary>
        /// <param name="title">ウィンドウのタイトル</param>
        /// <param name="leftbottom">ウィンドウの左下角の座標（原点：スクリーンの左下、右・上方向が正）</param>
        /// <param name="windowSize">ウィンドウのサイズ</param>
        /// <param name="headerLayout">ヘッダーパネル内の要素の配置方法（デフォルト：横に配置）</param>
        /// <param name="hooterLayout">フッターパネル内の要素の配置方法（デフォルト：横に配置）</param>
        /// <param name="uiInfo">UI要素のプロパティの指定（背景色とか）</param>
        /// <param name="draggable">ウィンドウをドラッグ可能にするか</param>
        /// <param name="canvasScale">UI要素のサイズを微調整。文字サイズのバランスやUIの配置を保ったままウィンドウを大きくしたいときに使う。</param>
        /// <param name="parent">親GameObject</param>
        /// <returns></returns>
        public void init_onScreen(string title = "", Vector2 leftbottom = default(Vector2), Vector2 windowSize = default(Vector2),
             LayoutType hooterLayout = LayoutType.Horizontal, UIInfo uiInfo = null, bool draggable = true,
             float canvasScale = 1f, GameObject parent = null)
        {
            LayoutGroup lg = SWHelper.CreateWindowWithCanvas_onScreen(leftbottom, windowSize,
                uiInfo: uiContainerPanel, layoutGroup: LayoutType.Vertical,
                draggable: draggable, canvasScale: canvasScale, parent: parent, goName: title + "-window");
            _SWindow(lg, title, hooterLayout, uiInfo, parent);
            onStartInit();
            onEndInit();
        }

        /// <summary>
        /// ウィンドウを作成して、ゲーム空間にUIを配置する。（CanvasのRenderModeがWorldSpaceになる）
        /// </summary>
        /// <param name="title">ウィンドウのタイトル</param>
        /// <param name="leftbottom">ウィンドウの左下角の座標（原点：スクリーンの左下、右・上方向が正）</param>
        /// <param name="windowSize">ウィンドウのサイズ</param>
        /// <param name="headerLayout">ヘッダーパネル内の要素の配置方法（デフォルト：横に配置）</param>
        /// <param name="hooterLayout">フッターパネル内の要素の配置方法（デフォルト：横に配置）</param>
        /// <param name="uiInfo">UI要素のプロパティの指定（背景色とか）</param>
        /// <param name="camera">どのカメラを通してUIを操作するかを指定</param>
        /// <param name="meterPerPx">１ピクセルをゲーム空間の何m（or 距離単位）に割り当てるか</param>
        /// <param name="canvasScale">ウィンドウ全体のサイズを微調整。文字サイズのバランスやUIの配置を保ったままウィンドウを大きくしたいときに使う。</param>
        /// <param name="parent">親GameObject</param>
        /// <returns></returns>
        public void init_onWorld(string title = "", Vector2 leftbottom = default(Vector2), Vector2 windowSize = default(Vector2),
            LayoutType hooterLayout = LayoutType.Horizontal, UIInfo uiInfo = null,
             Camera camera = null, float meterPerPx = 0.001f, float canvasScale = 1f, GameObject parent = null)
        {
            LayoutGroup lg = SWHelper.CreateWindowWithCanvas_onWorld(leftbottom, windowSize,
                uiInfo: uiContainerPanel, layoutGroup: LayoutType.Vertical,
                  parent: parent, camera: camera, meterPerPx: meterPerPx, canvasScale: canvasScale, goName: title + "-window");
            _SWindow(lg, title, hooterLayout, uiInfo, parent: parent);
            onStartInit();
            onEndInit();
        }
        protected internal virtual void onStartInit() { }
        protected internal virtual void onEndInit() { }

        /// <summary>
        /// キャプションボタン（ウィンドウ右上のボタン）を追加
        /// </summary>
        /// <param name="TaskOnClick"></param>
        /// <param name="labelStr"></param>
        /// <param name="texPath"></param>
        /// <param name="tex"></param>
        /// <param name="uiInfo"></param>
        /// <returns></returns>
        public Button addCaptionButton(UnityAction TaskOnClick, string labelStr = "", string texPath = "",
            Texture2D tex = null, Sprite sprite = null, int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null)
        {
            Button ret = caption.addButton(TaskOnClick, labelStr, texPath, tex, sprite, imgSize, imgColor, uiInfo);
            _disable_flexWidth(ret.gameObject);  //すべてのGameObjectのflexWidthを0に設定
            return ret;
        }
        protected internal void _disable_flexWidth(GameObject go)
        {
            LayoutElement le = go.GetComponent<LayoutElement>();
            if (le != null) le.flexibleWidth = 0f;
            SWHelper.Foreach(go, _disable_flexWidth);
        }

        /// <summary>
        /// ウィンドウの位置・サイズを、座標で指定
        /// （ウィンドウをスクリーン上に配置した場合のみ、正常に実行できる）
        /// </summary>
        /// <param name="left">ウィンドウ左下角の点のX座標</param>
        /// <param name="bottom">ウィンドウ左下角の点のY座標</param>
        /// <param name="width">ウィンドウの幅</param>
        /// <param name="height">ウィンドウの高さ</param>
        public void locate_byPosition(int? left, int? bottom, int? width, int? height)
        {
            RectTransform rt = container.goPanel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.zero;
            rt.sizeDelta = new Vector2(width ?? rt.sizeDelta.x, height ?? rt.sizeDelta.y);
            //rt.localPosition = new Vector2(left, bottom);
            rt.position = new Vector2(left ?? rt.position.x, bottom ?? rt.position.y);
            updateScrollContentWidth();
        }
        /// <summary>
        /// ウィンドウの位置・サイズを、スクリーン外周からの割合（PCT:PerCenTage）で指定
        /// （ウィンドウをスクリーン上に配置した場合のみ、正常に実行できる）
        /// </summary>
        /// <param name="left">スクリーン左端からの距離の割合</param>
        /// <param name="right">スクリーン右端からの距離の割合</param>
        /// <param name="top">スクリーン上端からの距離の割合</param>
        /// <param name="bottom">スクリーン下端からの距離の割合</param>
        public void locate_byMarginPct(float? left, float? right, float? top, float? bottom)
        {
            RectTransform rt = container.goPanel.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(left ?? rt.anchorMin.x, bottom ?? rt.anchorMin.y);
            rt.anchorMax = new Vector2((1 - right) ?? rt.anchorMax.x, (1 - top) ?? rt.anchorMax.y);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            updateScrollContentWidth();
        }
        /// <summary>
        /// ウィンドウの位置・サイズを、スクリーン外周からのピクセル数で指定
        /// （ウィンドウをスクリーン上に配置した場合のみ、正常に実行できる）
        /// </summary>
        /// <param name="left">スクリーン左端からのピクセル数</param>
        /// <param name="right">スクリーン右端からのピクセル数</param>
        /// <param name="top">スクリーン上端からのピクセル数</param>
        /// <param name="bottom">スクリーン下端からのピクセル数</param>
        public void locate_byMarginPx(int? left, int? right, int? top, int? bottom)
        {
            RectTransform rt = container.goPanel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left ?? rt.offsetMin.x, bottom ?? rt.offsetMin.y);
            rt.offsetMax = new Vector2((-right) ?? rt.offsetMax.x, (-top) ?? rt.offsetMin.y);
            updateScrollContentWidth();
        }

        public GameObject getCanvasGameObject_orNull()
        {
            if (container == null) return null;
            if (container.goPanel == null) return null;
            return container.goPanel.gameObject.getParent();
        }

        /// <summary>
        /// SWindowオブジェクトを削除。
        /// 実際には、WindowのGameObjectの１階層上の、Canvasを保持するGameObjectを削除する。
        /// </summary>
        public void Dispose()
        {
            var go = getCanvasGameObject_orNull();
            if (go != null) UnityEngine.Object.Destroy(go);
        }
        public void SetActive(bool isActive)
        { getCanvasGameObject_orNull()?.SetActive(isActive); }

        public bool GetActive()
        {
            var go = getCanvasGameObject_orNull();
            if (go != null) return go.activeSelf;
            return false;
        }

        public void setWindowScale(float scale)
        {
            container.goPanel.gameObject.getParent().GetComponent<Canvas>().scaleFactor = scale;
        }

        public void setWindowTitle(string title)
        {
            container.goPanel.gameObject.getParent().name = title;
            uiTitle.text = title;

        }

        /// <summary>毎フレーム実行される処理を登録。containerパネルに対応するGameObjectにアタッチされるので、ウィンドウを破棄したときに無効になる。</summary>
        public void addUpdateAction(UnityAction action)
        {
            actionRunner.addEvent(action);
        }

        protected virtual void onRepaint() { }
        public virtual void repaint()
        {
            this.updateScrollContentWidth();
            onRepaint();
        }

    }
}
