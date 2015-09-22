using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Layout;
using System.Windows.Forms;
using System.Drawing;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ������������ ���������� ���������
    /// </summary>
    public class VerticalLayout : LayoutEngine
    {
        private bool useTabIndex = false;

        /// <summary>
        /// ����������� ������� ���������� �������� ���������
        /// </summary>
        /// <param name="b">true - ������� �������� ��������� �� TabIndex,
        /// false - ������� �������� ��������� �� ������� �� ����������</param>
        public void UseTabIndex(bool b)
        {
            useTabIndex = b;
        }

        /// <summary>
        /// �����, ���������� ��� ������������ �������� ���������
        /// </summary>
        /// <param name="container"></param>
        /// <param name="layoutEventArgs"></param>
        /// <returns></returns>
        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            // �������, ����������� ������� � scroll area ������������� ��������
            Rectangle parentDisplayRectangle = parent.DisplayRectangle;
            // ���������� �������
            Size clientSize = parent.ClientSize;
            // ������� ������� �������
            Rectangle realParentRectangle = new Rectangle(parentDisplayRectangle.X,
                parentDisplayRectangle.Y,
                clientSize.Width, clientSize.Height);
            
            // ������ �������� ���������
            List<Control> list = new List<Control>();
            foreach (Control c in parent.Controls)
            {
                if (!c.Visible)
                {
                    continue;
                }
                list.Add(c);
            }

            // ���������� ��������� �� TabIndex'�, ����� �������� ����� ����������� � ������� �� ���������� �� ������������ �������
            if (useTabIndex)
            {
                list.Sort(delegate(Control c1, Control c2)
                {
                    return c1.TabIndex.CompareTo(c2.TabIndex);
                });
            }

            // ���������� �� ���������
            int layoutMinHeight = 0;    // ����� ����������� ����� ���� ���������
            int heightFreeSpace;        // ������, �� ������� ����� ��������� ��������������� ��������
            int resizeableControls = 0; // ���������� ��������������� ���������

            // ����������� � ������������ ������� ��������
            Rectangle proposal = new Rectangle(0, 0, realParentRectangle.Width, realParentRectangle.Height);

            // ���� ����������
            foreach (Control c in list)
            {
                if (c is IDynamicLayoutControl)
                {
                    Rectangle check = ((IDynamicLayoutControl)c).GetSizeRange(proposal);
                    layoutMinHeight += check.Y + c.Margin.Top + c.Margin.Bottom;
                    if ((((IDynamicLayoutControl)c).GetDirection() & ChangeSizeDirection.Vertical) == ChangeSizeDirection.Vertical)
                    {
                        // ������� ����� ���������� �� ������
                        resizeableControls++;
                    }
                }
                else
                {
                    // ����������� �������
                    layoutMinHeight += c.Size.Height + c.Margin.Top + c.Margin.Bottom;
                }
            }
            heightFreeSpace = Math.Max(realParentRectangle.Height - layoutMinHeight, 0);

            // ��������� ���������� ��������
            Point nextControlLocation = realParentRectangle.Location;

            // ������������ ���������
            foreach (Control c in list)
            {
                // ���� ������ ��������
                nextControlLocation.Offset(c.Margin.Left, c.Margin.Top);

                // ������������ �������� �� �����
                c.Location = nextControlLocation;

                // ������������ �� ��� ������
                c.Width = realParentRectangle.Width - (c.Margin.Left + c.Margin.Right);

                if (c is IDynamicLayoutControl) // �������, ����������� ���� ���������, �������� ����� �������� ������� �� ���������
                {
                    if ((((IDynamicLayoutControl)c).GetDirection() & ChangeSizeDirection.Vertical) == ChangeSizeDirection.Vertical)
                    {
                        // ��������� ������������ � ������������� ������� ��������
                        Rectangle check = ((IDynamicLayoutControl)c).GetSizeRange(proposal);

                        // ������� ����� ���������� � ��������
                        // ����� ������ ��� ��������
                        int newHeight = check.Y + heightFreeSpace / resizeableControls;

                        // �������� ����������� �� ����������� � ������������ ������
                        if (check.Height != 0)
                        {
                            newHeight = Math.Min(newHeight, check.Height);
                        }
                        if (check.Y != 0)
                        {
                            newHeight = Math.Max(newHeight, check.Y);
                        }

                        c.Height = newHeight;

                        heightFreeSpace -= c.Height - check.Y;
                        resizeableControls--;
                    }
                }

                nextControlLocation.X = realParentRectangle.X;
                nextControlLocation.Y += c.Height + c.Margin.Bottom;
            }

            // true, ���� ������������ ������� ������ ������� layout
            return false;
        }
    }
}
