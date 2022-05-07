using System;
using System.Globalization;

namespace interview
{
    internal class Program
    {
        private static DateTime _dateTimeNow;

        private static void Main(string[] args)
        {
            var demoTimeParameter = new DateTime(2022, 05, 07, 20, 55, 0);
            var demoTimeParameters = new[]
            {
                demoTimeParameter, demoTimeParameter.AddMinutes(-55),
                demoTimeParameter.AddMinutes(1), demoTimeParameter.AddHours(-1)
            };

            foreach (var x in demoTimeParameters)
            {
                _dateTimeNow = x;

                var result = VerifyProductExpirationDate("EEEEE2022050721");

                Console.WriteLine($"結果:{result}\r\n");
            }
        }

        private static string VerifyProductExpirationDate(string barcode)
        {
            var (isSuccess, errorMsg) = BarcodeValidator(barcode);

            if (isSuccess.Equals(false))
            {
                return errorMsg;
            }

            var expirationDate = barcode.Substring(5, 10);
            var dateTimeNow = _dateTimeNow;

            if (DateTime.TryParseExact(
                    expirationDate,
                    "yyyyMMddHH",
                    null,
                    DateTimeStyles.AssumeLocal,
                    out var expirationDateTime).Equals(false))
            {
                return "條碼中的保存時間格式錯誤";
            }

            Console.WriteLine($"系統時間: {dateTimeNow:yyyy/MM/dd HH:mm:ss}");
            Console.WriteLine($"條碼: {barcode}");
            Console.WriteLine($"商品期限: {expirationDateTime:yyyy/MM/dd HH:mm:ss}");

            var effectiveMinute =
                new TimeSpan(expirationDateTime.Ticks - dateTimeNow.Ticks).TotalMinutes;

            if (effectiveMinute >= 5 && effectiveMinute <= 60)
            {
                return "商品即將到期";
            }

            if (effectiveMinute < 5)
            {
                return "商品已過期";
            }

            return "商品還在有效期限內";
        }

        private static (bool isSuccess, string errorMsg) BarcodeValidator(string barcode)
        {
            var target = barcode.Trim();

            if (string.IsNullOrWhiteSpace(target))
            {
                return (false, "輸入的條碼不可為空");
            }

            if (target.Length < 15)
            {
                return (false, "條碼長度需要為15碼");
            }

            return (true, "");
        }
    }
}