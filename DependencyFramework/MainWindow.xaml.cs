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
using FrameworkComponent;

namespace DependencyFramework
{
    // Implement IDependencyUser to take advantage of DependencyManager
    public partial class MainWindow : Window, IDependencyUser
    {
        // Declare required event and dependency manager
        public event PropertyChangedEventHandler PropertyChanged;
        private DependencyManager dependencyManager;

        private string data = null;
        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                dependencyManager.Update(this, "Data"); // Data has updated
            }
        }

        private string data2 = null;
        [Dependency("Data")] // Data2 is dependent on Data
        public string Data2
        {
            get { return data2; }
            set
            {
                data2 = value + Data;
                dependencyManager.Update(this, "Data2"); // Data2 has updated
            }
        }

        [Dependency("Data")] // Data3 is dependent on Data
        [Dependency("Data2")] // Data3 is dependent on Data2
        public string Data3
        {
            get { return Data + " " + Data2; }
        }

        [Dependency("Data3")] // Data4 is dependent on Data3
        public string Data4
        {
            get { return Data3 + " " + Data3; }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            dependencyManager = new DependencyManager(this); // Create dependency manager for this object
        }

        // Trigger property changed event with the necessary arguments
        public void PropertyChangedTrigger(object sender, PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(sender, args);
        }
    }
}
