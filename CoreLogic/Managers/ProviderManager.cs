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

        var (lineList, index) = GetFileLine(id);

        Dictionary<int, string> newProperties = new Dictionary<int, string>()
        {
            {8, enable ? "true" : "false" }
        };

        List<string> patientData = ModifyArrayData(provider, newProperties);

        string rawData = string.Join(",", patientData);
        lineList[index] = rawData;
        File.WriteAllLines(_path, lineList);

        provider.Enable = enable;
        return provider;
    }

    private List<string> ModifyArrayData(Provider provider, Dictionary<int, string> newProperties)
    {
        List<string> patientData = provider.GetType()
            .GetProperties()
            .Select(property => property.GetValue(provider).ToString())
            .ToList();

        foreach ((int index, string property) in newProperties)
        {
            patientData[index] = property;
        }

        return patientData;
    }

    private (List<string>, int) GetFileLine(int id)
    {
        List<string> lst = File.ReadAllLines(_path).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();  
        int index = lst.FindIndex(x => x.Split(',')[0].Equals(id.ToString()));
        return (lst, index); 
    }

 public Provider? ReadProviderFromFile(int id)
    {
        ValidateIfFileExists();

        StreamReader reader = new StreamReader(_path);

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
     private void ValidateIfFileExists()
    {
        if (!File.Exists(_path))
        {
            throw new FileNotFoundException("Empty database.");
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
            || providerToCreate.PhoneNumber <= 0
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
        List<Provider> providers = new List<Provider>();

        if (listType.Equals("all") || listType.Equals("null"))
            option = 1;
        else if (listType.Equals("only-enable"))
            option = 2;
        else
            throw new Exception("Invalid header parameter");

        StreamReader reader = new StreamReader(_path);

        string? line = reader.ReadLine();

        while (line != null)
        {
            string[] providerInfo = line.Split(',');
            
            if (option == 1 || (option == 2 && Boolean.Parse(providerInfo[8])))
            {
                Provider provider = new Provider()
                {
                    ID = int.Parse(providerInfo[0]),
                    Name = providerInfo[1],
                    Address = providerInfo[2],
                    Category = providerInfo[3],
                    PhoneNumber = int.Parse(providerInfo[4]),
                    ContractRemainingDays = int.Parse(providerInfo[5]),
                    ContractExpirationDate = DateTime.Parse(providerInfo[6]),
                    ExpiredContract = Boolean.Parse(providerInfo[7]),
                    Enable = Boolean.Parse(providerInfo[8])
                };  
                
                providers.Add(provider);
            }

            line = reader.ReadLine();
        }

        reader.Close();
        return providers;
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
            || providerToUpdate.PhoneNumber <= 0
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
        StreamWriter writer = new StreamWriter(_path, true);
        string line = $"{provider.ID},{provider.Name},{provider.Address},{provider.Category},{provider.PhoneNumber},{provider.ContractRemainingDays},{provider.ContractExpirationDate},{provider.ExpiredContract},{provider.Enable}";
        writer.WriteLine(line);
        writer.Close();
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
        if (!File.Exists(_path))
        {
            return 1;
        }

        string lastLine = File.ReadLines(_path).Last();
        string[] providerInfo = lastLine.Split(',');
        int providerId = int.Parse(providerInfo[0]) + 1;

        return providerId;
    }

    public async Task<List<Provider>> GetSearchProviders(HttpClient _httpProviders)
    {
        var response = await _httpProviders.GetAsync(_backingService);

        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        List<SearchProviders> searchProviders = JsonSerializer.Deserialize<List<SearchProviders>>(json);
        List<Provider> providers = new List<Provider>();

        foreach (SearchProviders sp in searchProviders)
        {
            providers.Add(new Provider(sp.name,"", sp.address, "", sp.phone_number,""));
        }

        return providers;
    }
}