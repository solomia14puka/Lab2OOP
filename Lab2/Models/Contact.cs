using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Models;

public class Contact
{
    public string? FullName { get; set; }
    public string? Faculty { get; set; }
    public string? Department { get; set; }
    public string? Specialty { get; set; }
    public string? CollaborationType { get; set; }
    public string? Timeframe { get; set; }
    public Dictionary<string, string> Attributes { get; } = new();

    public override string ToString()
        => $"{FullName} | {Faculty} | {Department} | {Specialty} | {CollaborationType} | {Timeframe}";
}
