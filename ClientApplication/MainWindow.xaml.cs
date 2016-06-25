using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Dependee;

namespace ClientApplication
{
    // Implement IDependencyUser to take advantage of DependencyManager
    public partial class MainWindow : Window, IDependeeObject
    {
        // Declare required event and dependency manager
        public event PropertyChangedEventHandler PropertyChanged;

        public Data2VMIntermediate Data2VMInt { get; private set; } // Must be a property to route dependencies through

        private string data = null;
        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                DependeeManager.Instance.Update(this, "Data"); // Data has updated
            }
        }

        [Dependency("Data")] // Data3Visible is dependent on Data
        [Dependency("Data2VMInt.Data2VM.Data2")] // Data3Visible is dependent on Data2VMInt.Data2VM.Data2
        public bool Data3Visible { get { return !string.IsNullOrEmpty(Data) && !string.IsNullOrEmpty(Data2VMInt.Data2VM.Data2); } }

        [Dependency("Data")] // Data3 is dependent on Data
        [Dependency("Data2VMInt.Data2VM.Data2")] // Data3 is dependent on Data2VMInt.Data2VM.Data2
        public string Data3 { get { return Data + ", " + Data2VMInt.Data2VM.Data2; } }

        [Dependency("Data3Visible")] // Data4Visible is dependent on Data3Visible
        public bool Data4Visible { get { return Data3Visible; } }

        [Dependency("Data3")] // Data4 is dependent on Data3
        public string Data4 { get { return Data3 + "; " + Data3; } }
        
        public MainWindow()
        {
            Data2VMInt = new Data2VMIntermediate(this);
            DependeeManager.Instance.Manage(this); // Make this a managed object

            InitializeComponent();
        }

        // Trigger property changed event with the necessary arguments
        public void PropertyChangedTrigger(object sender, PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(sender, args);
        }
    }
}
