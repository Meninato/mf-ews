using Mf.Intr.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingWorker;

[IntrTable(Name = "Person")]
public class Person
{
    [IntrColumnKey]
    public string? Id { get; set; }
    public string? Name { get; set; }

    [IntrColumn(Name = "Desc")]
    public string? Description { get; set; }

    public bool? IsGood { get; set; }
    public decimal? Price { get; set; }
    public float? Balance { get; set; }
    public double? Amount { get; set; }
}
