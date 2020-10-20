## SimpleSMSBomber
Гибкий в настройке SMS Bomber, созданный на .Net Fremework C#.

Формат файла настроек Settings.json (должен находится в корне программы):
```json
{"sources": // Массив источников спама
	[
		{
			"uri": "https://mysite.com/api", // Uri для отправки запроса
			"method": "POST", // Метод запроса (POST/GET)
			"phone_parameter": "phone", // Название параметра запроса, содержащего в себе номер жертвы
			"cookies": [ // Массив Cookie
                {
                    "cookie": "mycookie", // Название Cookie
                    "value": "value" // Значение
                }
            ],
			"headers": [ // Массив заголовков запроса
                {
                    "header": "myheader", // Название заголовка
                    "value": "value" // Значение
                }
            ],
			"content_type": "application/json", // Заголовок Content-Type
			"accept": "application/json, text/javascript, */*; q=0.01", // Заголовок Accept
			"parameters": [ // Массив сторонних параметров запроса
                {
                    "parameter": "myparameter", // Название параметра
                    "value": "value" // Значение
                }
            ],
			"delay": 60000 // Задержка между запросами к источнику
		}
	],
	"proxies": [ // Массив с прокси
        {
            "ip": "127.0.0.1", // IP прокси
            "port": 8080 // Порт
        }
    ],
	"proxy_delay": 1, // Задержка между запросами к каждому источника для смены/включения прокси
	"multi_thread": true // Использование отдельного потока для каждого источника спама (true/false)
}
```
При поддержке форума SMM продвижения в социальных сетях - DataStock.biz
