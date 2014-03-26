using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyreVM
{
    public abstract class PortableDispatcher
    {
        public abstract void Invoke(Action action);
    }
}
