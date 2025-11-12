# JwtWorkshop (ПЗ-8: Security & Auth — JWT, OAuth2, Roles)

## Огляд
Цей проєкт реалізує практичне завдання №8 з дисципліни "Security & Auth", яке включає реалізацію автентифікації (JWT) та авторизації (RBAC) на основі ролей за допомогою **C# (.NET 8) Minimal APIs**.

## Стек
* **Мова:** C# 
* **Фреймворк:** .NET 8 (Minimal APIs) 

## Інструкція з Запуску
1. Створіть новий проєкт: `dotnet new web -n JwtWorkshop` (якщо ще не створили).
2. Перейдіть до каталогу: `cd JwtWorkshop`.
3. Встановіть необхідні пакети NuGet: `dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer` та `dotnet add package System.IdentityModel.Tokens.Jwt`.
4. Встановіть змінну середовища `JWT_SECRET`. Приклад для CMD: `set JWT_SECRET=dev_secret_change_me`.
5. Запустіть сервер: `dotnet run`. Сервер стартує на порту 5000/5107.

## Приклади Запитів та Очікувані Відповіді

Для виконання запитів використовуйте REST-клієнт (Postman / curl / Insomnia).

### 1. POST /login (Автентифікація)
Перевіряє облікові дані та повертає `access_token`.
* **Запит:** `POST http://localhost:5000/login`
* **Body:** `{"email":"user@example.com","password":"user123"}`
* **Очікувана відповідь (200 OK):** JSON з `access_token`, `token_type: "Bearer"`, `expires_in: 900`.
* **<img width="1280" height="592" alt="image" src="https://github.com/user-attachments/assets/4272ecfb-2f56-4ccb-82fa-ef52c5334281" /> ** (Скріншот успішного логіну користувача).

### 2. GET /profile (Захищений ресурс)
Доступний лише з валідним JWT.
* **Сценарій 1: Без токена**
    * **Запит:** `GET http://localhost:5000/profile`
    * **Очікувана відповідь:** **401 Unauthorized**.
    * **<img width="1280" height="800" alt="image" src="https://github.com/user-attachments/assets/fb98cf2d-f41d-441b-91d7-403cd6b5cd52" /> ** (Скріншот запиту без токена).
* **Сценарій 2: З валідним токеном**
    * **Запит:** `GET http://localhost:5000/profile` з заголовком `Authorization: Bearer <TOKEN>`
    * **Очікувана відповідь:** **200 OK** з JSON, що містить `{"user_id": ..., "role": ...}`.
    * **<img width="1280" height="800" alt="image" src="https://github.com/user-attachments/assets/122daf1c-5176-4e01-9bb5-2ef174e5b890" /> ** (Скріншот запиту з токеном user).

### 3. DELETE /users/{id} (Авторизація за ролями)
Доступний лише для ролі `admin`.
* **Сценарій 1: Звичайний користувач (role: user)**
    * **Запит:** `DELETE http://localhost:5000/users/5` з заголовком `Authorization: Bearer <USER_TOKEN>`
    * **Очікувана відповідь:** **403 Forbidden**.
    * **<img width="1280" height="800" alt="image" src="https://github.com/user-attachments/assets/6b0cac6a-4518-4227-9232-1bc351f41213" /> ** (Скріншот спроби видалення з токеном user).
* **Сценарій 2: Адміністратор (role: admin)**
    * **Запит:** `DELETE http://localhost:5000/users/5` з заголовком `Authorization: Bearer <ADMIN_TOKEN>`
    * **Очікувана відповідь:** **200 OK**.
    * **<img width="1280" height="800" alt="image" src="https://github.com/user-attachments/assets/db9cc72c-ad12-4eb1-a7e8-2bac2ff07d15" /> ** (Скріншот успішного видалення з токеном admin).

### 4. Крок 4. OAuth2 (Демонстрація)
Для цього кроку використовувався Google OAuth 2.0 Playground для демонстрації потоку Authorization Code Flow (Authorization Code → Access Token → Виклик API).
* **<img width="1280" height="700" alt="image" src="https://github.com/user-attachments/assets/30827477-2894-4067-9841-80ff0606ec0d" /> ** (Скріншот успішного запиту з Bearer-токеном до Google API).
