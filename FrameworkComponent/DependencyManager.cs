using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkComponent
{
    /// <summary>
    /// A dependency manager for the object it is given.
    /// Updates properties automatically based on their dependencies.
    /// </summary>
    public class DependencyManager
    {
        private IDependencyUser obj;
        private Dictionary<string, List<string>> DependencyMap = new Dictionary<string, List<string>>();

        public DependencyManager(IDependencyUser obj)
        {
            this.obj = obj;

            // Search all properties of given object
            PropertyInfo[] properties = obj.GetType().GetProperties();
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

                        // Add to dependency mapping if it does not already exist
                        if (!DependencyMap.ContainsKey(preReq))
                        {
                            DependencyMap[preReq] = new List<string>();
                        }

                        // Map dependency with its prerequisite
                        DependencyMap[preReq].Add(name);
                    }
                }
            }
        }

        // Update the given property and its dependencies
        public void Update(object sender, string name)
        {
            // Trigger event notification of update
            obj.PropertyChangedTrigger(sender, new PropertyChangedEventArgs(name));

            // Update each dependent
            foreach (var dependent in DependencyMap[name]) UpdateRecur(sender, dependent);
        }

        // Update the given property and its dependencies
        private void UpdateRecur(object sender, string name)
        {
            // Reset the property if possible
            var prop = obj.GetType().GetProperty(name);
            if (prop.SetMethod != null)
            {
                prop.SetMethod.Invoke(obj, new object[] { prop.GetMethod.Invoke(obj, new object[] { }) });
            }

            // Trigger event notification of update
            obj.PropertyChangedTrigger(sender, new PropertyChangedEventArgs(name));

            // Update each dependent
            if (!DependencyMap.ContainsKey(name)) return;
            foreach (var dependent in DependencyMap[name]) UpdateRecur(sender, dependent);
        }
    }
}
