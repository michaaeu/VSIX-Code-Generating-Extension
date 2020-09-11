using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VSIXProject.TemplateUtils;
using VSIXProject.TemplateUtils.TemplateSettings;

namespace VSIXProject
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TemplateListView : Window
    {
        public TTSettingsUtils SettingsUtils { get; private set; }

        public TemplateListView()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            SettingsUtils = new TTSettingsUtils();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new TemplateInfoViewModel(this);
        }
        private void DataGridRow_Selected(object sender, RoutedEventArgs e)
        {
            DataGridRow row = e.Source as DataGridRow;
            TemplateInfoRecord templateInfoRecord = row.Item as TemplateInfoRecord;

            if (templateInfoRecord.InteractiveSettings != null)
            {
                return;
            }
            templateInfoRecord.InteractiveSettings = SettingsUtils.GetInteractiveSettings(templateInfoRecord);
            SettingsUtils.UpdateDictionary(templateInfoRecord);
            // Immutable settings: f.e. output dir, output file name 
            templateInfoRecord.ImmutableSettings = SettingsUtils.GetImmutableSettings(templateInfoRecord);
        }
    }
}
