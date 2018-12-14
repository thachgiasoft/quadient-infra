namespace Infrastructure.Core.Cryptography
{

    public interface ICoreCryptography
    {
        byte[] ComputeHash(string plainText, HashAlgorithmEnum hashAlgorithm, string saltValue, StringConversionEnum stringConversion = StringConversionEnum.Base64);
        string ComputeHash(string plainText, HashAlgorithmEnum hashAlgorithm,StringConversionEnum stringConversion=StringConversionEnum.Base64);
        bool VerifyHash(string plainText, HashAlgorithmEnum hashAlgorithm, string hashValue, StringConversionEnum stringConversion = StringConversionEnum.Base64);
        string Encrypt(string plainText);
        string Encrypt(string plainText, string passPhrase, string saltValue, int passwordIterations);
        string Decrypt(string cipherText);
        string Decrypt(string cipherText, string passPhrase, string saltValue, int passwordIterations);
        string EncryptForUrl(string plainText);
        string DecryptForUrl(string cipherText);

        [System.Obsolete("This method is obsolete. Use method Encrypt()")]
        string OldEncrypt(string pClearText);
        [System.Obsolete("This method is obsolete. Use method Decrypt()")]
        string OldDecrypt(string pCipherText);

    }
}
