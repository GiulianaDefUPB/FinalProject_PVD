
namespace UPB.CoreLogic.Models;

public class Provider
{
    private int ID {get; set;}
    private string? Name {get; set;}
    private string? Address {get; set;}
    private string? Category {get; set;}
    private int PhoneNumber {get; set;}
    private int ContractRemainingDays {get; set;}
    private  DateTime ContractExpirationDate {get; set;}
    private bool ExpiredContract {get; set;}
    private bool Enable {get; set;}
}