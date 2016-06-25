using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dependee
{
    /// <summary>
    /// A dependency manager for the object it is given.
    /// Updates properties automatically based on their dependencies.
    /// </summary>
    public class DependeeManager
    {
        private Dictionary<string, List<DependeeInfo>> DependencyMap = new Dictionary<string, List<DependeeInfo>>();

        // Singleton instance of manager class
        private static DependeeManager instance;
        public static DependeeManager Instance
        {
            get
            {
                if (instance == null) instance = new DependeeManager();
                return instance;
            }
        }

        // Declare private constructor to prevent public construction
        private DependeeManager() { }

        // Manage the given dependee object
        public void Manage(IDependeeObject sender)
        {
            // Search all properties of given object
            PropertyInfo[] properties = sender.GetType().GetProperties();
            foreach (var property in properties)
            {
                // Check attributes on the property
                object[] attributes = property.GetCustomAttributes(true);
                var name = property.Name;
                foreach (var attribute in attributes)
                {
                    if (attribute is Dependency)
                    {
                        var dependency = attribute as Dependency;
                        var preReq = dependency.PreRequisite;

                        // Separate property name from path to sending object
                        var lastDotIndex = preReq.LastIndexOf('.');
                        var path = lastDotIndex != -1 ? preReq.Substring(0, lastDotIndex).Split(new char[] { '.' }) : null;
                        var preReqName = preReq.Substring(lastDotIndex + 1);

                        // Add to dependency mapping if it does not already exist
                        if (!DependencyMap.ContainsKey(preReqName))
                        {
                            DependencyMap[preReqName] = new List<DependeeInfo>();
                        }

                        // Map prerequisite to its dependent
                        DependencyMap[preReqName].Add(new DependeeInfo()
                        {
                            Sender = sender,
                            Path = path,
                            Name = name
                        });
                    }
                }
            }
        }

        // Update the given property and its dependencies
        public void Update(IDependeeObject sender, string name)
        {
            // Trigger event notification of update
            sender.PropertyChangedTrigger(sender, new PropertyChangedEventArgs(name));

            // Update each dependent
            foreach (var dependent in DependencyMap[name]) UpdateRecur(sender, dependent);
        }

        // Update the given property and its dependencies
        private void UpdateRecur(object sender, DependeeInfo dependent)
        {
            // Follow the path from dependent's sender to find owning object
            object resolvedSender = dependent.Sender;
            if (dependent.Path != null)
            {
                foreach (var item in dependent.Path)
                {
                    try
                    {
                        resolvedSender = resolvedSender.GetType().GetProperty(item).GetMethod.Invoke(resolvedSender, new object[] { });
                    }
                    catch
                    {
                        return; // Could not resolved sender, must not belong to specified object
                    }
                }
            }

            // Check if dependent's owning object is the sender of the event
            if (resolvedSender != sender)
            {
                return; // Sender is not the one specified by the Dependency(...) attribute
            }

            // Trigger event notification of update
            dependent.Sender.PropertyChangedTrigger(dependent.Sender, new PropertyChangedEventArgs(dependent.Name));

            // Update each dependent of the newly updated property
            if (!DependencyMap.ContainsKey(dependent.Name)) return;
            foreach (var child in DependencyMap[dependent.Name]) UpdateRecur(dependent.Sender, child);
        }

        // Structure to contain information about a dependee
        private class DependeeInfo
        {
            // The object owning this property
            public IDependeeObject Sender { get; set; }

            // The path from the owning object to the prerequsite value
            public string[] Path { get; set; }

            // The name of the dependent property
            public string Name { get; set; }
        }
    }
}
