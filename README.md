# OnlineJudge

## Вход и роли

При запуске API применяет миграции EF Core к PostgreSQL. Миграция создаёт таблицу `JwtSessions` и роли `user` и `admin`. Права на изменение схемы нужны учётной записи из `ConnectionStrings:DefaultConnection`. Уже зарегистрированные участники сохраняют аккаунты. При первом успешном входе сервер заменяет старый хеш пароля хешем ASP.NET Core Identity. После обновления нужно войти заново.

В среде Development сервер может работать с временным ключом подписи. Перезапуск в таком режиме завершает выданные сессии. Чтобы сохранять их между перезапусками, укажите постоянный ключ. В остальных средах ключ обязателен. Задайте переменную окружения **серверного процесса** `Jwt__SigningKey` через хранилище секретов. Значение должно содержать не менее 32 случайных байтов в Base64. Для генерации значения в текущей сессии PowerShell:

```powershell
$jwtBytes = New-Object byte[] 32
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($jwtBytes)
$rng.Dispose()
$env:Jwt__SigningKey = [Convert]::ToBase64String($jwtBytes)
```

| Операция | Запрос | Результат |
|---|---|---|
| Регистрация | `POST /api/user/create` с полями JSON `nickName`, `email`, `password` | `201` и публичный профиль. Занятый никнейм возвращает `409`. |
| Вход | `PUT /api/user/login` с полями `nickName` и `password` | `200` с `accessToken`, `tokenType: "Bearer"` и `expiresAt`. Неверные данные возвращают `401`. |
| Текущая сессия | `GET /api/user/isactive` | `200` с ID, никнеймом и ролью или `401`. |
| Выход | `PUT /api/user/logout` | `204`. Сервер отзывает предъявленный JWT, другие входы этого пользователя сохраняются. |

Для защищённых запросов передавайте заголовок `Authorization: Bearer <accessToken>` по HTTPS. JWT действует 30 минут. В Swagger UI введите значение `accessToken` через **Authorize**. React хранит токен в `sessionStorage` вкладки.

Публичная регистрация назначает роль `user`. Участник может отправлять решения и редактировать собственный профиль. Администратор также управляет задачами, темами, тестами и профилями участников. Чтобы назначить первого администратора, оператор PostgreSQL может выполнить в **psql** следующий запрос для уже проверенной учётной записи:

```sql
\prompt 'ID существующего пользователя: ' admin_id
UPDATE "Users" SET "RoleId" = (SELECT "Id" FROM "Roles" WHERE "Title" = 'admin')
WHERE "Id" = :'admin_id'::integer
RETURNING "Id", "NickName";
```

После смены роли пользователю нужно войти снова. Если в старой БД есть участники с одинаковым никнеймом, миграция укажет конфликтующее имя. Дубликаты нужно разрешить до следующего запуска.

Компилятор и процесс решения получают только переменные окружения, нужные инструментам. Для публичного сервера исполнение присланного кода требует отдельной изолированной среды: текущий обработчик запускает процесс с правами учётной записи API.

### Первый запуск на другом устройстве

Укажите подключение к целевой базе PostgreSQL через `ConnectionStrings__DefaultConnection`. При запуске API применит к ней недостающие миграции из проекта. Для пустой существующей базы пользователю PostgreSQL нужны права на создание таблиц и последующую запись данных. Если саму базу также должен создать API, нужны права на её создание.

После миграций API проверяет начальные данные при каждом запуске и добавляет отсутствующие записи.

| Справочник | Начальные записи |
|---|---|
| Роли | `user`, `admin` |
| Языки | `C++ 14 (64x)`, `Python 3.10`, `Java 11` |
| Вердикты | `Compiling`, `Testing`, `Accepted`, `Wrong Answer`, `Compile Error`, `Runtime Error`, `Time Limit`, `Memory Limit`, `Presentation Error`, `Unsupported Language`, `Judge Error` |
| Страны | Кыргызстан |
| Организации | Кут билим окуу-тарбия комплекси, Чөгөм орто мектеби |

На новой базе API также создаёт пользователя `admin` с ролью `admin` и почтой `erjanulukbekov04@gmail.com`. В режиме `Development` в новой копии проекта действует прежний тестовый пароль. Его хеш хранится в `appsettings.Development.json`. Как и раньше, локальный файл `.local/seed-admin.json` может переопределить `NickName`, `Email` и `PasswordHash` для разработки.

Для новой базы в остальных средах задайте серверному процессу `BootstrapAdmin__Password` и ключ `Jwt__SigningKey` перед первым запуском. Пароль из переменной окружения имеет приоритет и в режиме разработки. После создания администратора переменную с паролем можно убрать. Повторный запуск сохраняет пароль и роль существующего пользователя. Если никнейм или почта заняты другим аккаунтом, API запишет предупреждение и пропустит создание администратора.

Под администратором можно создать задачу в архиве, добавить тесты в редакторе задачи, создать тему и подтему на странице тем. Для создания организации в Swagger используйте `POST /api/organization/create` с телом `{"title":"Тестовая организация","description":"Для проверки"}`. Успех возвращает `201`, запрос с токеном обычного пользователя — `403`. Список организаций доступен через `GET /api/organization/get`.

## Swagger
![изображение](https://github.com/user-attachments/assets/a5c21fd5-7255-406e-b53a-b50ba90bf501)
![изображение](https://github.com/user-attachments/assets/82a41e90-ffd0-48bd-a48c-b8ad043068f9)
![изображение](https://github.com/user-attachments/assets/4fc7fb3f-fdee-4cde-9a1e-71175d95ccce)
![изображение](https://github.com/user-attachments/assets/7dec9985-f1ef-486f-accb-c6d9ca27710d)
![изображение](https://github.com/user-attachments/assets/c1da4560-003b-4765-8bd4-2f4af73f02cf)
![изображение](https://github.com/user-attachments/assets/4b316ec2-0f3b-4e6a-a9f2-4575b7dc9763)
![изображение](https://github.com/user-attachments/assets/b0498e7b-206c-477f-8228-3dd595848fc3)
![изображение](https://github.com/user-attachments/assets/015d3ff3-433f-4099-9fcd-1dd6f65aca9d)
![изображение](https://github.com/user-attachments/assets/62f9fe10-9107-4b7d-9de8-a4c6d07df44f)
![изображение](https://github.com/user-attachments/assets/bdd5dd3d-e8cc-4153-982c-13d4e16b9869)
![изображение](https://github.com/user-attachments/assets/7b32a999-6dc2-40c6-b01a-431f6c40b7fb)
![изображение](https://github.com/user-attachments/assets/9498f6ad-7637-4d18-a590-f0877214cf8c)

