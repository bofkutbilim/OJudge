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
<img width="2555" height="1244" alt="image" src="https://github.com/user-attachments/assets/57e95835-f77c-400e-a74e-c537a9a67858" />
<img width="2556" height="1243" alt="image" src="https://github.com/user-attachments/assets/eb3a7136-e1b5-47cf-a9f8-c53c45e218c6" />
<img width="2558" height="1243" alt="image" src="https://github.com/user-attachments/assets/4474117f-d4f6-426d-97d6-67ea204b0ec3" />
<img width="2545" height="1213" alt="image" src="https://github.com/user-attachments/assets/e4c2c1e0-441e-4479-bf34-e64138dc9747" />
<img width="2557" height="1248" alt="image" src="https://github.com/user-attachments/assets/034b7d40-1729-4888-ba91-3b7901326b23" />
<img width="2557" height="1241" alt="image" src="https://github.com/user-attachments/assets/81f1a1c9-edfe-483c-b764-c759e471314d" />
<img width="2557" height="1219" alt="image" src="https://github.com/user-attachments/assets/34b369a9-96e2-437e-9e70-71cb8dab42ac" />
<img width="2538" height="1230" alt="image" src="https://github.com/user-attachments/assets/f004fa86-3fa0-4bff-a0c9-efda4adb1dff" />
<img width="2543" height="1231" alt="image" src="https://github.com/user-attachments/assets/49f0ded3-a091-4dc9-92b1-6d9a96d588bf" />
<img width="2533" height="1237" alt="image" src="https://github.com/user-attachments/assets/8ab8c45a-edaa-4a8e-990c-592425f6ffe5" />

