namespace CRMBanks.SharedKernel.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string name, int id) : base($"{name} with id {id} was not found.") { }
}
