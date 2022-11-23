using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Reflection;

namespace uBUI
{

    /// <summary>uGUI Builder Panel</summary>
    public class BPanel
    {
        const int DEFAULT_GRIDPANEL_CELLWIDTH = 100;
        public readonly GameObject gameObject;
        public readonly LayoutGroup layoutGroup;

        public Vector2 Size { get { return gameObject.GetComponent<RectTransform>().rect.size; } }

        protected BPanel(LayoutGroup layoutGroup)
        {
            this.gameObject = layoutGroup.gameObject;
            this.layoutGroup = layoutGroup;
        }

        // **************************************** Create Panel Methods ****************************************
        public static BPanel CreateFromPanel(LayoutGroup layoutGroup) { return new BPanel(layoutGroup); }

        public static BPanel CreateVertical(GameObject parent, UIInfo uiInfo = null, string goName = "")
        { return new BPanel(BHelper.CreatePanel(uiInfo, layoutGroup: LayoutType.Vertical, parent: parent, goName: goName)); }

        public static BPanel CreateHorizontal(GameObject parent, UIInfo uiInfo = null)
        { return new BPanel(BHelper.CreatePanel(uiInfo, layoutGroup: LayoutType.Horizontal, parent: parent)); }
        public static BPanel CreateGrid(GameObject parent, int cellWidthPx = -1, float aspectRatio = 1, UIInfo uiInfo = null)
        {
            if (uiInfo == null) uiInfo = new UIInfo();
            GridLayoutGroup lg = (GridLayoutGroup)BHelper.CreatePanel(uiInfo.leFlexWeight(1, 0), layoutGroup: LayoutType.Grid, parent: parent);
            float width = DEFAULT_GRIDPANEL_CELLWIDTH;
            if (cellWidthPx >= 1) { width = cellWidthPx; }
            lg.cellSize = new Vector2(width, width * aspectRatio);
            return new BPanel(lg);
        }

        // **************************************** Add Panel Methods ****************************************
        public BPanel AddVerticalPanel(UIInfo uiInfo = null, string goName = "")
        {
            if (uiInfo == null) uiInfo = new UIInfo();
            return CreateVertical(gameObject, uiInfo, goName: goName);
        }

        public BPanel AddHorizontalPanel(UIInfo uiInfo = null) { return CreateHorizontal(gameObject, uiInfo); }

        public BPanel AddScrollPanel(LayoutType layoutType = LayoutType.Vertical, UIInfo uiInfo = null, string goName = "")
        {
            return BPanel.CreateFromPanel(
               BHelper.CreateScrollView(this.gameObject, contentPanelLayoutGroupType: layoutType, uiInfo: uiInfo, goName: goName));
        }

        /// <param name="cellWidthPx">GridのCellの横幅のピクセル数</param>
        /// <param name="aspectRatio">Cellのアスペクト比</param>
        public BPanel AddGridPanel(int cellWidthPx = -1, float aspectRatio = 1, UIInfo uiInfo = null)
        { return CreateGrid(gameObject, cellWidthPx, aspectRatio, uiInfo); }

        public InfiniteScroll AddInfiniteScrollPanel(Func<GameObject, int, InfiniteScroll.Item> itemBuildFunc, float itemHeight, int initialItemCount)
        {
            GameObject.Destroy(this.gameObject.GetComponent<VerticalLayoutGroup>());
            var ret = this.gameObject.AddComponent<InfiniteScroll>();
            ret.initFields(this, itemBuildFunc, itemHeight, initialItemCount);
            return ret;
        }

        // **************************************** Add Component Methods ****************************************
        public Text AddText(string label, UIInfo uiInfo = null)
        { return BHelper.CreateText(gameObject, label, uiInfo: uiInfo); }

        public Image AddSpacer(int height = 15) { return BHelper.CreateSpacer(gameObject, height); }

        public Image AddHorizontalBar(int height = BHelper.HR_HEIGHT, Color color = default(Color)) { return BHelper.CreateHorizontalBar(gameObject, height, color); }

        public Button AddButton(UnityAction TaskOnClick, string labelStr = "", string texPath = "",
            Texture2D tex = null, Sprite sprite = null, int imgSize = -1, Color? imgColor = null, UIInfo uiInfo = null)
        {
            // Spriteからボタンを作る場合
            if (sprite != null) return BHelper.CreateButton(gameObject, TaskOnClick: TaskOnClick, labelStr: labelStr, sprite: sprite, imgSize: imgSize, imgColor: imgColor, uiInfo: uiInfo);
            // labelStrとTexture2Dオブジェクトからボタンを作る場合
            if (texPath == "") return BHelper.CreateButton(gameObject, TaskOnClick: TaskOnClick, labelStr: labelStr, tex: tex, imgSize: imgSize, imgColor: imgColor, uiInfo: uiInfo);
            // texPathからTexture2Dを読込んでボタンを作る場合
            else return BHelper.CreateButton(gameObject, texPath: texPath, TaskOnClick: TaskOnClick, labelStr: labelStr, imgSize: imgSize, imgColor: imgColor, uiInfo: uiInfo);
        }

        /// <typeparam name="T">Type of `onValueChanged` argument. </typeparam>
        /// <param name="showValueDict">key : Display string of radio button. value : input for `onValueChanged`</param>
        /// <param name="layoutGroup">Layout of RadioButtons</param>
        public ToggleGroup AddRadioButton<T>(UnityAction<T> onValueChanged, Dictionary<string, T> showValueDict, int initialSelected = 0,
            LayoutType layoutGroup = LayoutType.Horizontal)
        { return BHelper.CreateRadioButton(gameObject, onValueChanged, showValueDict, initialSelected, layoutGroup); }

        public Slider AddSlider(UnityAction<float> onValueChanged, float initialValue = 0.5f, float max = 1f, float min = 0f, bool wholeNumbers = false)
        { return BHelper.CreateSlider(gameObject, onValueChanged, initialValue, max, min, wholeNumbers); }

        public Toggle AddToggle(UnityAction<bool> onValueChanged, string labelStr, bool isOn = true)
        { return BHelper.CreateToggle(gameObject, onValueChanged, labelStr, isOn: isOn); }

        public Image AddImage(string path, Vector2 size = default(Vector2), UIInfo uiInfo=null)
        {
            if (uiInfo == null) uiInfo = UIInfo.IMAGE_DEFAULT;            
            if (size != default(Vector2)) uiInfo = uiInfo.lePreferredSize(size.x, size.y);
            return BHelper.CreateImage(gameObject, path, width: (int)size.x, height: (int)size.y, uiInfo: uiInfo);
        }

        public Image addImage(Sprite sprite = null, Vector2 size = default(Vector2), Color color = default(Color))
        {
            UIInfo uiInfo = new UIInfo().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.lePreferredSize(size.x, size.y); // fit_Fixed(Vector2.zero, size);
            return BHelper.CreateImage(gameObject, sprite, uiInfo: uiInfo);
        }

        public Image addImage_byTexture2D(Texture2D tex = null, Vector2 size = default(Vector2), Color color = default(Color))
        {
            UIInfo uiInfo = new UIInfo().bgColor(color);
            if (size != default(Vector2)) uiInfo = uiInfo.lePreferredSize(size.x, size.y);
            return BHelper.CreateImage(gameObject, tex, uiInfo: uiInfo);
        }

        public InputField AddTextField(UnityAction<string> onEndEdit = null, UnityAction<string> onValueChanged = null,
            string initialText = "", UIInfo uiInfo = null, int areaLines = 1)
        { return BHelper.CreateInputField(gameObject, onEndEdit, onValueChanged, initialText, areaLines, uiInfo); }

        public void Clear(bool detatch = false)
        {
            if (!detatch)
            {
                foreach (Transform child in this.gameObject.gameObject.transform)
                    GameObject.Destroy(child.gameObject);
            }
            else
            {
                this.gameObject.gameObject.transform.DetachChildren();
            }
        }

    }

}
