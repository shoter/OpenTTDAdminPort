﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests.Assemblies.TestTypes
{
    public interface IFurnitureBox<TFurniture>
        where TFurniture : IFurniture
    {
        
    }
}
