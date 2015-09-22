using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// �������� �������
    /// </summary>
    public partial class ArrayEdit : UserControl, IMethodParameter
    {
		/// <summary>
		/// ������� �������������� ������� � �����
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		[Browsable(false)]
		public delegate string ObjectToText(object o);

		// ������� �������������� �������� � �����
		private ObjectToText itemToText;

        // �������� �������� ��������
        private Control valueEdit = null;
        // ��� ���������
        private Type elementType = null;
		// ��������
		private List<object> values = new List<object>();

        // ������������ ���������� ���������
        private int maxCount = 0;   //0 - �������������

		/// <summary>
		/// ������� �������������� �������� � �����
		/// </summary>
		[Browsable(false)]
		public ObjectToText ItemToText
		{
			get { return itemToText; }
			set { itemToText = value; }
		}

        /// <summary>
        /// ��� ���������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Type ElementType
        {
            get
            {
                return elementType;
            }
            set
            {
                if (elementType != value)
                {
                    elementType = value;

                    // �������� ����������� ��������� ��������
                    if (valueEdit != null)
                    {
                        panelValue.Controls.Remove(valueEdit);
                        valueEdit.Dispose();
                        valueEdit = null;
                    }

                    // ����� �������� ��������
                    if (value != null)
                    {
                        valueEdit = EditFactory.CreateEdit(value);

                        // ��������� ������ �������� ��� ��������
                        int minHeight = 0;
                        minHeight = valueEdit.MinimumSize.Height == 0 ? valueEdit.Size.Height : valueEdit.MinimumSize.Height;

                        int dy = minHeight - (panelValue.Size.Height - panelValue.Padding.Vertical);
                        if (dy > 0)
                        {
                            int splitDistance = splitContainer.SplitterDistance;
                            this.Height += dy;
                            splitContainer.Panel2MinSize += dy;
                            splitContainer2.Height += dy;
                            panelValue.Height += dy;
                            splitContainer.SplitterDistance = splitDistance;
                        }
                        valueEdit.Dock = DockStyle.Fill;
                        panelValue.Controls.Add(valueEdit);
                    }
                }
            }
        }

        /*
        /// <summary>
        /// �������� ��������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IParameter ElementParameter
        {
            get;
            set;
        }*/

        /// <summary>
        /// ��������
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object Value
        {
            get
            {
                // ������ ������
                if (values.Count == 0)
                    return null;

                // �������� ���� ���������
                if (elementType == null)
                    throw new InvalidOperationException("Element type is not specified");

                // ��������
                Array value = Array.CreateInstance(elementType, values.Count);
                for (int i = 0; i < value.Length; i++)
                {
                    value.SetValue(values[i], i);
                }
                return value;
            }
            set
            {
                IEnumerable array = value as IEnumerable;
                if (elementType == null)
                {
                    if (value != null)
                    {
                        Type type = value.GetType();
                        Type newElementType = type.GetElementType();
                        if (newElementType != null)
                        {
                            ElementType = newElementType;
                        }
                    }
                }
                listValues.BeginUpdate();
				clearInt();
                if (array != null)
                {
					foreach (object obj in array)
					{
						addValueInt(obj);
					}
                }
                listValues.EndUpdate();
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// �������� ��������
        /// </summary>
        public Control ValueEdit
        {
            get { return valueEdit; }
        }

        /// <summary>
        /// ����������� ������� ��������
        /// </summary>
        public bool IsNullable
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

		/// <summary>
		/// ������� ��������� ��������
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("�������, ����������� ��� ��������� ��������")]
		public event EventHandler ValueChanged;

        /// <summary>
        /// �����������
        /// </summary>
        public ArrayEdit()
        {
            InitializeComponent();
            bool enableButtons = listValues.SelectedIndex >= 0;
            enableListButtons(enableButtons);
        }

        /// <summary>
        /// �������� ������������� ��������
        /// </summary>
        /// <returns></returns>
        private object GetEditValue()
        {
            // ������������� ��������
            object value = null;
            if (valueEdit is IMethodParameter)
                value = ((IMethodParameter)valueEdit).Value;
            return value;
        }

        /// <summary>
        /// ��������� ������� ���������� �������� � ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValueAddClick(object sender, EventArgs e)
        {
            AddValue();
        }

        /// <summary>
        /// �������� ����� �������
        /// </summary>
        private void AddValue()
        {
            // ���������� ������
            object editingValue = GetEditValue();
            if (editingValue == null || string.IsNullOrEmpty(editingValue.ToString()))
            {
                // ������ ��������� ������ �������� � ������
                rejectEmptyValue();
            }
            else if (editingValue is IList)
            {
                IList list = (IList)editingValue;
                if (maxCount > 0 && listValues.Items.Count + list.Count > maxCount)
                {
                    MessageBox.Show("������� �������� ������� ����� ���������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
				for (int i = 0; i < list.Count; i++)
				{
					addValueInt(list[i]);
				}
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
            }
            else
            {
                if (maxCount > 0 && listValues.Items.Count >= maxCount)
                {
                    MessageBox.Show("���������� ������������ ���������� ���������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                addValueInt(editingValue);
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// ������ ��������� ������ ��������
        /// </summary>
        private static void rejectEmptyValue()
        {
            MessageBox.Show("������ ��������� ������ ��������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ��������� ������� �������� �������� �� ������
        private void OnValueDeleteClick(object sender, EventArgs e)
        {
            if (listValues.SelectedIndex >= 0)
            {
                int selectedIndex = listValues.SelectedIndex;
				removeValueAt(selectedIndex);
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
            }
        }

        // ��������� ������� ����������� �������� ���� �� ������
        private void OnValueDownClick(object sender, EventArgs e)
        {
            if (listValues.SelectedIndex >= 0 && listValues.SelectedIndex < listValues.Items.Count)
            {
                if (listValues.SelectedIndex == listValues.Items.Count - 1)
                {
                    // ��������� � ������
                    // ������ ������
					object tmp = values[listValues.SelectedIndex];
					values.RemoveAt(listValues.SelectedIndex);
					values.Insert(0, tmp);

                    object temp = listValues.Items[listValues.SelectedIndex];
                    listValues.Items.RemoveAt(listValues.SelectedIndex);
                    listValues.Items.Insert(0, temp);
                    listValues.SelectedIndex = 0;
                }
                else
                {
					object tmp = values[listValues.SelectedIndex + 1];
					values[listValues.SelectedIndex + 1] = values[listValues.SelectedIndex];
					values[listValues.SelectedIndex] = tmp;

                    object temp = listValues.Items[listValues.SelectedIndex + 1];
                    listValues.Items[listValues.SelectedIndex + 1] = listValues.Items[listValues.SelectedIndex];
                    listValues.Items[listValues.SelectedIndex] = temp;
                    listValues.SelectedIndex = listValues.SelectedIndex + 1;
                }
            }
        }

        // ��������� ������� ��������� �������� � ������
        private void OnValueEditClick(object sender, EventArgs e)
        {
            if (listValues.SelectedIndex >= 0)
            {
                object getEditValue = GetEditValue();
                if (getEditValue == null || string.IsNullOrEmpty(getEditValue.ToString()))
                {
                    // ������ ��������� ������ �������� � ������
                    rejectEmptyValue();
                }
                else if (getEditValue is IList)
                {
                    IList list = (IList)getEditValue;
					if (list.Count > 0)
					{
						values[listValues.SelectedIndex] = list[0];
						listValues.Items[listValues.SelectedIndex] = convertToString(list[0]);
					}
                }
                else
                {
					values[listValues.SelectedIndex] = getEditValue;
					listValues.Items[listValues.SelectedIndex] = convertToString(getEditValue);
                }
            }
        }

        // ��������� ������� ����������� �������� ����� �� ������
        private void OnValueUpClick(object sender, EventArgs e)
        {
            if (listValues.SelectedIndex >= 0 && listValues.SelectedIndex < listValues.Items.Count)
            {
                if (listValues.SelectedIndex == 0)
                {
                    // ������ � ������
                    // ������ ���������
					object tmp = values[0];
					values.RemoveAt(0);
					values.Add(tmp);
					
					object temp = listValues.Items[0];
                    listValues.Items.RemoveAt(0);
                    listValues.Items.Add(temp);
                    listValues.SelectedIndex = listValues.Items.Count - 1;
                }
                else
                {
					object tmp = values[listValues.SelectedIndex - 1];
					values[listValues.SelectedIndex - 1] = values[listValues.SelectedIndex];
					values[listValues.SelectedIndex] = tmp;

                    object temp = listValues.Items[listValues.SelectedIndex - 1];
                    listValues.Items[listValues.SelectedIndex - 1] = listValues.Items[listValues.SelectedIndex];
                    listValues.Items[listValues.SelectedIndex] = temp;
                    listValues.SelectedIndex = listValues.SelectedIndex - 1;
                }
            }
        }

        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArrayEdit_Leave(object sender, EventArgs e)
        {
            // listValues.SelectedIndex = -1;
        }

        /// <summary>
        /// ����� ������ ������ ���� ������� ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listValues.SelectedIndex >= 0)
            {
                enableListButtons(true);
                ((IMethodParameter)valueEdit).Value = values[listValues.SelectedIndex];
            }
            else
            {
                enableListButtons(false);
				try
				{
					((IMethodParameter)valueEdit).Value = null;
				}
				catch (ArgumentNullException)
				{
				}
            }
        }

        /// <summary>
        /// ��������� �������� enabled ��� ������ ������ �� ������
        /// </summary>
        /// <param name="enableButtons"></param>
        private void enableListButtons(bool enableButtons)
        {
            buttonUpValue.Enabled = enableButtons;
            buttonDownValue.Enabled = enableButtons;
            buttonEditValue.Enabled = enableButtons;
            buttonDeleteValue.Enabled = enableButtons;
        }

		private void addValueInt(object val)
		{
			values.Add(val);
			listValues.Items.Add(convertToString(val));
		}

		private void clearInt()
		{
			listValues.Items.Clear();
			values.Clear();
		}

		private void removeValueAt(int selectedIndex)
		{
			listValues.Items.RemoveAt(selectedIndex);
			values.RemoveAt(selectedIndex);
			if (listValues.Items.Count >= selectedIndex)
			{
				if (listValues.Items.Count == selectedIndex)
					selectedIndex--;
				listValues.SelectedIndex = selectedIndex;
			}
		}

		private string convertToString(object o)
		{
			if (itemToText != null)
				return itemToText(o);
			if (o == null)
				return string.Empty;
			/*if (o.GetType().IsEnum)
				return EnumType.ConvertToString(o);*/
			return o.ToString();
		}
    }
}
