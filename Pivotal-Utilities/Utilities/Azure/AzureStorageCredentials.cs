using System;
using System.Collections.Generic;
using System.Text;

namespace Pivotal.Utilities.Azure
{

    public class AzureStorageCredentials : AzureCredentials
    {
        public string Protocol = "https";

        public string ConnectionString
        {
            get
            {
                var formatter = new BasicAzureStorageConnectionStringFormatter();

                return formatter.Format(Protocol, AccountName, Token);
            }
        }

        public string Endpoint
        {
            get
            {
                if (!string.IsNullOrEmpty(AccountName))
                {
                    return string.Format("https://{0}.blob.core.windows.net", AccountName);
                }

                return null;
            }
        }
    }
}
