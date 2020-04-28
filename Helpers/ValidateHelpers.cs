using System.Text;

namespace iTunesSearcher.Helpers
{
    static class ValidateHelpers
    {
        public static string ValidateArtist(this string artist)
        {
            var temp = artist.Trim().ToLower().Replace(' ', '+');

            StringBuilder valid = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] == '+' && temp[i + 1] == '+')
                    valid.Append("");
                else
                    valid.Append(temp[i]);
            }

            return valid.ToString();
        }
    }
}
