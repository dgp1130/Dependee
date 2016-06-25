using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    // Intermediate class to test that dependencies can follow multiple levels of indirection
    public class Data2VMIntermediate
    {
        public Data2ViewModel Data2VM { get; private set; }

        public Data2VMIntermediate(MainWindow owner)
        {
            Data2VM = new Data2ViewModel(owner);
        }
    }
}
