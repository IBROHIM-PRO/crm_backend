namespace CRMBanks.SharedKernel.Common.Interfaces;

public interface IWithId<T>
{
    T Id { get; set; }
}