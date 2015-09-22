using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// ��������� ������������� ����������
    /// </summary>
    public interface IDynamicLayoutControl
    {
        /// <summary>
        /// ��������� ������������ � ������������� ������� �������� � ����������� �� ������������ �������
        /// </summary>
        /// <param name="proposal">������� �������, � ������� ����� ���� ������ �������. ������� X,Y; �������� Width, Height</param>
        /// <returns>�������� �������� ��������. X,Y - ����������� �������, Width, Height - ������������.</returns>
        Rectangle GetSizeRange(Rectangle proposal);

        /// <summary>
        /// �����������, � ������� ����� ������������� �������
        /// </summary>
        /// <returns></returns>
        ChangeSizeDirection GetDirection();
    }
}
