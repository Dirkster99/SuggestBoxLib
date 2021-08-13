using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuggestBoxLib.Model
{
    public class Tip
    {
        public Tip(string value)
        {
            Value = value;
        }
        
        public string Value { get; }
    }
}
