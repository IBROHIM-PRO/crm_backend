using System.ComponentModel.DataAnnotations;
using CRMBanks.SharedKernel.Common.Interfaces;

namespace CRMBanks.SharedKernel.Common.AbstractClasses;

public abstract class EntityBase : IEntity
{
    [Key] public int Id { get; set; }
}