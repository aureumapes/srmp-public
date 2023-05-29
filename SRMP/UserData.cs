using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer
{
    [Serializable]
    public struct UserData
    {
        public Guid UUID;
        public bool CheckDLC;
        public List<string> IgnoredMods;
    }
}
