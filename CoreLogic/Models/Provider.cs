namespace UPB.CoreLogic.Models;

public class Provider
{
    public int ID {get; set;}
    public string? Name {get; set;}
    public string? Address {get; set;}
    public string? Category {get; set;}
    public int PhoneNumber {get; set;}
    public int ContractRemainingDays {get; set;}
    public  DateTime ContractExpirationDate {get; set;}
    public bool ExpiredContract {get; set;}
    public bool Enable {get; set;}
}