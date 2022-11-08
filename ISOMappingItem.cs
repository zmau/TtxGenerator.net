using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TtxGenerator.net
{
    public class ISOMappingItem
    {
        public string CoverageType, CoverageSubtype, ISOPolicyType, ISOCoverageType, ISOLossType;

        public string AsLine()
        {   // TODO where does this CommercialPackage,, come from ? , I guess it may be different.
            return $"CommercialPackage,,{CoverageType},{CoverageSubtype},{ISOPolicyType},{ISOCoverageType},{ISOLossType}";
        }
    }
}
