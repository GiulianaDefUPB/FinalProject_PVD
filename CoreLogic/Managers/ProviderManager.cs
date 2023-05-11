using System.Reflection;
using UPB.CoreLogic.Models;
using System.Text.Json;

namespace UPB.CoreLogic.Managers;

public class ProviderManager
{
    private readonly string _path;
    private readonly string _backingService;

    public ProviderManager(string filePath, string backingService)
    {
        _path = filePath;
        _backingService = backingService;
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


    private Provider? ReadProviderFromFile(int id)
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

    private void WriteProviderToFile(Provider provider)
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

    private void DeleteProviderFromFile(int id)
    {
        ValidateIfFileExists();

        string json = File.ReadAllText(_path);
        List<Provider>? providers = JsonSerializer.Deserialize<List<Provider>>(json);

        if (providers != null)
        {
            providers.RemoveAll(provider => provider.ID == id);
            json = JsonSerializer.Serialize(providers);
            File.WriteAllText(_path, json);
        }
    }

    private Provider UpdateProviderToFile(int id, Provider providerToUpdate, Provider foundProvider)
    {
        string json = File.ReadAllText(_path);
        List<Provider>? providers = JsonSerializer.Deserialize<List<Provider>>(json);

        if (providers != null)
        {
            int index = providers.FindIndex(provider => provider.ID == id);

            if (index != -1)
            {
                int remainingDays = (int)providerToUpdate.ContractExpirationDate.Subtract(DateTime.Today).TotalDays;
                bool isExpired = remainingDays < 0;

                foundProvider.Name = providerToUpdate.Name;
                foundProvider.Address = providerToUpdate.Address;
                foundProvider.Category = providerToUpdate.Category;
                foundProvider.PhoneNumber = providerToUpdate.PhoneNumber;
                foundProvider.ContractRemainingDays = remainingDays;
                foundProvider.ContractExpirationDate = providerToUpdate.ContractExpirationDate;
                foundProvider.ExpiredContract = isExpired;

                providers[index] = foundProvider;
                json = JsonSerializer.Serialize(providers);
                File.WriteAllText(_path, json);
            }
        }

        return foundProvider;
    }

    private int GenerateId ()
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

    public async Task<List<Provider>> GetSearchProviders(HttpClient httpProviders)
    {
        var response = await httpProviders.GetAsync(_backingService);

        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        List<SearchProvider>? searchProviders = JsonSerializer.Deserialize<List<SearchProvider>>(json);
        List<Provider> providers = new List<Provider>();

        if (searchProviders != null)
        {
            foreach (SearchProvider sp in searchProviders)
            {
                Provider createdProvider = new Provider()
                {
                    ID = sp.id,
                    Name = sp.business_name,
                    Address = sp.full_address,
                    PhoneNumber = sp.phone_number
                };

                providers.Add(createdProvider);
            }

        }
       
        return providers;
    }
}