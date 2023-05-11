# Final Project - Group 4 (Providers Management)

_This web API project manages a provider model´s CRUD operations. It also uses a backing service to get n random providers. Additionally, it writes all the data in JSON files based on the environment and uses error exceptions and logs to provide visibility to the behavior of the running app._

## Group members: 

[@davalossebastian](https://github.com/sebicsbics)  

[@delgadillolaura](https://github.com/delgadillolaura)  

[@defilippisgiuliana](https://github.com/GiulianaDefUPB)  

[@mercadomateo](https://github.com/mateo2803)  


## Table of Contents
- [Group_Members](#Group_Members)
- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)

## Installation

In order to work in this project, you must install the Swashbuckle.AspNetCore and Serilog dependencies. Additionally, you can install the MOD header chrome extension.

## Usage

In order to use this application, you have to execute the command "dotnet run" on the FinalProjectPVD/FinalProjectPVD/ folder (on the FinalProjectPVD.csproj level). Afterwards, you have to enter to the provided URL incluiding the swagger endpoint (e.g. http://localhost:5069/swagger) in order to access the swagger UI. Within the swagger UI you can test the API endpoints.

## Configuration

### Configuration Instructions

1. Step 1: Add a new environment configuration file on the FinalProjectPVD.csproj file level --> appsettings.\[environmentName\].json
2. Step 2: Add the title and path properties.
3. Step 3: Within the Path property, add the ProvidersFilePath property.

### Example Configuration

{
    "Title": "Practice in \[environmentName\]",
    "Path": {
      "ProvidersFilePath": "../Providers_\[environmentName\].json"
    },
    "Serilog": {
        "WriteTo": [
          {
            "Name": "File",
            "Args": {
              "path": "../Providers_\[environmentName\].log",
              "rollingInterval": "Day",
              "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}"
            }
          }
        ],
        "Enrich": [ "FromLogContext" ]
      }
}