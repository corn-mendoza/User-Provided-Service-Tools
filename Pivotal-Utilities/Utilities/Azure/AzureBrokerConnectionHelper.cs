using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using Pivotal.Utilities;

namespace Pivotal.Utilities.Azure
{
    public class AzureBrokerConnectionHelper : CFEnvironmentVariables
    {

        public string GetConnectionStringForAzureDbService(string serviceTypeName, string serviceInstanceName = "", IDbConnectionStringFormatter formatter = null)
        {
            string connectionString = null;

            if (Object.ReferenceEquals(null, vcap_services_data) == false)
            {
                dynamic serviceInfo = GetInfoForService(serviceTypeName, serviceInstanceName);

                if (Object.ReferenceEquals(null, serviceInfo) == false)
                {
                    var cstring = Convert.ToString(serviceInfo.credentials.connectionstring);
                    if (string.IsNullOrEmpty(cstring))
                    {
                        // Default to use a Basic MS SQL Server connection string, if our formatter was not specified.
                        if (formatter == null)
                            formatter = new BasicSQLServerConnectionStringFormatter();

                        var host = Convert.ToString(serviceInfo.credentials.sqlServerFullyQualifiedDomainName);
                        var username = Convert.ToString(serviceInfo.credentials.databaseLogin);
                        var password = Convert.ToString(serviceInfo.credentials.databaseLoginPassword);
                        //var port = Convert.ToString(serviceInfo.credentials.port);
                        var databaseName = Convert.ToString(serviceInfo.credentials.sqldbName);

                        connectionString = formatter.Format(host, username, password, databaseName, "1433");
                    }
                    else
                    {
                        connectionString = cstring;
                    }
                }
            }
            return connectionString;
        }


        public string GetConnectionStringForMessagingService(string serviceTypeName, string serviceInstanceName = "", IMessageBusConnectionStringFormatter formatter = null)
        {
            string connectionString = null;

            if (Object.ReferenceEquals(null, vcap_services_data) == false)
            {
                dynamic serviceInfo = GetInfoForService(serviceTypeName, serviceInstanceName);

                if (Object.ReferenceEquals(null, serviceInfo) == false)
                {
                    // Default to use a Basic Azure Message Bus connection string, if our formatter was not specified.
                    if (formatter == null)
                        formatter = new BasicAzureMessageBusConnectionStringFormatter();

                    var endpoint = Convert.ToString(serviceInfo.credentials.namespace_name);
                    var keyname = Convert.ToString(serviceInfo.credentials.shared_access_key_name);
                    var token = Convert.ToString(serviceInfo.credentials.shared_access_key_value);

                    connectionString = formatter.Format(endpoint, keyname, token);
                }
            }
            return connectionString;
        }

        public AzureSearchCredentials GetAzureSearchCredentials(string serviceTypeName, string serviceInstanceName = "")
        {
            if (Object.ReferenceEquals(null, vcap_services_data) == false)
            {
                dynamic serviceInfo = GetInfoForService(serviceTypeName, serviceInstanceName);

                if (Object.ReferenceEquals(null, serviceInfo) == false)
                {
                    //var protocol = Convert.ToString(serviceInfo.credentials.protocol);
                    var token = Convert.ToString(serviceInfo.credentials.SearchServiceApiKey);
                    var name = Convert.ToString(serviceInfo.credentials.SearchServiceName);
                    var index = Convert.ToString(serviceInfo.credentials.IndexName);

                    var _ret = new AzureSearchCredentials()
                    {
                        ServiceName = serviceInstanceName,
                        ServiceType = serviceTypeName,
                        AccountName = name,
                        Token = token,
                        Index = index
                    };
                    return _ret;
                }
            }
            return null;
        }


        public AzureStorageCredentials GetAzureStorageCredentials(string serviceTypeName, string serviceInstanceName = "")
        {
            if (Object.ReferenceEquals(null, vcap_services_data) == false)
            {
                dynamic serviceInfo = GetInfoForService(serviceTypeName, serviceInstanceName);

                if (Object.ReferenceEquals(null, serviceInfo) == false)
                {
                    var acctname = Convert.ToString(serviceInfo.credentials.storage_account_name);
                    var token = Convert.ToString(serviceInfo.credentials.primary_access_key);

                    var _ret = new AzureStorageCredentials()
                    {
                        ServiceName = serviceInstanceName,
                        ServiceType = serviceTypeName,
                        AccountName = acctname,
                        Token = token
                    };
                    return _ret;
                }
            }
            return null;
        }

    }

    public class BasicAzureMessageBusConnectionStringFormatter : IMessageBusConnectionStringFormatter
    {
        public string Format(string endpoint, string keyname, string token)
        {
            return $"Endpoint=sb://{endpoint}.servicebus.windows.net;SharedAccessKeyName={keyname};SharedAccessKey={token};";
        }
    }

    public class BasicAzureStorageConnectionStringFormatter : IStorageConnectionStringFormatter
    {
        public string Format(string protocol, string acctname, string token)
        {
            return $"DefaultEndpointsProtocol={protocol};AccountName={acctname};AccountKey={token}";
        }
    }

}