using System.Reflection;
using UPB.CoreLogic.Models;
namespace UPB.CoreLogic.Managers;

public class ProviderManager
{
    private readonly string path;

    public ProviderManager(string filePath)
    {
        path = filePath;
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
        ValidateFileExists();
        ValidateId(id);

        Provider provider = this.GetProviderFromFile(id);
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
        File.WriteAllLines(path, lineList);

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
        List<string> lst = File.ReadAllLines(path).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();  
        int index = lst.FindIndex(x => x.Split(',')[0].Equals(id.ToString()));
        return (lst, index); 
    }

    private Provider GetProviderFromFile(int id)
    {
        StreamReader reader = new StreamReader(path);

        int numProperties = typeof(Provider).GetProperties().Length;
        string line;
        string[] dataArray = new string[numProperties];
        Provider foundProvider = null;

        while (!reader.EndOfStream)
        {
            line = reader.ReadLine();
            dataArray = line.Split(",");

            if (Int32.Parse(dataArray[0]) == id)
            {
                foundProvider = new Provider()
                {
                    ID = Int32.Parse(dataArray[0]),
                    Name = dataArray[1],
                    Address = dataArray[2],
                    Category = dataArray[3],
                    PhoneNumber = Int32.Parse( dataArray[4]),
                    ContractRemainingDays =Int32.Parse( dataArray[5]),
                    ContractExpirationDate = DateTime.Parse(dataArray[6]),
                    ExpiredContract =  Boolean.Parse(dataArray[7]),
                    Enable = Boolean.Parse(dataArray[8])
                }; 
                break;
            }   
        }
        
        reader.Close();
        
        return foundProvider;
    }

     private void ValidateFileExists()
    {
        if (!File.Exists(path))
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
}

