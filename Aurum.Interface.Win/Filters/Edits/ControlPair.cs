using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Пара контролов, связанных в один
    /// </summary>
    /// <ToDo priority="middle">AutoSize. Необходимо, чтобы контрол устанавливал свой размер согласно дочерним контролам,
    /// но не через OnLayout, так как это приводит к повторному вызову и неадекватному изменению размеров
    /// дочерних контролов.</ToDo>
    internal class ControlPair : Control, IDynamicLayoutControl
    {
        // Первый контрол
        private Control control1 = null;
        // Второй контрол
        private Control control2 = null;
        // Флаг источника события изменения контролов
        private bool externalChange = true;

        /// <summary>
        /// Первый контрол
        /// </summary>
        /// <exception cref="ArgumentException">Значение не должно равняться второму контролу</exception>
        [Browsable(true), Category("Appearance"), Description("Первый контрол")]
        [DefaultValue(null)]
        public Control Control1
        {
            get
            {
                return control1;
            }
            set
            {
                if (value != null && value == control2)
                    throw new ArgumentException("Value must be different of control2");
                if (control1 != value)
                {
                    externalChange = false;
                    if (control1 != null)
                        Controls.Remove(control1);
                    control1 = value;
                    if (control1 != null)
                        Controls.Add(control1);
                    externalChange = true;
                    this.Height = Math.Max(this.Height, control1.Height + this.Margin.Vertical);
                }
            }
        }

        /// <summary>
        /// Второй контрол
        /// </summary>
        /// <exception cref="ArgumentException">Значение не должно равняться первому контролу</exception>
        [Browsable(true), Category("Appearance"), Description("Второй контрол")]
        [DefaultValue(null)]
        public Control Control2
        {
            get
            {
                return control2;
            }
            set
            {
                if (value != null && value == control1)
                    throw new ArgumentException("Value must be different of control1");
                if (control2 != value)
                {
                    externalChange = false;
                    if (control2 != null)
                        Controls.Remove(control2);
                    control2 = value;
                    if (control2 != null)
                        Controls.Add(control2);
                    externalChange = true;
                    this.Height = Math.Max(this.Height, control2.Height + this.Margin.Vertical);
                }
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ControlPair()
        {
            // Контрол не имеет фокуса
            SetStyle(ControlStyles.Selectable, false);
        }

        /// <summary>
        /// Направления, в которых может растягиваться контрол
        /// </summary>
        /// <returns></returns>
        public ChangeSizeDirection GetDirection()
        {
            if (control1 is IDynamicLayoutControl)
            {
                if (control2 is IDynamicLayoutControl)
                {
                    return ((IDynamicLayoutControl)control1).GetDirection() | ((IDynamicLayoutControl)control2).GetDirection();
                }
                else
                {
                    return ((IDynamicLayoutControl)control1).GetDirection();
                }
            }
            else if (control2 is IDynamicLayoutControl)
            {
                return ((IDynamicLayoutControl)control2).GetDirection();
            }
            else
            {
                return ChangeSizeDirection.None;
            }
        }

        /// <summary>
        /// Получить минимальный и максимальный размер контрола в зависимости от предложенной области
        /// </summary>
        /// <param name="proposal">Предложенная область</param>
        /// <returns>Минимальный и максимальный размер контрола</returns>
        public Rectangle GetSizeRange(Rectangle proposal)
        {
            Rectangle r = new Rectangle();
            r.X = MinimumSize.Width == 0 ? proposal.X : Math.Max(MinimumSize.Width, proposal.X);
            r.Y = MinimumSize.Height == 0 ? proposal.Y : Math.Max(MinimumSize.Height, proposal.Y);
            r.Width = MaximumSize.Width == 0 ? proposal.Width : Math.Min(MaximumSize.Width, proposal.Width);
            r.Height = MaximumSize.Height == 0 ? proposal.Height : Math.Min(MaximumSize.Height, proposal.Height);

            List<Control> list = new List<Control>();
            list.Add(control1);
            list.Add(control2);
            foreach (Control c in list)
            {
                if (c is IDynamicLayoutControl)
                {
                    Rectangle check = ((IDynamicLayoutControl)c).GetSizeRange(proposal);
                    if (check.Y != 0)
                        r.Y = Math.Max(check.Y, c.MinimumSize.Height);
                    else r.Y = Math.Max(r.Y, c.Size.Height);
                }
                else
                {
                    if (c.MinimumSize.Height != 0) r.Y = Math.Max(r.Y, c.MinimumSize.Height);
                    else r.Y = Math.Max(r.Y, c.Size.Height);
                }
            }

            if (control1.MaximumSize.Height != 0) r.Height = Math.Min(r.Height, control1.MaximumSize.Height);
            //else r.Height = Math.Min(r.Height, control1.Size.Height);
            if (control2.MaximumSize.Height != 0) r.Height = Math.Min(r.Height, control2.MaximumSize.Height);
            //else r.Height = Math.Min(r.Height, control2.Size.Height);

            return r;
        }

        /// <summary>
        /// Обработка события добавления контрола
        /// </summary>
        /// <param name="e">Параметры</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            // Добавление контрола в пару
            if (externalChange)
            {
                if (control1 == null)
                    control1 = e.Control;
                else if (control2 == null)
                    control2 = e.Control;
            }
            base.OnControlAdded(e);
        }

        /// <summary>
        /// Обработка события удаления контрола
        /// </summary>
        /// <param name="e">Параметры</param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            // Удаление контрола из пары
            if (externalChange)
            {
                if (control1 == e.Control)
                    control1 = null;
                else if (control2 == e.Control)
                    control2 = null;
            }
            base.OnControlRemoved(e);
        }

        /// <summary>
        /// Обработка события расположения контролов
        /// </summary>
        /// <param name="levent">Параметры</param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            // Расположение дочерних контролов по горизонтали
            if (control1 != null && control2 != null)
            {
                int control2X = control1.Width + control1.Margin.Horizontal;
                int control2Width = this.Size.Width - control2X - control2.Margin.Horizontal;
                int control2Height = this.Size.Height - control2.Margin.Vertical;
                control1.SetBounds(control1.Margin.Left, control1.Margin.Top, 0, 0, BoundsSpecified.Location);
                control2.SetBounds(control2X + control2.Margin.Left, control2.Margin.Top, control2Width, control2Height, BoundsSpecified.All);
            }

            // Базовый обработчик
            base.OnLayout(levent);
        }
    }
}
