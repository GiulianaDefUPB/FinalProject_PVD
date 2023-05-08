using UPB.CoreLogic.Models;

namespace UPB.CoreLogic.Managers;


public class ProviderManager
{
    private string _filePath;

    public ProviderManager()
    {
        _filePath = "./providers.txt";
    }

    public Provider Create(Provider providerToCreate)
    {
        int remainingDays = (int)providerToCreate.ContractExpirationDate.Subtract(DateTime.Today).TotalDays;
        bool isExpired = remainingDays < 0;

        Provider createdProvider = new Provider()
        {
            ID = providerToCreate.ID,
            Name = providerToCreate.Name,
            Address = providerToCreate.Address,
            Category = providerToCreate.Category,
            PhoneNumber = providerToCreate.PhoneNumber,
            ContractRemainingDays = remainingDays,
            ContractExpirationDate =  providerToCreate.ContractExpirationDate,
            ExpiredContract = isExpired,
            Enable = providerToCreate.Enable
        };
        
        WriteProviderToFile(createdProvider);

        return createdProvider;
    }

    public void WriteProviderToFile(Provider provider)
    {
        StreamWriter writer = new StreamWriter(_filePath, true);
        string line = $"{provider.ID},{provider.Name},{provider.Address},{provider.Category},{provider.PhoneNumber},{provider.ContractRemainingDays},{provider.ContractExpirationDate},{provider.ExpiredContract},{provider.Enable}";
        writer.WriteLine(line);
        writer.Close();
    }
}

