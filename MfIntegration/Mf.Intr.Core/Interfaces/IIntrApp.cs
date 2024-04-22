using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Core.Interfaces;
public interface IIntrApp
{
    IIntrScheduler CompanyScheduler { get; }
}
