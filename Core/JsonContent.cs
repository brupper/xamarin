using System.Text;

// ReSharper disable once CheckNamespace
namespace System.Net.Http
{
    public class JsonContent : StringContent
    {
        public JsonContent(string json) : base(json, Encoding.UTF8, "application/json")
        {

        }
    }
}
