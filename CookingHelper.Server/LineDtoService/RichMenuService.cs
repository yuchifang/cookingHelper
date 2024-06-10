using System.Net.Http.Headers;
using CookingHelper.LineDto;
using CookingHelper.Enum;
using CookingHelper.ProviderGroup;
using System.Text;


namespace CookingHelper.LineDtoService
{
    public class RichMenuService
    {
        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "YourAccessToken";
        private readonly string channelSecret = "YourChannelSecret";

        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();

        public RichmenuService()
        {
        }

        public async void ValidateRichMenu()
        {

        }

        public async void CreateRichMenu()
        {

        }

        public async void GetRichMenuList()
        {

        }

        public async void UploadRichMenuImage()
        {

        }

        public async void SetDefaultRichMenu()
        {

        }
    }
}