using Aurum.Menu.Controllers;
using Aurum.Menu.Model;
using Aurum.Menu.Utils;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Aurum.Menu.Win
{
    public class TreeListMenusControl : XtraUserControl
    {
        private SingleChoiceAction action;
        private IContainer components;
        private TreeList treeList1;
        private TreeListColumn treeListColumn1;
        private ImageList imageList1;
        private BarManager barManager1;
        private Bar bar1;
        private BarDockControl barDockControlTop;
        private BarDockControl barDockControlBottom;
        private BarDockControl barDockControlLeft;
        private BarDockControl barDockControlRight;
        private BarButtonItem barButtonItemExpandAll;
        private BarButtonItem barButtonItemCollapseAll;
        private BarButtonItem barButtonItemRefresh;
        private TreeListColumn treeListColumn2;
        public TreeListMenusControl()
        {
            this.InitializeComponent();
            this.treeList1.Nodes.Clear();
        }
        public void InitializeItems(SingleChoiceAction action, IModelMenu startupMenuItem)
        {
            if (action == null || action.Items.Count <= 0)
            {
                return;
            }
            this.action = action;
            this.createMenus(startupMenuItem);
        }
        private void createMenus(IModelMenu startupMenuItem)
        {
            this.BeginUpdate();
            try
            {
                TreeListHelper.TreeListMenusItemsAdd(this.action, this.treeList1, this.imageList1);
                if (this.treeList1.FocusedNode == null)
                {
                    if (startupMenuItem != null && startupMenuItem.StartupItem != null)
                    {
                        ChoiceActionItem choiceActionItem = this.action.Items.RecursiveFind((ChoiceActionItem a) => a.Items, delegate(ChoiceActionItem a)
                        {
                            ViewMenuItem viewMenuItem = a as ViewMenuItem;
                            return viewMenuItem != null && viewMenuItem.Model == startupMenuItem.StartupItem;
                        });
                        if (choiceActionItem != null && (!choiceActionItem.Active || !choiceActionItem.Enabled))
                        {
                            choiceActionItem = this.action.Items.RecursiveFind((ChoiceActionItem a) => a.Items, (ChoiceActionItem a) => a.Active && a.Enabled && a is ViewMenuItem);
                        }
                        if (choiceActionItem != null)
                        {
                            string itemPath = choiceActionItem.GetItemPath();
                            TreeListNode treeListNode = this.treeList1.FindNodeByFieldValue("ItemPath", itemPath);
                            if (treeListNode != null)
                            {
                                this.treeList1.SetFocusedNode(treeListNode);
                            }
                        }
                        else
                        {
                            this.treeList1.SetFocusedNode(this.treeList1.Nodes.FirstNode);
                        }
                    }
                    else
                    {
                        this.treeList1.SetFocusedNode(this.treeList1.Nodes.FirstNode);
                    }
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }
        public void BeginUpdate()
        {
            this.treeList1.BeginUpdate();
        }
        public void EndUpdate()
        {
            this.treeList1.EndUpdate();
        }
        protected virtual bool IsConfirmed(ChoiceActionItem item)
        {
            if (!(item is ActionMenuItem))
            {
                return true;
            }
            bool result = true;
            ActionBase action = (item as ActionMenuItem).Action;
            if (action == null)
            {
                return true;
            }
            string formattedConfirmationMessage = action.GetFormattedConfirmationMessage();
            if (!string.IsNullOrEmpty(formattedConfirmationMessage))
            {
                result = (WinApplication.Messaging.GetUserChoice(formattedConfirmationMessage, action.Caption, MessageBoxButtons.YesNo) == DialogResult.Yes);
            }
            return result;
        }
        public void Execute(TreeListNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node.Nodes.Count > 0)
            {
                return;
            }
            ChoiceActionItem choiceActionItem = node.Tag as ChoiceActionItem;
            if (choiceActionItem == null)
            {
                return;
            }
            if (!this.IsConfirmed(choiceActionItem))
            {
                return;
            }

            // workaround for xpand master-detail
            // master-detail подписывается на событие Executing ВСЕХ action главного фрейма и отменяет их, когда в интерфейсе выбран detail
            // убираем эту подписку с нашего action
            Aurum.Menu.Utils.EventHelper.RemoveEventHandler(this.action, "Executing", d => d.Method != null && d.Method.Name != null && d.Method.Name.Contains("PushExecutionToNestedFrame"));

            this.action.DoExecute(choiceActionItem);
        }
        private void barButtonItemExpandAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.treeList1.ExpandAll();
        }
        private void barButtonItemCollapseAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.treeList1.CollapseAll();
        }
        private void barButtonItemRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.action != null && this.action.Controller != null)
            {
                ((MenuController)this.action.Controller).RefreshItems();
            }
        }
        private void treeList1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                this.Execute(this.treeList1.Selection[0]);
            }
        }
        private void MenuItemsActionContainer_Enter(object sender, EventArgs e)
        {
            this.treeList1.Focus();
        }
        private void treeList1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Point pt = new Point(e.X, e.Y);
            TreeListHitInfo treeListHitInfo = this.treeList1.CalcHitInfo(pt);
            if (treeListHitInfo.Node == null)
            {
                return;
            }
            if (treeListHitInfo.Node.ParentNode == null)
            {
                return;
            }
            this.Execute(treeListHitInfo.Node);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new Container();
            this.treeList1 = new TreeList();
            this.treeListColumn1 = new TreeListColumn();
            this.treeListColumn2 = new TreeListColumn();
            this.imageList1 = new ImageList(this.components);
            this.barDockControlTop = new BarDockControl();
            this.barDockControlBottom = new BarDockControl();
            this.barDockControlLeft = new BarDockControl();
            this.barDockControlRight = new BarDockControl();
            this.barManager1 = new BarManager(this.components);
            this.bar1 = new Bar();
            this.barButtonItemExpandAll = new BarButtonItem();
            this.barButtonItemCollapseAll = new BarButtonItem();
            this.barButtonItemRefresh = new BarButtonItem();
            ((ISupportInitialize)this.treeList1).BeginInit();
            ((ISupportInitialize)this.barManager1).BeginInit();
            base.SuspendLayout();
            this.treeList1.Appearance.FocusedCell.BackColor = SystemColors.Highlight;
            this.treeList1.Appearance.FocusedCell.ForeColor = SystemColors.HighlightText;
            this.treeList1.Appearance.FocusedCell.Options.UseBackColor = true;
            this.treeList1.Appearance.FocusedCell.Options.UseForeColor = true;
            this.treeList1.Columns.AddRange(new TreeListColumn[]
			{
				this.treeListColumn1,
				this.treeListColumn2
			});
            this.treeList1.Parent = this;
            this.treeList1.Location = new Point(0, 31);
            this.treeList1.Size = new Size(326, 394);
            this.treeList1.Dock = DockStyle.Fill;
            this.treeList1.TabIndex = 0;
            this.treeList1.Name = "treeList1";
            this.treeList1.BeginUnboundLoad();
            object[] array = new object[2];
            array[0] = "root";
            this.treeList1.AppendNode(array, -1);
            object[] array2 = new object[2];
            array2[0] = "t1";
            this.treeList1.AppendNode(array2, 0, 1, 1, -1);
            object[] array3 = new object[2];
            array3[0] = "t11";
            this.treeList1.AppendNode(array3, 1, 2, 2, -1);
            this.treeList1.EndUnboundLoad();
            this.treeList1.OptionsBehavior.AllowIncrementalSearch = true;
            this.treeList1.OptionsBehavior.AutoPopulateColumns = false;
            this.treeList1.OptionsBehavior.Editable = false;
            this.treeList1.OptionsBehavior.ExpandNodesOnIncrementalSearch = true;
            this.treeList1.OptionsLayout.StoreAppearance = true;
            this.treeList1.OptionsMenu.EnableColumnMenu = false;
            this.treeList1.OptionsMenu.EnableFooterMenu = false;
            this.treeList1.OptionsPrint.UsePrintStyles = true;
            this.treeList1.OptionsView.ShowHorzLines = false;
            this.treeList1.OptionsView.ShowIndicator = false;
            this.treeList1.OptionsView.ShowVertLines = false;
            this.treeList1.OptionsView.ShowColumns = false;
            this.treeList1.SelectImageList = this.imageList1;
            this.treeList1.KeyDown += new KeyEventHandler(this.treeList1_KeyDown);
            this.treeList1.MouseDoubleClick += new MouseEventHandler(this.treeList1_MouseDoubleClick);
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.MinWidth = 87;
            this.treeListColumn1.Width = 81;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.Caption = "Name";
            this.treeListColumn1.FieldName = "Name";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn2.Caption = "ItemPath";
            this.treeListColumn2.FieldName = "ItemPath";
            this.treeListColumn2.Name = "treeListColumn2";
            /*this.imageList1.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageList1.ImageStream");
            this.imageList1.TransparentColor = Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "computer.ico");
            this.imageList1.Images.SetKeyName(1, "folder.ico");
            this.imageList1.Images.SetKeyName(2, "window.ico");
            this.imageList1.Images.SetKeyName(3, "tree_open.ico");
            this.imageList1.Images.SetKeyName(4, "tree_close.ico");
            this.imageList1.Images.SetKeyName(5, "DevExpress.ExpressApp.Images.Images.MenuBar_Refresh.png");
            this.imageList1.Images.SetKeyName(6, "MenuBar_Refresh.ico");*/
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Parent = this;
            this.barDockControlTop.Location = new Point(0, 0);
            this.barDockControlTop.Dock = DockStyle.Top;
            this.barDockControlTop.Size = new Size(326, 31);
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Parent = this;
            this.barDockControlBottom.Location = new Point(0, 425);
            this.barDockControlBottom.Dock = DockStyle.Bottom;
            this.barDockControlBottom.Size = new Size(326, 0);
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Parent = this;
            this.barDockControlLeft.Location = new Point(0, 31);
            this.barDockControlLeft.Dock = DockStyle.Left;
            this.barDockControlLeft.Size = new Size(0, 394);
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Parent = this;
            this.barDockControlRight.Location = new Point(326, 31);
            this.barDockControlRight.Dock = DockStyle.Right;
            this.barDockControlRight.Size = new Size(0, 394);
            this.barManager1.Bars.AddRange(new Bar[]
			{
				this.bar1
			});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            // this.barManager1.Images = this.imageList1;
            this.barManager1.Items.AddRange(new BarItem[]
			{
				this.barButtonItemExpandAll,
				this.barButtonItemCollapseAll,
				this.barButtonItemRefresh
			});
            this.barManager1.MaxItemId = 3;
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new LinkPersistInfo[]
			{
				new LinkPersistInfo(this.barButtonItemExpandAll),
				new LinkPersistInfo(this.barButtonItemCollapseAll),
				new LinkPersistInfo(this.barButtonItemRefresh, true)
			});
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DisableClose = true;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Visible = false;
            this.bar1.Text = "Tools";
            this.barButtonItemExpandAll.Caption = "Expand All";
            this.barButtonItemExpandAll.Id = 0;
            this.barButtonItemExpandAll.Name = "barButtonItemExpandAll";
            this.barButtonItemExpandAll.ItemClick += new ItemClickEventHandler(this.barButtonItemExpandAll_ItemClick);
            this.barButtonItemCollapseAll.Caption = "Collapse All";
            this.barButtonItemCollapseAll.Id = 1;
            this.barButtonItemCollapseAll.Name = "barButtonItemCollapseAll";
            this.barButtonItemCollapseAll.ItemClick += new ItemClickEventHandler(this.barButtonItemCollapseAll_ItemClick);
            this.barButtonItemRefresh.Caption = "Refresh";
            this.barButtonItemRefresh.Id = 2;
            this.barButtonItemRefresh.Name = "barButtonItemRefresh";
            this.barButtonItemRefresh.ItemClick += new ItemClickEventHandler(this.barButtonItemRefresh_ItemClick);
            this.AutoScaleDimensions = new SizeF(6, 13);
            this.Size = new Size(326, 425);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(this.treeList1);
            base.Controls.Add(this.barDockControlLeft);
            base.Controls.Add(this.barDockControlRight);
            base.Controls.Add(this.barDockControlBottom);
            base.Controls.Add(this.barDockControlTop);
            base.Name = "TreeListMenusControl";
            base.Enter += new EventHandler(this.MenuItemsActionContainer_Enter);
            ((ISupportInitialize)this.treeList1).EndInit();
            ((ISupportInitialize)this.barManager1).EndInit();
            base.ResumeLayout(false);
        }
    }

    internal static class TreeListHelper
    {
        private static TreeList _treeList;
        private static ImageList _imageList;
        private static SingleChoiceAction _action;
        public static void TreeListMenusItemsAdd(SingleChoiceAction action, TreeList treeList, ImageList imageList)
        {
            TreeListHelper._treeList = treeList;
            TreeListHelper._action = action;
            TreeListHelper._imageList = imageList;
            TreeListHelper.InitializeImage();
            TreeListHelper.CreateMenus();
        }
        private static void InitializeImage()
        {
            TreeListHelper._imageList.Images.Clear();
            TreeListHelper.AddImage("ModelEditor_Default"); // default
            TreeListHelper.AddImage("ModelEditor_Business_Object_Model");   // menu
            TreeListHelper.AddImage("BO_Folder");       // folder
            TreeListHelper.AddImage("BO_List");       // view
            TreeListHelper.AddImage("Action_SimpleAction");       // action
        }
        private static void CreateMenus()
        {
            object obj = null;
            if (TreeListHelper._treeList.FocusedNode != null)
            {
                obj = TreeListHelper._treeList.FocusedNode.GetValue(TreeListHelper._treeList.Columns[1].FieldName);
            }
            List<object> list = new List<object>();
            TreeListHelper.TreeListExpanded(TreeListHelper._treeList.Nodes, list);
            TreeListHelper._treeList.ClearNodes();
            foreach (ChoiceActionItem current in TreeListHelper._action.Items)
            {
                TreeListNode treeListNode = TreeListHelper._treeList.AppendNode(new object[]
				{
					current.Caption,
					current.GetItemPath()
				}, null);
                treeListNode.Tag = current;
                if (!TreeListHelper.SetTreeListNodeImage(treeListNode, current.ImageName))
                {
                    TreeListHelper.SetTreeListNodeImage(treeListNode, "ModelEditor_Business_Object_Model");
                }
                TreeListHelper.CreateMenu(treeListNode, current);
                TreeListHelper._treeList.FocusedNode = null;
            }
            foreach (object current2 in list)
            {
                TreeListNode treeListNode2;
                if ((treeListNode2 = TreeListHelper._treeList.FindNodeByFieldValue(TreeListHelper._treeList.Columns[1].FieldName, current2)) != null)
                {
                    treeListNode2.Expanded = true;
                }
            }
            if (obj != null)
            {
                TreeListNode treeListNode3 = TreeListHelper._treeList.FindNodeByFieldValue(TreeListHelper._treeList.Columns[1].FieldName, obj);
                if (treeListNode3 != null)
                {
                    TreeListHelper._treeList.SetFocusedNode(treeListNode3);
                }
            }
        }
        private static void CreateMenu(TreeListNode parentNode, ChoiceActionItem parentItem)
        {
            if (parentItem == null || parentItem.Items.Count <= 0)
            {
                return;
            }
            foreach (ChoiceActionItem current in
                from a in parentItem.Items
                where a.Active
                select a)
            {
                if (!current.Enabled)
                {
                    ChoiceActionItem item1 = current;
                    using (IEnumerator<string> enumerator2 = current.Enabled.GetKeys().Where((string key) => !item1.Enabled[key]).GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            string current2 = enumerator2.Current;
                        }
                        continue;
                    }
                }
                TreeListNode treeListNode = TreeListHelper._treeList.AppendNode(new object[]
				{
					current.Caption,
					current.GetItemPath()
				}, parentNode);
                treeListNode.Tag = current;

                if (!TreeListHelper.SetTreeListNodeImage(treeListNode, current.ImageName))
                {
                    if (current is MenuFolderItem)
                    {
                        TreeListHelper.SetTreeListNodeImage(treeListNode, "BO_Folder");
                    }
                    else if (current is ActionMenuItem)
                    {
                        TreeListHelper.SetTreeListNodeImage(treeListNode, "Action_SimpleAction");
                    }
                    else if (current is ViewMenuItem)
                    {
                        TreeListHelper.SetTreeListNodeImage(treeListNode, "BO_List");
                    }
                    else
                    {
                        TreeListHelper.SetTreeListNodeImage(treeListNode, "ModelEditor_Default");
                    }
                }
                TreeListHelper.CreateMenu(treeListNode, current);
            }
        }
        public static void UpdateMenuTreeList(TreeList treeList, ChoiceActionItem parentItem)
        {
            if (treeList == null)
            {
                throw new ArgumentNullException("treeList");
            }
            object obj = null;
            if (treeList.FocusedNode != null)
            {
                obj = treeList.FocusedNode.GetValue(TreeListHelper._treeList.Columns[1].FieldName);
            }
            TreeListHelper._treeList = treeList;
            TreeListHelper._imageList = (ImageList)treeList.SelectImageList;
            List<object> list = new List<object>();
            TreeListHelper.TreeListExpanded(TreeListHelper._treeList.Nodes, list);
            treeList.ClearNodes();
            TreeListHelper.CreateMenu(null, parentItem);
            foreach (object current in list)
            {
                TreeListNode treeListNode;
                if ((treeListNode = TreeListHelper._treeList.FindNodeByFieldValue(TreeListHelper._treeList.Columns[1].FieldName, current)) != null)
                {
                    treeListNode.Expanded = true;
                }
            }
            if (obj != null)
            {
                TreeListNode treeListNode2 = treeList.FindNodeByFieldValue(TreeListHelper._treeList.Columns[1].FieldName, obj);
                if (treeListNode2 != null)
                {
                    treeList.SetFocusedNode(treeListNode2);
                }
            }
        }
        private static void TreeListExpanded(TreeListNodes treeListNodes, ICollection<object> nodesPath)
        {
            if (treeListNodes == null)
            {
                return;
            }
            if (treeListNodes.Count > 0)
            {
                foreach (TreeListNode treeListNode in treeListNodes)
                {
                    if (treeListNode.Expanded)
                    {
                        nodesPath.Add(treeListNode.GetValue(TreeListHelper._treeList.Columns[1].FieldName));
                        TreeListHelper.TreeListExpanded(treeListNode.Nodes, nodesPath);
                    }
                }
            }
        }
        private static bool SetTreeListNodeImage(TreeListNode node, string imageName)
        {
            if (string.IsNullOrEmpty(imageName) || node == null)
            {
                return false;
            }
            int num = TreeListHelper.AddImage(imageName);
            if (num < 0)
            {
                return false;
            }
            node.SelectImageIndex = num;
            node.ImageIndex = num;
            node.StateImageIndex = num;
            return true;
        }
        private static int AddImage(string imageName)
        {
            if (!TreeListHelper._imageList.Images.ContainsKey(imageName))
            {
                Image image = ImageLoader.Instance.GetImageInfo(imageName).Image;
                if (image != null)
                {
                    TreeListHelper._imageList.Images.Add(imageName, image);
                }
            }
            return TreeListHelper._imageList.Images.IndexOfKey(imageName);
        }
    }
}
