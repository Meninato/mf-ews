﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Workers;
public enum WorkStatus
{
    Partial,
    AllSucceed,
    AllFailed,

    NotStarted
}
