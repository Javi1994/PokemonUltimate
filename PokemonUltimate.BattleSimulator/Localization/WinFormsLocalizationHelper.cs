using System;
using System.Windows.Forms;
using PokemonUltimate.Core.Infrastructure.Localization;
using PokemonUltimate.Core.Services;

namespace PokemonUltimate.BattleSimulator.Localization
{
    /// <summary>
    /// Windows Forms specific localization helpers for BattleSimulator.
    /// Extends LocalizationHelper with UI framework dependent functionality.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// </remarks>
    public static class WinFormsLocalizationHelper
    {
        /// <summary>
        /// Gets the current localization provider instance.
        /// </summary>
        private static ILocalizationProvider Provider => LocalizationService.Instance;

        #region ComboBox Helpers

        /// <summary>
        /// Populates a ComboBox with enum values using translated display names.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="comboBox">The ComboBox to populate.</param>
        /// <param name="includeNone">Whether to include a "(None)" option at the start.</param>
        /// <param name="filter">Optional filter function to exclude certain enum values.</param>
        public static void PopulateEnumComboBox<TEnum>(ComboBox comboBox, bool includeNone = false, Func<TEnum, bool> filter = null) where TEnum : struct, Enum
        {
            if (comboBox == null)
                throw new ArgumentNullException(nameof(comboBox));

            comboBox.Items.Clear();

            if (includeNone)
            {
                comboBox.Items.Add("(None)");
            }

            var allValues = Enum.GetValues<TEnum>();
            var provider = Provider;

            foreach (var value in allValues)
            {
                if (filter != null && !filter(value))
                    continue;

                string displayName = LocalizationHelper.GetEnumDisplayName(value, provider);
                comboBox.Items.Add(new EnumDisplayItem<TEnum>(value, displayName));
            }
        }

        /// <summary>
        /// Gets the enum value from a ComboBox item.
        /// </summary>
        public static TEnum GetEnumFromComboBox<TEnum>(ComboBox comboBox) where TEnum : struct, Enum
        {
            if (comboBox?.SelectedItem == null)
                throw new InvalidOperationException("No item selected in ComboBox");

            if (comboBox.SelectedItem is EnumDisplayItem<TEnum> displayItem)
                return displayItem.Value;

            if (comboBox.SelectedItem is TEnum enumValue)
                return enumValue;

            if (comboBox.SelectedItem is string str && str == "(None)")
            {
                return default(TEnum);
            }

            throw new ArgumentException($"Invalid ComboBox item type: {comboBox.SelectedItem.GetType()}");
        }

        /// <summary>
        /// Sets the selected enum value in a ComboBox.
        /// </summary>
        public static void SetSelectedEnum<TEnum>(ComboBox comboBox, TEnum value) where TEnum : struct, Enum
        {
            if (comboBox == null)
                return;

            foreach (var item in comboBox.Items)
            {
                if (item is EnumDisplayItem<TEnum> displayItem && displayItem.Value.Equals(value))
                {
                    comboBox.SelectedItem = item;
                    return;
                }

                if (item is TEnum enumValue && enumValue.Equals(value))
                {
                    comboBox.SelectedItem = item;
                    return;
                }
            }
        }

        #endregion

        #region DataGridView Helpers

        /// <summary>
        /// Populates a DataGridView column with translated enum values.
        /// </summary>
        public static void TranslateDataGridViewColumn(DataGridView dataGridView, string columnName, Func<object, string> translateFunc)
        {
            if (dataGridView == null || string.IsNullOrEmpty(columnName))
                return;

            var column = dataGridView.Columns[columnName];
            if (column == null)
                return;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[columnName].Value != null)
                {
                    var translated = translateFunc(row.Cells[columnName].Value);
                    row.Cells[columnName].Value = translated;
                }
            }
        }

        #endregion

        #region MessageBox Helpers

        /// <summary>
        /// Shows a localized MessageBox.
        /// </summary>
        public static DialogResult ShowLocalizedMessage(string key, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information, params object[] args)
        {
            try
            {
                var message = Provider.GetString(key, args);
                return MessageBox.Show(message, LocalizationHelper.GetApplicationTitle(), buttons, icon);
            }
            catch
            {
                return MessageBox.Show(key, LocalizationHelper.GetApplicationTitle(), buttons, icon);
            }
        }

        /// <summary>
        /// Shows a localized error message.
        /// </summary>
        public static void ShowLocalizedError(string key, params object[] args)
        {
            ShowLocalizedMessage(key, MessageBoxButtons.OK, MessageBoxIcon.Error, args);
        }

        /// <summary>
        /// Shows a localized warning message.
        /// </summary>
        public static void ShowLocalizedWarning(string key, params object[] args)
        {
            ShowLocalizedMessage(key, MessageBoxButtons.OK, MessageBoxIcon.Warning, args);
        }

        /// <summary>
        /// Shows a localized question message and returns the result.
        /// </summary>
        public static DialogResult ShowLocalizedQuestion(string key, params object[] args)
        {
            return ShowLocalizedMessage(key, MessageBoxButtons.YesNo, MessageBoxIcon.Question, args);
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Wrapper class to display enum values with translated names in ComboBox.
        /// </summary>
        private class EnumDisplayItem<TEnum> where TEnum : struct, Enum
        {
            public TEnum Value { get; }
            private readonly string _displayName;

            public EnumDisplayItem(TEnum value, string displayName)
            {
                Value = value;
                _displayName = displayName;
            }

            public override string ToString()
            {
                return _displayName;
            }
        }

        #endregion
    }
}
