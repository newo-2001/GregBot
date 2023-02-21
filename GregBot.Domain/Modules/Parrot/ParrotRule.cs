using System.Collections.Generic;
using GregBot.Domain.Models;

namespace GregBot.Domain.Modules.Parrot; 

public delegate SendableMessage? ParrotRule(string message);
public delegate IEnumerable<ParrotRule> ParrotRuleProvider();