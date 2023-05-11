using System.Reflection;
using UPB.CoreLogic.Models;
using System.Text.Json;

namespace UPB.CoreLogic.Managers;

public class ProviderManager
{

    private readonly string _path;

    public ProviderManager(string filePath)
    {
        _path = filePath;
    }

    public Provider Enable(int id)
    {
        return UpdateProviderStatus(id, true);
    }

    public Provider Disable(int id)
    {
        return UpdateProviderStatus(id, false);
    }

    private Provider UpdateProviderStatus(int id, bool enable)
    {
        ValidateId(id);

        Provider provider = ReadProviderFromFile(id);

        if (provider == null)
        {
            throw new Exception($"Provider with ID {id} not found.");
        }

        provider.Enable = enable;

        string json = File.ReadAllText(_path);
        List<Provider>? providers = JsonSerializer.Deserialize<List<Provider>>(json);
        int index = providers.FindIndex(provider => provider.ID == id);
        
        providers[index] = provider;
        
        json = JsonSerializer.Serialize(providers);
        File.WriteAllText(_path, json);
       
        return provider;
    }


 public Provider? ReadProviderFromFile(int id)
    {
         ValidateIfFileExists();

        string json = File.ReadAllText(_path);
        if (!string.IsNullOrEmpty(json))
        {
            List<Provider>? providers = JsonSerializer.Deserialize<List<Provider>>(json);

            if (providers != null)
            {
                Provider? foundProvider = providers.Find(provider => provider.ID == id);
                return foundProvider;
            }
        }

        return null;
    }
     private void ValidateIfFileExists()
    {
         if (!File.Exists(_path))
        {
            using var streamWriter = File.CreateText(_path);
            streamWriter.WriteLine("[]"); // Add an empty array as the default content
        }
    }

    private void ValidateId(int id)
    {
        if (id < 0)
        {
            throw new ArgumentException($"Invalid ID: {id}");
        }
    }

    public Provider Create(Provider providerToCreate)
    {
         if (string.IsNullOrEmpty(providerToCreate.Name)
            || string.IsNullOrEmpty(providerToCreate.Address)
            || string.IsNullOrEmpty(providerToCreate.Category)
            || string.IsNullOrEmpty(providerToCreate.PhoneNumber)
            || providerToCreate.ContractExpirationDate == DateTime.MinValue)
        {
            throw new Exception("Invalid input parameters");
        }

        if (providerToCreate.Category != "BASKET" && providerToCreate.Category != "SOCCER")
        {
            throw new Exception("Invalid category. Category can only be BASKET or SOCCER");
        }

        if (ReadProviderFromFile(providerToCreate.ID) != null)
        {
            throw new Exception("Provider with the same ID already exists");
        }

        int remainingDays = (int)providerToCreate.ContractExpirationDate.Subtract(DateTime.Today).TotalDays;
        bool isExpired = remainingDays < 0;

        Provider createdProvider = new Provider()
        {
            ID = GenerateId(),
            Name = providerToCreate.Name,
            Address = providerToCreate.Address,
            Category = providerToCreate.Category,
            PhoneNumber = providerToCreate.PhoneNumber,
            ContractRemainingDays = remainingDays,
            ContractExpirationDate =  providerToCreate.ContractExpirationDate,
            ExpiredContract = isExpired
        };
        
        WriteProviderToFile(createdProvider);

        return createdProvider;
    }

    public Provider GetById(int id)
    {
        ValidateId(id);

        Provider? providerToGet = ReadProviderFromFile(id);
       
        if (providerToGet == null)
        {
            throw new Exception("Provider not found");
        }

        int remainingDays = (int)providerToGet.ContractExpirationDate.Subtract(DateTime.Today).TotalDays;
        bool isExpired = remainingDays < 0;

        providerToGet.ContractRemainingDays = remainingDays;
        providerToGet.ExpiredContract = isExpired;

        return providerToGet;
    }

    public List<Provider> Get(string listType)
    {
        ValidateIfFileExists();

        int option = 0;

        if (listType == null ||
            listType.Equals("all", StringComparison.OrdinalIgnoreCase) ||
            listType.Equals("null", StringComparison.OrdinalIgnoreCase))
            option = 1;
        else if (listType.Equals("only-enable", StringComparison.OrdinalIgnoreCase))
            option = 2;
        else
            throw new Exception("Invalid header parameter.");

        List<Provider> providers = new List<Provider>();
        string json = File.ReadAllText(_path);

        providers = JsonSerializer.Deserialize<List<Provider>>(json);

        if (option == 1)
            return providers;
        else
            return providers.Where(provider => provider.Enable).ToList();
    }

    public Provider Delete(int id)
    {
        ValidateId(id);

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
        ValidateId(id);
        
        if (string.IsNullOrEmpty(providerToUpdate.Name)
            || string.IsNullOrEmpty(providerToUpdate.Address)
            || string.IsNullOrEmpty(providerToUpdate.Category)
            ||  string.IsNullOrEmpty(providerToUpdate.PhoneNumber)
            || providerToUpdate.ContractExpirationDate == DateTime.MinValue)
        {
            throw new Exception("Invalid input parameters");
        }

        if (providerToUpdate.Category != "BASKET" && providerToUpdate.Category != "SOCCER")
        {
            throw new Exception("Invalid category. Category can only be BASKET or SOCCER");
        }

        Provider? foundProvider = ReadProviderFromFile(id);
        
        if (foundProvider == null)
        {
            throw new Exception("Provider not found");
        }

        Provider updatedProvider =  UpdateProviderToFile(id, providerToUpdate,foundProvider);

        return updatedProvider;
    }

    public void WriteProviderToFile(Provider provider)
    {
         ValidateIfFileExists();

        List<Provider>? providers = new List<Provider>();
        string json = File.ReadAllText(_path);

        if (!string.IsNullOrEmpty(json))
        {
            providers = JsonSerializer.Deserialize<List<Provider>>(json);
        }

        if (providers != null)
        {
            providers.Add(provider);
            json = JsonSerializer.Serialize(providers);
            File.WriteAllText(_path, json);
        }
    }

    public void DeleteProviderFromFile(int id)
    {
        List<string> lst = File.ReadAllLines(_path).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();  
        lst.RemoveAll(x => x.Split(',')[0].Equals(id.ToString()));  
        File.WriteAllLines(_path, lst);
    }

    public Provider UpdateProviderToFile(int id, Provider providerToUpdate, Provider foundProvider)
    {
        List<string> lst = File.ReadAllLines(_path).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();  
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

        string rawData = $"{foundProvider.ID},{foundProvider.Name},{foundProvider.Address},{foundProvider.Category},{foundProvider.PhoneNumber},{foundProvider.ContractRemainingDays},{foundProvider.ContractExpirationDate},{foundProvider.ExpiredContract},{foundProvider.Enable}";
        lst[index] = rawData;
        File.WriteAllLines(_path, lst);  

        return foundProvider;
    }

    public int GenerateId ()
    {
        int newId = 1;

        if (File.Exists(_path))
        {
            string json = File.ReadAllText(_path);
            if (!string.IsNullOrEmpty(json))
            {
                List<Provider>? providers = JsonSerializer.Deserialize<List<Provider>>(json);
                if (providers != null && providers.Count > 0)
                {
                    newId = providers.Max(provider => provider.ID) + 1;
                }
            }
        }
        return newId;
   }
}
