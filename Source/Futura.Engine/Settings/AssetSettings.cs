﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futura.Engine.Settings
{
    [Name("Asset Handling")]
    public class AssetSettings
    {
        [Name("Automaticall reimport assets")]
        public bool AutomaticCheckForFileChange = true;
    }
}
