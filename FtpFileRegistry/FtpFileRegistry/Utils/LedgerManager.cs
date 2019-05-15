using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using FtpFileRegistry.Models;
using FtpFileRegistry.Workers;

namespace FtpFileRegistry.Utils
{
    public class LedgerManager
    {
        private const string LedgerFileName = "ledger.csv";
        private static readonly string LedgerLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LedgerFileName);
        private static List<LedgerRowModel> _ledger = new List<LedgerRowModel>();
        private static LedgerManager _instance;

        private LedgerManager()
        {
            var ledgerFile = new FileInfo(LedgerLocation);
            if(!ledgerFile.Exists)
                SaveLedger(false);
        }

        public LedgerManager AddDatabaseToLedger(LedgerRowModel rowModel)
        {
            LoadLedger();
            if(!_ledger.Contains(rowModel))
                _ledger.Add(rowModel);
            return this;
        }

        public static LedgerManager GetManager()
        {
            return _instance ?? (_instance = new LedgerManager());
        }

        public static void LoadLedger()
        {
            var settings = SettingsLoader.LoadSettings();
            var downloader = new Downloader(AppDomain.CurrentDomain.BaseDirectory, settings.FtpTargetPath + "//" + LedgerFileName, false);
            var ledgerFile = new FileInfo(LedgerLocation);

            downloader.Start();
            while (downloader.DownloadResult == Downloader.Result.InProgress)
            {
                Application.DoEvents();
            }

            if (!ledgerFile.Exists)
            {
                SaveLedger();
            }

            using (var reader = new StreamReader(LedgerLocation))
            using (var csv = new CsvReader(reader))
            {
                _ledger = csv.GetRecords<LedgerRowModel>().ToList();
            }
        }

        public static void SaveLedger(bool upload = true)
        {
            using (var streamWriter = new StreamWriter(LedgerLocation))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.WriteRecords(_ledger);
            }
            if(upload)
                new Uploader(LedgerLocation, false).Start();
        }
    }
}
