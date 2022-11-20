using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace uBUI
{

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
            GridLayoutGroup lg = (GridLayoutGroup)SWHelper.CreatePanel(uiInfo.fitW(UIInfo.Fit.Flexible), layoutGroup: LayoutType.Grid, parent: parent);
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
            //if (uiInfo.is_fit_UnSpecified()) uiInfo = uiInfo.fitW(UIInfo.Fit.Flexible);
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
            UIInfo uiInfo = new UIInfo().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.fitWH(UIInfo.Fit.Fixed, size); //.fit_Fixed(Vector2.zero, size);
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
            UIInfo uiInfo = new UIInfo().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.fitWH(UIInfo.Fit.Fixed, size); // fit_Fixed(Vector2.zero, size);
            return SWHelper.CreateImage(goPanel, sprite, uiInfo: uiInfo);
        }
        public Image addImage_byTexture2D(Texture2D tex = null, Vector2 size = default(Vector2), Color color = default(Color))
        {
            if (color == default(Color)) color = Color.white;
            UIInfo uiInfo = new UIInfo().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.fitWH(UIInfo.Fit.Fixed, size);
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

}
