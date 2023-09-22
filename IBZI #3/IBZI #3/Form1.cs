using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace IBZI__3
{
    public partial class Form1 : Form
    {
        private string generatedKey;

        public Form1()
        {
            InitializeComponent();

            comboBox1.Items.Add("128 бит");
            comboBox1.Items.Add("256 бит");
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string plainText = textBox1.Text;
            string key = generatedKey;
            int keySizeInBits = comboBox1.SelectedItem.ToString() == "256 бит" ? 256 : 128;

            string encryptedText = Encrypt(plainText, key, keySizeInBits);

            textBox3.Text = encryptedText;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string cipherText = textBox3.Text;
            string key = generatedKey;
            int keySizeInBits = comboBox1.SelectedItem.ToString() == "256 бит" ? 256 : 128;

            string decryptedText = Decrypt(cipherText, key, keySizeInBits);

            textBox2.Text = decryptedText;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int keySizeInBits = 128; // По умолчанию 128 бит
            if (comboBox1.SelectedItem.ToString() == "256 бит")
            {
                keySizeInBits = 256;
            }

            byte[] keyBytes = GenerateRandomKey(keySizeInBits / 8); // Передайте размер в байтах
            generatedKey = BitConverter.ToString(keyBytes).Replace("-", "");
            textBox4.Text = generatedKey;
        }

        private byte[] GenerateRandomKey(int keySizeInBytes)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] key = new byte[keySizeInBytes];
                rng.GetBytes(key);
                return key;
            }
        }

        private string Encrypt(string plainText, string key, int keySizeInBits)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Преобразуем ключ в байтовый массив нужной длины
                int keySizeInBytes = keySizeInBits / 8;
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                // Если ключ короче, чем ожидаемая длина, увеличьте его до нужной длины
                if (keyBytes.Length != keySizeInBytes)
                {
                    Array.Resize(ref keyBytes, keySizeInBytes);
                }

                aesAlg.Key = keyBytes;
                aesAlg.Mode = CipherMode.CFB;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    byte[] encryptedBytes = msEncrypt.ToArray();
                    byte[] combinedBytes = new byte[aesAlg.IV.Length + encryptedBytes.Length];
                    Array.Copy(aesAlg.IV, combinedBytes, aesAlg.IV.Length);
                    Array.Copy(encryptedBytes, 0, combinedBytes, aesAlg.IV.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(combinedBytes);
                }
            }
        }

        private string Decrypt(string cipherText, string key, int keySizeInBits)
        {
            keySizeInBits = keySizeInBits == 256 ? 256 : 128;
            byte[] combinedBytes = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CFB;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Определите длину ключа в байтах
                int keySizeInBytes = keySizeInBits / 8;

                // Преобразуйте ключ в байтовый массив нужной длины
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);

                // Если ключ короче, чем ожидаемая длина, увеличьте его до нужной длины
                if (keyBytes.Length != keySizeInBytes)
                {
                    Array.Resize(ref keyBytes, keySizeInBytes);
                }

                aesAlg.Key = keyBytes;

                if (combinedBytes.Length <= aesAlg.IV.Length)
                {
                    throw new InvalidOperationException("Недопустимый шифротекст.");
                }

                byte[] iv = new byte[aesAlg.IV.Length];
                byte[] encryptedBytes = new byte[combinedBytes.Length - aesAlg.IV.Length];
                Array.Copy(combinedBytes, iv, iv.Length);
                Array.Copy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
                {
                    using (var msDecrypt = new MemoryStream(encryptedBytes))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
