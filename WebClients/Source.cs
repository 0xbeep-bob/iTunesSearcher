using System.Net.Http;
using System.Threading.Tasks;

namespace iTunesSearcher.WebClients
{
    public class Source
    {
        public static Source Instance => new Source();
        private Source() { }

        HttpClient client = new HttpClient();

        public async Task<string> GetDataAsync(string source)
        {
            var response = await client.GetAsync(source);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
