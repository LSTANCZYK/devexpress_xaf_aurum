using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Aurum.Interface.Win.Filters
{
	/// <summary>
	/// Редактор дробного числа
	/// </summary>
	/// <ToDo priority="low">Добавить в техническую документацию 
	/// общие рекомендации создания редакторов (диаграмма состояний):
	/// 1. При установке значения, выполняются все проверки корректности
	/// 2. При установке свойств, влияющих на корректность значения, значение адаптируется под новые свойства
	/// 3. При редактировании значение также динамически меняется, если введенное значение некорректное, 
	///    то сохраняется предыдущее корректное значение, но редактор не меняется
	/// 4. При выходе из редактора, если редактор допускает некорректный ввод, обновить редактор 
	///    в соответствии с корректным значением
	/// 5. Для контроля событий использовать виртуальные методы (рекомендация MS)
	/// </ToDo>
	/// <ToDo priority="low">Вычисление выражений, вводимых в строке</ToDo>
	/// <ToDo priority="low">ErrorProvider для отображения ошибки некорректного значения пользователю</ToDo>
	public class DecimalEdit : TextBox, IMethodParameter
	{
        // Формат отображения дробных чисел
        private static NumberFormatInfo numberFormat = null;

        /// <summary>
        /// Формат отображения дробных чисел
        /// </summary>
        public static NumberFormatInfo NumberFormat
        {
            get
            {
                if (numberFormat == null)
                {
                    numberFormat = new NumberFormatInfo();
                    numberFormat.NumberDecimalSeparator = ".";
                    numberFormat.NumberGroupSeparator = " ";
                }
                return numberFormat;
            }
        }

		// Значение
		private decimal? value = null;
		private decimal? minValue = null;
		private decimal? maxValue = null;

		// Возможность пустого значения
		private bool isNullable = true;

		// Режим редактирования
		private bool editing = false;
		private bool editingSuccess = true;

		// Формат
		private string textFormat = String.Empty;
		private NumberFormatInfo displayFormat;
		private NumberFormatInfo editFormat;

		// Отображение обязательного значения
		private static string emptyText = "<нет>";
		private static Color grayedTextColor = SystemColors.GrayText;
		private static Color textColor = SystemColors.WindowText;

		// Подсказка для пустого значения
		private string hint = null;

		// фокус
		private bool _focused = false;

		/// <summary>
		/// Формат текста по умолчанию
		/// </summary>
		protected virtual string DefaultTextFormat
		{
			get { return String.Empty; }
		}

		/// <summary>
		/// Значение по умолчанию
		/// </summary>
		private decimal? DefaultValue
		{
			get
			{
				if (isNullable)
					return null;
				else if (minValue.HasValue)
					return minValue;
				else if (maxValue.HasValue)
					return maxValue;
				else
					return 0;
			}
		}

		/// <summary>
		/// Количество знаков после запятой
		/// </summary>
		[Browsable(true), Category("Appearance"), Description("Количество знаков после запятой")]
		[DefaultValue(2)]
		public int Digits
		{
			get
			{
				return displayFormat.NumberDecimalDigits;
			}
			set
			{
				displayFormat.NumberDecimalDigits = value;
				editFormat.NumberDecimalDigits = value;
				SetText();
			}
		}

		/// <summary>
		/// Возможность пустого значения
		/// </summary>
		[Browsable(true), Category("Appearance"), Description("Возможность пустого значения")]
		[DefaultValue(true)]
		public bool IsNullable
		{
			get
			{
				return isNullable;
			}
			set
			{
				if (value != isNullable)
				{
					isNullable = value;

					// Корректировка значения
					if (!isNullable)
						SetValueChecked(DefaultValue);
				}
			}
		}

		/// <summary>
		/// Подсказка для пустого значения
		/// </summary>
		[Browsable(true), Description("Текст для пустого значения")]
		[DefaultValue(null)]
		public string Hint
		{
			get { return hint; }
			set
			{
				hint = value;
				SetText();
			}
		}

		/// <summary>
		/// Максимальное значение
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(null), Description("Максимальное значение")]
		public decimal? MaxValue
		{
			get
			{
				return maxValue;
			}
			set
			{
				if (value.HasValue && minValue.HasValue && value.Value < minValue.Value)
					throw new ArgumentException("MaxValue must be not less then MinValue", "value");

				maxValue = value;

				// Корректировка значения
				if (maxValue.HasValue && this.value.HasValue && this.value.Value > maxValue.Value)
					SetValueChecked(maxValue);
			}
		}

		/// <summary>
		/// Минимальное значение
		/// </summary>
		[Browsable(true), Category("Behavior"), DefaultValue(null), Description("Минимальное значение")]
		public decimal? MinValue
		{
			get
			{
				return minValue;
			}
			set
			{
				if (value.HasValue && maxValue.HasValue && value.Value > maxValue.Value)
					throw new ArgumentException("MinValue must be not greater then MaxValue", "value");

				minValue = value;

				// Корректировка значения
				if (minValue.HasValue && this.value.HasValue && this.value.Value < minValue.Value)
					SetValueChecked(minValue);
			}
		}

		/// <summary>
		/// Многострочность редактора
		/// </summary>
		/// <remarks>Редактор числа не переводится в многострочный режим</remarks>
		[Browsable(false)]
		public override bool Multiline
		{
			get { return base.Multiline; }
			set { }
		}

		/// <summary>
		/// Текст
		/// </summary>
		/// <remarks>Свойство не связано со значением, в дизайне не сохраняется</remarks>
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		/// <summary>
		/// Формат текста
		/// </summary>
		[Browsable(true), Category("Appearance"), Description("Формат текста")]
		public string TextFormat
		{
			get
			{
				return textFormat;
			}
			set
			{
				textFormat = value;
				SetText();
			}
		}

		/// <summary>
		/// Значение
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Value
		{
			get
			{
				if (isNullable)
				{
					return value;
				}
				else
				{
					return value != null ? value : DefaultValue;
				}
			}
			set
			{
				if (value == null || string.IsNullOrEmpty(value.ToString()))
				{
					if (isNullable)
						SetValue(null, true);
					else
						throw new ArgumentNullException("Value", "Число не может быть null");
				}
				else
				{
					SetValue(Convert.ToDecimal(value), true);
				}
			}
		}

		/// <summary>
		/// Значение
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual decimal? DecimalValueNullable
		{
			get { return value; }
			set { Value = (object)value; }
		}

		/// <summary>
		/// Значение
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual decimal DecimalValue
		{
			get { return value.HasValue ? value.Value : 0; }
			set { Value = (object)value; }
		}

		/// <summary>
		/// Событие изменения значения
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Событие, возникающее при изменении значения")]
		public event EventHandler ValueChanged;


		/// <summary>
		/// Конструктор
		/// </summary>
		public DecimalEdit()
		{
			// Формат отображения числа
			displayFormat = new NumberFormatInfo();
			displayFormat.NumberDecimalSeparator = NumberFormat.NumberDecimalSeparator;
			displayFormat.NumberGroupSeparator = NumberFormat.NumberGroupSeparator;

			// Формат редактирования числа
			editFormat = new NumberFormatInfo();
			editFormat.NumberDecimalSeparator = NumberFormat.NumberDecimalSeparator;
			editFormat.NumberGroupSeparator = "";

			// Формат текста
			textFormat = DefaultTextFormat;

			SetText();
		}

		/// <summary>
		/// Получение фокуса
		/// </summary>
		/// <param name="e">Параметры</param>
		protected override void OnEnter(EventArgs e)
		{
			// Режим редактирования
			editing = true;

			// Если возвращение после неуспешной валидации, то обновление текста пропускается
			if (!(CausesValidation && !editingSuccess))
				SetText();

			base.OnEnter(e);
		}

		/// <summary>
		/// Нажатие клавиши, первичная валидация ввода
		/// </summary>
		/// <param name="e">Параметры</param>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);

			// Клавиша обработана
			if (e.Handled)
				return;

			// Фильтр клавиш
			if (!Char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',' && e.KeyChar != '-' && e.KeyChar != '\b')
			{
				e.Handled = true;
				return;
			}

			// Десятичный разделитель
			if (e.KeyChar == ',')
				e.KeyChar = '.';

			// Десятичный разделитель
			if (e.KeyChar == '.' && base.Text.Contains("."))
			{
				e.Handled = true;
				return;
			}

			// Десятичный разделитель
			if (e.KeyChar == '.' && Digits == 0)
			{
				e.Handled = true;
				return;
			}

			// Минус
			if (e.KeyChar == '-' && base.Text.Contains("-"))
			{
				e.Handled = true;
				return;
			}

			// Минус
			if (e.KeyChar == '-' && minValue.HasValue && minValue.Value >= 0)
			{
				e.Handled = true;
				return;
			}

			// Количество знаков после запятой (ввод цифры)
			int decimalSep = base.Text.IndexOf('.');
			if (Char.IsDigit(e.KeyChar) &&
				decimalSep >= 0 &&
				SelectionStart > decimalSep &&
				TextLength - decimalSep - 1 >= editFormat.NumberDecimalDigits)
			{
				e.Handled = true;
				return;
			}

			// Количество знаков после запятой (ввод десятичного разделителя)
			if (e.KeyChar == '.' &&
				TextLength - SelectionStart > editFormat.NumberDecimalDigits)
			{
				e.Handled = true;
				return;
			}
		}

		/// <summary>
		/// Потеря фокуса
		/// </summary>
		/// <param name="e">Параметры</param>
		protected override void OnLeave(EventArgs e)
		{
			editing = false;

			// Если валидация отсутствует, то редактирование завершается последним корректным значением
			if (!CausesValidation)
			{
				SetText();
				editingSuccess = true;
			}

			base.OnLeave(e);
		}

		/// <summary>
		/// Изменение текста
		/// </summary>
		/// <param name="e">Параметры</param>
		protected override void OnTextChanged(EventArgs e)
		{
			// Изменение значения
			if (editing)
			{
				decimal d;
				if (String.IsNullOrEmpty(base.Text))
					SetValue(null, false);
				else if (decimal.TryParse(base.Text, NumberStyles.Number, editFormat, out d))
					SetValue(d, false);
			}

			base.OnTextChanged(e);
		}

		/// <summary>
		/// Валидация значения
		/// </summary>
		/// <param name="e">Параметры</param>
		protected override void OnValidating(CancelEventArgs e)
		{
			base.OnValidating(e);

			// Значение некорректное
			if (!editingSuccess)
			{
				//e.Cancel = true;
				value = DefaultValue;
			}
		}

		/// <summary>
		/// Успешная валидация значения
		/// </summary>
		/// <param name="e">Параметры</param>
		protected override void OnValidated(EventArgs e)
		{
			base.OnValidated(e);

			// Установка текста
			SetText();
		}

		// Установка текста согласно текущего значения и режима редактирования
		private void SetText()
		{
			string text = null;
			base.ForeColor = textColor;
			if (value.HasValue)
			{
				text = value.Value.ToString("N", editing ? editFormat : displayFormat);
				if (!editing && !String.IsNullOrEmpty(textFormat))
					text = String.Format(textFormat, text);
			}
			else
			{
				// Показ предупреждения о пустом значении
				if (!editing && isNullable)
				{
					if (!string.IsNullOrEmpty(hint))
						text = hint;
					else
						text = emptyText;
					base.ForeColor = grayedTextColor;
				}
			}
			base.Text = text;
		}

		// Установка значения при изменении Value (external) или редактировании пользователя (internal)
		private void SetValue(decimal? newValue, bool external)
		{
			editingSuccess = false;
			// Валидация значения
			try
			{
				if (!isNullable && !newValue.HasValue)
					throw new Exception("Введите значение");
				if (newValue.HasValue && minValue.HasValue && newValue.Value < minValue.Value)
					throw new Exception("Значение не должно быть меньше " + minValue.ToString());
				if (newValue.HasValue && maxValue.HasValue && newValue.Value > maxValue.Value)
					throw new Exception("Значение не должно быть больше " + maxValue.ToString());
				editingSuccess = true;
			}
			// Обработка исключения при некорректном значении
			catch (Exception)
			{
				if (external)
					throw;
				else
					return;
			}

			// Установка нового значения
			if (newValue != this.value)
			{
				this.value = newValue;

				// Установка текста
				if (external)
					SetText();

				// Вызов события изменения значения
				OnValueChanged(new EventArgs());
				if (ValueChanged != null)
				{
					ValueChanged(this, new EventArgs());
				}
			}
		}

		/// <summary>
		/// Значение изменено
		/// </summary>
		/// <param name="eventargs"></param>
		protected virtual void OnValueChanged(EventArgs eventargs)
		{
		}

		// Установка значения при изменении свойств, связанных с корректностью значения
		private void SetValueChecked(decimal? newValue)
		{
			if (this.value != newValue)
			{
				this.value = newValue;

				// Установка текста
				SetText();

				// Вызов события изменения значения
				OnValueChanged(new EventArgs());
				if (ValueChanged != null)
				{
					ValueChanged(this, new EventArgs());
				}
			}
		}

		// Сериализация TextFormat
		private bool ShouldSerializeTextFormat()
		{
			return textFormat != DefaultTextFormat;
		}

		/// <summary>
		/// Получение фокуса
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if (MouseButtons == MouseButtons.None)
			{
				SelectAll();
				_focused = true;
			}
		}

		/// <summary>
		/// Потеря фокуса
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			_focused = false;
		}

		/// <summary>
		/// Событие отпускания кнопки мыши
		/// </summary>
		/// <param name="mevent"></param>
		protected override void OnMouseUp(MouseEventArgs mevent)
		{
			base.OnMouseUp(mevent);
			if (!_focused && this.SelectionLength == 0)
			{
				_focused = true;
				SelectAll();
			}
		}
	}
}
