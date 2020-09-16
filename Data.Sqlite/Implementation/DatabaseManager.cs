using Brupper.Data.Sqlite.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace Brupper.Data.Sqlite.Implementation
{
    public class DatabaseManager : IDatabaseManager
    {
        #region Members

        public string DatabaseName { get; set; }
            = "default.db";

        public string DatabasePath { get; private set; }

        private static readonly string CreateKey = "Create";
        private static readonly string QuerySeparator = ";;";
        private static readonly AsyncLock initLock = new AsyncLock();

        private static bool isInitiated;

        private readonly Assembly embeddedScriptAssembly;
        private readonly string embeddedResourceBaseName;

        private readonly ISqliteService sqliteService;
        private readonly IFileSystem fileSystemService;

        #endregion

        #region Contructors

        public DatabaseManager(ISqliteService sqliteService, IFileSystem fileSystemService)
            : this(sqliteService, fileSystemService, typeof(DatabaseManager).GetTypeInfo().Assembly, "Brupper.Data.Sqlite.Scripts")
        {
        }

        protected DatabaseManager(
            ISqliteService sqliteService,
            IFileSystem fileSystemService,
            Assembly embeddedScriptAssembly,
            string embeddedResourceBaseName)
        {
            this.sqliteService = sqliteService;
            this.fileSystemService = fileSystemService;
            this.embeddedScriptAssembly = embeddedScriptAssembly;
            this.embeddedResourceBaseName = embeddedResourceBaseName;

            isInitiated = false;
        }

        #endregion

        public async Task<ISqliteService> GetSqliteServiceAsync()
        {
            await InitializeSqliteServiceAsync();
            return sqliteService;
        }

        public Task ResetDatabaseAsync()
        {
            ResetConnection();

            try
            {
                File.Delete(DatabaseName);
                return InitializeSqliteServiceAsync();
            }
            catch
            {
                throw new Exception("Could not delete database");
            }
        }

        public async Task InitializeSqliteServiceAsync()
        {
            DatabasePath = Path.Combine(fileSystemService.AppDataDirectory, DatabaseName);

            if (!sqliteService.IsInitialized | !isInitiated)
            {
                using (await initLock.LockAsync())
                {
                    await CopyDatabaseIfNotExistsAsync(DatabasePath, DatabaseName);

                    if (!sqliteService.IsInitialized)
                    {
                        sqliteService.InitConnection(DatabasePath);
                        if (sqliteService.IsInitialized)
                        {
                            await PrepareDatabaseAsync();
                            isInitiated = true;

                            await Task.Delay(1500);
                        }
                        else
                        {
                            throw new Exception("Cannot create database file.");
                        }
                    }
                }
            }
        }

        private async Task PrepareDatabaseAsync(ISqliteService service = null)
        {
            var sqlite = service ?? sqliteService;
            var resources = GetSqlResources();
            var baseResourceName = $"{embeddedResourceBaseName}.{DatabaseName}.";
            var version = await sqlite.GetVersionAsync();

            if (version == SqlDbVersion.None)
            {
                await InitializeDb(baseResourceName, resources, sqlite);
            }
            else
            {
                await UpdateDb(baseResourceName, version, resources, sqlite);
            }
        }

        private List<KeyValuePair<string, string>> GetSqlResources()
        {
            var results = new List<KeyValuePair<string, string>>();

            foreach (var name in embeddedScriptAssembly.GetManifestResourceNames())
            {
                using (var reader = new StreamReader(embeddedScriptAssembly.GetManifestResourceStream(name)))
                {
                    var content = reader.ReadToEnd();
                    results.Add(!name.StartsWith(embeddedResourceBaseName)
                        ? new KeyValuePair<string, string>(embeddedResourceBaseName + "." + name, content)
                        : new KeyValuePair<string, string>(name, content));
                }
            }

            return results;
        }

        private static async Task InitializeDb(
            string baseResourceName,
            List<KeyValuePair<string, string>> resources,
            ISqliteService sqliteService)
        {
            var expectedResourceName = baseResourceName + CreateKey;
            var index = resources.FindIndex(kvp => kvp.Key.StartsWith(expectedResourceName));
            if (index == -1)
            {
                throw new InvalidOperationException($"Cannot create initial table: Cannot find resource that started with {expectedResourceName}");
            }

            await ApplyPatch(sqliteService, resources[index].Value);
        }

        private static async Task UpdateDb(
            string baseResourceName,
            SqlDbVersion currentVersion,
            IEnumerable<KeyValuePair<string, string>> resources,
            ISqliteService sqliteService)
        {
            var scripts = GetSortedScriptToExecute(baseResourceName, currentVersion, resources);

            await sqliteService.ExecuteAsync("PRAGMA foreign_keys = OFF");

            foreach (var script in scripts)
            {
                await ApplyPatch(sqliteService, script.Value);
            }

            await sqliteService.ExecuteAsync("PRAGMA foreign_keys = ON");
        }

        private static Task ApplyPatch(ISqliteService sqliteService, string script)
        {
            script = script.Trim();

            var queries = script.Split(new[] { QuerySeparator }, StringSplitOptions.RemoveEmptyEntries);

            return sqliteService.ExecuteScriptAsync(queries);
        }

        private static List<KeyValuePair<string, string>> GetSortedScriptToExecute(
            string baseResourceName,
            SqlDbVersion currentVersion,
            IEnumerable<KeyValuePair<string, string>> resources)
        {
            var scripts = new List<KeyValuePair<string, string>>();

            foreach (var resource in resources)
            {
                var resourceName = resource.Key;

                if (resourceName.EndsWith(".sql"))
                {
                    var splitted = resourceName.Split('.');
                    if (splitted.Length > 1)
                    {
                        var version = splitted[splitted.Length - 2];
                        var content = resource.Value;
                        var versionCompare = int.TryParse(version, out int versionNumber);
                        versionCompare = int.TryParse(currentVersion.Value ?? "0", out int currentVersionNumber) && versionCompare;

                        if (version != CreateKey && versionCompare && versionNumber > currentVersionNumber)
                        {
                            scripts.Add(new KeyValuePair<string, string>(version, content));
                        }
                    }
                }
            }

            // Sort by version
            scripts.Sort((v1, v2) => int.Parse(v1.Key).CompareTo(int.Parse(v2.Key)));

            return scripts;
        }

        private void ResetConnection()
        {
            sqliteService.CloseConnection();
        }

        private async Task CopyDatabaseIfNotExistsAsync(string dbPath, string fileName)
        {
            if (!File.Exists(dbPath))
            {
                var stream = await fileSystemService.OpenAppPackageFileAsync(fileName);
                using (var fs = File.Create(dbPath))
                {
                    stream.CopyTo(fs);
                }
            }
        }
    }
}
