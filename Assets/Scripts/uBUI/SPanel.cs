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

        protected SPanel(LayoutGroup layoutGroup)
        {
            this.goPanel = layoutGroup.gameObject;
            this.layoutGroup = layoutGroup;
        }

        public static SPanel CreateFromPanel(LayoutGroup layoutGroup) { return new SPanel(layoutGroup); }

        public static SPanel CreateVertical(GameObject parent, UIInfo uiInfo = null, string goName = "")
        { return new SPanel(SWHelper.CreatePanel(uiInfo, layoutGroup: LayoutType.Vertical, parent: parent, goName: goName)); }

        public static SPanel CreateHorizontal(GameObject parent, UIInfo uiInfo = null)
        { return new SPanel(SWHelper.CreatePanel(uiInfo, layoutGroup: LayoutType.Horizontal, parent: parent)); }
        public static SPanel CreateGrid(GameObject parent, int cellWidthPx = -1, float aspectRatio = 1, UIInfo uiInfo = null)
        {
            if (uiInfo == null) uiInfo = new UIInfo();
            GridLayoutGroup lg = (GridLayoutGroup)SWHelper.CreatePanel(uiInfo.leFlexWeight(1, 0), layoutGroup: LayoutType.Grid, parent: parent);
            float width = DEFAULT_GRIDPANEL_CELLWIDTH;
            if (cellWidthPx >= 1) { width = cellWidthPx; }
            lg.cellSize = new Vector2(width, width * aspectRatio);
            return new SPanel(lg);
        }

        public SPanel addPanel_Vertical(UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = new UIInfo();
            return CreateVertical(goPanel, uiInfo, goName: goName);
        }

        public SPanel addPanel_Horizontal(UIInfo uiInfo = null) { return CreateHorizontal(goPanel, uiInfo); }

        public SPanel addPanel_Scroll(LayoutType layoutType = LayoutType.Vertical, UIInfo uiInfo = null, string goName = "")
        {
            return SPanel.CreateFromPanel(
               SWHelper.CreateScrollView(this.goPanel, contentPanelLayoutGroupType: layoutType, uiInfo: uiInfo, goName: goName));
        }

        /// <param name="cellWidthPx">GridのCellの横幅のピクセル数</param>
        /// <param name="aspectRatio">Cellのアスペクト比</param>
        public SPanel addPanel_Grid(int cellWidthPx = -1, float aspectRatio = 1, UIInfo uiInfo = null)
        { return CreateGrid(goPanel, cellWidthPx, aspectRatio, uiInfo); }

        public InfiniteScroll addInfiniteScroll(Func<GameObject, int, InfiniteScroll.Item> itemBuildFunc, float itemHeight, int initialItemCount)
        {
            GameObject.Destroy(this.goPanel.GetComponent<VerticalLayoutGroup>());
            var ret = this.goPanel.AddComponent<InfiniteScroll>();
            ret.initFields(this, itemBuildFunc, itemHeight, initialItemCount);
            return ret;
        }

        // **************************************** Add Components ****************************************
        public Text addText(string label, UIInfo uiInfo = null)
        { return SWHelper.CreateText(goPanel, label, uiInfo: uiInfo); }

        public Image addSpacer(int height = 15) { return SWHelper.CreateSpacer(goPanel, height); }

        public Image addHorizontalBar(int height = SWHelper.HR_HEIGHT, Color color = default(Color)) { return SWHelper.CreateHorizontalBar(goPanel, height, color); }

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

        /// <typeparam name="T">Type of `onValueChanged` argument. </typeparam>
        /// <param name="showValueDict">key : Display string of radio button. value : input for `onValueChanged`</param>
        /// <param name="layoutGroup">Layout of RadioButtons</param>
        public ToggleGroup addRadioButton<T>(UnityAction<T> onValueChanged, Dictionary<string, T> showValueDict, int initialSelected = 0,
            LayoutType layoutGroup = LayoutType.Horizontal)
        { return SWHelper.CreateRadioButton(goPanel, onValueChanged, showValueDict, initialSelected, layoutGroup); }

        public Slider addSlider(UnityAction<float> onValueChanged, float initialValue = 0.5f, float max = 1f, float min = 0f, bool wholeNumbers = false)
        { return SWHelper.CreateSlider(goPanel, onValueChanged, initialValue, max, min, wholeNumbers); }

        public Toggle addToggle(UnityAction<bool> onValueChanged, string labelStr, bool isOn = true)
        { return SWHelper.CreateToggle(goPanel, onValueChanged, labelStr, isOn: isOn); }

        public Image addImage(string path, Vector2 size = default(Vector2), UIInfo uiInfo=null)
        {
            if (uiInfo == null) uiInfo = UIInfo.IMAGE_DEFAULT;            
            if (size != default(Vector2)) uiInfo = uiInfo.lePreferredSize(size.x, size.y);
            return SWHelper.CreateImage(goPanel, path, width: (int)size.x, height: (int)size.y, uiInfo: uiInfo);
        }

        public Image addImage(Sprite sprite = null, Vector2 size = default(Vector2), Color color = default(Color))
        {
            UIInfo uiInfo = new UIInfo().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.lePreferredSize(size.x, size.y); // fit_Fixed(Vector2.zero, size);
            return SWHelper.CreateImage(goPanel, sprite, uiInfo: uiInfo);
        }

        public Image addImage_byTexture2D(Texture2D tex = null, Vector2 size = default(Vector2), Color color = default(Color))
        {
            UIInfo uiInfo = new UIInfo().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.lePreferredSize(size.x, size.y);
            return SWHelper.CreateImage(goPanel, tex, uiInfo: uiInfo);
        }

        public InputField addTextField(UnityAction<string> onEndEdit = null, UnityAction<string> onValueChanged = null,
            string initialText = "", UIInfo uiInfo = null, int areaLines = 1)
        { return SWHelper.CreateInputField(goPanel, onEndEdit, onValueChanged, initialText, areaLines, uiInfo); }

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
