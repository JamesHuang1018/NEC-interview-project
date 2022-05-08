using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace interview
{
    internal class Program
    {
        private static DateTime _dateTimeNow;

        private static void Main(string[] args)
        {
            Console.WriteLine("題目一: \r\n");
            QuestionOne(); 
            Console.WriteLine("------ \r\n");
            Console.WriteLine("題目二: \r\n");
            QuestionTwo();
        }

        private static void QuestionOne()
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

        private static void QuestionTwo()
        {
            _dateTimeNow = new DateTime(2015, 10, 27);
            var file = new[]
            {
                "IFaaaaaa20130101.1", "IFbbbbbb20141201.2", "IFcccccc20150101.3",
                "IFdddddd20151019.1", "IFeeeeee20151019.1", "IFffffff20151020.2",
                "IFgggggg20151021.3", "IFhhhhhh20151022.4", "IFiiiiii20151023.5",
                "IFjjjjjj20151024.6", "IFkkkkkk20151025.7", "IFllllll20151026.1",
                "IFmmmmmm20151027.2"
            };

            var effectiveFiles = RemoveOutdatedFiles(file, 6);
            var result = string.Join("\r\n", effectiveFiles);

            Console.WriteLine($"系統時間: {_dateTimeNow:yyyy/MM/dd HH:mm:ss}");
            Console.WriteLine($"有效檔案: \r\n{result}");
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

        private static IEnumerable<string> RemoveOutdatedFiles(
            IEnumerable<string> file,
            int obsoleteDay)
        {
            var fileInfo = file.Select(x =>
            {
                var (isSuccess, errorMsg) = FileNameValidator(x);

                if (isSuccess.Equals(false))
                {
                    throw new ArgumentException($"{x} => {errorMsg}");
                }

                var fileDate = x.Substring(8, 8);
                var dateTimeNow = _dateTimeNow;

                if (DateTime.TryParseExact(
                        fileDate,
                        "yyyyMMdd",
                        null,
                        DateTimeStyles.AssumeLocal,
                        out var fileDateTime).Equals(false))
                {
                    throw new ArgumentException($"{x} => 檔名中的時間格式錯誤");
                }

                var differenceDays =
                    new TimeSpan(dateTimeNow.Ticks - fileDateTime.Ticks).TotalDays;

                return new
                {
                    fileName = x,
                    differenceDays
                };
            });

            var effectiveFiles = fileInfo.Where(x => x.differenceDays <= obsoleteDay)
                .Select(x => x.fileName);

            return effectiveFiles;
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

        private static (bool isSuccess, string errorMsg) FileNameValidator(
            string fileName)
        {
            var target = fileName.Trim();

            if (string.IsNullOrWhiteSpace(target))
            {
                return (false, "檔案名稱不可為空");
            }

            if (target.Length < 18)
            {
                return (false, "檔案名稱長度需要18碼");
            }

            return (true, "");
        }
    }
}