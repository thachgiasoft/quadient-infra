using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Infrastructure.Core.Configuration;
using Infrastructure.Exceptions.CryptographyExceptions;
using System.Linq;

namespace Infrastructure.Core.Cryptography
{
    /// <summary>
    /// Kripto islemlerini yoneten siniftir.
    /// </summary>
    public class CoreCryptography : ICoreCryptography
    {
        private readonly CoreConfiguration _coreConfiguration;
        // Define min and max salt sizes.
        const int MinSaltSize = 4;
        const int MaxSaltSize = 8;
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private readonly string _passPhrase;
        private readonly string _saltValue;
        private readonly int _passwordIterations;


        /// <summary>
        /// CoreCryptography sinifi icin default constructor.
        /// </summary>
        /// <param name="coreConfiguration">Crypto islemleri icin gerekli parametrelerin okunacagi config dosyasi.</param>
        public CoreCryptography(CoreConfiguration coreConfiguration)
        {
            _coreConfiguration = coreConfiguration;
            _passPhrase = _coreConfiguration.CryptographyConf.PassPhrase;
            _saltValue = _coreConfiguration.CryptographyConf.SaltValue;
            _passwordIterations = _coreConfiguration.CryptographyConf.PasswordIterations;
        }

        /// <summary>
        /// Girilen parametrelere gore plainText in ozetini alip byte[] olarak doner.
        /// </summary>
        /// <param name="plainText">Ozeti alinacak metin</param>
        /// <param name="hashAlgorithm">Ozet algoritmasi</param>
        /// <param name="saltValue">Tuzlama degeri</param>
        /// <param name="stringConversion">Tuzlama degerini byte[] e cevirmek icin kullanilacak yontem</param>
        /// <returns>Parametre olarak verilen acik metnin ozet bilgisini btye[] olarak doner.</returns>
        public byte[] ComputeHash(string plainText, HashAlgorithmEnum hashAlgorithm, string saltValue, StringConversionEnum stringConversion = StringConversionEnum.Base64)
        {
            // If salt is not specified, generate it on the fly.
            byte[] saltBytes = null;
            if (saltValue == null)
            {
                // Generate a random number for the size of the salt.
                var random = new Random();
                var saltSize = random.Next(MinSaltSize, MaxSaltSize);

                // Allocate a byte array, which will hold the salt.
                saltBytes = new byte[saltSize];

                // Initialize a random number generator.
                var rng = new RNGCryptoServiceProvider();

                // Fill the salt with cryptographically strong byte values.
                rng.GetNonZeroBytes(saltBytes);
            }
            else
            {
                if (stringConversion == StringConversionEnum.Base64)
                    saltBytes = Convert.FromBase64String(saltValue);
                else if (stringConversion == StringConversionEnum.Hex)
                    saltBytes = StringToByteArray(saltValue);

            }
            // Convert plain text into a byte array.
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Allocate array, which will hold plain text and salt.
            var plainTextWithSaltBytes =
                    new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (var i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (var i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            // Because we support multiple hashing algorithms, we must define
            // hash object as a common (abstract) base class. We will specify the
            // actual hashing algorithm class later during object creation.
            HashAlgorithm hash;


            // Initialize appropriate hashing algorithm class.
            switch (hashAlgorithm)
            {
                case HashAlgorithmEnum.SHA1:
                    hash = new SHA1Managed();
                    break;

                case HashAlgorithmEnum.SHA256:
                    hash = new SHA256Managed();
                    break;

                case HashAlgorithmEnum.SHA384:
                    hash = new SHA384Managed();
                    break;

                case HashAlgorithmEnum.SHA512:
                    hash = new SHA512Managed();
                    break;

                case HashAlgorithmEnum.MD5:
                    hash = new MD5CryptoServiceProvider();
                    break;
                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            // Compute hash value of our plain text with appended salt.
            var hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            var hashWithSaltBytes = new byte[hashBytes.Length +
                                                saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (var i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (var i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            return hashWithSaltBytes;
        }


        /// <summary>
        /// Girilen parametrelere gore plainText in ozetini alip byte[] olarak doner.
        /// </summary>
        /// <param name="plainText">Ozeti alinacak metin</param>
        /// <param name="hashAlgorithm">Ozet algoritmasi</param>
        /// <param name="stringConversion">Tuzlama degerini byte[] e cevirmek icin kullanilacak yontem</param>
        /// <returns>Parametre olarak verilen acik metnin ozet bilgisini string olarak doner.</returns>
        public string ComputeHash(string plainText, HashAlgorithmEnum hashAlgorithm, StringConversionEnum stringConversion = StringConversionEnum.Base64)
        {
            // Convert result into a base64-encoded string.
            var hashValue = string.Empty;
            if (stringConversion == StringConversionEnum.Base64)
                hashValue = Convert.ToBase64String(ComputeHash(plainText, hashAlgorithm, string.Empty));
            else if (stringConversion == StringConversionEnum.Hex)
                hashValue = ByteArrayToHexString(ComputeHash(plainText, hashAlgorithm, string.Empty));
            // Return the result.
            return hashValue;
        }

        /// <summary>
        /// Belirtilen metnin ozetini alarak hashValue parametresi ile belirtilen ozet degeriyle karsilastirir.
        /// </summary>
        /// <param name="plainText">Ozeti alinip karsilastirilacak acik metin</param>
        /// <param name="hashAlgorithm">Ozet algoritmasi</param>
        /// <param name="hashValue">Mevcut ozet degeri</param>
        /// <param name="stringConversion">Hash degerini byte[] e cevirmek icin secilen yontem</param>
        /// <returns>Belirtilen hashValue degeri ile yeni hesaplanan ozet degerinin ayni olup olmadigi (true,false) bilgisini dondurur.</returns>
        public bool VerifyHash(string plainText, HashAlgorithmEnum hashAlgorithm, string hashValue, StringConversionEnum stringConversion = StringConversionEnum.Base64)
        {
            // Convert base64-encoded hash value into a byte array.
            byte[] hashWithSaltBytes = null;
            if (stringConversion == StringConversionEnum.Base64)
                hashWithSaltBytes = Convert.FromBase64String(hashValue);
            else if (stringConversion == StringConversionEnum.Hex)
                hashWithSaltBytes = StringToByteArray(hashValue);
            // We must know size of hash (without salt).
            int hashSizeInBits;

            // Size of hash is based on the specified algorithm.
            switch (hashAlgorithm)
            {
                case HashAlgorithmEnum.SHA1:
                    hashSizeInBits = 160;
                    break;

                case HashAlgorithmEnum.SHA256:
                    hashSizeInBits = 256;
                    break;

                case HashAlgorithmEnum.SHA384:
                    hashSizeInBits = 384;
                    break;

                case HashAlgorithmEnum.SHA512:
                    hashSizeInBits = 512;
                    break;
                case HashAlgorithmEnum.MD5:
                    hashSizeInBits = 128;
                    break;
                default: // Must be MD5
                    hashSizeInBits = 128;
                    break;
            }

            // Convert size of hash from bits to bytes.
            var hashSizeInBytes = hashSizeInBits / 8;

            // Make sure that the specified hash value is long enough.
            if (hashWithSaltBytes.Length < hashSizeInBytes)
                return false;

            // Allocate array to hold original salt bytes retrieved from hash.
            var saltBytes = new byte[hashWithSaltBytes.Length -
                                        hashSizeInBytes];

            // Copy salt from the end of the hash to the new array.
            for (var i = 0; i < saltBytes.Length; i++)
                saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

            // Compute a new hash string.
            var expectedHashString = string.Empty;
            if (stringConversion == StringConversionEnum.Base64)
                expectedHashString = Convert.ToBase64String(ComputeHash(plainText, hashAlgorithm, Convert.ToBase64String(saltBytes)));
            else if (stringConversion == StringConversionEnum.Hex)
                expectedHashString = ByteArrayToHexString(ComputeHash(plainText, hashAlgorithm, ByteArrayToHexString(saltBytes)));

            // If the computed hash matches the specified hash,
            // the plain text value must be correct.
            return (hashValue == expectedHashString);
        }

        /// <summary>
        /// Girilen acik metni sifreler.
        /// </summary>
        /// <param name="plainText">Sifrelenmek istenen acik metin.</param>
        /// <returns>Sifreli metin bilgisini dondurur.</returns>
        public string Encrypt(string plainText)
        {
            return Encrypt(plainText, _passPhrase, _saltValue, _passwordIterations);
        }

        /// <summary>
        /// Girilen acik metni belirtilen parametreleri kullanarak sifreler.
        /// </summary>
        /// <param name="plainText">Sifrelenmek istenen acik metin.</param>
        /// <param name="passPhrase">Sifrelemede kullanilacak key uretimi icin anahtar kelime.</param>
        /// <param name="saltValue">Tuzlama degeri.</param>
        /// <param name="passwordIterations">Sifrelemede kullanilacak key uretimi icin donus sayisi.</param>
        /// <returns>Sifreli metin bilgisini dondurur.</returns>
        public string Encrypt(string plainText, string passPhrase, string saltValue, int passwordIterations)
        {

            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(passPhrase))
                throw new ArgumentNullException("passPhrase");

            string outStr;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.
            var saltBytes = Encoding.ASCII.GetBytes(saltValue);

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(passPhrase, saltBytes, passwordIterations);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            catch (Exception exception)
            {
                throw new EncryptionFailedException("Veri şifrelenirken hata oluştu! Şifrelenecek Veri : " + plainText, exception) { ValueToEncrypt = plainText };
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Girilen sifreli metni cozumler.
        /// </summary>
        /// <param name="cipherText">Sifreli metin.</param>
        /// <returns>Acik metin bilgisini dondurur.</returns>
        public string Decrypt(string cipherText)
        {
            return Decrypt(cipherText, _passPhrase, _saltValue, _passwordIterations);
        }

        /// <summary>
        /// Girilen sifreli metni belirtilen parametreleri kullanarak cozumler.
        /// </summary>
        /// <param name="cipherText">Sifreli metin.</param>
        /// <param name="passPhrase">Cozumlemede kullanilacak key uretimi icin anahtar kelime.</param>
        /// <param name="saltValue">Tuzlama degeri.</param>
        /// <param name="passwordIterations">Cozumlemede kullanilacak key uretimi icin donus sayisi.</param>
        /// <returns>Acik metin bilgisini dondurur.</returns>
        public string Decrypt(string cipherText, string passPhrase, string saltValue, int passwordIterations)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(passPhrase))
                throw new ArgumentNullException("passPhrase");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;
            var saltBytes = Encoding.ASCII.GetBytes(saltValue);

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(passPhrase, saltBytes, passwordIterations);

                // Create the streams used for decryption.                
                var bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new DecryptionFailedException("Veri deşifre edilirken hata oluştu. Şifreli Veri : " + cipherText, exception) { ValueToDecrypt = cipherText };
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        /// <summary>
        /// Belirtilen stream i byte[] e cevirir.
        /// </summary>
        /// <param name="s">Okunacak stream bilgisi.</param>
        /// <returns>Stream i okuyarak byte[] e cevirip byte[] degerini dondurur.</returns>
        private byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        /// <summary>
        /// Belirtilen acik metni url icin html encode kullanarak sifreler.
        /// </summary>
        /// <param name="plainText">Acik metin.</param>
        /// <returns>Belirtilen degeri url de kullanılabilecek formatta sifreli olarak dondurur.</returns>
        public string EncryptForUrl(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            var enctext = Encrypt(plainText);
            enctext = enctext.Replace("+", "*").Replace("/", "|"); //Url de patlayan kod için replace edildi.
            var enctextencoded = HttpUtility.UrlEncode(enctext);
            return enctextencoded;
        }

        /// <summary>
        /// Sifrelenmis url metnini cozumler.
        /// </summary>
        /// <param name="cipherText">Sifreli metin.</param>
        /// <returns>Acik metin bilgisini dondurur.</returns>
        public string DecryptForUrl(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            var dctextdecoded = HttpUtility.UrlDecode(cipherText);
            dctextdecoded = dctextdecoded.Replace("*", "+").Replace("|", "/"); //Url de patlayan kod için replace edildi.
            var plaintext = Decrypt(dctextdecoded);

            return plaintext;
        }

        private const string Password = "tcicisleri2009";
        /// <summary>
        /// Verilen texti sifreler.
        /// </summary>
        /// <param name="clearText">Şifrelenicek Text</param>
        /// <returns>string</returns>
        [Obsolete("This method is obsolete. Use method Encrypt()")]
        public string OldEncrypt(string pClearText)
        {
            if (string.IsNullOrEmpty(pClearText)) return string.Empty;

            string encriptedText = string.Empty;
            string retValue = string.Empty;
            try
            {
                byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(pClearText);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                encriptedText = Convert.ToBase64String(encryptedData);
                retValue = encriptedText.Replace("+", "*");
                retValue = retValue.Replace("/", "|");
                return retValue;
            }
            catch (Exception exc)
            {
                string mesaj = @"GelenDeger = '" + pClearText + "'" +
                                " \r\n| SifreliDeger = '" + encriptedText + "'" +
                                " \r\n| DonenDeger = '" + retValue + "'";

                exc.Data.Add("mesaj", mesaj);
                exc.Data.Add("method", "Encrypt");
                throw exc;
            }
        }

        /// <summary>
        /// Verilen şifreli texti deşifre eder.
        /// </summary>
        /// <param name="cipherText">Şifrelenmiş Text</param>
        /// <returns>string</returns>
        [Obsolete("This method is obsolete. Use method Decrypt()")]
        public string OldDecrypt(string pCipherText)
        {
            if (string.IsNullOrEmpty(pCipherText)) return string.Empty;

            string sifreliText = string.Empty;
            string retValue = string.Empty;
            try
            {
                sifreliText = pCipherText.Replace("*", "+");
                sifreliText = sifreliText.Replace("|", "/");
                byte[] cipherBytes = Convert.FromBase64String(sifreliText);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                retValue = System.Text.Encoding.Unicode.GetString(decryptedData);
                return retValue;
            }
            catch (Exception exc)
            {
                string mesaj = @"GelenDeger = '" + pCipherText + "'" +
                                " \r\n| OrjinalSifreliDeger = '" + sifreliText + "'" +
                                " \r\n| DonenDeger = '" + retValue + "'";

                exc.Data.Add("mesaj", mesaj);
                exc.Data.Add("method", "Decrypt");
                throw exc;
            }
        }

        [Obsolete("This method is obsolete. Use method Encrypt()")]
        private byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }

        [Obsolete("This method is obsolete. Use method Decrypt()")]
        private byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }

        /// <summary>
        /// Verilen hexadecimal string degeri byte[] e cevirir.
        /// </summary>
        /// <param name="hex">byte[] e cevrilmesi istenen string parametre</param>
        /// <returns>Girilen parametrenin byte[] karsiligini dondurur.</returns>
        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Verilen byte[] degerini hexadecimal string e cevirir.
        /// </summary>
        /// <param name="data">hexadecimal e cevrilmesi istenen byte[] parametre</param>
        /// <returns>Girilen parametrenin hexadecimal string karsiligini dondurur.</returns>
        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}