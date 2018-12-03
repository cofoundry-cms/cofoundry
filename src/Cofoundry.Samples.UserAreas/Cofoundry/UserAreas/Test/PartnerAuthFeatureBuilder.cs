using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Samples.UserAreas
{
    public class AuthFeature
    {
        public Type[] PreAuthPipelines { get; set; }
        public Type[] AuthPipelines { get; set; }
        public Type[] PostAuthPipelines { get; set; }

        public string AuthMethods { get; set; }
    }

    public class PartnerAuthFeatureBuilder
    {
        public void GetFeatures()
        {

        }
    }
}
