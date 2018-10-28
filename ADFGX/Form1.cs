using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADFGX
{
    public partial class Form1 : Form
    {
        string alphabet = "ABCČDEFGHIJKLMNOPRSŠTUVZŽ".ToLower();
        string numbers = "0123456789";
        string adfgvx = "adfgvx".ToUpper();
        string textInFile;


        public Form1()
        {
            InitializeComponent();
        }

        private void openFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files|*.txt";
            openFileDialog1.Title = "Select a text File";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;
                textInFile = System.IO.File.ReadAllText(@filename);
                fileText.Text = textInFile;
            }
        }



        private void fileText_TextChanged(object sender, EventArgs e)
        {

        }

        private void encryptBtn_Click(object sender, EventArgs e)
        {
            string encText = encrypt(textInFile.ToLower());
            fileText.Text = encText;
            System.IO.File.WriteAllText(@Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\encryptedADFGVX.txt", encText);

        }

        private void decryptBtn_Click(object sender, EventArgs e)
        {
            string decText = decrypt(textInFile.ToLower());
            fileText.Text = decText;
            System.IO.File.WriteAllText(@Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\decryptedADFGVX.txt", decText);

        }

        private string encrypt(string text)
        {
            string encrypted = "";
            text = cleanText(text);
            Dictionary<char, string> square = generateSquare(keyText.Text);
            
            List<char> someList = new List<char>();
            foreach(char c in square.Keys)
            {
                someList.Add(c);
            }

            for(int i = 0; i < text.Length; i++)
            {
                encrypted = encrypted + square[text[i]];
            }
            string key = keyText.Text;
            List<string> keyMatrix = sortMatrix(encrypted, key);
            for (int i = 0; i < keyMatrix.Count; i++)
            {
                encrypted += keyMatrix[i].Substring(1);
            }
            return encrypted;
        }
        private string decrypt(string text)
        {
            string decrypted = "";
            text = text.ToUpper();
            Dictionary<string, char> square = generateDecryptionSquare(keyText.Text);
            string key = keyText.Text;
            List<string> keyMatrix = new List<string>(key.Length);

            for (int i = 0; i < key.Length; i++)
            {
                keyMatrix.Add(key[i].ToString());
            }
            keyMatrix.Sort();
            string sortedKey = "";
            for(int i = 0; i < keyMatrix.Count; i++)
            {
                sortedKey += keyMatrix[i];
            }
 
            List<string> fittedText = fitTextToSquare(text, sortedKey);
            List<string> unsortedList = unsortMatrix(fittedText);
            string unsorted = "";
            for (int i = 0; i < unsortedList.Count; i++)
            {
                unsorted += unsortedList[i].Substring(1);
            }
            for (int i = 0; i < unsorted.Length; i = i + 2)
            {
                decrypted = decrypted + square[unsorted[i].ToString() + unsorted[i+1].ToString()];
            }
            return decrypted;
        }

        private List<string> fitTextToSquare(string text, string key)
        {
            List<string> keyMatrix = new List<string>(key.Length);

            for (int i = 0; i < key.Length; i++)
            {
                keyMatrix.Add(key[i].ToString());
            }
            var sizesDict = new Dictionary<char, int>();
            foreach (char c in key)
            {
                sizesDict.Add(c, 0);
            }

            int idx = 0;
            foreach (char _ in text)
            {
                sizesDict[key[idx]]++;
                idx = idx == key.Length - 1 ? 0 : idx + 1;
            }
            int matrixSpot = 0;
            for (int i = 0; i < text.Length; i = i + 2)
            {
                keyMatrix[matrixSpot] += text[i].ToString() + text[i + 1].ToString();
 
                if (keyMatrix[matrixSpot].Length == sizesDict[keyMatrix[matrixSpot][0]])
                {
                    matrixSpot++;
                }
            }
            return keyMatrix;
        }

        private string cleanText(string text)
        {
            string cleanedText = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (alphabet.IndexOf(text[i]) > -1 || numbers.IndexOf(text[i]) > -1)
                {
                    cleanedText += text[i];
                }
            }
            return cleanedText;
        }

        private Dictionary<char, string> generateSquare(string text)
        {
            Dictionary<char, string> dict = new Dictionary<char, string>();
            string selectedChars = text.Substring(0, 6);
            string addedChars = "";
            int charsCounter = 0;
            int numCounter = 0;
            for(int i = 0; i < 6; i++)
            {
                for(int j = 0; j < 6; j++)
                {
                    if(charsCounter < 6) {
                        if(addedChars.IndexOf(selectedChars[charsCounter]) > -1) { 
                        dict.Add(selectedChars[charsCounter], (adfgvx[i].ToString() + adfgvx[j].ToString()));
                        addedChars = addedChars + selectedChars[charsCounter];
                        } else
                        {
                            j--;
                        }
                        charsCounter++;
                    } else
                    {
                        if (j % 2 > 0 && numCounter < 10)
                        {
                            dict.Add(numbers[numCounter], (adfgvx[i].ToString() + adfgvx[j].ToString()));
                            addedChars = addedChars + numbers[numCounter];
                            numCounter++;
                        } else {
                        for (int k = 0; k < alphabet.Length; k++)
                        {
                            if(addedChars.IndexOf(alphabet[k]) < 0)
                            {
                                dict.Add(alphabet[k], (adfgvx[i].ToString() + adfgvx[j].ToString()));
                                addedChars = addedChars + alphabet[k];
                                break;
                            }
                        }
                        }
                    }
    
                }
            }
            return dict;
        }

        private Dictionary<string, char> generateDecryptionSquare(string text)
        {
            Dictionary<string, char> dict = new Dictionary<string, char>();
            string selectedChars = text.Substring(0, 6);
            string addedChars = "";
            int charsCounter = 0;
            int numCounter = 0;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (charsCounter < 6)
                    {
                        if (dict.ContainsKey((adfgvx[i].ToString() + adfgvx[j].ToString()))) {
                            
                        } else { 
                        if (addedChars.IndexOf(selectedChars[charsCounter]) > -1)
                        {
                            dict.Add((adfgvx[i].ToString() + adfgvx[j].ToString()), selectedChars[charsCounter]);
                            addedChars = addedChars + selectedChars[charsCounter];
                        }
                        else
                        {
                            j--;
                        }
                        }
                        charsCounter++;
                    }
                    else
                    {
                        if (j % 2 > 0 && numCounter < 10)
                        {
                            if (dict.ContainsKey((adfgvx[i].ToString() + adfgvx[j].ToString())))
                            {
                                numCounter++;
                                continue;
                            }
                            dict.Add((adfgvx[i].ToString() + adfgvx[j].ToString()), numbers[numCounter]);
                            addedChars = addedChars + numbers[numCounter];
                            numCounter++;
                        } else { 
                        for (int k = 0; k < alphabet.Length; k++)
                        {
                            
                            if (addedChars.IndexOf(alphabet[k]) < 0)
                            {
                                if (dict.ContainsKey((adfgvx[i].ToString() + adfgvx[j].ToString()))) {
                                    continue;
                                }
                                dict.Add((adfgvx[i].ToString() + adfgvx[j].ToString()), alphabet[k]);
                                addedChars = addedChars + alphabet[k];
                                break;
                            }
                        }
                        }
                    }
                    
                }
            }
            return dict;
        }

        private List<string> sortMatrix(string prevStepResult, string key)
        {
            string sorted = "";
            List<string> keyMatrix = new List<string>(key.Length);

            for (int i = 0; i < key.Length; i++)
            {
                keyMatrix.Add(key[i].ToString());
            }
            int matrixSpot = 0;
            for(int i = 0; i < prevStepResult.Length; i = i + 2)
            {
                keyMatrix[matrixSpot] += prevStepResult[i].ToString() + prevStepResult[i + 1].ToString();
                if(matrixSpot == keyMatrix.Count - 1)
                {
                    matrixSpot = 0;
                } else
                {
                    matrixSpot++;
                }
            }
            
            keyMatrix.Sort();
  
            return keyMatrix;
        }
        
        private List<string> unsortMatrix(List<string> sorted) {
            string key = keyText.Text;
            List<string> unsorted = new List<string>(key.Length);
            
            for(int i = 0; i < key.Length; i++)
            {
                for(int j = 0; j < sorted.Count; j++)
                {
                    if(sorted[j].ToCharArray()[0] == key[i])
                    {
                        unsorted.Add(sorted[j]);
                    }
                }
            }
            return unsorted;
        }

    }
}
