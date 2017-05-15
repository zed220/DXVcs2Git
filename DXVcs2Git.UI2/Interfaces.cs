using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public interface IParameterizedMifRegistrator<T> : IMifRegistrator {
        T Parameter { get; set; }
    }

    public interface IMifRegistrator : IDisposable {
        void RegisterUI();
        //(string, string) SaveState();
        bool LoadState(string logicalstate, string visualState);
        void Reset();
    }
}
