using System.Reflection;
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

    public Provider GetById(int id)
    {
        if (id < 0)
        {
            throw new Exception("Invalid ID");
        }

        Provider? providerToGet = ReadProviderFromFile(id);
       
        if (providerToGet == null)
        {
            throw new Exception("Provider not found");
        }

        return providerToGet;
    }

    public Provider Delete(int id)
    {
        if (id < 0)
        {
            throw new Exception("Invalid ID");
        }

        Provider? providerToDelete = ReadProviderFromFile(id);

        if (providerToDelete == null)
        {
            throw new Exception("Provider not found");
        }

        DeleteProviderFromFile(id);

        return providerToDelete;
    }

    public Provider Update(int id, Provider providerToUpdate)
    {
        if (providerToUpdate.ID < 0 
            || string.IsNullOrEmpty(providerToUpdate.Name)
            || string.IsNullOrEmpty(providerToUpdate.Address)
            || string.IsNullOrEmpty(providerToUpdate.Category)
            || providerToUpdate.PhoneNumber <= 0
            || providerToUpdate.ContractExpirationDate == DateTime.MinValue)
        {
            throw new Exception("Invalid input parameters.");
        }

        Provider? foundProvider = ReadProviderFromFile(id);
        
        if (foundProvider == null)
        {
            throw new Exception("Provider not found");
        }

        Provider updatedProvider =  UpdateProviderToFile(id, providerToUpdate,foundProvider);

        return updatedProvider;
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

    public void DeleteProviderFromFile(int id)
    {
        List<string> lst = File.ReadAllLines(_filePath).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();  
        lst.RemoveAll(x => x.Split(',')[0].Equals(id.ToString()));  
        File.WriteAllLines(_filePath, lst);
    }

    public Provider UpdateProviderToFile(int id, Provider providerToUpdate, Provider foundProvider)
    {
        List<string> lst = File.ReadAllLines(_filePath).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();  
        int index = lst.FindIndex(x => x.Split(',')[0].Equals(id.ToString())); 
        
        List<String> patientData = new List<string>();
        
        foreach (PropertyInfo property in foundProvider.GetType().GetProperties())
        {
            patientData.Add(property.GetValue(foundProvider).ToString());
        }

        int remainingDays = (int)providerToUpdate.ContractExpirationDate.Subtract(DateTime.Today).TotalDays;
        bool isExpired = remainingDays < 0;

        foundProvider.Name = providerToUpdate.Name;
        foundProvider.Address = providerToUpdate.Address;
        foundProvider.Category = providerToUpdate.Category;
        foundProvider.PhoneNumber = providerToUpdate.PhoneNumber;
        foundProvider.ContractRemainingDays = remainingDays;
        foundProvider.ContractExpirationDate =  providerToUpdate.ContractExpirationDate;
        foundProvider.ExpiredContract = isExpired;
        foundProvider.Enable = providerToUpdate.Enable;

        string rawData = $"{foundProvider.ID},{foundProvider.Name},{foundProvider.Address},{foundProvider.Category},{foundProvider.PhoneNumber},{foundProvider.ContractRemainingDays},{foundProvider.ContractExpirationDate},{foundProvider.ExpiredContract},{foundProvider.Enable}";
        lst[index] = rawData;
        File.WriteAllLines(_filePath, lst);  

        return foundProvider;
    }
}