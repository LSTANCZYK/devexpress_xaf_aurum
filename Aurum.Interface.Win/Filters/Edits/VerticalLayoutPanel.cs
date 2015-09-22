using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ѕанель с VerticalLayout
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
        /// ќпределение пор€дка следовани€ дочерних контролов
        /// </summary>
        /// <param name="b">true - пор€док дочерних контролов по TabIndex,
        /// false - пор€док дочерних контролов по пор€дку их добавлени€</param>
        public void UseTabIndex(bool b)
        {
            engine.UseTabIndex(b);
        }

        /// <summary>
        ///  онструктор
        /// </summary>
        public VerticalLayoutPanel()
            : base()
        {
            SetScrollState(ScrollStateHScrollVisible, false);
            DoubleBuffered = true;
        }
    }
}
