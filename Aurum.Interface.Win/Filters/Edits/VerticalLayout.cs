using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Layout;
using System.Windows.Forms;
using System.Drawing;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// Вертикальная планировка контролов
    /// </summary>
    public class VerticalLayout : LayoutEngine
    {
        private bool useTabIndex = false;

        /// <summary>
        /// Определение порядка следования дочерних контролов
        /// </summary>
        /// <param name="b">true - порядок дочерних контролов по TabIndex,
        /// false - порядок дочерних контролов по порядку их добавления</param>
        public void UseTabIndex(bool b)
        {
            useTabIndex = b;
        }

        /// <summary>
        /// Метод, вызываемый для расположения дочерних элементов
        /// </summary>
        /// <param name="container"></param>
        /// <param name="layoutEventArgs"></param>
        /// <returns></returns>
        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            // Область, учитывающая паддинг и scroll area родительского контрола
            Rectangle parentDisplayRectangle = parent.DisplayRectangle;
            // Клиентская область
            Size clientSize = parent.ClientSize;
            // Реально видимая область
            Rectangle realParentRectangle = new Rectangle(parentDisplayRectangle.X,
                parentDisplayRectangle.Y,
                clientSize.Width, clientSize.Height);
            
            // Список дочерних контролов
            List<Control> list = new List<Control>();
            foreach (Control c in parent.Controls)
            {
                if (!c.Visible)
                {
                    continue;
                }
                list.Add(c);
            }

            // Сортировка контролов по TabIndex'у, иначе контролы будут расположены в порядке их добавления на родительский контрол
            if (useTabIndex)
            {
                list.Sort(delegate(Control c1, Control c2)
                {
                    return c1.TabIndex.CompareTo(c2.TabIndex);
                });
            }

            // Статистика по контролам
            int layoutMinHeight = 0;    // Сумма минимальных высот всех контролов
            int heightFreeSpace;        // Высота, на которую можно увеличить увеличивающиеся контролы
            int resizeableControls = 0; // Количество увеличивающихся контролов

            // Минимальные и максимальные размеры контрола
            Rectangle proposal = new Rectangle(0, 0, realParentRectangle.Width, realParentRectangle.Height);

            // Сбор статистики
            foreach (Control c in list)
            {
                if (c is IDynamicLayoutControl)
                {
                    Rectangle check = ((IDynamicLayoutControl)c).GetSizeRange(proposal);
                    layoutMinHeight += check.Y + c.Margin.Top + c.Margin.Bottom;
                    if ((((IDynamicLayoutControl)c).GetDirection() & ChangeSizeDirection.Vertical) == ChangeSizeDirection.Vertical)
                    {
                        // Контрол может изменяться по высоте
                        resizeableControls++;
                    }
                }
                else
                {
                    // Стандартный контрол
                    layoutMinHeight += c.Size.Height + c.Margin.Top + c.Margin.Bottom;
                }
            }
            heightFreeSpace = Math.Max(realParentRectangle.Height - layoutMinHeight, 0);

            // Положение следующего контрола
            Point nextControlLocation = realParentRectangle.Location;

            // Расположение контролов
            foreach (Control c in list)
            {
                // Учет сдвига контрола
                nextControlLocation.Offset(c.Margin.Left, c.Margin.Top);

                // Расположение контрола на форме
                c.Location = nextControlLocation;

                // Растягивание на всю ширину
                c.Width = realParentRectangle.Width - (c.Margin.Left + c.Margin.Right);

                if (c is IDynamicLayoutControl) // Контрол, реализующий этот интерфейс, возможно может изменять размеры по вертикали
                {
                    if ((((IDynamicLayoutControl)c).GetDirection() & ChangeSizeDirection.Vertical) == ChangeSizeDirection.Vertical)
                    {
                        // Получение минимального и максимального размера контрола
                        Rectangle check = ((IDynamicLayoutControl)c).GetSizeRange(proposal);

                        // Контрол может измениться в размерах
                        // Новая высота для контрола
                        int newHeight = check.Y + heightFreeSpace / resizeableControls;

                        // Проверка ограничений на минимальный и максимальный размер
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

            // true, если родительский контрол должен вызвать layout
            return false;
        }
    }
}
