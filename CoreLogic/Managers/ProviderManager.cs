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
         if (providerToCreate.ID < 0 
            || string.IsNullOrEmpty(providerToCreate.Name)
            || string.IsNullOrEmpty(providerToCreate.Address)
            || string.IsNullOrEmpty(providerToCreate.Category)
            || providerToCreate.PhoneNumber <= 0
            || providerToCreate.ContractExpirationDate == DateTime.MinValue)
        {
            throw new Exception("Invalid input parameters.");
        }

        if (ReadProviderFromFile(providerToCreate.ID) != null)
        {
            throw new Exception("Provider with the same ID already exists");
        }

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

    public Provider? ReadProviderFromFile(int id)
    {
        if (!File.Exists(_filePath))
        {
            return null;
        }

        StreamReader reader = new StreamReader(_filePath);

        string? line = reader.ReadLine();
        while (line != null)
        {
            string[] providerInfo = line.Split(',');
            int providerId = int.Parse(providerInfo[0]);

            if (providerId == id)
            {
                reader.Close();
                Provider foundProvider = new Provider()
                {
                    ID = providerId,
                    Name = providerInfo[1],
                    Address = providerInfo[2],
                    Category = providerInfo[3],
                    PhoneNumber = int.Parse(providerInfo[4]),
                    ContractRemainingDays = int.Parse(providerInfo[5]),
                    ContractExpirationDate = DateTime.Parse(providerInfo[6]),
                    ExpiredContract = Boolean.Parse(providerInfo[7]),
                    Enable = Boolean.Parse(providerInfo[8])
                };
                return foundProvider;
            }

            line = reader.ReadLine();
        }

        reader.Close();
        return null;
    }

    public void WriteProviderToFile(Provider provider)
    {
        StreamWriter writer = new StreamWriter(_filePath, true);
        string line = $"{provider.ID},{provider.Name},{provider.Address},{provider.Category},{provider.PhoneNumber},{provider.ContractRemainingDays},{provider.ContractExpirationDate},{provider.ExpiredContract},{provider.Enable}";
        writer.WriteLine(line);
        writer.Close();
    }
}

