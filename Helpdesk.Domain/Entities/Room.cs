using Helpdesk.Domain.Abstraction;

namespace Helpdesk.Domain.Entities;

public class Room : IEntity {
    public Guid Id { get; set; }
    public string RoomNumber { get; protected set; }
    public virtual Article Article { get; protected set; }
    public string IpAddress { get; protected set; }
    public string MacAddress { get; protected set; }

    protected Room() {
        
    }
    
    public Room(string roomNumber, Article article, string ipAddress, string macAddress) {
        RoomNumber = roomNumber;
        Article = article;
        IpAddress = ipAddress;
        MacAddress = macAddress;
    }

    public override string ToString() {
        return $"Номер блока: {Article.Number} \nНазвание блока: {Article.Name} \nНомер комнаты: {RoomNumber} \nIp Address: {IpAddress}\nMac Address: {MacAddress}";
    }
}