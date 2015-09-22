using System;
using System.Collections.Generic;
using System.Text;

namespace Aurum.Interface.Win.Filters
{
    /// <summary>
    /// �����������, � ������� ����� ������������� �������
    /// </summary>
    [Flags]
    public enum ChangeSizeDirection
    {
        /// <summary>
        /// �� ����� �������������
        /// </summary>
        None = 0, 
        /// <summary>
        /// ����� ������������� �� ���������
        /// </summary>
        Vertical = 1, 
        /// <summary>
        /// ����� ������������� �� �����������
        /// </summary>
        Horizontal = 2, 
        /// <summary>
        /// ����� ������������� �� ��������� � �����������
        /// </summary>
        Both = Vertical | Horizontal
    }
}
