using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ������ � VerticalLayout
    /// </summary>
    public class VerticalLayoutPanel : Panel
    {
        private VerticalLayout engine = null;

        /// <summary>
        /// LayoutEngine
        /// </summary>
        public override LayoutEngine LayoutEngine
        {
            get
            {
                if (engine == null)
                {
                    engine = new VerticalLayout();
                }
                return engine;
            }
        }

        /// <summary>
        /// ����������� ������� ���������� �������� ���������
        /// </summary>
        /// <param name="b">true - ������� �������� ��������� �� TabIndex,
        /// false - ������� �������� ��������� �� ������� �� ����������</param>
        public void UseTabIndex(bool b)
        {
            engine.UseTabIndex(b);
        }

        /// <summary>
        /// �����������
        /// </summary>
        public VerticalLayoutPanel()
            : base()
        {
            SetScrollState(ScrollStateHScrollVisible, false);
            DoubleBuffered = true;
        }
    }
}
