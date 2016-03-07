using System.Collections.Generic;

namespace ApplicationRequestRouter
{
    public class RouterOptions
    {
        public RouterOptions()
        {
            Routes = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Routes { get; set; }        
    }
}