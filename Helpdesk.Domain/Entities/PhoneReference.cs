using Helpdesk.Domain.Abstraction;

namespace Helpdesk.Domain.Entities;

public class PhoneReference : IEntity {
    public Guid Id { get; set; }
    public string? Position { get; protected set; }
    public string? Department { get; protected set; }
    public string? FullName { get; protected set; }
    public long? InternalNumber { get; protected set; }
    public long? CityNumber { get; protected set; }
    public string? Email { get; protected set; }
    public string SearchColumn { get; protected set; }

    protected PhoneReference() {
        
    }
    
    public PhoneReference(string position, string department, string fullName, long? internalNumber, long? cityNumber, string email) {
        Position = position;
        Department = department;
        FullName = fullName;
        InternalNumber = internalNumber;
        CityNumber = cityNumber;
        Email = email;
        SearchColumn = $"{Position} {Department} {FullName} {InternalNumber} {CityNumber} {Email}";
    }

    public override string ToString() {
        return $"Отделение: {Department}\nДолжность: {Position}\nФИО: {FullName}\nВнутренний: {InternalNumber}\nГородской: {CityNumber}\nE-mail: {Email}";
    }
}