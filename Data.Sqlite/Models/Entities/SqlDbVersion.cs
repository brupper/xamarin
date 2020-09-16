namespace Brupper.Data.Sqlite.Models.Entities
{
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SqlDbVersion
    {
        /// <summary> Gets object when no version exists. </summary>
        public static SqlDbVersion None { get; } = new SqlDbVersion { Value = string.Empty };

        /// <summary> Gets or sets the version value as string </summary>
        public string Value { get; set; }

        private string DebuggerDisplay => this == SqlDbVersion.None ? "None" : Value;

    }
}
