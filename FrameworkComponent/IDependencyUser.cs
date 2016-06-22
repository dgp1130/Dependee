using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkComponent
{
    /// <summary>
    /// Interface for a user of the DependencyManager
    /// </summary>
    public interface IDependencyUser : INotifyPropertyChanged
    {
        /// <summary>
        /// Trigger the PropertyChanged event from INotifyPropertyChanged with the parameters given
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Arguments for the property which has changed.</param>
        void PropertyChangedTrigger(object sender, PropertyChangedEventArgs args);
    }
}
