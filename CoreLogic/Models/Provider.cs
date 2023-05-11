using System.ComponentModel;

namespace UPB.CoreLogic.Models;

public class Provider
{
    public int ID {get; set;}
    public string? Name {get; set;}
    public string? Address {get; set;}
    public string? Category {get; set;}
    public string? PhoneNumber {get; set;}
    public int ContractRemainingDays {get; set;}
    public  DateTime ContractExpirationDate {get; set;}
    public bool? ExpiredContract {get; set;}
    [DefaultValue(false)]
    public bool? Enable {get; set;} = false;
}