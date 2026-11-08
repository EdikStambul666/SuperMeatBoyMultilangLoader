
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace SuperMeatBoyMultilangLoader
{
    class SuperMeatBoyLanguagePatcher
    {
        private Dictionary<string, byte> languages =
            new Dictionary<string, byte>();

        private string exePath;
        private string language;

        string[] languageNames = {
            "debug", "english", "japanese", "german",
            "french", "spanish", "italian", "korean",
            "chinese", "portuguese", "chinese-fontonly",
            "russian", "poilish"
        };

        public SuperMeatBoyLanguagePatcher()
        {
            for (int i = 0; i < languageNames.Length; i++)
            {
                languages.Add(languageNames[i], (byte)i);
            }
        }

        public string[] LanguageNames
        {
            get { return languageNames; }
        }

        public string Language
        {
            get { return language; }
            set { language = value.ToLower(); }
        }

        public string SuperMeatBoyPath
        {
            get { return exePath; }
            set { exePath = value; }
        }

        public void Launch()
        {
            Process process = new Process();

            Console.WriteLine(
                AppDomain.CurrentDomain.BaseDirectory);

            process.StartInfo.FileName = exePath;
            process.StartInfo.WorkingDirectory =
                Path.GetDirectoryName(
                    Path.GetFullPath(exePath));

            process.Start();

            IntPtr baseAddress =
                (from ProcessModule mod in process.Modules
                 where mod.ModuleName ==
                     Path.GetFileName(exePath)
                 select mod.BaseAddress)
                .SingleOrDefault();

            if (baseAddress == IntPtr.Zero)
            {
                Console.WriteLine(
                    "ERROR: Game module not found!");
                return;
            }

            IntPtr getLanguageAddress =
                IntPtr.Add(baseAddress, 0x4490);

            Console.WriteLine(
                "Game process ID: " + process.Id);

            Console.WriteLine(
                "Base address: 0x{0:X8}",
                baseAddress.ToInt32());

            Console.WriteLine(
                "Patch address: 0x{0:X8}",
                getLanguageAddress.ToInt32());

            Console.WriteLine(
                "Selected language: " + language);

            Console.WriteLine(
                "Language code: " + LanguageCode);

            byte[] newData = new byte[]
            {
                0xB8,
                LanguageCodeInByte,
                0x00,
                0x00,
                0x00,
                0xC3
            };

            int writtenBytes = 0;

            Memory memory = new Memory();
            memory.ReadProcess = process;
            memory.Open();

            Console.WriteLine("Attempting patch...");

            memory.Write(
                getLanguageAddress,
                newData,
                out writtenBytes);

            Console.WriteLine(
                "Bytes written: " + writtenBytes);

            Console.WriteLine(
                "Expected bytes: " + newData.Length);

            if (writtenBytes == newData.Length)
            {
                Console.WriteLine(
                    "Write completed.");
            }
            else
            {
                Console.WriteLine(
                    "WARNING: Incomplete memory write!");
            }

            Console.WriteLine(
                "NOTE: Write success does not prove the address is correct.");
        }

        public int LanguageCode
        {
            get
            {
                byte code = 0x00;
                languages.TryGetValue(language, out code);
                return (int)code;
            }
        }

        public byte LanguageCodeInByte
        {
            get
            {
                byte code = 0x00;
                languages.TryGetValue(language, out code);
                return code;
            }
        }
    }
}
