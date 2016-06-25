using Dependee;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    // Separate view model to test dependencies between different objects
    public class Data2ViewModel : IDependeeObject
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainWindow owner { get; private set; } // Must be a public property to route dependencies through it

        [Dependency("owner.Data")] // Data2Visible is dependent on owner.Data
        public bool Data2Visible { get { return !string.IsNullOrEmpty(owner.Data); } }

        private string data2 = null;
        public string Data2
        {
            get { return data2; }
            set
            {
                data2 = value;
                DependeeManager.Update(this, "Data2"); // Data2 has updated
            }
        }

        public Data2ViewModel(MainWindow owner)
        {
            DependeeManager.Manage(this); // Make this a managed object

            this.owner = owner;
        }

        public void PropertyChangedTrigger(object sender, PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(sender, args);
        }
    }
}
