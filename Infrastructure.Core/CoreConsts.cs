namespace Infrastructure.Core
{
    public class CoreConsts
    {
        public const string DefaultLoggerKey = "core.logger";

        public const string CoreConfigurationSample = @"  <configSections>" +
      "<section name=\"coreConfiguration\" type=\"Infrastructure.Core.Configuration.CoreConfiguration, Infrastructure.Core\" requirePermission=\"false\" />" +
      "</configSections>" +
          "<coreConfiguration>" +
      "<DynamicDiscovery Enabled=\"true\" />" +
      "<Engine Type=\"\" />" +
      "<Cryptography SaltValue=\"d41d8cd98f00b204e9800998ecf8427e\" PassPhrase=\"74be16979710d4c4e7c6647856088456\" PasswordIterations=\"5\"></Cryptography>" +
    "</coreConfiguration>";

    }
}