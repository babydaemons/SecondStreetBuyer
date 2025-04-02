using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondStreetBuyer
{
    public class ItemLinks
    {
        public const string MensWare = "https://www.2ndstreet.jp/search?goodsType=1&other%5B%5D=nflg&sortBy=arrival";
        public const string LadiesWare = "https://www.2ndstreet.jp/search?goodsType=2&other%5B%5D=nflg&sortBy=arrival";
        public const string FassionGoods = "https://www.2ndstreet.jp/search?goodsType=3&other%5B%5D=nflg&sortBy=arrival";
        public const string Luxury = "https://www.2ndstreet.jp/search?brand%5B%5D=000278&brand%5B%5D=000286&brand%5B%5D=000293&brand%5B%5D=000296&brand%5B%5D=000172&brand%5B%5D=000196&brand%5B%5D=000226&brand%5B%5D=000227&brand%5B%5D=000251&brand%5B%5D=000272&brand%5B%5D=000482&brand%5B%5D=000579&brand%5B%5D=000583&brand%5B%5D=000589&brand%5B%5D=000590&brand%5B%5D=000614&brand%5B%5D=000629&brand%5B%5D=000668&brand%5B%5D=000745&brand%5B%5D=000337&brand%5B%5D=000378&brand%5B%5D=000430&brand%5B%5D=001042&brand%5B%5D=001137&brand%5B%5D=001151&brand%5B%5D=001168&brand%5B%5D=001235&brand%5B%5D=001326&brand%5B%5D=000920&brand%5B%5D=000924&brand%5B%5D=000931&brand%5B%5D=001729&brand%5B%5D=001735&brand%5B%5D=001504&brand%5B%5D=002753&brand%5B%5D=004021&brand%5B%5D=004517&brand%5B%5D=004597&brand%5B%5D=004820&brand%5B%5D=004854&brand%5B%5D=004857&brand%5B%5D=004858&brand%5B%5D=004374&brand%5B%5D=004375&brand%5B%5D=004377&brand%5B%5D=004378&brand%5B%5D=005495&brand%5B%5D=007005&brand%5B%5D=008132&brand%5B%5D=009860&brand%5B%5D=008384&brand%5B%5D=007776&brand%5B%5D=010721&brand%5B%5D=004856&brand%5B%5D=013677&brand%5B%5D=001599&brand%5B%5D=000507&brand%5B%5D=000535&brand%5B%5D=000147&brand%5B%5D=000219&brand%5B%5D=016961&brand%5B%5D=018111&brand%5B%5D=018108&brand%5B%5D=001200&brand%5B%5D=001264&brand%5B%5D=001355&other%5B%5D=nflg&sortBy=arrival";
        public const string PopularBrands = "https://www.2ndstreet.jp/search?brand%5B%5D=016199&brand%5B%5D=018753&brand%5B%5D=023943&brand%5B%5D=006393&brand%5B%5D=003453&brand%5B%5D=003835&brand%5B%5D=003103&brand%5B%5D=000923&brand%5B%5D=006215&brand%5B%5D=019740&brand%5B%5D=015615&brand%5B%5D=012861&brand%5B%5D=000010&brand%5B%5D=003236&brand%5B%5D=005198&brand%5B%5D=016976&brand%5B%5D=016883&brand%5B%5D=019539&brand%5B%5D=003217&brand%5B%5D=020038&brand%5B%5D=019587&brand%5B%5D=014479&brand%5B%5D=006589&brand%5B%5D=002052&brand%5B%5D=001666&brand%5B%5D=015980&brand%5B%5D=016472&brand%5B%5D=016558&brand%5B%5D=011369&brand%5B%5D=004545&brand%5B%5D=014108&brand%5B%5D=019579&brand%5B%5D=006403&brand%5B%5D=011087&brand%5B%5D=016579&brand%5B%5D=004639&brand%5B%5D=019770&brand%5B%5D=016622&other%5B%5D=nflg&sortBy=arrival";

        private IPage _page;

        public ItemLinks(IPage page)
        {
            _page = page;
        }

        public async Task<IEnumerable<Item>> GetItemLinks(string url)
        {
            await _page.GotoAsync(url);
            var items = await _page.QuerySelectorAllAsync("#searchResultListWrapper > ul > li");
            var itemList = new List<Item>();

            foreach (var item in items)
            {
                var linkElem = await item.QuerySelectorAsync("a");
                var urlPath = await linkElem?.GetAttributeAsync("href") ?? string.Empty;
                var fullUrl = string.IsNullOrEmpty(urlPath) ? string.Empty : $"https://www.2ndstreet.jp{urlPath}";

                var imgElem = await item.QuerySelectorAsync("img");
                var imageUrl = await imgElem?.GetAttributeAsync("src") ?? string.Empty;

                var brand = await item.QuerySelectorAsync(".itemCard_brand") is IElementHandle brandElem
                    ? await brandElem.InnerTextAsync()
                    : string.Empty;

                var name = await item.QuerySelectorAsync(".itemCard_name") is IElementHandle nameElem
                    ? await nameElem.InnerTextAsync()
                    : string.Empty;

                var size = await item.QuerySelectorAsync(".itemCard_size") is IElementHandle sizeElem
                    ? await sizeElem.InnerTextAsync()
                    : string.Empty;

                var priceText = await item.QuerySelectorAsync(".itemCard_price") is IElementHandle priceElem
                    ? await priceElem.InnerTextAsync()
                    : "¥0";

                // 数値部分のみ抽出してintに変換（カンマや¥を削除）
                var price = int.TryParse(
                    new string(priceText.Where(char.IsDigit).ToArray()),
                    out var parsedPrice
                ) ? parsedPrice : 0;

                itemList.Add(new Item
                {
                    Url = fullUrl,
                    Brand = brand,
                    Name = name,
                    Size = size,
                    Price = price
                });
            }

            return itemList;
        }
    }

    public struct Item
    {
        public string Url;
        public string ImageUrl;
        public string Brand;
        public string Name;
        public string Size;
        public int Price;
    }
}
